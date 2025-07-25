# EQ to WOW Converter
Converts the EverQuest assets from the original game into World of Warcraft 3.3.5

<img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/WestFreeportInterior.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/ErudinDay.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/KelethinHighFog.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Neriak.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Lavastorm.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/AkanonMusings.jpg?raw=true" width="300"/>

# Current State
It is currently still in heavy development and things are changing daily.  Currently these features are completed (for classic):
- Map Geometry & Collision
- Ladders
- Water/Lava
- Zone Lines
- Fog / Exterior Environment Lighting
- Vertex Colors (on WMOs)
- Point Sounds (campfires, running water, etc)
- Ambient Sounds
- Music
- Creatures
- Transports (boats, lifts)
- Equipment (including held equipment visuals, minus particles)
- Quests
- Armor visuals
- Tradeskills
- Spell Particles

What's currently missing (all in a mid-state / work in progress):
- Creature Spells
- Player Spells
- Equipment Particles

# Requirements
- Windows build environment
- AzerothCore based WoW 3.3.5 server, with the mod-everquest mod installed (https://github.com/NathanHandley/mod-everquest)
- Installed and unpatched client of the EverQuest Trilogy
- Installed 3.3.5 WoW client, US version (with nosignature patch) 
- If you want armor textures, install a texture pack.  A human one can be found here: https://github.com/NathanHandley/EQWOWConverter-HumTexturePack

# Instructions
1. Set all of your paths propertly in Configuration.cs ("Paths and Files" section)
2. (Optional) Install a texture pack, such as one here: https://github.com/NathanHandley/EQWOWConverter-HumTexturePack
3. Build and use EQWOWConverter and run the command "Perform Everything"
4. Deploy your files. Alternately, you can set the configs in Deployment Rules so that they deploy for you
5. Regenerate map/vmap files for the server

# Special Thanks
In no particular order...
- Dan Wilkins/Nick Gal/(others) for Lantern Extractor (https://github.com/LanternEQ/LanternExtractor) - This saved a lot of time trying to get EQ data exported
- Also the community behind Lantern Extractor on their Discord.  Special callouts to Kicnlag, Silvae, Wiz, and Eldrich.
- The people behind https://wowdev.wiki - Navigating the WoW file formats would have been near impossible without this documentation
- WoW Modding Community Discord - For the one-off problem questions I've run into thus far (special callout to Aleist3r, Titi, Soup Aura, Stoneharry)
- Jarl Gullberg and team working on libwarcraft (https://github.com/WowDevTools/libwarcraft) - Whenever confused by elements outlined in wowdev.wiki, this code worked as a reference sanity check
