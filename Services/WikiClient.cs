using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace Gw2Mcp.Services;

public class WikiClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheTTL = TimeSpan.FromHours(24);

    public WikiClient(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<string> SearchAsync(string query, int limit, CancellationToken cancellationToken = default)
    {
        var normalizedQuery = query.Trim().ToLowerInvariant();
        var cacheKey = $"wiki:search:{normalizedQuery}:{limit}";

        if (_cache.TryGetValue(cacheKey, out string? cached))
            return cached!;

        var searchUrl = $"api.php?action=query&format=json&list=search&srsearch={Uri.EscapeDataString(query)}&srlimit={limit}&srprop=size|wordcount|timestamp|snippet";

        using var searchRequest = new HttpRequestMessage(HttpMethod.Get, searchUrl);
        using var searchResponse = await _httpClient.SendAsync(searchRequest, cancellationToken);
        searchResponse.EnsureSuccessStatusCode();

        var searchJson = await searchResponse.Content.ReadAsStringAsync(cancellationToken);
        using var searchDoc = JsonDocument.Parse(searchJson);

        var resultsArray = searchDoc.RootElement
            .GetProperty("query").GetProperty("search");

        var results = new List<Dictionary<string, JsonElement>>();
        foreach (var item in resultsArray.EnumerateArray())
        {
            var title = item.GetProperty("title").GetString()!;
            var extract = await GetPageExtractAsync(title, cancellationToken);

            results.Add(new Dictionary<string, JsonElement>
            {
                ["title"] = item.GetProperty("title"),
                ["snippet"] = item.GetProperty("snippet"),
                ["pageid"] = item.GetProperty("pageid"),
                ["size"] = item.GetProperty("size"),
                ["wordcount"] = item.GetProperty("wordcount"),
                ["timestamp"] = item.GetProperty("timestamp"),
                ["url"] = JsonSerializer.Deserialize<JsonElement>($"\"https://wiki.guildwars2.com/wiki/{Uri.EscapeDataString(title.Replace(' ', '_'))}\""),
                ["extract"] = JsonSerializer.Deserialize<JsonElement>($"\"{JsonEncodedText.Encode(extract ?? "")}\""),
            });
        }

        var result = JsonSerializer.Serialize(new
        {
            query,
            total = results.Count,
            results = results.Select(r => new
            {
                title = r["title"].GetString(),
                snippet = CleanSnippet(r["snippet"].GetString() ?? ""),
                url = r["url"].GetString(),
                pageid = r["pageid"].GetInt32(),
                size = r["size"].GetInt32(),
                wordcount = r["wordcount"].GetInt32(),
                timestamp = r["timestamp"].GetString(),
                extract = r["extract"].GetString()
            })
        }, new JsonSerializerOptions { WriteIndented = true });

        _cache.Set(cacheKey, result, CacheTTL);
        return result;
    }

    private async Task<string?> GetPageExtractAsync(string title, CancellationToken cancellationToken)
    {
        var cacheKey = $"wiki:page:{title}";

        if (_cache.TryGetValue(cacheKey, out string? cached))
            return cached;

        var extractUrl = $"api.php?action=query&format=json&prop=extracts&titles={Uri.EscapeDataString(title)}&exintro=true&explaintext=true&exchars=500";

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, extractUrl);
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            var pages = doc.RootElement.GetProperty("query").GetProperty("pages");
            foreach (var page in pages.EnumerateObject())
            {
                if (page.Value.TryGetProperty("extract", out var extractEl))
                {
                    var extract = extractEl.GetString() ?? "";
                    _cache.Set(cacheKey, extract, CacheTTL);
                    return extract;
                }
            }
        }
        catch
        {
            // Non-critical — return null if extract fails
        }

        return null;
    }

    private static string CleanSnippet(string snippet)
    {
        return snippet
            .Replace("<span class=\"searchmatch\">", "")
            .Replace("</span>", "")
            .Replace("&quot;", "\"")
            .Replace("&amp;", "&")
            .Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Trim();
    }
}