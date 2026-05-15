using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Gw2Mcp.Tools;

[McpServerToolType]
public class CommerceTools
{
    private readonly Gw2ApiClient _client;

    public CommerceTools(Gw2ApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Get current buy and sell listings from the trading post for specific items.")]
    public async Task<string> GetListings(
        [Description("Comma-separated item IDs, e.g. '19683,19709'. Omit to get all available IDs.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        CancellationToken cancellationToken = default)
    {
        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync("v2/commerce/listings", page.Value, ids: ids, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync("v2/commerce/listings", ids, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get current buy and sell prices for items on the trading post.")]
    public async Task<string> GetPrices(
        [Description("Comma-separated item IDs, e.g. '19683,19709'. Omit to get all available IDs.")] string? ids = null,
        [Description("Page number for pagination (0-based)")] int? page = null,
        CancellationToken cancellationToken = default)
    {
        if (page.HasValue)
            return await _client.GetPaginatedWithIdsAsync("v2/commerce/prices", page.Value, ids: ids, apiKey: null, cancellationToken: cancellationToken);

        return await _client.GetWithIdsAsync("v2/commerce/prices", ids, apiKey: null, cancellationToken: cancellationToken);
    }

    [McpServerTool, Description("Get the gem exchange rate when selling gems for coins. Returns coins_per_gem and total coins received.")]
    public async Task<string> GetExchangeCoins(
        [Description("The amount of gems to exchange")] int quantity,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync($"v2/commerce/exchange/gems?quantity={quantity}", apiKey: null, cancellationToken);
    }

    [McpServerTool, Description("Get the gem exchange rate when buying gems with coins. Returns coins_per_gem and total gems received.")]
    public async Task<string> GetExchangeGems(
        [Description("The amount of coins to exchange")] int quantity,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync($"v2/commerce/exchange/coins?quantity={quantity}", apiKey: null, cancellationToken);
    }

    [McpServerTool, Description("Get current or historical trading post transactions for the authenticated account.")]
    public async Task<string> GetTransactions(
        [Description("Transaction timeframe: 'current' or 'history'")] string timeframe,
        [Description("Transaction direction: 'buys' or 'sells'")] string direction,
        [Description("GW2 API key. If not provided, uses GW2_API_KEY environment variable")] string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.GetAsync($"v2/commerce/transactions/{timeframe}/{direction}", apiKey, cancellationToken);
    }
}