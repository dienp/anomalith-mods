# Inventory Storage Preview Delta

## MODIFIED Requirements

### Requirement: Display storage count for an Inventory item

The Inventory hover tooltip SHALL display the selected item's count in Base storage on a visually distinct line below the description.

#### Scenario: View a carried item

- **WHEN** the player hovers or selects a carried item and its normal Inventory tooltip opens
- **THEN** the tooltip includes the quantity in Base storage without adding the carried quantity
- **AND** the storage line is spaced from the description and shows the Storage tab icon beside a larger bold quantity formatted as × followed by the count

#### Scenario: View an item that is absent from storage

- **WHEN** a hovered item has no copies in storage
- **THEN** the tooltip shows a storage count of zero

#### Scenario: Move to a different item or leave Inventory

- **WHEN** the selected item changes or the native tooltip closes
- **THEN** the count follows the native tooltip lifecycle
