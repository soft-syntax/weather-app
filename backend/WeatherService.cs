
// ========================================================
// WeatherService.cs
// Professional Weather Service for Weather App
// Supports OpenWeatherMap API
// ========================================================

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WeatherApp
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/";
        private readonly string _apiKey;

        public WeatherService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WeatherApp/1.0");
        }

        /// <summary>
        /// Get current weather by city name
        /// </summary>
        public async Task<WeatherResponse> GetCurrentWeatherAsync(string city)
        {
            try
            {
                string url = $"weather?q={Uri.EscapeDataString(city)}&units=metric&appid={_apiKey}";
                var response = await _httpClient.GetFromJsonAsync<WeatherResponse>(url);

                if (response == null)
                    throw new Exception("Failed to retrieve weather data.");

                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get 5-day forecast
        /// </summary>
        public async Task<ForecastResponse> GetForecastAsync(string city)
        {
            try
            {
                string url = $"forecast?q={Uri.EscapeDataString(city)}&units=metric&appid={_apiKey}";
                var response = await _httpClient.GetFromJsonAsync<ForecastResponse>(url);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Forecast error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get weather by coordinates (lat, lon)
        /// </summary>
        public async Task<WeatherResponse> GetWeatherByCoordinatesAsync(double lat, double lon)
        {
            string url = $"weather?lat={lat}&lon={lon}&units=metric&appid={_apiKey}";
            return await _httpClient.GetFromJsonAsync<WeatherResponse>(url);
        }
    }

    // ====================== MODELS ======================

    public class WeatherResponse
    {
        [JsonPropertyName("name")]
        public string CityName { get; set; } = string.Empty;

        [JsonPropertyName("main")]
        public MainInfo Main { get; set; } = new();

        [JsonPropertyName("weather")]
        public List<WeatherInfo> Weather { get; set; } = new();

        [JsonPropertyName("wind")]
        public WindInfo Wind { get; set; } = new();

        [JsonPropertyName("sys")]
        public SysInfo Sys { get; set; } = new();

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class MainInfo
    {
        [JsonPropertyName("temp")]
        public double Temperature { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }
    }

    public class WeatherInfo
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;

        public string IconUrl => $"https://openweathermap.org/img/wn/{Icon}@2x.png";
    }

    public class WindInfo
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }

    public class SysInfo
    {
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("sunrise")]
        public long Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public long Sunset { get; set; }
    }

    public class ForecastResponse
    {
        [JsonPropertyName("list")]
        public List<ForecastItem> List { get; set; } = new();

        [JsonPropertyName("city")]
        public CityInfo City { get; set; } = new();
    }

    public class ForecastItem
    {
        [JsonPropertyName("dt_txt")]
        public string DateText { get; set; } = string.Empty;

        [JsonPropertyName("main")]
        public MainInfo Main { get; set; } = new();

        [JsonPropertyName("weather")]
        public List<WeatherInfo> Weather { get; set; } = new();
    }

    public class CityInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
