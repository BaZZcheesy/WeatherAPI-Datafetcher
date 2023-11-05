using M347_Data_Fetcher.Api;
using M347_Data_Fetcher.Data;
using Refit;
using MudBlazor.Services;
using Marten;
using Weasel.Core;
using System.Timers;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using JasperFx.CodeGeneration.Frames;
using MudBlazor.Charts;
using M347_Data_Fetcher.Pages;



//using IDocumentSession Session;
Console.WriteLine("Program started");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var gitHubApi = RestService.For<IWeatherApi>("https://api.weatherapi.com/");
builder.Services.AddSingleton(gitHubApi);
builder.Services.AddMudServices();


// This is the absolute, simplest way to integrate Marten into your
// .NET application with Marten's default configuration
builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


var WeatherApi = app.Services.GetService<IWeatherApi>();

var Store = app.Services.GetService<IDocumentStore>();

Response? jsonConverted;

//Default cityName
string cityName = "Gais AR";

setTimers();
async Task WeatherGet(string cityName)
{
    //TODO: create function to accept parameters from another GUI/User Interface
    string response = string.Empty;

    //TODO: Improve exception handling

    try
    {
        //Fetch data from WeatherAPI
        response = await WeatherApi.GetWeather("1c39ca929b4b4d23a4364338231708", cityName);

        WeatherFetcherStatus.isRunning = true;
        jsonConverted = JsonConvert.DeserializeObject<Response>(response);

        // Open a session for querying, loading, and updating documents
        await using var session = Store.LightweightSession();

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
        Console.WriteLine(DateTime.Now.ToString() + "DB Insert with location: " + weather.Location.Name + " sucsessfull" );
    }
    catch (Exception e)
    {
        //Changes variable that is needed to show the state of the service
        WeatherFetcherStatus.isRunning = false;
    }
}



void setTimers()
{
    System.Timers.Timer aTimer;

    aTimer = new System.Timers.Timer(5000);
    aTimer.Elapsed += OnTimedEvent;
    aTimer.Start();
    Console.WriteLine("");
}

async void OnTimedEvent(Object source, ElapsedEventArgs e)
{
    await WeatherGet(cityName);
}

//TODO: Setup TCP listener

//Method to check if the Location got sent by another Interface
//bool locationFromGUI()
//{
//    return true;
//}

//bool locationNotFound()
//{
//    if(response)
//}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
