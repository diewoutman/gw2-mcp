# GW2 MCP Server

A [Model Context Protocol](https://modelcontextprotocol.io/) server for the [Guild Wars 2 API](https://api.guildwars2.com/), built with .NET 8.

Exposes 28 tools covering all major GW2 API endpoints — account info, characters, trading post, items, recipes, WvW, guilds, maps, and more.

## Installation

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build from source

```bash
git clone https://github.com/your-org/gw2-mcp.git
cd gw2-mcp
dotnet build --configuration Release
```

### Download release binary

Grab the latest release archive for your platform from the [Releases](https://github.com/your-org/gw2-mcp/releases) page:

| Platform | File |
|----------|------|
| Linux x64 | `gw2-mcp-{version}-linux-x64.tar.gz` |
| Linux ARM64 | `gw2-mcp-{version}-linux-arm64.tar.gz` |
| macOS x64 | `gw2-mcp-{version}-osx-x64.tar.gz` |
| macOS ARM64 | `gw2-mcp-{version}-osx-arm64.tar.gz` |
| Windows x64 | `gw2-mcp-{version}-win-x64.zip` |
| Windows ARM64 | `gw2-mcp-{version}-win-arm64.zip` |

Extract and make executable (Linux/macOS):

```bash
tar -xzf gw2-mcp-0.1.0-linux-x64.tar.gz
chmod +x gw2-mcp
```

### Configuring with OpenCode

Add the server to your OpenCode configuration (e.g. `~/.config/opencode/config.json` or your project-level `.opencode/config.json`):

```json
{
  "mcpServers": {
    "gw2": {
      "command": "/path/to/gw2-mcp",
      "env": {
        "GW2_API_KEY": "YOUR_API_KEY_HERE"
      }
    }
  }
}
```

If you prefer not to set the API key globally, you can omit the `env` block and pass `apiKey` as a parameter to any authenticated tool call instead.

After updating the config, restart OpenCode. The GW2 tools will be available automatically.

> **Tip:** To generate a GW2 API key, go to [https://account.arena.net/applications](https://account.arena.net/applications) and create a key with the permissions you need.

## Tools

### Account (requires API key)

| Tool | Description |
|------|-------------|
| `GetAccount` | Get account info — ID, name, world, guilds, creation date |
| `GetBank` | Get the contents of the account's bank |
| `GetMaterials` | Get the account's material storage |
| `GetTokenInfo` | Get information about the supplied API key (name, permissions) |

### Characters (requires API key)

| Tool | Description |
|------|-------------|
| `GetCharacters` | List all character names on the account |
| `GetCharacter` | Get details for a specific character — race, profession, level, equipment, bags |

### Commerce / Trading Post

| Tool | Description |
|------|-------------|
| `GetListings` | Get buy and sell listings for items |
| `GetPrices` | Get current buy/sell prices for items |
| `GetExchangeCoins` | Gem → coins exchange rate |
| `GetExchangeGems` | Coins → gem exchange rate |
| `GetTransactions` | Get current or historical buy/sell transactions (requires API key) |

### Items

| Tool | Description |
|------|-------------|
| `GetItems` | Get item details — name, icon, type, rarity, level, flags |
| `GetMaterials` | Get material category names and item IDs |
| `GetRecipes` | Get recipe details — type, output, ingredients, disciplines |
| `SearchRecipes` | Search recipes by input ingredient or output item ID |
| `GetSkins` | Get skin details — name, type, flags, icon |

### Maps

| Tool | Description |
|------|-------------|
| `GetContinents` | Get continent info — name, dimensions, floors |
| `GetMaps` | Get map details — name, level range, region, continent |

### World vs World

| Tool | Description |
|------|-------------|
| `GetMatches` | Get all currently running WvW matches |
| `GetMatchDetails` | Get scores and map details for a specific WvW match |
| `GetObjectiveNames` | Get localized WvW objective names |

### Guilds

| Tool | Description |
|------|-------------|
| `GetGuildById` | Look up a guild by its ID |
| `GetGuildByName` | Look up a guild by its name |

### Miscellaneous

| Tool | Description |
|------|-------------|
| `GetBuild` | Get the current GW2 build ID |
| `GetColors` | Get dye color info — names, RGB components |
| `GetFiles` | Get in-game asset file URLs (icons, maps, etc.) |
| `GetQuaggans` | Get quaggan image URLs |
| `GetWorlds` | Get world/server info — name, population |

## Common Parameters

Most tools share these optional parameters:

| Parameter | Description |
|-----------|-------------|
| `apiKey` | GW2 API key. Falls back to `GW2_API_KEY` env var if not provided |
| `ids` | Comma-separated IDs to filter results (e.g. `"19683,19709"`) |
| `page` | Page number for pagination (0-based) |
| `lang` | Language code: `en`, `de`, `es`, `fr`, `zh`, `ko` |

## Releasing

Push a branch named `release/{version}` (e.g. `release/1.0.0`) to trigger the GitHub Actions workflow. It will:

1. Build self-contained single-file binaries for all 6 platforms
2. Update the version in `Gw2Mcp.csproj`
3. Commit the version bump
4. Create a GitHub Release with all platform artifacts attached

## License

MIT