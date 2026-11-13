# Design

## Context

The game builds settings rows using a native selector prefab. The current overlays have separate keyboard controls.

## Goals / Non-Goals

Add a native ON/OFF selector for Pinned Item and keep existing overlay behavior and shortcuts available.

## Decisions

Bind the visibility flag in the tracker configuration so F8, the Other Settings selector and the tracker use one source of truth. Gate the Dive checklist and markers and the Deploy material sources using that flag. The tracker continues to work with its shortcut when installed on its own.

## Risks / Trade-offs

The integration depends on this build's Options holder and prefab. If they cannot be resolved, the mods still work through their existing shortcuts.
