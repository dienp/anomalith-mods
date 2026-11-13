# ANOMALITH Mods

BepInEx mods for the Windows x64 Unity Mono build of [ANOMALITH on Steam](https://store.steampowered.com/app/4017880/ANOMALITH/).

## Quick install

1. Open the folder containing `Anomalith.exe`. In Steam, right-click ANOMALITH and choose **Manage → Browse local files**.
2. If needed, extract [BepInEx 5.4.23.6 for Windows x64](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.6) into that folder. Launch the game once, then close it.
3. Install [ScriptEngine r11.2](https://github.com/BepInEx/BepInEx.Debug/releases/tag/r11.2) by placing `ScriptEngine.dll` in `BepInEx/plugins`.
4. Download the [release ZIP](https://github.com/dienp/anomalith-mods/releases/download/v1.0.2/anomalith-mods-v1.0.2.zip) and extract it into the folder containing `Anomalith.exe`. Both mod DLLs go into `BepInEx/scripts`.
5. Launch the game. The ZIP enables automatic loading, so both mods start without **F6**.

If you already customized ScriptEngine, keep your existing
`BepInEx/config/com.bepis.bepinex.scriptengine.cfg` when extracting and set
`LoadOnStart = true` under `[General]` while the game is closed.

If you installed the old FOV mod, remove `BepInEx/plugins/AnomalithFov.dll` while the game is closed and use ANOMALITH's Camera settings to adjust FOV.

## Mods

| Mod | What it does |
| --- | --- |
| [Upgrade Tracker](apps/anomalith-upgrade-tracker/README.md) | Pins Workshop recipes, highlights material sources on Deploy, and shows which weapons fit the attachment highlighted in **Workshop → Development → Attachment**. |
| [Minimap](apps/anomalith-minimap/README.md) | Shows the native floor plan, player position, and points of interest during gameplay. |

## Build from source

Use a local copy of ANOMALITH, the .NET 8 SDK, BepInEx 5, and ScriptEngine.
Each mod README has build commands. The Upgrade Tracker also includes
instructions to regenerate its material source index from your game files.

Run `npm ci` at the repository root to install the pinned OpenSpec CLI for the
validation commands in each mod README.
