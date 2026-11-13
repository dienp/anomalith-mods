## Context

The native ingredient row rectangles end at the quantity column. The Workshop toggle currently uses the rightmost row as its horizontal anchor.

## Decisions

- Read the native panel rectangle for the horizontal anchor and convert its right edge into the Workshop menu's local coordinates. The button keeps its right pivot as its label changes width.
- Keep the existing vertical spacing above the native rows and the existing selection and shortcut behavior.
