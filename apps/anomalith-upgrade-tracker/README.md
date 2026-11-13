# ANOMALITH Upgrade Tracker

A Windows x64 BepInEx plugin for the Unity Mono edition of ANOMALITH. It shows
one Workshop recipe's material requirements during gameplay at Base and in
Dives. It highlights nearby pickups during a Dive, and Deploy shows where to
look for the materials you still need.

## Controls

- Click **Pin recipe** at the right edge of Required Materials in the Workshop, or press
  **F7**. Use **Unpin recipe** or **F7** again to clear the same selection.
  Pinning a different recipe replaces the current pin.
- **Options → Other Settings → Pinned Item** shows or hides pinned recipe
  guidance when the minimap mod is installed. **F8** changes the same setting
  at Base and during Dives. The setting covers the checklist, pickup markers, source
  hints, and Deploy annotations. The Workshop retains the pin when guidance is off.
- **F9** shows or hides source hints and the current map section.
- **Left Shift + F7** clears the pin.

The gameplay checklist follows the Workshop's Required Materials layout,
with the pinned item's category icon, larger material icons, shortage colors,
and right-aligned owned/required counts that combine your bag and storage.
OV appears only as a short status when it blocks crafting.
Nearby ground pickups appear during Dives when
the camera can see them and you still need their materials.

Once all materials are held, the tracker collapses to the recipe name and a
status. **Ready to build at the Workshop** means the game's development check
passes. The tracker shows **Built** after you complete the pinned step. If an
older pin asks you to pin it again, highlight the recipe at the Workshop and
use **Pin recipe** to add the recipe identity.

The sources panel combines the game's listed zones with an index of enemy
encounters. Deploy uses the same index for every pin, including recipes
that need materials from several areas. [Material source evidence](docs/material-sources.md)
explains the location coverage and how to refresh the index after a game update.

On **Deploy**, a teal mark beside a map means it has a source for at least
one outstanding material. Discovered area tiles with area-level sources get
a teal outline and nearby material labels. The panel below the map list
shows the pinned recipe and its Workshop category icon. Its material rows
show outstanding requirements with Workshop icons and right-aligned
owned/required counts. It identifies possible loot and lists
matching areas when the graph is crowded.
The list also shows region-only and unavailable destinations. These hints
follow the active pin and disappear when the recipe is built or its materials
are complete.

In **Workshop → Development → Attachment**, the compatible weapons panel
lists the revealed weapons that fit the highlighted attachment, one per row.
Weapons that still need development appear under **Not developed**.

A pin captures the selected recipe step. Pin the next step again after developing an upgrade.
Controls and the pin are stored in
`BepInEx/config/labs.anomalith.upgradetracker.cfg`.

## Build and install

Requires the .NET 8 SDK, ANOMALITH for Windows x64, BepInEx 5.4.23.6, and
ScriptEngine 11.2. The project targets `netstandard2.1` and builds against
the local game and BepInEx assemblies. It has no NuGet dependencies,
environment variables, external services, or local package dependencies.

From the repository root in PowerShell:

```powershell
$game = 'E:/Games/ANOMALITH'
dotnet build apps/anomalith-upgrade-tracker/AnomalithUpgradeTracker.csproj -c Release -p:AnomalithDir=$game
Copy-Item -LiteralPath 'apps/anomalith-upgrade-tracker/bin/Release/netstandard2.1/AnomalithUpgradeTracker.dll' `
    -Destination (Join-Path $game 'BepInEx/scripts')
```

The build embeds its debug symbols for ScriptEngine. Press **F6** in-game to
load or reload it, or let ScriptEngine's file watcher handle the change.
Remove the DLL from `BepInEx/scripts` and reload to unload the tracker.

## Verify

See [runtime verification](docs/verification.md) for the local test and
reproduction steps.

```powershell
npm run spec -- apps/anomalith-upgrade-tracker validate --all --strict
```
