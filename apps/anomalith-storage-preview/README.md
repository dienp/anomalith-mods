# ANOMALITH Inventory Storage Preview

Hover or select an item in the Inventory grid to see the Storage icon and
quantity, such as `x 001`, on a separate line at the bottom of its tooltip.
Turn it ON or OFF under **Options → Other Settings → Inventory Storage Preview**.
It defaults to ON and remembers your choice.

## Install and build

For the Windows x64 Unity Mono build with BepInEx 5.4.23.6, place
`AnomalithStoragePreview.dll` in `BepInEx/plugins`. A local ScriptEngine
development setup can load the same DLL from `BepInEx/scripts`.

Building requires a local game install, BepInEx, and the .NET 8 SDK.
The plugin targets `netstandard2.1` and has no additional packages or
environment variables.

```powershell
dotnet build apps/anomalith-storage-preview/AnomalithStoragePreview.csproj -c Release -p:AnomalithDir=E:/Games/ANOMALITH
npm run spec -- apps/anomalith-storage-preview validate --all --strict
```

See [verification](docs/verification.md) for gameplay checks.
