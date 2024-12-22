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
 - Including Spawn points and pathing

# Requirements
- Windows build environment
- AzerothCore based WoW 3.3.5 server
- BLPNG Converter (or other PNG -> BLP conversion software)
- Installed and unpatched client of the EverQuest Trilogy
- Installed 3.3.5 WoW client, US version (with nosignature patch)

# Instructions
**Extracting Everquest Assets** 

Set all of your paths propertly in Configuration.cs ("Paths and Files" section)
Build and use EQWOWConverter and run the command "Extract EQ Data"

**Condition the EQ objects** 

Use EQWOWConverter and run the command "Condition Exported EQ Data"

 **Converting EQ texture assets into BLP**

Use BLPNG Converter.  Use option BLP > Choose Folder  (select the conditioned assets folder)

**Converting Everquest data into World of Warcraft data**

Use EQWOWConverter and command "Convert EQ Data to WOW"

**Update the AzerothCore Database**

Run the scripts located into the exports directory subfolder \AzerothCoreSQLScripts\ (unless you set it to do it automatically in the config)

**Regenerate map/vmap files for AzerothCore**

Re-run the map_extractor, vmap4_assembler, etc.
NOTE: Requires a vmap4_extractor with at least this pull request merged: https://github.com/azerothcore/azerothcore-wotlk/pull/20822

# Special Thanks
In no particular order...
- Dan Wilkins/Nick Gal/(others) for Lantern Extractor (https://github.com/LanternEQ/LanternExtractor) - This saved a lot of time trying to get EQ data exported
- The people behind https://wowdev.wiki - Navigating the WoW file formats would have been near impossible without this documentation
- WoW Modding Community Discord - For the one-off problem questions I've run into thus far (special callout to Aleist3r, Titi, Soup Aura)
- Jarl Gullberg and team working on libwarcraft (https://github.com/WowDevTools/libwarcraft) - Whenever confused by elements outlined in wowdev.wiki, this code worked as a reference sanity check
