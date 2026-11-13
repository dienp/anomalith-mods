## Context

The gameplay checklist renders native Workshop icons and text. Deploy currently renders a separate Storage-style summary and chooses a position clear of map controls and area tiles.

## Decisions

Allow the Workshop checklist to render inside the Deploy overlay. Use the same header and material rows with a width that fits the map column. Show each material's source notes below its row. Use the existing collision checks to position the measured panel and preserve the graph annotations.

Read owned and required values from the same inventory calculation as the gameplay checklist.
