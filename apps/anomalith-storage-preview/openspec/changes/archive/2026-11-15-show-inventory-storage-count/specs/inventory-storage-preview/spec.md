# Inventory Storage Preview Delta

## Purpose

Show how many copies of the selected inventory item are in Base storage.

## ADDED Requirements

### Requirement: Display storage count for an Inventory item

The Inventory hover tooltip SHALL display the selected item's count in Base storage.

#### Scenario: View a carried item

- **WHEN** the player hovers or selects a carried item and its normal Inventory tooltip opens
- **THEN** the tooltip includes the quantity in Base storage without adding the carried quantity

#### Scenario: View an item that is absent from storage

- **WHEN** a hovered item has no copies in storage
- **THEN** the tooltip shows a storage count of zero

#### Scenario: Move to a different item or leave Inventory

- **WHEN** the selected item changes or the native tooltip closes
- **THEN** the count follows the native tooltip lifecycle
