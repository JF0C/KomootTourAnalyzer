// See https://aka.ms/new-console-template for more information
using KomootTourAnalyzer.Services;
using Microsoft.Extensions.Configuration;

Console.WriteLine(TimePrefix() + "Starting Tour Analyzer");

var config = ReadConfig();
var auth = new AuthenticationService(config);
var authResult = await auth.Login();
Console.WriteLine(TimePrefix() + authResult);
var tourService = new TourLoaderService(auth, config);
//var tours = await tourService.GetToursPaged(3, 0);
//Console.WriteLine(tours);
var summary = await tourService.GetSummarizedData(m => Console.WriteLine(TimePrefix() + m));
Console.WriteLine(TimePrefix() + "Total distance [km]:       " + (summary?.DistanceInMeters / 1000.0 ?? 0).ToString("0.00"));
Console.WriteLine(TimePrefix() + "Total elevation up [km]:   " + (summary?.ElevationUpInMeters / 1000.0 ?? 0).ToString("0.00"));
Console.WriteLine(TimePrefix() + "Total elevation down [km]: " + (summary?.ElevationDownInMeters / 1000.0 ?? 0).ToString("0.00"));
Console.WriteLine(TimePrefix() + "Total time in motion:      " + TimeSpan.FromSeconds(summary?.SecondsInMotion ?? 0));
Console.ReadLine();

static IConfiguration ReadConfig()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile("appsettings.local.json", optional: true);
    return builder.Build();
}

static string TimePrefix()
{
    return $"[{DateTime.Now.ToString("HH:mm:ss.fff")}]: ";
}