
using KomootTourAnalyzer.Services;
using Microsoft.Extensions.Configuration;

namespace KomootTourAnalyzer;

public class Executor
{
    private readonly ITourLoader tourLoader;
    private readonly ITourAnalyzer tourAnalyzer;
    private readonly ITourStorage tourStorage;
    private readonly IConfiguration configuration;
    public Executor(IConfiguration configuration)
    {
        var authentication = new AuthenticationService(configuration);
        tourLoader = new TourLoader(configuration, authentication, null);
        tourAnalyzer = new TourAnalyzer(tourLoader, null);
        tourStorage = new TourStorage(tourLoader);
        this.configuration = configuration;
    }

    public async Task Execute(){
        await tourAnalyzer.Analyze();
        tourAnalyzer.Print();
        await tourStorage.SaveTours(configuration["JsonSaveFilePath"] ?? throw new KeyNotFoundException("JsonSaveFilePath must be set"));
        await tourStorage.SaveToursAsCsv(configuration["CsvSaveFilePath"] ?? throw new KeyNotFoundException("JsonSaveFilePath must be set"));
    }
}