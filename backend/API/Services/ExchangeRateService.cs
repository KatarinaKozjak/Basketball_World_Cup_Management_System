using System.Text.Json;
namespace API.Services;
public interface IExchangeRateService{Task<decimal> GetRateAsync(string from,string to,CancellationToken ct=default);} 

public class ExchangeRateService(HttpClient http,ILogger<ExchangeRateService> log):IExchangeRateService{
    public async Task<decimal> GetRateAsync(string from,string to,CancellationToken ct=default){
        from=from.ToUpperInvariant();
        to=to.ToUpperInvariant();
        if(from==to)return 1m;
        try{using var r=await http.GetAsync($"https://open.er-api.com/v6/latest/{from}",ct);
            r.EnsureSuccessStatusCode();
            using var doc=JsonDocument.Parse(await r.Content.ReadAsStringAsync(ct));
            if(doc.RootElement.GetProperty("rates").TryGetProperty(to,out var rate))return rate.GetDecimal();
        }catch(Exception e){
            log.LogWarning(e,"Kursni API nije dostupan, koristi se rezervni kurs.");
        }
        var fallback=new Dictionary<string,decimal>{{"USD",1.08m},{"RSD",117.2m},{"QAR",3.93m},{"EUR",1m}};
        if(from=="EUR"&&fallback.TryGetValue(to,out var v))return v;
        if(to=="EUR"&&fallback.TryGetValue(from,out v))return 1m/v;
        throw new InvalidOperationException($"Nije dostupan kurs {from}/{to}.");
    }
}
