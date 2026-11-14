# ANOMALITH Quick Weapon Switch

Quick Weapon Switch speeds up the normal firearm swap animation. In
**Options → Other Settings**, use **Quick Weapon Switch** to turn it ON or
OFF and **Switch Speed** to select 1.0×–3.0× in 0.1× steps. It starts
enabled at 2.0× and remembers both settings.

While enabled, one mouse-wheel notch cycles your assigned weapons immediately.
Scrolling in either direction skips empty shortcuts and wraps around.

## Install

For the Windows x64 Unity Mono build with BepInEx 5.4.23.6, place
`AnomalithQuickSwitch.dll` in `BepInEx/plugins`. For local ScriptEngine
development, place it in `BepInEx/scripts` and replace the script DLL
when rebuilding. Use a single copy of the DLL.

## Build and verify

Requires a local ANOMALITH installation with BepInEx and the .NET 8 SDK.
The plugin targets `netstandard2.1` and uses Harmony from BepInEx.
It has no additional packages or environment variables.

```powershell
dotnet build apps/anomalith-quick-switch/AnomalithQuickSwitch.csproj -c Release -p:AnomalithDir=E:/Games/ANOMALITH
npm run spec -- apps/anomalith-quick-switch validate --all --strict
```

See [verification](docs/verification.md) for the live test.
