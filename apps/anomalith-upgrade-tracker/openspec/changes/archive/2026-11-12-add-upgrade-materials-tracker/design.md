# Design

## Context

The game reads the current Workshop selection through `DevMenuBase.m_CurrentSelector` and the material list through `DevelopmetData.GetLevelNeedMatData`. The Workshop uses `Inventory.GetTotalItemCount` to combine bag and storage.

## Goals / Non-Goals

Capture the visible recipe step as a snapshot. Provide a read-only checklist for the materials only. Players can repin the next step after developing an upgrade.

## Decisions

- Check the game's foreground Workshop dialog and read its selected item through one private field. Apply the game's hidden and maximum-level checks.
- Store one recipe snapshot as JSON in BepInEx configuration using the game's Newtonsoft.Json assembly. Copy the name, IDs, and quantities so the pin can outlive Workshop objects and plugin instances.
- Check the native Dive state and HUD visibility. Refresh counts at most four times per second while the checklist is visible and draw it with Unity IMGUI. Unity owns the plugin's update and draw callbacks.
- Build with embedded debug symbols for the installed ScriptEngine. Confirm the selected recipe in the Workshop, retain it across reload, and observe the checklist in a Dive.

## Risks / Trade-offs

The selection field is specific to the inspected game build. If it is missing, startup should log a compatibility failure and disable the tracker. The pin describes the exact recipe step selected at the Workshop. Repin to track a different step.
