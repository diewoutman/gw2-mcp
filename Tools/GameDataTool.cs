using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class GameDataTool
{
    private readonly Gw2ApiClient _client;

    public GameDataTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get game data — professions, races, specializations, skills, traits, masteries, legends, pets, currencies, dungeons, raids, stories, titles, and more.")]
    public async Task<string> Gw2GameData(
        [Description("What to retrieve: currencies, professions, races, specializations, skills, traits, masteries, legends, pets, dungeons, raids, stories, seasons, titles, itemstats, dailycrafting, mapchests, worldbosses, quests, backstory/answers, backstory/questions")] string endpoint,
        [Description("Comma-separated IDs. Omit to list all.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("Language: en, de, es, fr, zh, ko")] string? lang = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        var apiPath = normalized switch
        {
            "currencies" => "v2/currencies",
            "professions" => "v2/professions",
            "races" => "v2/races",
            "specializations" => "v2/specializations",
            "skills" => "v2/skills",
            "traits" => "v2/traits",
            "masteries" => "v2/masteries",
            "legends" => "v2/legends",
            "pets" => "v2/pets",
            "dungeons" => "v2/dungeons",
            "raids" => "v2/raids",
            "stories" => "v2/stories",
            "seasons" => "v2/stories/seasons",
            "titles" => "v2/titles",
            "itemstats" => "v2/itemstats",
            "dailycrafting" => "v2/dailycrafting",
            "mapchests" => "v2/mapchests",
            "worldbosses" => "v2/worldbosses",
            "quests" => "v2/quests",
            "backstory/answers" => "v2/backstory/answers",
            "backstory/questions" => "v2/backstory/questions",
            _ => null
        };

        if (apiPath is null)
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: currencies, professions, races, specializations, skills, traits, masteries, legends, pets, dungeons, raids, stories, seasons, titles, itemstats, dailycrafting, mapchests, worldbosses, quests, backstory/answers, backstory/questions\"}}";

        if (page.HasValue && normalized is "skills" or "currencies" or "stories" or "itemstats")
            return await _client.GetPaginatedWithIdsAsync(apiPath, page.Value, ids: ids, lang: lang, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync(apiPath, ids, lang, apiKey: null, cancellationToken: cancellationToken);
    }
}