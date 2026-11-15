# Proposal

## Why

The minimap hides for modal dialogue, but characters can speak through a separate HUD panel while the player is still moving. Hide the map so the player can focus on the conversation.

## What Changes

- Hide the minimap during ordinary NPC dialogue and in-game conversations shown through the side-message panel.
- Restore the minimap after the conversation when gameplay and the Minimap setting allow it.

## Capabilities

### Modified Capabilities

- `live-minimap` covers both modal and in-game dialogue.

## Impact

The visibility check reads existing game state. It requires no additional dependencies or settings.
