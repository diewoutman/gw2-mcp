using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class AccountTools
{
    private readonly Gw2ApiClient _client;

    public AccountTools(Gw2ApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Get account information for the authenticated user. Returns account ID, name, world, guilds, and creation date.")]
    public async Task<string> GetAccount(
        [Description("GW2 API key. If not provided, uses GW2_API_KEY environment variable")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync("v2/account", apiKey, cancellationToken);
    }

    [McpServerTool, Description("Get the contents of the account's bank. Returns array of item slots with id and count.")]
    public async Task<string> GetBank(
        [Description("GW2 API key. If not provided, uses GW2_API_KEY environment variable")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync("v2/account/bank", apiKey, cancellationToken);
    }

    [McpServerTool, Description("Get the contents of the account's material storage. Returns array of materials with id, category, and count.")]
    public async Task<string> GetMaterials(
        [Description("GW2 API key. If not provided, uses GW2_API_KEY environment variable")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync("v2/account/materials", apiKey, cancellationToken);
    }

    [McpServerTool, Description("Get information about the supplied API key. Returns key name, permissions, and ID.")]
    public async Task<string> GetTokenInfo(
        [Description("GW2 API key. If not provided, uses GW2_API_KEY environment variable")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync("v2/tokeninfo", apiKey, cancellationToken);
    }
}