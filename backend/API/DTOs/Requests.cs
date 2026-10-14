using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public record TicketItemRequest(long MatchId,[Range(1,20)] int SeatCount);

public class PurchaseTicketRequest{
    [Required] public string FirstName{get;set;}=""; 
    [Required] public string LastName{get;set;}=""; 
    [Required] public string Address{get;set;}=""; 
    [Required] public string PostalCode{get;set;}=""; 
    [Required] public string City{get;set;}=""; 
    [Required] public string Country{get;set;}=""; 
    [Required,EmailAddress] public string Email{get;set;}=""; 
    [Required,EmailAddress] public string EmailConfirmation{get;set;}=""; 
    [Required,MinLength(1)] public List<TicketItemRequest> Items{get;set;}=[]; 
    [Required] public string CurrencyCode{get;set;}="EUR";
    public string? PromoCode{get;set;} 
}

public class AccessTicketRequest{
    [Required,EmailAddress] public string Email{get;set;}="";
    [Required] public string AccessCode{get;set;}="";
}

public class ModifyTicketRequest:AccessTicketRequest{
    public List<TicketItemRequest> AddItems{get;set;}=[];
    public List<long> RemoveItemIds{get;set;}=[];
}

public class ChampionshipUpdateRequest{
    [Required]public string Name{get;set;}="";
    [Required]public string Location{get;set;}="";
    public DateTime StartsAt{get;set;}
    public DateTime EndsAt{get;set;}
    public DateTime Discount10Until{get;set;}
    public string? Description{get;set;}
}

public class GroupRequest{
    [Required]public string Code{get;set;}="";
    public long ChampionshipId{get;set;}=1;
}

public class TeamRequest{
    [Required]public string Name{get;set;}="";
    public int PlayerCount{get;set;}
    public string? Coach{get;set;}
    public long GroupId{get;set;}
    public int Wins{get;set;}
    public int Losses{get;set;}
    public int Points{get;set;}
}

public class StadiumRequest{
    [Required]public string Name{get;set;}="";
    [Required]public string City{get;set;}="";
    [Required]public string Address{get;set;}="";
    [Range(1,int.MaxValue)]public int Capacity{get;set;}
}

public class MatchRequest{
    public DateTime StartsAt{get;set;}
    [Range(0.01,double.MaxValue)]public decimal BasePriceEur{get;set;}
    public long StadiumId{get;set;}
    public long Team1Id{get;set;}
    public long Team2Id{get;set;}
}

public class CurrencyRequest{
    [Required,StringLength(3,MinimumLength=3)]public string Code{get;set;}="";
    public bool IsAllowed{get;set;}=true;
}
