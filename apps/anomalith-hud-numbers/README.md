# ANOMALITH HUD Numbers

A Windows x64 BepInEx plugin that shows current and maximum HP and MP to
the left of the game's HUD bars. It reads the player's native status values
and refreshes ten times per second. Numbers follow the bars' visibility
and HUD scaling. The HP number follows the health bar's current color.
The labels use the game's font and receive no input.

HP rounds up so a living character with less than 1 HP shows 1. MP rounds
down to show whole points currently available. Maximum values round up.

Use **Options → Other Settings → HP/MP Numbers** to turn the numbers ON or
OFF. The preference is saved in
`BepInEx/config/labs.anomalith.hudnumbers.cfg` as `General.Enabled`.

## Build and install

Requires the .NET 8 SDK, a local Windows x64 ANOMALITH installation, and
BepInEx 5. Close the game before replacing the startup plugin.

```powershell
dotnet build apps/anomalith-hud-numbers/AnomalithHudNumbers.csproj -c Release -p:AnomalithDir=E:/Games/ANOMALITH
Copy-Item -LiteralPath apps/anomalith-hud-numbers/bin/Release/netstandard2.1/AnomalithHudNumbers.dll -Destination E:/Games/ANOMALITH/BepInEx/plugins
npm run spec -- apps/anomalith-hud-numbers validate --all --strict
```
