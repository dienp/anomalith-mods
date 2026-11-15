# Inventory Storage Preview Specification

## Purpose

Show how many copies of the selected inventory item are in Base storage.

## Requirements

### Requirement: Display storage count for an Inventory item

The Inventory hover tooltip SHALL display the selected item's count in Base storage on a visually distinct line below the description.

#### Scenario: View a carried item

- **WHEN** the player hovers or selects a carried item and its normal Inventory tooltip opens
- **THEN** the tooltip includes the quantity in Base storage without adding the carried quantity
- **AND** a bright Storage tab icon sits centered vertically beside the bold quantity on an inset line
- **AND** the quantity uses the Storage screen's lowercase x and three-digit minimum format, such as x 001

#### Scenario: View an item that is absent from storage

- **WHEN** a hovered item has no copies in storage
- **THEN** the tooltip shows x 000

#### Scenario: Move to a different item or leave Inventory

- **WHEN** the selected item changes or the native tooltip closes
- **THEN** the count follows the native tooltip lifecycle

### Requirement: Toggle storage counts in Other Settings

Inventory Storage Preview SHALL provide an ON/OFF selector under Options → Other Settings and default to ON.

#### Scenario: Disable the storage preview

- **WHEN** the player switches Inventory Storage Preview OFF
- **THEN** the next Inventory tooltip omits the storage row and the game remembers the setting

#### Scenario: Re-enable the storage preview

- **WHEN** the player switches Inventory Storage Preview ON
- **THEN** the next Inventory tooltip shows the storage row
