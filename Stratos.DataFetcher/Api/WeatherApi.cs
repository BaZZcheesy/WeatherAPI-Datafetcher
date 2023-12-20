using Refit;

namespace Stratos.DataFetcher.Api
{
    public interface IWeatherApi
    {
        [Get("/v1/current.json?key={key}&q={location}")]
        Task<string> GetWeather(string key, string location);
    }
}
