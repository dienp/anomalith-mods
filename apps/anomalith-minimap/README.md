# ANOMALITH Minimap

A Windows x64 BepInEx plugin for ANOMALITH's Unity Mono build. It shows the
native floor plan and your current position and facing in the bottom-left
corner in the Base and during Dives. The plan follows the player and switches
floors automatically. The translucent square matches the full width of the
health bar.

The minimap uses the native map symbols for main and side objectives, doors
and area connections, portals, and available fast-travel points on the
current floor.

The minimap hides during menus, dialogs, cutscenes, transitions, and on floors
where the game has no map. Use **Options → Other Settings → Minimap** or
press **F10** to turn it off or on. The setting and key binding are stored in
`BepInEx/config/labs.anomalith.minimap.cfg`. If the upgrade tracker is
loaded, Other Settings also shows **Pinned Item**.
While off, the minimap stops polling landmarks and keeps the current floor
image ready so it can turn back on without reloading it.
See [runtime verification](docs/verification.md).

## Build and install

Requires the .NET 8 SDK, a local Windows x64 ANOMALITH installation, BepInEx
5.4.23.6. The project targets `netstandard2.1` and
references assemblies from the game and loader. It has no NuGet dependencies,
environment variables or external services.

```powershell
$game = 'E:/Games/ANOMALITH'
dotnet build apps/anomalith-minimap/AnomalithMinimap.csproj -c Release -p:AnomalithDir=$game
Copy-Item -LiteralPath 'apps/anomalith-minimap/bin/Release/netstandard2.1/AnomalithMinimap.dll' -Destination (Join-Path $game 'BepInEx/plugins')
npm run spec -- apps/anomalith-minimap validate --all --strict
```

BepInEx loads the mod when the game starts. Close the game before replacing
or removing the DLL.
