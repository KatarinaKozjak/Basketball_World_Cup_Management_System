using API.DataAccess;
using API.Domain;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
namespace API.Controllers;
[ApiController,Route("api/admin")]public class AdminController(ApplicationDbContext db,IMemoryCache cache):ControllerBase{

    [HttpGet("championship")]
    public async Task<IActionResult> GetChampionship(CancellationToken ct)=>Ok(await db.Championships.AsNoTracking().FirstAsync(ct));

    [HttpPut("championship/{id:long}")]
    public async Task<IActionResult> UpdateChampionship(long id,ChampionshipUpdateRequest r,CancellationToken ct){
        var x=await db.Championships.FindAsync([id],ct);
        if(x is null)return NotFound();
        x.Name=r.Name;
        x.Location=r.Location;
        x.StartsAt=r.StartsAt;
        x.EndsAt=r.EndsAt;
        x.Discount10Until=r.Discount10Until;
        x.Description=r.Description;
        await db.SaveChangesAsync(ct);
        cache.Remove("home-data");
        return Ok(x);
    }

    [HttpGet("groups")]
    public async Task<IActionResult> Groups(CancellationToken ct)=>Ok(await db.Groups.AsNoTracking().Include(x=>x.Teams).ToListAsync(ct));

    [HttpPost("groups")]
    public async Task<IActionResult> AddGroup(GroupRequest r,CancellationToken ct){
        var x=new Group{Code=r.Code,ChampionshipId=r.ChampionshipId};
        db.Groups.Add(x);
        await db.SaveChangesAsync(ct);
        cache.Remove("home-data");
        return Ok(x);
    }

    [HttpPut("groups/{id:long}")]
    public async Task<IActionResult> UpdateGroup(long id,GroupRequest r,CancellationToken ct){
        var x=await db.Groups.FindAsync([id],ct);
        if(x is null)return NotFound();
        x.Code=r.Code;
        x.ChampionshipId=r.ChampionshipId;
        await db.SaveChangesAsync(ct);
        cache.Remove("home-data");
        return Ok(x);
    }

    [HttpDelete("groups/{id:long}")]
    public async Task<IActionResult> DeleteGroup(long id,CancellationToken ct){
        var x=await db.Groups.FindAsync([id],ct);
        if(x is null)return NotFound();
        db.Groups.Remove(x);
        await db.SaveChangesAsync(ct);
        cache.Remove("home-data");
        return NoContent();
    }

    [HttpGet("teams")]
    public async Task<IActionResult> Teams(CancellationToken ct)=>Ok(await db.Teams.AsNoTracking().Include(x=>x.Group).ToListAsync(ct));

    [HttpPost("teams")]
    public async Task<IActionResult> AddTeam(TeamRequest r,CancellationToken ct){
        var x=new Team{Name=r.Name,PlayerCount=r.PlayerCount,Coach=r.Coach,GroupId=r.GroupId,Wins=r.Wins,Losses=r.Losses,Points=r.Points};
        db.Teams.Add(x);
        await db.SaveChangesAsync(ct);
        cache.Remove("home-data");
        return Ok(x);
    }

    [HttpPut("teams/{id:long}")]
    public async Task<IActionResult> UpdateTeam(long id,TeamRequest r,CancellationToken ct){
        var x=await db.Teams.FindAsync([id],ct);
        if(x is null)return NotFound();
        x.Name=r.Name;
        x.PlayerCount=r.PlayerCount;
        x.Coach=r.Coach;
        x.GroupId=r.GroupId;
        x.Wins=r.Wins;
        x.Losses=r.Losses;
        x.Points=r.Points;
        await db.SaveChangesAsync(ct);
        cache.Remove("home-data");
        return Ok(x);
    }

    [HttpDelete("teams/{id:long}")]
    public async Task<IActionResult> DeleteTeam(long id,CancellationToken ct){
        var x=await db.Teams.FindAsync([id],ct);
        if(x is null)return NotFound();
        db.Teams.Remove(x);
        await db.SaveChangesAsync(ct);
        cache.Remove("home-data");
        return NoContent();
    }

    [HttpGet("stadiums")]
    public async Task<IActionResult> Stadiums(CancellationToken ct)=>Ok(await db.Stadiums.AsNoTracking().ToListAsync(ct));

    [HttpPost("stadiums")]
    public async Task<IActionResult> AddStadium(StadiumRequest r,CancellationToken ct){
        var x=new Stadium{Name=r.Name,City=r.City,Address=r.Address,Capacity=r.Capacity};
        db.Stadiums.Add(x);
        await db.SaveChangesAsync(ct);
        return Ok(x);
    }

    [HttpPut("stadiums/{id:long}")]
    public async Task<IActionResult> UpdateStadium(long id,StadiumRequest r,CancellationToken ct){
        var x=await db.Stadiums.FindAsync([id],ct);
        if(x is null)return NotFound();
        x.Name=r.Name;
        x.City=r.City;
        x.Address=r.Address;
        x.Capacity=r.Capacity;
        await db.SaveChangesAsync(ct);
        return Ok(x);
    }

    [HttpDelete("stadiums/{id:long}")]
    public async Task<IActionResult> DeleteStadium(long id,CancellationToken ct){
        var x=await db.Stadiums.FindAsync([id],ct);
        if(x is null)return NotFound();
        db.Stadiums.Remove(x);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpGet("matches")]
    public async Task<IActionResult> Matches(CancellationToken ct)=>Ok(await db.Matches.AsNoTracking().Include(x=>x.Team1).Include(x=>x.Team2).Include(x=>x.Stadium).ToListAsync(ct));

    [HttpPost("matches")]
    public async Task<IActionResult> AddMatch(MatchRequest r,CancellationToken ct){
        if(r.Team1Id==r.Team2Id)return BadRequest(new{message="Timovi moraju biti različiti."});
        var x=new Match{StartsAt=r.StartsAt,BasePriceEur=r.BasePriceEur,StadiumId=r.StadiumId,Team1Id=r.Team1Id,Team2Id=r.Team2Id};
        db.Matches.Add(x);
        await db.SaveChangesAsync(ct);
        return Ok(x);
    }

    [HttpPut("matches/{id:long}")]
    public async Task<IActionResult> UpdateMatch(long id,MatchRequest r,CancellationToken ct){
        var x=await db.Matches.FindAsync([id],ct);
        if(x is null)return NotFound();
        x.StartsAt=r.StartsAt;
        x.BasePriceEur=r.BasePriceEur;
        x.StadiumId=r.StadiumId;
        x.Team1Id=r.Team1Id;
        x.Team2Id=r.Team2Id;
        await db.SaveChangesAsync(ct);
        return Ok(x);
    }

    [HttpDelete("matches/{id:long}")]
    public async Task<IActionResult> DeleteMatch(long id,CancellationToken ct){
        var x=await db.Matches.FindAsync([id],ct);
        if(x is null)return NotFound();
        db.Matches.Remove(x);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpGet("currencies")]
    public async Task<IActionResult> Currencies(CancellationToken ct)=>Ok(await db.Currencies.AsNoTracking().ToListAsync(ct));

    [HttpPost("currencies")]
    public async Task<IActionResult> AddCurrency(CurrencyRequest r,CancellationToken ct){
        var x=new Currency{Code=r.Code.ToUpper(),IsAllowed=r.IsAllowed};
        db.Currencies.Add(x);
        await db.SaveChangesAsync(ct);
        return Ok(x);
    }

    [HttpPut("currencies/{id:long}")]
    public async Task<IActionResult> UpdateCurrency(long id,CurrencyRequest r,CancellationToken ct){
        var x=await db.Currencies.FindAsync([id],ct);
        if(x is null)return NotFound();
        x.Code=r.Code.ToUpper();
        x.IsAllowed=r.IsAllowed;
        await db.SaveChangesAsync(ct);
        return Ok(x);
    }
}
