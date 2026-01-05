using MyCorsApp.Models;

namespace MyCorsApp.Services;

public interface ISeoAnalyzer
{
    Task<List<SeoIssue>> AnalyzeAsync(string url);
}
