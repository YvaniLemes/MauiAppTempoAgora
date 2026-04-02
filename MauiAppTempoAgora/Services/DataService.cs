using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            // Use sua chave ativa do OpenWeatherMap
            string chave = "ccb5920f163cfefdae69316f034fb9ba";

            // Corrigindo a URL (sem o "+")
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&units=metric&appid={chave}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp = await client.GetAsync(url);

                if (resp.IsSuccessStatusCode)
                {
                    string json = await resp.Content.ReadAsStringAsync();
                    var rascunho = JObject.Parse(json);

                    // Converter sunrise/sunset de Unix para DateTime legível
                    var sunriseUnix = (long?)rascunho["sys"]?["sunrise"] ?? 0;
                    var sunsetUnix = (long?)rascunho["sys"]?["sunset"] ?? 0;

                    DateTime sunrise = DateTimeOffset.FromUnixTimeSeconds(sunriseUnix).ToLocalTime().DateTime;
                    DateTime sunset = DateTimeOffset.FromUnixTimeSeconds(sunsetUnix).ToLocalTime().DateTime;

                    // Preencher objeto Tempo
                    t = new Tempo
                    {
                        lat = (double?)rascunho["coord"]?["lat"] ?? 0,
                        lon = (double?)rascunho["coord"]?["lon"] ?? 0,
                        description = (string?)rascunho["weather"]?[0]?["description"] ?? "Indefinido",
                        main = (string?)rascunho["weather"]?[0]?["main"] ?? "Indefinido",
                        temp_min = (double?)rascunho["main"]?["temp_min"] ?? 0,
                        temp_max = (double?)rascunho["main"]?["temp_max"] ?? 0,
                        speed = (double?)rascunho["wind"]?["speed"] ?? 0,
                        visibility = (int?)rascunho["visibility"] ?? 0,
                        sunrise = sunrise.ToString("HH:mm"),
                        sunset = sunset.ToString("HH:mm")
                    }; // Fecha obj do tempo.
                } // Fecha if se o status do servidor for de sucesso 
            } // Fecha laço using

            return t;
        }
    }
}
