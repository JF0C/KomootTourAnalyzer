
using KomootTourAnalyzer.Services;
using Microsoft.Extensions.Configuration;

namespace KomootTourAnalyzer;

public class Executor
{
    private readonly ITourLoader tourLoader;
    private readonly IConfiguration configuration;
    private readonly ITourAnalyzer tourAnalyzer;
    public Executor(IConfiguration configuration)
    {
        this.configuration = configuration;
        var authentication = new AuthenticationService(configuration);
        tourLoader = new TourLoader(configuration, authentication);
        tourAnalyzer = new TourAnalyzer(tourLoader, null);
    }

    public async Task Execute(){
        await tourAnalyzer.Analyze();
        tourAnalyzer.Print();
    }
}