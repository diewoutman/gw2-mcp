using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Gw2Mcp;

public class Gw2ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string? _defaultApiKey;

    public Gw2ApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _defaultApiKey = configuration["GW2_API_KEY"];
    }

    public async Task<string> GetAsync(string endpoint, string? apiKey = null, CancellationToken cancellationToken = default)
    {
        var effectiveKey = apiKey ?? _defaultApiKey;

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        if (!string.IsNullOrEmpty(effectiveKey))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", effectiveKey);
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var errorJson = JsonSerializer.Serialize(new
            {
                error = $"GW2 API returned {(int)response.StatusCode}: {response.ReasonPhrase}",
                details = errorBody
            });
            return errorJson;
        }

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string> GetPaginatedAsync(string endpoint, int page, int? pageSize = null, string? apiKey = null, CancellationToken cancellationToken = default)
    {
        var separator = endpoint.Contains('?') ? '&' : '?';
        var url = $"{endpoint}{separator}page={page}";
        if (pageSize.HasValue)
        {
            url += $"&page_size={pageSize.Value}";
        }

        return await GetAsync(url, apiKey, cancellationToken);
    }

    public async Task<string> GetWithIdsAsync(string endpoint, string? ids = null, string? lang = null, string? apiKey = null, CancellationToken cancellationToken = default)
    {
        var separator = endpoint.Contains('?') ? '&' : '?';
        var url = endpoint;

        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(ids))
            queryParams.Add($"ids={Uri.EscapeDataString(ids)}");
        if (!string.IsNullOrEmpty(lang))
            queryParams.Add($"lang={Uri.EscapeDataString(lang)}");

        if (queryParams.Count > 0)
            url += separator + string.Join("&", queryParams);

        return await GetAsync(url, apiKey, cancellationToken);
    }

    public async Task<string> GetPaginatedWithIdsAsync(string endpoint, int page, int? pageSize = null, string? ids = null, string? lang = null, string? apiKey = null, CancellationToken cancellationToken = default)
    {
        var separator = endpoint.Contains('?') ? '&' : '?';
        var url = $"{endpoint}{separator}page={page}";
        if (pageSize.HasValue)
            url += $"&page_size={pageSize.Value}";
        if (!string.IsNullOrEmpty(ids))
            url += $"&ids={Uri.EscapeDataString(ids)}";
        if (!string.IsNullOrEmpty(lang))
            url += $"&lang={Uri.EscapeDataString(lang)}";

        return await GetAsync(url, apiKey, cancellationToken);
    }
}