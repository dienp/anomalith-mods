# Material sources

The tracker reads `spawnMap` from the game's loaded Dive Report data and uses
the game's localized map strings. On Deploy it also matches positive item
weights against the map's assigned item drop tables and placement variants.
These matches are possible loot. A listed zone can cover multiple sections
and encounter variants.

The embedded enemy index matches enemy and variant IDs in the shipped
encounter, rush, quest, and area scenes to the game's enemy drop tables. It
includes allowed random variants, reused area layouts, and inherited drop
tables. The current index covers 44 materials across 76 area IDs. Deploy
shows only discovered tiles, with a zone hint where no exact match is available.

| Material | Example indexed source |
| --- | --- |
| Crystal of Madness | Sentinel on Block E Roof, Yotsubadai Heights |
| Protective Equipment Fragments | Unknown Trooper in Area 05, Under the Overpass, and in Yotsubadai Heights Area 02 |
| Communication Device Spring | Unknown Trooper (Sniper) in Area 05, Under the Overpass |

The marks indicate possible sources across encounter layouts. Quest progress,
enemy level, random rolls, and spawn conditions can change what appears on a
given visit. Randomly generated special dungeon layouts and encounters that
cannot be tied to a shipped area are outside the index.

To refresh the embedded index after a game update, run this command from
the repository root with [uv](https://docs.astral.sh/uv/) and the local Windows
game installation, then rebuild the tracker. The generator reads the game files
and writes the item, area, and enemy name keys to `Data/EnemySources.tsv`.

```powershell
uv run --no-project --with UnityPy==1.25.4 --with TypeTreeGeneratorAPI==0.0.10 python apps/anomalith-upgrade-tracker/tools/index_enemy_sources.py E:/Games/ANOMALITH
```
