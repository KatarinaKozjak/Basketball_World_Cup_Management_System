using API.DataAccess;
using API.Domain;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace API.Services;

public interface ITicketService
{
    Task<object> PurchaseAsync(PurchaseTicketRequest request, CancellationToken ct);
    Task<object?> FindAsync(string email, string code, CancellationToken ct);
    Task<object> ModifyAsync(ModifyTicketRequest request, CancellationToken ct);
    Task CancelAsync(AccessTicketRequest request, CancellationToken ct);
}

public class TicketService(ApplicationDbContext db, IExchangeRateService rates) : ITicketService
{
    private static string RandomCode(string prefix, int bytes = 8) =>
        prefix + Convert.ToHexString(RandomNumberGenerator.GetBytes(bytes));

    public async Task<object> PurchaseAsync(PurchaseTicketRequest request, CancellationToken ct)
    {
        if (!string.Equals(request.Email, request.EmailConfirmation, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Email adrese se ne poklapaju.");

        var currency = await db.Currencies.SingleOrDefaultAsync(
            x => x.Code == request.CurrencyCode.ToUpper() && x.IsAllowed, ct)
            ?? throw new ArgumentException("Izabrana valuta nije dozvoljena.");

        var matchIds = request.Items.Select(x => x.MatchId).Distinct().ToList();
        var matches = await db.Matches
            .Include(x => x.Stadium)
            .Include(x => x.Team1)
            .Include(x => x.Team2)
            .Where(x => matchIds.Contains(x.Id))
            .ToListAsync(ct);

        if (matches.Count != matchIds.Count)
            throw new ArgumentException("Jedna ili više utakmica ne postoje.");

        foreach (var item in request.Items)
        {
            var match = matches.Single(x => x.Id == item.MatchId);
            var sold = await db.TicketItems
                .Where(x => x.MatchId == item.MatchId && x.Ticket.Status == TicketStatus.Active)
                .SumAsync(x => (int?)x.SeatCount, ct) ?? 0;

            if (sold + item.SeatCount > match.Stadium.Capacity)
                throw new InvalidOperationException($"Nema dovoljno mesta za utakmicu {match.Id}.");
        }

        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Email == request.Email, ct);
        if (customer is null)
        {
            customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address,
                PostalCode = request.PostalCode,
                City = request.City,
                Country = request.Country,
                Email = request.Email
            };
            db.Customers.Add(customer);
        }
        else
        {
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Address = request.Address;
            customer.PostalCode = request.PostalCode;
            customer.City = request.City;
            customer.Country = request.Country;
        }

        PromoCode? usedPromoCode = null;
        decimal discountPercent = 0;
        var championship = await db.Championships.FirstAsync(ct);

        if (DateTime.UtcNow.Date <= championship.Discount10Until.Date)
            discountPercent += 10;

        if (!string.IsNullOrWhiteSpace(request.PromoCode))
        {
            usedPromoCode = await db.PromoCodes.SingleOrDefaultAsync(
                x => x.Code == request.PromoCode &&
                     x.Status == PromoCodeStatus.Active &&
                     x.ExpiresAt >= DateTime.UtcNow, ct)
                ?? throw new ArgumentException("Promo-kod nije validan.");

            discountPercent += 5;
            usedPromoCode.Status = PromoCodeStatus.Used;
        }

        var subtotal = request.Items.Sum(i =>
            matches.Single(m => m.Id == i.MatchId).BasePriceEur * i.SeatCount);
        var exchangeRate = await rates.GetRateAsync("EUR", currency.Code, ct);

        var ticket = new Ticket
        {
            AccessCode = RandomCode("TKT-", 6),
            Customer = customer,
            Currency = currency,
            ExchangeRate = exchangeRate,
            SubtotalEur = subtotal,
            DiscountPercent = discountPercent,
            TotalAmount = Math.Round(subtotal * (1 - discountPercent / 100) * exchangeRate, 2),
            UsedPromoCode = usedPromoCode,
            Status = TicketStatus.Active
        };

        foreach (var item in request.Items)
        {
            var match = matches.Single(m => m.Id == item.MatchId);
            ticket.Items.Add(new TicketItem
            {
                MatchId = item.MatchId,
                Match = match,
                SeatCount = item.SeatCount,
                UnitPriceEur = match.BasePriceEur
            });
        }

        ticket.GeneratedPromoCode = new PromoCode
        {
            Code = RandomCode("PROMO-", 5),
            Status = PromoCodeStatus.Active,
            ExpiresAt = championship.EndsAt.AddDays(-1),
            SourceTicket = ticket
        };

        db.Tickets.Add(ticket);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return ToDto(ticket);
    }

    public async Task<object?> FindAsync(string email, string code, CancellationToken ct)
    {
        var ticket = await TicketQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.AccessCode == code && x.Customer.Email == email, ct);

        return ticket is null ? null : ToDto(ticket);
    }

    public async Task<object> ModifyAsync(ModifyTicketRequest request, CancellationToken ct)
    {
        var ticket = await TicketQuery()
            .SingleOrDefaultAsync(x => x.AccessCode == request.AccessCode && x.Customer.Email == request.Email, ct)
            ?? throw new KeyNotFoundException("Karta nije pronađena.");

        if (ticket.Status == TicketStatus.Cancelled)
            throw new InvalidOperationException("Otkazana karta se ne može menjati.");

        foreach (var itemId in request.RemoveItemIds.Distinct())
        {
            var item = ticket.Items.SingleOrDefault(x => x.Id == itemId);
            if (item is not null)
            {
                db.TicketItems.Remove(item);
                ticket.Items.Remove(item);
            }
        }

        var addMatchIds = request.AddItems.Select(x => x.MatchId).Distinct().ToList();
        var matches = await db.Matches
            .Include(x => x.Stadium)
            .Include(x => x.Team1)
            .Include(x => x.Team2)
            .Where(x => addMatchIds.Contains(x.Id))
            .ToListAsync(ct);

        if (matches.Count != addMatchIds.Count)
            throw new ArgumentException("Jedna ili više utakmica ne postoje.");

        foreach (var item in request.AddItems)
        {
            var match = matches.Single(x => x.Id == item.MatchId);
            var sold = await db.TicketItems
                .Where(x => x.MatchId == item.MatchId && x.Ticket.Status == TicketStatus.Active)
                .SumAsync(x => (int?)x.SeatCount, ct) ?? 0;

            if (sold + item.SeatCount > match.Stadium.Capacity)
                throw new InvalidOperationException($"Nema dovoljno mesta za utakmicu {match.Id}.");

            ticket.Items.Add(new TicketItem
            {
                MatchId = item.MatchId,
                Match = match,
                SeatCount = item.SeatCount,
                UnitPriceEur = match.BasePriceEur
            });
        }

        ticket.SubtotalEur = ticket.Items.Sum(x => x.UnitPriceEur * x.SeatCount);
        ticket.TotalAmount = Math.Round(
            ticket.SubtotalEur * (1 - ticket.DiscountPercent / 100) * ticket.ExchangeRate, 2);
        ticket.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return ToDto(ticket);
    }

    public async Task CancelAsync(AccessTicketRequest request, CancellationToken ct)
    {
        var ticket = await db.Tickets
            .Include(x => x.Customer)
            .Include(x => x.GeneratedPromoCode)
            .SingleOrDefaultAsync(x => x.AccessCode == request.AccessCode && x.Customer.Email == request.Email, ct)
            ?? throw new KeyNotFoundException("Karta nije pronađena.");

        if (ticket.Status == TicketStatus.Cancelled)
            return;

        ticket.Status = TicketStatus.Cancelled;
        ticket.UpdatedAt = DateTime.UtcNow;
        ticket.GeneratedPromoCode.Status = PromoCodeStatus.Invalid;
        await db.SaveChangesAsync(ct);
    }

    private IQueryable<Ticket> TicketQuery() => db.Tickets
        .Include(x => x.Customer)
        .Include(x => x.Currency)
        .Include(x => x.GeneratedPromoCode)
        .Include(x => x.UsedPromoCode)
        .Include(x => x.Items).ThenInclude(x => x.Match).ThenInclude(x => x.Team1)
        .Include(x => x.Items).ThenInclude(x => x.Match).ThenInclude(x => x.Team2);

    private static object ToDto(Ticket ticket) => new
    {
        ticket.Id,
        ticket.AccessCode,
        ticket.CreatedAt,
        ticket.UpdatedAt,
        status = ticket.Status.ToString(),
        customer = new { ticket.Customer.FirstName, ticket.Customer.LastName, ticket.Customer.Email },
        currency = ticket.Currency.Code,
        ticket.ExchangeRate,
        ticket.SubtotalEur,
        ticket.DiscountPercent,
        ticket.TotalAmount,
        generatedPromoCode = ticket.GeneratedPromoCode.Code,
        usedPromoCode = ticket.UsedPromoCode?.Code,
        items = ticket.Items.Select(i => new
        {
            i.Id,
            i.MatchId,
            i.SeatCount,
            i.UnitPriceEur,
            match = $"{i.Match.Team1.Name} - {i.Match.Team2.Name}",
            i.Match.StartsAt
        })
    };
}
