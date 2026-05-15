using System.ComponentModel;
using ModelContextProtocol.Server;
using Gw2Mcp.Services;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class CommerceTool
{
    private readonly Gw2ApiClient _client;

    public CommerceTool(Gw2ApiClient client) => _client = client;

    [McpServerTool, Description("Get trading post prices, listings, gem exchange rates, or account transactions.")]
    public async Task<string> Gw2Commerce(
        [Description("What to retrieve: 'prices', 'listings', 'exchange/gems', 'exchange/coins', 'transactions', or 'delivery'")] string endpoint,
        [Description("For exchange: quantity of gems or coins. For transactions: 'current/history/buys/sells' format, e.g. 'current/buys'")] string? param = null,
        [Description("Comma-separated item IDs, e.g. '19683,19709'")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        [Description("GW2 API key. Required for transactions and delivery.")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = endpoint.ToLowerInvariant().Trim('/');

        return normalized switch
        {
            "prices" => page.HasValue
                ? await _client.GetPaginatedWithIdsAsync("v2/commerce/prices", page.Value, ids: ids, apiKey: null, cancellationToken: cancellationToken)
                : await _client.GetWithIdsAsync("v2/commerce/prices", ids, apiKey: null, cancellationToken: cancellationToken),

            "listings" => page.HasValue
                ? await _client.GetPaginatedWithIdsAsync("v2/commerce/listings", page.Value, ids: ids, apiKey: null, cancellationToken: cancellationToken)
                : await _client.GetWithIdsAsync("v2/commerce/listings", ids, apiKey: null, cancellationToken: cancellationToken),

            "exchange/gems" when int.TryParse(param, out var qty) =>
                await _client.GetAsync($"v2/commerce/exchange/gems?quantity={qty}", apiKey: null, cancellationToken: cancellationToken),

            "exchange/coins" when int.TryParse(param, out var qty) =>
                await _client.GetAsync($"v2/commerce/exchange/coins?quantity={qty}", apiKey: null, cancellationToken: cancellationToken),

            "transactions" when !string.IsNullOrEmpty(param) =>
                await _client.GetAsync($"v2/commerce/transactions/{param}", apiKey, cancellationToken),

            "delivery" =>
                await _client.GetAsync("v2/commerce/delivery", apiKey, cancellationToken),

            _ => $"{{\"error\": \"Unknown commerce endpoint '{endpoint}'. Valid: prices, listings, exchange/gems, exchange/coins, transactions, delivery\"}}"
        };
    }
}