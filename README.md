# EQ to WOW Converter
Converts the EverQuest assets from the original game into World of Warcraft 3.3.5

<img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/WestFreeportInterior.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/ErudinDay.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/KelethinHighFog.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Neriak.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Lavastorm.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/AkanonMusings.jpg?raw=true" width="300"/>

# Current State
It is currently still in heavy development and things are changing daily.  Currently these features are completed (for classic):
- Map Geometry & Collision
- Ladders
- Water/Lava
- Zone Lines
- Fog / Environment Lighting
- Vertex Colors (on WMOs)

# Requirements
- Windows build environment
- AzerothCore based WoW 3.3.5 server
- LaternExtractor fork (https://github.com/NathanHandley/LanternExtractor)
- BLPNG Converter (or other PNG -> BLP conversion software)
- WDBX Editor (or other DBC editor)
- Ladik's MPQ Editor (or other MPQ editor)
- Installed and unpatched client of the EverQuest Trilogy

# Instructions
**Extracting Everquest Assets** 

Use LaternExtractor (https://github.com/NathanHandley/LanternExtractor)
- Edit 'settings.txt' inside the respective Release/Debug binary folder
- Set the Everquest directory
- Set ModelExportFormat = 0 (Intermediate)
- Set ExportHiddenGeometry = true
- Set ExportZoneWithObjects = true
- Set ExportCharacterToSingleFolder = true
- Set ExportEquipmentToSingleFolder = true
- Set ExportSoundsToSingleFolder = true
- Set CopyMusic = true
- Execute LaternExtractor.exe
- Enter command "all", push enter

**Configure and build EQWOWConverter**

Before building, set your Paths propertly in Configuration.cs.  There are 4 paths to set there (all 4 in "Paths" section)

**Condition the EQ objects** 

Use EQWOWConverter and run the command "Condition Exported EQ Data"

 **Converting EQ texture assets into BLP**

Use BLPNG Converter.  Use option BLP > Choose Folder  (select the conditioned assets folder)

**Converting Everquest data into World of Warcraft data**

Use EQWOWConverter
- Run command "Convert EQ Data to WOW"

**Update related DBC files**

Use WDBX Editor and go into the exports directory and import the csv script files in \DBCUpdateScripts\.

IMPORTANT: Make sure to load these in the proper order. With WDBX, select "Import New" and "Take Newest" as the options (leave "Has Header Row?" checked)

**Package everything up**

Copy all of the output files from EQWOWConverter and the DBC changes into a MPQ and deploy.  Important: Make sure the max file count inside the MPQ is set to something quite large (65536) preferrably (In Ladik's MPQ Editor, Operations > Set Max File Count)

**Update the AzerothCore Database**

Run the scripts located into the exports directory subfolder \AzerothCoreSQLScripts\

**Regenerate map files for AzerothCore**

- Re-run the map_extractor, mmaps_generator, vmap4_assembler, vmap4_extractor (this is optional UNTIL creatures are implemented)
NOTE: Having trouble with this, so this may not work for you as well.  Will debug later.

# Special Thanks
In no particular order...
- Dan Wilkins/Nick Gal/(others) for Lantern Extractor (https://github.com/LanternEQ/LanternExtractor) - This saved a lot of time trying to get EQ data exported
- The people behind https://wowdev.wiki - Navigating the WoW file formats would have been near impossible without this documentation
- WoW Modding Community Discord - For the one-off problem questions I've run into thus far (special callout to Aleist3r and Titi)
- Jarl Gullberg and team working on libwarcraft (https://github.com/WowDevTools/libwarcraft) - Whenever confused by elements outlined in wowdev.wiki, this code worked as a reference sanity check
