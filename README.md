# EQ to WOW Converter
Converts the EverQuest assets from the original game into World of Warcraft 3.3.5

<img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/WestFreeportInterior.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Nagafen.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/KelethinHighFog.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Neriak.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/OnABoat.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Mistmoore.jpg?raw=true" width="300"/>

To see video references, go here: https://www.youtube.com/@WoWEverQuest

# Current State
It is currently still in heavy development and things are changing daily.  Currently these features are completed:
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

Right now only classic (pre-Kunark) is in a semi-polished state, but you can enable Kunark and/or Velious content by setting the configuration values starting with GENERATE_EQ_EXPANSION_ID_.  Many things, such as zone lines and water volumes will not work in Kunark and above right now.

# Requirements
- Windows build environment
- AzerothCore based WoW 3.3.5 server, with the mod-everquest mod installed (https://github.com/NathanHandley/mod-everquest)
- Installed unpatched client of the EverQuest Trilogy
- Installed 3.3.5a WoW client, US version (which must be patched to not check for MD5 signatures, similar to other modded projects) 
- If you want armor textures, install a texture pack.  A human one can be found here: https://github.com/NathanHandley/EQWOWConverter-HumTexturePack

# Instructions
1. Set the configuration values in Configuration.cs within the first two sections ("Paths and Files" and "Deployment Rules", at the top)
2. (Optional) Install a texture pack, such as one here: https://github.com/NathanHandley/EQWOWConverter-HumTexturePack
3. Build and use EQWOWConverter and run the command "Perform Everything"
4. Deploy your files manually or automatically (see below in "Deploying the Files").
5. Regenerate map/vmap files for the server per AzerothCore instructions

# Deploying the Files
Automatic Deployment: Set the configuration.cs values in "Deployment Rules"

Manual Deployment:
1. Run the .sql files located in <PATH_WORKING_FOLDER>/WOWExports/SQLScripts against your databases, with /Characters/ against your characters database and /World/ against your world database
2. Copy contents of <PATH_WORKING_FOLDER>/WOWExports/DBCFilesServer into your server's dbc files location (typically located in the "data" folder during AzerothCore setup)
3. Place the patch file inside <PATH_WORKING_FOLDER>/WOWExports as defined by configuration value PATH_CLIENT_PATCH_FILE_NAME_NO_EXT into your wow client data folder (<PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER>/Data/enUS/)

# Special Thanks
In no particular order...
- Dan Wilkins/Nick Gal/(others) for Lantern Extractor (https://github.com/LanternEQ/LanternExtractor) - This saved a lot of time trying to get EQ data exported
- Also the community behind Lantern Extractor on their Discord.  Special callouts to Kicnlag, Silvae, Wiz, and Eldrich.
- The people behind https://wowdev.wiki - Navigating the WoW file formats would have been near impossible without this documentation
- WoW Modding Community Discord - For the one-off problem questions I've run into thus far (special callout to Aleist3r, Titi, Soup Aura, Stoneharry)
- Jarl Gullberg and team working on libwarcraft (https://github.com/WowDevTools/libwarcraft) - Whenever confused by elements outlined in wowdev.wiki, this code worked as a reference sanity check
