# Pong-for-arcade-cabinet
Pong game (with powerups!) made in Unity, deployed to a coffee table arcade cabinet

## Setup Notes
- Need TextMeshPro
- Using new input system. You may need to restart the editor to get bindings to work.

## Architecture Notes
Each Player has a "PlayerData" scriptableObject that is their source of truth. They have a paddle in the scene, but the data - score, powerups, etc - live in the scriptableObject. The game is written in a way to have any number of players.

### Powerups
Powerups are events, and not objects. They are stored as an enum. The This is a simple and flexible setup, but has some downsides. The upside is that powerups can live anywhere.
Instead of "launch all orange balls forward" being a script that lives with the paddle, it lives on the ball, and just changes which player it's listening to events from.

- All the powerup components listen to when you get or activate any powerup. They have to check if it's the one they care about or not. This game isn't at a scope where that's a problem.
- It's really easy to hook UI up to the playerData scripable object.
- Configuring the settings for powerups is annoying. We have to make a table, stored in the PowerupManager, that keeps a list. If we forget to populate a powerup, it won't work.
