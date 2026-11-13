## Context

Deploy already renders map rows and area tiles with stable map IDs. The tracker has combined inventory counts, documented zones, and a verified Sentinel location.

## Decisions

- Show an accent beside native map rows that have matching sources. On the selected map use subtle teal tile outlines, the native typeface, and dark, compact item labels. Put a label near its tile wherever it fits. Use the map's source list for dense clusters or entries without a visible tile. Preserve the native selected row, quest icons, graph connections and focus.
- Read map IDs from the native menu. Render a non-interactive overlay in the same canvas and align it to the native rects. Rebuild it when the selected map, shortages, or layout changes and remove it on unload.
- Match positive material weights from native area loot tables with authored placement variants and present them as possible loot. Keep the verified Block E Roof encounter and the Dive Report zone fallback. Show zone-only information for any item without area evidence and identify documented maps that are absent from Deploy.
- Respect the game's discovered-area display. A missing material may have sources the player cannot see or access yet. No pin, inventory, or game settings migration is needed.
