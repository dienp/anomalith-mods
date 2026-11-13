# Upgrade materials tracking

## Purpose

Carry the material requirements for one selected Workshop upgrade into a Dive.

## ADDED Requirements

### Requirement: Pin one Workshop upgrade

The tracker SHALL let the player pin the currently highlighted, revealed recipe with material costs in a supported Workshop development or reinforcement menu. A new pin SHALL replace the previous pin.

#### Scenario: Pin a Workshop recipe

- **WHEN** the player presses F7 on a revealed recipe with material costs and a development level still available
- **THEN** the tracker captures that recipe's current material requirements and confirms the pin

#### Scenario: Ineligible selection

- **WHEN** the highlighted recipe is hidden, fully developed, has no material costs, or is outside a supported Workshop menu
- **THEN** the tracker preserves any existing pin

#### Scenario: Clear a pin

- **WHEN** the player presses Shift+F7
- **THEN** the tracker clears the pin and its checklist

### Requirement: Display live shortages during a Dive

The tracker SHALL display each material's remaining requirement based on the game's combined bag and storage count. The checklist SHALL appear during active Dive gameplay, and F8 SHALL toggle its visibility without clearing the pin.

#### Scenario: Read a shortfall

- **WHEN** a pinned recipe requires three of a material and the player has one across the bag and storage
- **THEN** the checklist shows two still needed

#### Scenario: Reach a material requirement

- **WHEN** the combined material count reaches or exceeds the recipe requirement
- **THEN** the checklist shows that material as ready

#### Scenario: Leave active Dive gameplay

- **WHEN** the player is at the title, at base, in the training room, or in a loading transition
- **THEN** the Dive checklist is hidden

### Requirement: Preserve the pin across ScriptEngine reload

The tracker SHALL store its single pinned recipe in its own local configuration and restore it after a ScriptEngine reload. The tracker SHALL read game state without writing to inventory, game settings, or saves.

#### Scenario: Reload with a pin

- **WHEN** ScriptEngine reloads the plugin after a recipe has been pinned
- **THEN** the reloaded instance retains the recipe and obtains current counts when the checklist is next shown
