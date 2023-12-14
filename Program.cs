using M347_Data_Fetcher.Api;
using M347_Data_Fetcher.Data;
using M347_Data_Fetcher.Pages;
using Refit;
using MudBlazor.Services;
using Marten;
using Weasel.Core;
using System.Timers;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using JasperFx.CodeGeneration.Frames;
using MudBlazor.Charts;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using M347_Data_Fetcher.Processes;



//using IDocumentSession Session;
Console.WriteLine("Program started");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var gitHubApi = RestService.For<IWeatherApi>("https://api.weatherapi.com/");
builder.Services.AddSingleton(gitHubApi);
builder.Services.AddMudServices();
builder.Services.AddSingleton<TcpConnection>();

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

Console.WriteLine(builder.Configuration.GetConnectionString("Marten"));

var WeatherApi = app.Services.GetService<IWeatherApi>();

var Store = app.Services.GetService<IDocumentStore>();

//Default cityName
string cityName = "Gais AR";

Response? jsonConverted;

var tcp = new TcpConnection();

var thread = new ThreadStart(tcp.TcpConnectionInit);
var backgroundThread = new Thread(thread);
backgroundThread.Start();

setTimers();
async Task WeatherGet(string cityName)
{
    //TODO: create function to accept parameters from another GUI/User Interface
    string response = string.Empty;
    WeatherFetcherStatus.currentCity = cityName;

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
        Console.WriteLine(DateTime.Now.ToString() + "DB Insert with location: " + weather.Location.Name + " successfull" );
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

    aTimer = new System.Timers.Timer(10000);
    aTimer.Elapsed += OnTimedEvent;
    aTimer.Start();
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
