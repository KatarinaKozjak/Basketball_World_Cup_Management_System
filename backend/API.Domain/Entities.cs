using System.ComponentModel.DataAnnotations;
namespace API.Domain;

public enum TicketStatus { Active, Cancelled }

public enum PromoCodeStatus { Active, Used, Invalid }

public class Championship { 
    public long Id {get;set;} 
    [MaxLength(150)] public string Name {get;set;}="Qatar 2027"; 
    [MaxLength(200)] public string Location {get;set;}="Qatar"; 
    public DateTime StartsAt {get;set;} 
    public DateTime EndsAt {get;set;} 
    public DateTime Discount10Until {get;set;} 
    [MaxLength(1000)] public string? Description {get;set;} 
    public ICollection<Group> Groups {get;set;}=new List<Group>(); 
}

public class Group { 
    public long Id {get;set;} 
    [MaxLength(30)] public string Code {get;set;}="";
    public long ChampionshipId {get;set;} 
    public Championship Championship {get;set;}=null!; 
    public ICollection<Team> Teams {get;set;}=new List<Team>(); 
}

public class Team { 
    public long Id {get;set;} 
    [MaxLength(100)] public string Name {get;set;}="";
    public int PlayerCount {get;set;} 
    public string? Coach {get;set;} 
    public int Wins {get;set;} 
    public int Losses {get;set;} 
    public int Points {get;set;} 
    public long GroupId {get;set;} 
    public Group Group {get;set;}=null!; 
}

public class Stadium { 
    public long Id {get;set;} 
    [MaxLength(120)] public string Name {get;set;}=""; 
    [MaxLength(100)] public string City {get;set;}=""; 
    [MaxLength(200)] public string Address {get;set;}=""; 
    public int Capacity {get;set;} 
    public ICollection<Match> Matches {get;set;}=new List<Match>(); 
}

public class Match { 
    public long Id {get;set;} 
    public DateTime StartsAt {get;set;} 
    public decimal BasePriceEur {get;set;} 
    public long StadiumId {get;set;} 
    public Stadium Stadium {get;set;}=null!; 
    public long Team1Id {get;set;} 
    public Team Team1 {get;set;}=null!; 
    public long Team2Id {get;set;} 
    public Team Team2 {get;set;}=null!; 
    public ICollection<TicketItem> TicketItems {get;set;}=new List<TicketItem>(); 
}

public class Currency { 
    public long Id {get;set;} 
    [MaxLength(3)] public string Code {get;set;}="EUR"; 
    public bool IsAllowed {get;set;}=true; 
}

public class Customer { 
    public long Id {get;set;} 
    [MaxLength(80)] public string FirstName {get;set;}=""; 
    [MaxLength(80)] public string LastName {get;set;}=""; 
    [MaxLength(200)] public string Address {get;set;}=""; 
    [MaxLength(20)] public string PostalCode {get;set;}=""; 
    [MaxLength(100)] public string City {get;set;}=""; 
    [MaxLength(100)] public string Country {get;set;}=""; 
    [MaxLength(180)] public string Email {get;set;}=""; 
    public ICollection<Ticket> Tickets {get;set;}=new List<Ticket>(); 
}

public class PromoCode { 
    public long Id {get;set;} 
    [MaxLength(30)] public string Code {get;set;}=""; 
    public PromoCodeStatus Status {get;set;}=PromoCodeStatus.Active; 
    public DateTime ExpiresAt {get;set;} 
    public long? SourceTicketId {get;set;} 
    public Ticket? SourceTicket {get;set;} 
    public long? UsedByTicketId {get;set;} 
    public Ticket? UsedByTicket {get;set;} 
}

public class Ticket { 
    public long Id {get;set;} 
    [MaxLength(30)] public string AccessCode {get;set;}=""; 
    public DateTime CreatedAt {get;set;}=DateTime.UtcNow; 
    public DateTime UpdatedAt {get;set;}=DateTime.UtcNow; 
    public TicketStatus Status {get;set;}=TicketStatus.Active; 
    public long CustomerId {get;set;} 
    public Customer Customer {get;set;}=null!; 
    public long CurrencyId {get;set;} 
    public Currency Currency {get;set;}=null!;
    public decimal ExchangeRate {get;set;}=1;
    public decimal SubtotalEur {get;set;}
    public decimal DiscountPercent {get;set;}
    public decimal TotalAmount {get;set;}
    public long? UsedPromoCodeId {get;set;}
    public PromoCode? UsedPromoCode {get;set;}
    public PromoCode GeneratedPromoCode {get;set;}=null!;
    public ICollection<TicketItem> Items {get;set;}=new List<TicketItem>(); 
}

public class TicketItem { 
    public long Id {get;set;} 
    public long TicketId {get;set;} 
    public Ticket Ticket {get;set;}=null!; 
    public long MatchId {get;set;} 
    public Match Match {get;set;}=null!; 
    public int SeatCount {get;set;} 
    public decimal UnitPriceEur {get;set;} 
}
