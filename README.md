# GW2 MCP Server

A [Model Context Protocol](https://modelcontextprotocol.io/) server for the [Guild Wars 2 API](https://api.guildwars2.com/) and [GW2 Wiki](https://wiki.guildwars2.com/), built with .NET 10.

18 parameterized tools covering the full GW2 API (~150 endpoints) plus wiki search, with built-in caching and rate limiting.

## Features

- **18 MCP tools** covering ~150 API endpoints via parameterized `endpoint` selection
- **Wiki search** — query the GW2 wiki directly
- **Tiered caching** — static data cached 24h, dynamic data 5min, wiki 24h
- **Rate limiting** — semaphore-based concurrency control (10 concurrent requests)
- **Cross-platform** — self-contained single-file binaries for Linux, macOS, Windows

## Installation

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (for building from source)

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
tar -xzf gw2-mcp-0.2.0-linux-x64.tar.gz
chmod +x gw2-mcp
```

### Configuring with OpenCode

Add the server to your OpenCode configuration. For global config use `~/.config/opencode/opencode.json`, for per-project config use `.opencode/opencode.json`:

```json
{
  "mcp": {
    "gw2": {
      "type": "local",
      "command": ["/absolute/path/to/gw2-mcp"],
      "environment": {
        "GW2_API_KEY": "YOUR_API_KEY_HERE"
      }
    }
  }
}
```

Key points:
- **`type`** must be `"local"` for stdio-based servers
- **`command`** must be an **array** of strings — first element is the binary path, subsequent elements are arguments
- **`environment`** is optional — omit it and pass `apiKey` per tool call instead

Restart OpenCode after updating the config. Verify with `/mcps` — the `gw2` server should appear.

> **Tip:** To generate a GW2 API key, go to [https://account.arena.net/applications](https://account.arena.net/applications) and create a key with the permissions you need.

## Tools

### `Gw2Account`
Access account data. `endpoint` selects what to retrieve:
- `account`, `bank`, `materials`, `achievements`, `dyes`, `minis`, `skins`, `wallet`, `titles`, `raids`, `masteries`, `recipes`, `finishers`, `outfits`, `luck`, `wvw`, `inventory`, `buildstorage`, `dailycrafting`, `dungeons`, `gliders`, `emotes`, `mailcarriers`, `mapchests`, `mastery/points`, `mounts/skins`, `mounts/types`, `novelties`, `pvp/heroes`, `legendaryarmory`, `worldbosses`, `homestead/decorations`, `homestead/glyphs`, `wizardsvault/daily`, `wizardsvault/weekly`, `wizardsvault/special`, `wizardsvault/listings`, `home/cats`, `home/nodes`, `skiffs`, `jadebots`
- All require `apiKey` (or `GW2_API_KEY` env var)

### `Gw2TokenInfo`
Get API key information — name, permissions, ID. Requires `apiKey`.

### `Gw2Characters`
List all character names. Requires `apiKey`.

### `Gw2Character`
Get character details. `endpoint` selects: `full` (default), `backstory`, `buildtabs`, `buildtabs/active`, `core`, `crafting`, `equipment`, `equipmenttabs`, `equipmenttabs/active`, `heropoints`, `inventory`, `quests`, `recipes`, `skills`, `specializations`, `training`. Requires `apiKey`.

### `Gw2Commerce`
Trading post data. `endpoint` selects: `prices`, `listings`, `exchange/gems`, `exchange/coins`, `transactions`, `delivery`. Use `param` for gem/coin quantities or transaction type (e.g. `current/buys`).

### `Gw2Item`
Item data. `endpoint` selects: `items`, `skins`, `materials`, `itemstats`. Supports `ids`, `page`, `lang`.

### `Gw2Recipe`
Recipe data. `endpoint`: `recipes` for details, `search` to find by `input` or `output` item ID.

### `Gw2GuildSearch`
Search guilds by name.

### `Gw2Guild`
Guild data. `endpoint` selects: `details`, `log`, `members`, `ranks`, `stash`, `storage`, `treasury`, `teams`, `upgrades`. Most require `apiKey` with guild permissions.

### `Gw2GuildData`
Guild metadata. `endpoint`: `permissions` or `upgrades`.

### `Gw2Wvw`
WvW data. `endpoint`: `matches`, `abilities`, `objectives`, `ranks`, `upgrades`, `rewardtracks`, `timers`. Supports `ids`, `lang`.

### `Gw2WvwMatchDetail`
Match details. `detail` selects: `overview`, `scores`, `stats`. Requires `worldId`.

### `Gw2WvwObjectiveNames`
Localized WvW objective names (v1 endpoint). Supports `lang`.

### `Gw2Pvp`
PvP game data. `endpoint`: `amulets`, `heroes`, `ranks`, `rewardtracks`, `runes`, `sigils`, `seasons`. Supports `ids`, `lang`.

### `Gw2PvpAccount`
Authenticated PvP data. `endpoint`: `games`, `standings`, `stats`. Requires `apiKey`.

### `Gw2Achievement`
Achievement data. `endpoint`: `achievements`, `categories`, `daily`, `daily/tomorrow`, `groups`. Supports `ids`, `page`, `lang`.

### `Gw2Cosmetic`
Cosmetic items. `endpoint`: `minis`, `outfits`, `gliders`, `finishers`, `novelties`, `emotes`, `mailcarriers`, `jadebots`, `skiffs`, `mounts/skins`, `mounts/types`. Supports `ids`, `page`, `lang`.

### `Gw2GameData`
General game data. `endpoint`: `currencies`, `professions`, `races`, `specializations`, `skills`, `traits`, `masteries`, `legends`, `pets`, `dungeons`, `raids`, `stories`, `seasons`, `titles`, `itemstats`, `dailycrafting`, `mapchests`, `worldbosses`, `quests`, `backstory/answers`, `backstory/questions`. Supports `ids`, `page`, `lang`.

### `Gw2Map`
Map data. `endpoint`: `continents`, `maps`. Supports `ids`, `page`, `lang`.

### `Gw2Homestead`
Homestead and home instance data. `endpoint`: `decorations`, `categories`, `glyphs`, `cats`, `nodes`. Supports `ids`, `page`, `lang`.

### `Gw2WizardsVault`
Wizard's Vault data. `endpoint`: `listings`, `objectives`. Supports `ids`.

### `Gw2WikiSearch`
Search the GW2 wiki. Pass `query` and optional `limit` (default 5). Returns titles, snippets, page URLs, and 500-character extracts.

### `Gw2Build`
Current GW2 build ID. No parameters.

### `Gw2Misc`
Miscellaneous data. `endpoint`: `colors`, `files`, `quaggans`, `worlds`, `subtoken`. Supports `ids`, `page`, `lang`. `subtoken` requires `apiKey`.

## Common Parameters

| Parameter | Description |
|-----------|-------------|
| `endpoint` | Selects which API resource to query (each tool lists valid values) |
| `apiKey` | GW2 API key. Falls back to `GW2_API_KEY` env var if not provided |
| `ids` | Comma-separated IDs to filter results (e.g. `"19683,19709"`) |
| `page` | Page number for pagination (0-based) |
| `lang` | Language code: `en`, `de`, `es`, `fr`, `zh`, `ko` |
| `name` | Character name (for character tools) |
| `guildId` | Guild ID (for guild tools) |
| `query` | Search query (for wiki search) |
| `limit` | Max results to return (for wiki search, default 5) |

## Caching

| Data type | TTL | Examples |
|-----------|-----|---------|
| Static (items, currencies, professions...) | 24 hours | `/v2/items`, `/v2/professions`, `/v2/worlds` |
| Dynamic (account, wallet, transactions...) | 5 minutes | `/v2/account`, `/v2/commerce/prices` |
| Wiki content | 24 hours | Search results, page extracts |

## Releasing

Push a branch named `release/{version}` (e.g. `release/1.0.0`) to trigger the GitHub Actions workflow. It will:

1. Build self-contained single-file binaries for all 6 platforms
2. Update the version in `Gw2Mcp.csproj`
3. Commit the version bump
4. Create a GitHub Release with all platform artifacts attached

## License

MIT