## Why

Pinning is currently a hidden keyboard action, and the Deploy list gives material names without identifying the recipe. The Workshop and Deploy screens should make the same pin visible.

## What Changes

- Add a native-styled pin toggle next to Required Materials in eligible Workshop recipe views. Use the same toggle for mouse clicks and the configured pin key.
- Name the pinned recipe on Deploy and render outstanding materials and quantities with Storage-style rows.
- Keep the Deploy source notes and area markers, single-pin persistence, and existing clear shortcut.

## Capabilities

### Modified Capabilities

- `upgrade-materials-tracking` adds the Workshop toggle and shows recipe identity and counts on Deploy.

## Impact

Workshop and Deploy UI in the existing BepInEx tracker. The mod continues to read game state and stores the pin in its own configuration.
