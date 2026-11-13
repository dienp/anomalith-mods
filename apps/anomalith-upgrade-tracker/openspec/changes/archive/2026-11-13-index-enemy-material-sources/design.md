# Design

## Context

The game stores enemy identities and variants in area scenes and materials in a separate loot table. A single material can have several areas and enemies.

## Goals / Non-Goals

Use one index across recipes, preserve area discovery, and keep the Deploy screen fast. Quest, level, and random spawn conditions are represented as possible encounters, with no prediction for the next visit.

## Decisions

Generate the index from the installed game at build time as a separate command. Read serialized encounter components and match their enemy and variant to the native loot table. Preserve verified map-scoped drop conditions, deduplicate matches, and keep the game's normal item loot and Dive Report fallbacks. Embed only the derived mappings so ScriptEngine still deploys one DLL. Group source hints by material and limit the summary height while marking every supported visible tile.

## Risks / Trade-offs

Game updates can change scene layouts and the index may need regeneration. Unresolved dynamic encounter variants are left to the documented zone fallback.
