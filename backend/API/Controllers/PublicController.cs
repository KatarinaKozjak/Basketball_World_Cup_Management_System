using API.DataAccess;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
namespace API.Controllers;

[ApiController,Route("api/public")]public class PublicController(ApplicationDbContext db,IMemoryCache cache):ControllerBase{

    [HttpGet("home")]
    public async Task<IActionResult> Home(CancellationToken ct){
        var result=await cache.GetOrCreateAsync("home-data",async e=>{
            e.AbsoluteExpirationRelativeToNow=TimeSpan.FromMinutes(10);
            var c=await db.Championships.AsNoTracking().Include(x=>x.Groups).ThenInclude(x=>x.Teams).FirstOrDefaultAsync(ct);
            return new{championship=c,groups=c?.Groups.OrderBy(x=>x.Code).Select(g=>new{g.Id,g.Code,teams=g.Teams.OrderBy(t=>t.Name).Select(t=>new{t.Id,t.Name,t.Coach,t.Wins,t.Losses,t.Points})})};
        });
        return Ok(result);
    }

    [HttpGet("matches")]
    public async Task<IActionResult> Matches(CancellationToken ct)=>Ok(await db.Matches.AsNoTracking().Include(x=>x.Team1).Include(x=>x.Team2).Include(x=>x.Stadium).OrderBy(x=>x.StartsAt).Select(x=>new{x.Id,x.StartsAt,x.BasePriceEur,team1=x.Team1.Name,team2=x.Team2.Name,stadium=x.Stadium.Name,x.Stadium.Capacity,sold=x.TicketItems.Where(i=>i.Ticket.Status==API.Domain.TicketStatus.Active).Sum(i=>(int?)i.SeatCount)??0}).ToListAsync(ct));
    
    [HttpGet("currencies")]
    public async Task<IActionResult> Currencies(CancellationToken ct)=>Ok(await db.Currencies.AsNoTracking().Where(x=>x.IsAllowed).OrderBy(x=>x.Code).ToListAsync(ct));}
