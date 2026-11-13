# Design

## Context

The game builds settings rows using a native selector prefab. The current overlays have separate keyboard controls.

## Goals / Non-Goals

Add a native ON/OFF selector for Minimap and keep existing overlay behavior and shortcuts available.

## Decisions

The minimap plugin appends native `OptionItemSelect` rows to the active Other Settings holder. Row values are backed by plugin config entries and the menu uses its own input and scroll handling. The rows are never registered in the game's global option data. Remove rows and restore native selection on unload.

## Risks / Trade-offs

The integration depends on this build's Options holder and prefab. If they cannot be resolved, the mods still work through their existing shortcuts.
