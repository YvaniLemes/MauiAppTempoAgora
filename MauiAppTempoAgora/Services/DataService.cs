using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            // Use sua chave ativa do OpenWeatherMap
            string chave = "ccb5920f163cfefdae69316f034fb9ba";

            
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&units=metric&appid={chave}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp;
                try
                {
                    resp = await client.GetAsync(url);
                }
                catch (HttpRequestException) //Erro de conexão - Fichário ag07
                {
                    throw new Exception("Erro de Conexão! Verifique sua internet e tente novamente.");
                }

                //Erro Cidade não encontrada ERRO 404 NOTFOUND - Fichário ag07
                if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Ops... Cidade não encontrada! Tente novamente.");
                }

                
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception($"Erro ao buscar dados: {resp.ReasonPhrase}");
                }

                
                string json = await resp.Content.ReadAsStringAsync();

                var rascunho = JObject.Parse(json);

                DateTime time = new();
                DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                t = new()
                {
                    lat = (double)rascunho["coord"]["lat"],
                    lon = (double)rascunho["coord"]["lon"],
                    description = (string)rascunho["weather"][0]["description"],
                    main = (string)rascunho["weather"][0]["main"],
                    temp_min = (double)rascunho["main"]["temp_min"],
                    temp_max = (double)rascunho["main"]["temp_max"],
                    speed = (double)rascunho["wind"]["speed"],
                    visibility = (int)rascunho["visibility"],
                    sunrise = sunrise.ToString(),
                    sunset = sunset.ToString()
                }; // Fecha objeto do Tempo
            } // Fecha laço do using

            return t;
        }
    }
}