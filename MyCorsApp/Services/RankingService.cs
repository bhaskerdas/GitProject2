using MyCorsApp.Models;

namespace MyCorsApp.Services;

public class RankingService : IRankingService
{
    public Task<RankingData> GetRankingAsync(string url)
    {
        // Alexa is retired.
        // Google Ranking requires a paid API (e.g., SERP API) or scraping which is not reliable/allowed here.
        // We return simulated data for demonstration.

        var data = new RankingData
        {
            AlexaRanking = "Retired (Service Discontinued May 2022)",
            GoogleRanking = "Simulated: Top 10"
        };

        return Task.FromResult(data);
    }
}
