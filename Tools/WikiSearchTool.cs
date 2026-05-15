using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class WikiSearchTool
{
    private readonly WikiClient _client;

    public WikiSearchTool(WikiClient client) => _client = client;

    [McpServerTool, Description("Search the Guild Wars 2 wiki for information about game content — items, mechanics, guides, lore, and more.")]
    public async Task<string> Gw2WikiSearch(
        [Description("Search query for wiki content (e.g. 'Dragon Bash', 'currencies', 'legendary weapons')")] string query,
        [Description("Maximum number of results to return (default: 5)")] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        return await _client.SearchAsync(query, limit, cancellationToken);
    }
}