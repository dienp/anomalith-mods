# Design

## Context

The tracker already reads live shortages during active Dives. Native `Event_Item` objects expose rolled ground loot, and `ObjectBase.ObstructionLayerMask` identifies scenery that blocks the game's own interaction tests. The paused game's Repository tab supplies the Storage row prefab.

## Goals / Non-Goals

Show available pickups and source hints for the current pin while preserving the native inventory and save systems. Use the game's local map names.

## Decisions

- Search active ground pickups on a bounded interval while shortages exist. Use native IDs, distance from the active camera, projection, and the obstruction mask to draw up to four labels. Tie their visibility to the existing Dive checklist switch.
- Copy the native Repository prefab's name, count, and divider visuals into a read-only HUD canvas. Leave native navigation and item transfer components on their original objects. Destroy the canvas with the plugin on reload.
- Read the shipped Dive Report `spawnMap` data for broad zone hints. Resolve localized names through `StringManager`. Supplement Crystal of Madness with the verified Sentinel encounter on `map_209`.
- Test the marker against a live ground pickup and verify the Crystal source display independently of the encounter's randomized drop.

## Risks / Trade-offs

Enemy and item placements depend on area and progression. Reported zones indicate where to look. Pickup labels require live, resolved items in the camera's view.
