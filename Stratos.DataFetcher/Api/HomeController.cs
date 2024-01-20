using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stratos.DataFetcher.Data;
using Stratos.DataFetcher.Pages;
using Stratos.DataFetcher.Processes;

namespace Stratos.DataFetcher.Api
{
    [ApiController]
    [Route("/api/changestate")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult changestate([FromBody] string body)
        {
            WeatherFetcherStatus.currentCity = body;

            var jsonConverted = JsonConvert.DeserializeObject<RequestBody>(body);

            if (jsonConverted.State == true)
            {
                WeatherFetcherStatus.isRunning = true;
                if (WeatherGetter.isTimerRunning == true)
                {
                    return Ok();
                }
                else
                {
                    WeatherGetter.aTimer.Start();
                    WeatherGetter.isTimerRunning = true;
                    WeatherFetcherStatus.isRunning = true;
                    return Ok();
                }
            }
            else if (jsonConverted.State == false)
            {
                if (WeatherGetter.isTimerRunning == false)
                {
                    return Ok();
                }
                else 
                {
                    WeatherGetter.isTimerRunning = false;
                    WeatherFetcherStatus.isRunning = false;
                    WeatherGetter.aTimer.Stop();
                }
            }

            return Ok();
        }
    }
}
