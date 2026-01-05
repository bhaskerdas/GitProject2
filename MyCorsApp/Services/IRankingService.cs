using MyCorsApp.Models;

namespace MyCorsApp.Services;

public interface IRankingService
{
    Task<RankingData> GetRankingAsync(string url);
}
