# Design

Use the game's `SideMessage.Instance.IsEnable()` signal in the existing minimap visibility check. The game enables this panel for a sequence of side messages and disables it once the sequence and its closing animation end. This covers gaps between lines and works with voice volume at zero. Preserve the loaded map while the overlay is hidden so it can resume without reloading the floor image.
