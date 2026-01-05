namespace MyCorsApp.Models;

public class SeoReport
{
    public string Url { get; set; } = string.Empty;
    public RankingData Ranking { get; set; } = new();
    public List<SeoIssue> Issues { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}

public class RankingData
{
    public string GoogleRanking { get; set; } = "N/A";
    public string AlexaRanking { get; set; } = "N/A";
}

public class SeoIssue
{
    public string Severity { get; set; } = "Info"; // Error, Warning, Info
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
