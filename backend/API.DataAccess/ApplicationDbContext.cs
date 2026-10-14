using API.Domain;
using Microsoft.EntityFrameworkCore;
namespace API.DataAccess;

 public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):DbContext(options){

    public DbSet<Championship> Championships=>Set<Championship>();
    public DbSet<Group> Groups=>Set<Group>();
    public DbSet<Team> Teams=>Set<Team>();
    public DbSet<Stadium> Stadiums=>Set<Stadium>();
    public DbSet<Match> Matches=>Set<Match>();
    public DbSet<Currency> Currencies=>Set<Currency>();
    public DbSet<Customer> Customers=>Set<Customer>();
    public DbSet<Ticket> Tickets=>Set<Ticket>();
    public DbSet<TicketItem> TicketItems=>Set<TicketItem>();
    public DbSet<PromoCode> PromoCodes=>Set<PromoCode>();

    protected override void OnModelCreating(ModelBuilder b){
        b.Entity<Championship>().ToTable("championships");
        b.Entity<Group>().ToTable("groups");
        b.Entity<Team>().ToTable("teams");
        b.Entity<Stadium>().ToTable("stadiums");
        b.Entity<Match>().ToTable("matches");
        b.Entity<Currency>().ToTable("currencies");
        b.Entity<Customer>().ToTable("customers");
        b.Entity<Ticket>().ToTable("tickets");
        b.Entity<TicketItem>().ToTable("ticket_items");
        b.Entity<PromoCode>().ToTable("promo_codes");

        b.Entity<Group>().HasOne(x=>x.Championship)
            .WithMany(x=>x.Groups)
            .HasForeignKey(x=>x.ChampionshipId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Team>().HasOne(x=>x.Group)
            .WithMany(x=>x.Teams)
            .HasForeignKey(x=>x.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Match>().HasOne(x=>x.Stadium)
            .WithMany(x=>x.Matches)
            .HasForeignKey(x=>x.StadiumId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Match>().HasOne(x=>x.Team1)
            .WithMany()
            .HasForeignKey(x=>x.Team1Id)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Match>().HasOne(x=>x.Team2)
            .WithMany()
            .HasForeignKey(x=>x.Team2Id)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Ticket>().HasIndex(x=>x.AccessCode)
            .IsUnique(); 
        
        b.Entity<PromoCode>().HasIndex(x=>x.Code)
            .IsUnique(); 
        
        b.Entity<Currency>().HasIndex(x=>x.Code)
            .IsUnique();

        b.Entity<Ticket>().Property(x=>x.Status)
            .HasConversion<string>();
        
        b.Entity<PromoCode>().Property(x=>x.Status)
            .HasConversion<string>();

        b.Entity<Ticket>().HasOne(x=>x.UsedPromoCode)
            .WithOne(x=>x.UsedByTicket)
            .HasForeignKey<Ticket>(x=>x.UsedPromoCodeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Ticket>().HasOne(x=>x.GeneratedPromoCode)
            .WithOne(x=>x.SourceTicket)
            .HasForeignKey<PromoCode>(x=>x.SourceTicketId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<TicketItem>().HasOne(x=>x.Ticket)
            .WithMany(x=>x.Items)
            .HasForeignKey(x=>x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
        
        b.Entity<TicketItem>().HasOne(x=>x.Match)
            .WithMany(x=>x.TicketItems)
            .HasForeignKey(x=>x.MatchId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Championship>().HasData(new Championship{Id=1,Name="FIBA Svetsko prvenstvo Qatar 2027",Location="Doha, Qatar",StartsAt=new DateTime(2027,8,27),EndsAt=new DateTime(2027,9,12),Discount10Until=new DateTime(2027,5,31),Description="Aplikacija za pregled prvenstva i kupovinu karata."});
        b.Entity<Currency>().HasData(new Currency{Id=1,Code="EUR",IsAllowed=true},new Currency{Id=2,Code="USD",IsAllowed=true},new Currency{Id=3,Code="RSD",IsAllowed=true},new Currency{Id=4,Code="QAR",IsAllowed=true});
    }
 }
