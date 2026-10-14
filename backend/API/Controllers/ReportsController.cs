using API.DataAccess;
using API.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers;

[ApiController,Route("api/reports")]public class ReportsController(ApplicationDbContext db):ControllerBase{

    [HttpGet("by-match")]
    public async Task<IActionResult> ByMatch(CancellationToken ct)=>Ok(await db.Matches.AsNoTracking().Include(x=>x.Team1).Include(x=>x.Team2).OrderBy(x=>x.StartsAt).Select(x=>new{x.Id,match=x.Team1.Name+" - "+x.Team2.Name,x.StartsAt,ticketsSold=x.TicketItems.Where(i=>i.Ticket.Status==TicketStatus.Active).Sum(i=>(int?)i.SeatCount)??0}).ToListAsync(ct));
    
    [HttpGet("by-day")]
    public async Task<IActionResult> ByDay(CancellationToken ct)=>Ok(await db.Tickets.AsNoTracking().GroupBy(x=>x.CreatedAt.Date).OrderBy(x=>x.Key).Select(x=>new{date=x.Key,tickets=x.Count()}).ToListAsync(ct));
}
