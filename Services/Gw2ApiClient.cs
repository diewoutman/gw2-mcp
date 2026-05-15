using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Gw2Mcp.Services;

public class Gw2ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly string? _defaultApiKey;
    private readonly SemaphoreSlim _rateLimitSemaphore = new(10, 10);
    private static readonly TimeSpan StaticCacheTTL = TimeSpan.FromHours(24);
    private static readonly TimeSpan DynamicCacheTTL = TimeSpan.FromMinutes(5);

    public Gw2ApiClient(HttpClient httpClient, IMemoryCache cache, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _cache = cache;
        _defaultApiKey = configuration["GW2_API_KEY"];
    }

    public async Task<string> GetAsync(string endpoint, string? apiKey = null, CancellationToken cancellationToken = default)
    {
        var effectiveKey = apiKey ?? _defaultApiKey;
        var cacheKey = BuildCacheKey(endpoint, effectiveKey);

        if (_cache.TryGetValue(cacheKey, out string? cached))
            return cached!;

        await _rateLimitSemaphore.WaitAsync(cancellationToken);
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            if (!string.IsNullOrEmpty(effectiveKey))
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", effectiveKey);

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Serialize(new
                {
                    error = $"GW2 API returned {(int)response.StatusCode}: {response.ReasonPhrase}",
                    details = errorBody
                });
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var ttl = IsStaticEndpoint(endpoint) ? StaticCacheTTL : DynamicCacheTTL;
            _cache.Set(cacheKey, body, ttl);
            return body;
        }
        finally
        {
            _rateLimitSemaphore.Release();
        }
    }

    public async Task<string> GetWithIdsAsync(string endpoint, string? ids = null, string? lang = null, string? apiKey = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrlWithParams(endpoint, ids, lang);
        return await GetAsync(url, apiKey, cancellationToken);
    }

    public async Task<string> GetPaginatedWithIdsAsync(string endpoint, int page, int? pageSize = null, string? ids = null, string? lang = null, string? apiKey = null, CancellationToken cancellationToken = default)
    {
        var separator = endpoint.Contains('?') ? '&' : '?';
        var url = $"{endpoint}{separator}page={page}";
        if (pageSize.HasValue) url += $"&page_size={pageSize.Value}";
        if (!string.IsNullOrEmpty(ids)) url += $"&ids={Uri.EscapeDataString(ids)}";
        if (!string.IsNullOrEmpty(lang)) url += $"&lang={Uri.EscapeDataString(lang)}";

        return await GetAsync(url, apiKey, cancellationToken);
    }

    private static string BuildCacheKey(string endpoint, string? apiKey)
    {
        var keyHash = apiKey != null ? Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(apiKey)))[..8] : "anon";
        return $"gw2:{keyHash}:{endpoint}";
    }

    private static string BuildUrlWithParams(string endpoint, string? ids, string? lang)
    {
        var separator = endpoint.Contains('?') ? '&' : '?';
        var url = endpoint;
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(ids)) queryParams.Add($"ids={Uri.EscapeDataString(ids)}");
        if (!string.IsNullOrEmpty(lang)) queryParams.Add($"lang={Uri.EscapeDataString(lang)}");
        if (queryParams.Count > 0) url += separator + string.Join("&", queryParams);
        return url;
    }

    private static bool IsStaticEndpoint(string endpoint)
    {
        var staticSegments = new[] { "v2/build", "v2/colors", "v2/currencies", "v2/professions",
            "v2/races", "v2/specializations", "v2/traits", "v2/skills", "v2/masteries",
            "v2/materials", "v2/items", "v2/skins", "v2/minis", "v2/outfits", "v2/gliders",
            "v2/finishers", "v2/novelties", "v2/emotes", "v2/dungeons", "v2/raids",
            "v2/titles", "v2/wvw/abilities", "v2/wvw/ranks", "v2/wvw/upgrades",
            "v2/pvp/amulets", "v2/pvp/ranks", "v2/pvp/heroes", "v2/pvp/seasons",
            "v2/worlds", "v2/maps", "v2/continents", "v2/files", "v2/quaggans",
            "v2/itemstats", "v2/stories", "v2/quests", "v2/pets", "v2/legends",
            "v2/recipes", "v2/guild/permissions", "v2/guild/upgrades",
            "v2/homestead", "v2/home", "v2/backstory" };

        return staticSegments.Any(s => endpoint.StartsWith(s));
    }
}