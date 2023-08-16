// See https://aka.ms/new-console-template for more information
using KomootTourAnalyzer.Services;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

var config = ReadConfig();
var auth = new AuthenticationService(config);
var authResult = await auth.Login(config["KomootUsername"], config["KomootPassword"]);
Console.WriteLine(authResult);
var tourService = new TourLoaderService(auth.Cookies, config);
//var tours = await tourService.GetToursPaged(3, 0);
//Console.WriteLine(tours);
var summary = await tourService.GetSummarizedData(Console.WriteLine);
Console.WriteLine("Total distance [km]:       " + (summary?.DistanceInMeters / 1000.0 ?? 0).ToString("0.00"));
Console.WriteLine("Total elevation up [km]:   " + (summary?.ElevationUpInMeters / 1000.0 ?? 0).ToString("0.00"));
Console.WriteLine("Total elevation down [km]: " + (summary?.ElevationDownInMeters / 1000.0 ?? 0).ToString("0.00"));
Console.WriteLine("Total time in motion:      " + TimeSpan.FromSeconds(summary?.SecondsInMotion ?? 0));
Console.ReadLine();

static IConfiguration ReadConfig()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false);

    return builder.Build();
}
