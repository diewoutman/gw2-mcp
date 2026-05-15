using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Gw2Mcp;
using Gw2Mcp.Services;
using Gw2Mcp.Tools;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.AddHttpClient<Gw2ApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.guildwars2.com/");
    client.DefaultRequestHeaders.Add("User-Agent", "gw2-mcp/0.2.0");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<WikiClient>(client =>
{
    client.BaseAddress = new Uri("https://wiki.guildwars2.com/");
    client.DefaultRequestHeaders.Add("User-Agent", "gw2-mcp/0.2.0");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddMemoryCache();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();