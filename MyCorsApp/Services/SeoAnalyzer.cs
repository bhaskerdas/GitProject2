using HtmlAgilityPack;
using MyCorsApp.Models;

namespace MyCorsApp.Services;

public class SeoAnalyzer : ISeoAnalyzer
{
    private readonly HttpClient _httpClient;

    public SeoAnalyzer(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SeoIssue>> AnalyzeAsync(string url)
    {
        var issues = new List<SeoIssue>();

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                issues.Add(new SeoIssue
                {
                    Severity = "Error",
                    Type = "Broken Link",
                    Description = $"URL returned status code: {response.StatusCode}"
                });
                return issues;
            }

            var content = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            // 1. Check Title
            var titleNode = doc.DocumentNode.SelectSingleNode("//title");
            if (titleNode == null || string.IsNullOrWhiteSpace(titleNode.InnerText))
            {
                issues.Add(new SeoIssue
                {
                    Severity = "Error",
                    Type = "Missing Title",
                    Description = "The page does not have a <title> tag."
                });
            }
            else if (titleNode.InnerText.Length > 60)
            {
                issues.Add(new SeoIssue
                {
                    Severity = "Warning",
                    Type = "Title Length",
                    Description = $"Title is too long ({titleNode.InnerText.Length} chars). Recommended max is 60."
                });
            }

            // 2. Check Meta Description
            var metaDesc = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");
            if (metaDesc == null || string.IsNullOrWhiteSpace(metaDesc.GetAttributeValue("content", "")))
            {
                issues.Add(new SeoIssue
                {
                    Severity = "Error",
                    Type = "Missing Meta Description",
                    Description = "The page does not have a meta description."
                });
            }
            else
            {
                var len = metaDesc.GetAttributeValue("content", "").Length;
                if (len > 160)
                {
                    issues.Add(new SeoIssue
                    {
                        Severity = "Warning",
                        Type = "Meta Description Length",
                        Description = $"Meta description is too long ({len} chars). Recommended max is 160."
                    });
                }
            }

            // 3. Check H1
            var h1Nodes = doc.DocumentNode.SelectNodes("//h1");
            if (h1Nodes == null || h1Nodes.Count == 0)
            {
                issues.Add(new SeoIssue
                {
                    Severity = "Error",
                    Type = "Missing H1",
                    Description = "The page does not have an <h1> tag."
                });
            }
            else if (h1Nodes.Count > 1)
            {
                issues.Add(new SeoIssue
                {
                    Severity = "Warning",
                    Type = "Multiple H1",
                    Description = $"The page has {h1Nodes.Count} <h1> tags. It is recommended to have only one."
                });
            }

            // 4. Check Images for Alt text
            var imgNodes = doc.DocumentNode.SelectNodes("//img");
            if (imgNodes != null)
            {
                foreach (var img in imgNodes)
                {
                    if (string.IsNullOrWhiteSpace(img.GetAttributeValue("alt", "")))
                    {
                        var src = img.GetAttributeValue("src", "unknown");
                        issues.Add(new SeoIssue
                        {
                            Severity = "Warning",
                            Type = "Missing Image Alt",
                            Description = $"Image with src '{src}' is missing an alt attribute."
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            issues.Add(new SeoIssue
            {
                Severity = "Error",
                Type = "Crawl Error",
                Description = $"Failed to crawl URL: {ex.Message}"
            });
        }

        return issues;
    }
}
