# Design

## Context

The tracker computes material shortfalls independently of the Workshop. The
Workshop also checks OV, development level, prerequisite equipment, and
whether the selected recipe can still be developed.

## Goals / Non-Goals

Show a compact result once the material checklist is complete. Keep the
existing read-only behavior and supported Workshop categories.

## Decisions

- Persist the native recipe ID, step, category, and parent weapon ID for
  reinforcement. Resolve the recipe from the live development catalogs and
  call its native development check. Apply the attachment menu's additional
  already-developed check.
- Keep the title and status visible in the compact card. Hide ingredient rows
  and source information while the material shortfall is zero or the pinned
  recipe step is already developed. Check for a completed step before reporting
  material shortfalls.
- Read older pins with their existing material list. In the local test, verify
  the former Anti-Matter Rifle recipe against the live catalog and retain the
  player's newer pin.

## Risks / Trade-offs

The resolved recipe can move to a different step after development. Show the
completed step as built until the player selects a new pin. If the game cannot
resolve an older pin, show materials ready and direct the player to pin it again.
