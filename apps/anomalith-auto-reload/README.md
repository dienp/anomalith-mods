# ANOMALITH Auto Reload

Auto Reload starts the normal reload when your active firearm runs empty and
the game allows it. Reloads use the game's animation and ammo rules.

## Install and configure

For the Windows x64 Unity Mono build with BepInEx 5.4.23.6, place
`AnomalithAutoReload.dll` in `BepInEx/plugins` and launch ANOMALITH. The mod is
enabled by default. To disable it, close the game and set `Enabled = false`
under `[Gameplay]` in `BepInEx/config/labs.anomalith.autoreload.cfg`.

## Build and verify

Requires the .NET 8 SDK and a local ANOMALITH installation with BepInEx. The
plugin targets `netstandard2.1` and uses Harmony from BepInEx. It has no NuGet
dependencies or environment variables.

```powershell
dotnet build apps/anomalith-auto-reload/AnomalithAutoReload.csproj -c Release -p:AnomalithDir=E:/Games/ANOMALITH
npm run spec -- apps/anomalith-auto-reload validate --all --strict
```

See [verification](docs/verification.md) for the gameplay check.
