# Design

Append a storage-only count to the native inventory popup description before its layout pass. Use the displayed item's ID and the game's repository count API. Patch only the Inventory item overload and retain native hover, selection, sizing, positioning, and screen clamping. If the expected layout call cannot be located, leave the native tooltip available.
