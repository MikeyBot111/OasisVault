# OasisVault

> **⚠️ Archived – old project, no longer maintained.**
> Written in 2023 for the **Oasis** Unturned servers. It's kept here for reference only. It may not work with current versions of Unturned or RocketMod.

A RocketMod plugin for **Unturned** that gives each player a personal **virtual storage vault**. The vault opens as a storage window in game, and its contents are saved in **MySQL**, so items survive deaths, wipes of the map storage and server restarts.

## What it does

- `/vault` opens a storage grid for the player (the size can be changed in the config, 8×8 by default)
- Items are saved to MySQL with position, rotation, stack size, durability and metadata
- A cooldown between uses stops spamming
- Admins can look into any player's vault with `/vault spy <steamid>`
- The database table is created automatically on first load

## Commands & permissions

| Command | Permission | Description |
|---|---|---|
| `/vault` | `OasisVault.vault` (+ `vault`) | Open your vault |
| `/vault spy <steamid>` | admin | Look into another player's vault |

## Config

| Key | Default |
|---|---|
| `DatabaseAddress` | `localhost` |
| `DatabaseUsername` | `root` |
| `DatabasePassword` | *(empty)* |
| `DatabaseName` | `unturned` |
| `DatabaseTableName` | `OasisVault` |
| `DatabasePort` | `3306` |
| `VaultWidth` / `VaultHeight` | `8` / `8` |
| `MaxCooldown` | `8` |

The repo only has placeholder defaults. Real database details are set in the server's plugin config.

## Tech

- C# / .NET Framework 4.8
- RocketMod, MySqlConnector (NuGet)
- `lib/` holds the reference assemblies needed to build

## Build

Restore NuGet packages, open `OasisVault.sln` in Visual Studio and build it. Then copy the DLL and its dependencies into `Rocket/Plugins`.

## Part of the Oasis project

[OasisEssential](https://github.com/MikeyBot111/OasisEssential) · [OasisTeleport](https://github.com/MikeyBot111/OasisTeleport) · [OasisKits](https://github.com/MikeyBot111/OasisKits) · [OasisRanks](https://github.com/MikeyBot111/OasisRanks) · [OasisVault](https://github.com/MikeyBot111/OasisVault) · [OasisDiscord](https://github.com/MikeyBot111/OasisDiscord) · [OasisCustomCommands](https://github.com/MikeyBot111/OasisCustomCommands) · [WebsiteOasis](https://github.com/MikeyBot111/WebsiteOasis) · [oasis-server-discord](https://github.com/MikeyBot111/oasis-server-discord)
