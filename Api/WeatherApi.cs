using M347_Data_Fetcher.Data;
using Refit;

namespace M347_Data_Fetcher.Api
{
    public interface IWeatherApi
    {
        [Get("/v1/current.json?key={key}&q={location}")]
        Task<string> GetWeather(string key, string location);
    }
}
