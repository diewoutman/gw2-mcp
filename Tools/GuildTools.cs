using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class GuildTools
{
    private readonly Gw2ApiClient _client;

    public GuildTools(Gw2ApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Get guild details by guild ID. Returns guild name, tag, and emblem info.")]
    public async Task<string> GetGuildById(
        [Description("The guild ID to look up")] string guildId,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync($"v1/guild_details?guild_id={Uri.EscapeDataString(guildId)}", apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get guild details by guild name. Returns guild ID, name, tag, and emblem info.")]
    public async Task<string> GetGuildByName(
        [Description("The guild name to look up")] string guildName,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync($"v1/guild_details?guild_name={Uri.EscapeDataString(guildName)}", apiKey: null, cancellationToken: cancellationToken);
    }
}