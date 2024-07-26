// See https://aka.ms/new-console-template for more information
using KomootTourAnalyzer;
using KomootTourAnalyzer.Services;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Starting Tour Analyzer");

var config = ReadConfig();
var executor = new Executor(config);
await executor.Execute();
Console.ReadLine();

static IConfiguration ReadConfig()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile("appsettings.local.json", optional: true);
    return builder.Build();
}
