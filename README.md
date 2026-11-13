# ANOMALITH Mods

Source for three BepInEx mods for the Windows Unity Mono build of ANOMALITH.

| Mod | What it does |
| --- | --- |
| [Upgrade Tracker](apps/anomalith-upgrade-tracker/README.md) | Pins Workshop recipes, tracks missing materials, and highlights possible sources on the Deploy map. |
| [Minimap](apps/anomalith-minimap/README.md) | Shows the native floor plan, player position, and points of interest during gameplay. |
| [FOV](apps/anomalith-fov/README.md) | Optional vertical field-of-view override for the native gameplay cameras. |

Use a local copy of ANOMALITH, the .NET 8 SDK, and BepInEx 5. The Minimap and
Upgrade Tracker use ScriptEngine. Each mod README has its build and install
commands. The Upgrade Tracker includes a derived material source index and
instructions to regenerate it from your game files.

Run `npm ci` at the repository root to install the pinned OpenSpec CLI for the
validation commands in each mod README.
