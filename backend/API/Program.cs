using API.DataAccess;
using API.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder=WebApplication.CreateBuilder(args);

var cs=builder.Configuration.GetConnectionString("DefaultConnection")??throw new InvalidOperationException("Nedostaje DefaultConnection.");

builder.Services.AddDbContext<ApplicationDbContext>(o=>o.UseMySql(cs,ServerVersion.AutoDetect(cs)));

builder.Services.AddControllers().AddJsonOptions(o=>o.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IExchangeRateService,ExchangeRateService>();
builder.Services.AddScoped<ITicketService,TicketService>();
builder.Services.AddCors(o=>o.AddPolicy("frontend",p=>p.WithOrigins("http://localhost:5173","http://localhost:3000").AllowAnyHeader().AllowAnyMethod()));

var app=builder.Build();

app.UseMiddleware<ErrorMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("frontend");
app.MapControllers();

if(app.Configuration.GetValue("Database:EnsureCreated",false)){
    using var scope=app.Services.CreateScope();
    var db=scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();
}
app.Run();

public class ErrorMiddleware(RequestDelegate next,ILogger<ErrorMiddleware> log){
    public async Task Invoke(HttpContext c){
        try{await next(c);}
        catch(Exception e){
            log.LogError(e,"Greška");
            c.Response.StatusCode=e switch{ArgumentException=>400,KeyNotFoundException=>404,InvalidOperationException=>409,_=>500};
            await c.Response.WriteAsJsonAsync(new{message=e.Message});
        }
    }
}
