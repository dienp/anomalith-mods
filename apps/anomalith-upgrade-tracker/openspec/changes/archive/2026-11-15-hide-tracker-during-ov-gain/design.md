# Design

Read the gameplay HUD's MoneyUI animation coroutine and gain step, then
follow FadeOut until the animation completes. Use the HUD instance only.
Check after the game's Update so it can take priority in the same frame.
Keep refreshing requirements and pickup markers while the panel is hidden.
The regular and fallback checklist presentations use the same visibility state.
