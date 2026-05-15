using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class WizardsVaultTool
{
    private readonly Gw2ApiClient _client;

    public WizardsVaultTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get Wizard's Vault listings and objectives.")]
    public async Task<string> Gw2WizardsVault(
        [Description("What to retrieve: 'listings' or 'objectives'")] string endpoint,
        [Description("Comma-separated IDs. Omit to list all.")] string? ids = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');
        var apiPath = normalized switch
        {
            "listings" => "v2/wizardsvault/listings",
            "objectives" => "v2/wizardsvault/objectives",
            _ => null
        };

        if (apiPath is null)
            return $"{{\"error\": \"Unknown endpoint '{endpoint}'. Valid: listings, objectives\"}}";

        return await _client.GetWithIdsAsync(apiPath, ids, apiKey: null, cancellationToken: cancellationToken);
    }
}