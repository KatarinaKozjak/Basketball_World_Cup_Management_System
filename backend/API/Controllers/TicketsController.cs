using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;

[ApiController,Route("api/tickets")]public class TicketsController(ITicketService service):ControllerBase{

    [HttpPost]
    public async Task<IActionResult> Purchase(PurchaseTicketRequest r,CancellationToken ct)=>Ok(await service.PurchaseAsync(r,ct));

    [HttpPost("find")]
    public async Task<IActionResult> Find(AccessTicketRequest r,CancellationToken ct){
        var x=await service.FindAsync(r.Email,r.AccessCode,ct);
        return x is null?NotFound(new{message="Karta nije pronađena."}):Ok(x);
    } 

    [HttpPut]
    public async Task<IActionResult> Modify(ModifyTicketRequest r,CancellationToken ct)=>Ok(await service.ModifyAsync(r,ct));

    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel(AccessTicketRequest r,CancellationToken ct){
        await service.CancelAsync(r,ct);
        return NoContent();
    }
}
