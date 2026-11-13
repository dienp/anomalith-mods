# ANOMALITH FOV

A Windows x64 BepInEx plugin that overrides vertical FOV for the ANOMALITH
cameras controlled by its native gameplay FOV setting. The override starts at
65° and applies through the game's camera controller without editing the save.

## Build and install

Use the .NET 8 SDK, the Windows Unity Mono build of ANOMALITH, and BepInEx
5.4.23.6 (x64). The C# plugin targets `netstandard2.1` and references the local
game, Unity, Cinemachine, BepInEx, and bundled Harmony assemblies. It requires no
environment variables, external services, or other monorepo packages.

From the repository root in PowerShell, with the game closed for installation:

```powershell
$game = 'E:/Games/ANOMALITH'
dotnet build apps/anomalith-fov/AnomalithFov.csproj -c Release -p:AnomalithDir=$game
Copy-Item -LiteralPath 'apps/anomalith-fov/bin/Release/netstandard2.1/AnomalithFov.dll' `
    -Destination (Join-Path $game 'BepInEx/plugins')
```

Launch the game to generate `BepInEx/config/labs.anomalith.fov.cfg`. Set
`VerticalFov` to a whole number from 40 through 100 and restart the game. To
restore the game's FOV setting, set `Enabled = false` or remove the plugin DLL
and restart. The game's own FOV slider stores its ordinary value for use when
the override is disabled.

## Verify

Load into gameplay and look in `BepInEx/LogOutput.log` for the applied lens
value and saved option. [Runtime checks](docs/verification.md) describe the
test steps and coverage.

```powershell
npm run spec -- apps/anomalith-fov validate --all --strict
```
