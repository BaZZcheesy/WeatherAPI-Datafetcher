using System.Timers;
using Marten;
using Newtonsoft.Json;
using Stratos.DataFetcher.Api;
using Stratos.DataFetcher.Data;
using Stratos.DataFetcher.Pages;

namespace Stratos.DataFetcher.Processes
{
    public class WeatherGetter
    {
        private IWeatherApi _weatherApi;
        private IDocumentStore _store;

        // Constructor DI (dependency injection)
        public WeatherGetter(IWeatherApi weatherapi, IDocumentStore store)
        {
            _weatherApi = weatherapi;
            _store = store;
        }
        
        public void setTimers()
        {
            System.Timers.Timer aTimer;

            aTimer = new System.Timers.Timer(10000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Start();
        }

        async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            LocationValidator.Validate();
            WeatherGet(LocationValidator.cityName);
        }

        async Task WeatherGet(string cityName)
        {
            Response? jsonConverted;

            string response = string.Empty;

            WeatherFetcherStatus.currentCity = cityName;


            try
            {
                //Fetch data from WeatherAPI
                response = await _weatherApi.GetWeather("1c39ca929b4b4d23a4364338231708", cityName);

                WeatherFetcherStatus.isRunning = true;
                jsonConverted = JsonConvert.DeserializeObject<Response>(response);

                // Open a session for querying, loading, and updating documents
                await using var session = _store.LightweightSession();

                var weather = new Response
                {
                    dateInserted = DateTime.Now.ToString(),
                    Current = jsonConverted.Current,
                    Location = jsonConverted.Location
                };

                //Insert the instanciated weather object into the database
                session.Store(weather);

                await session.SaveChangesAsync();

                //Changes variable that is needed to show the state of the service
                WeatherFetcherStatus.isRunning = true;

                //Console.WriteLine() for debugging and additional info
                Console.WriteLine(DateTime.Now.ToString() + "DB Insert with location: " + weather.Location.Name + " successfull");
            }
            catch (Exception e)
            {
                //Changes variable that is needed to show the state of the service
                WeatherFetcherStatus.isRunning = false;
            }
        }
    }
}
