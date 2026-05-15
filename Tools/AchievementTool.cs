using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class AchievementTool
{
    private readonly Gw2ApiClient _client;

    public AchievementTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get achievement details, categories, daily achievements, or groups.")]
    public async Task<string> Gw2Achievement(
        [Description("What to retrieve: 'achievements', 'categories', 'daily', 'daily/tomorrow', or 'groups'")] string endpoint,
        [Description("Comma-separated IDs. Omit to list all.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        var apiPath = normalized switch
        {
            "achievements" => "v2/achievements",
            "categories" => "v2/achievements/categories",
            "daily" => "v2/achievements/daily",
            "daily/tomorrow" => "v2/achievements/daily/tomorrow",
            "groups" => "v2/achievements/groups",
            _ => null
        };

        if (apiPath is null)
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: achievements, categories, daily, daily/tomorrow, groups\"}}";

        if (normalized is "daily" or "daily/tomorrow")
            return await _client.GetAsync(apiPath, apiKey: null, cancellationToken: cancellationToken);

        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync(apiPath, page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync(apiPath, ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }
}