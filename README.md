# EQ to WOW Converter
Converts the EverQuest assets from the original three game releases (Initial, Ruins of Kunark, Scars of Velious) into World of Warcraft 3.3.5

<img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Loading-Velious.png?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/WestFreeportGate.png?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Rivervale.png?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/HighKeep.png?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/FirionaVie.png?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/CrystalCaverns.png?raw=true" width="300"/>

# Current State
It is currently still in heavy development and things are changing daily.  Currently the maps are outputting with static objects, but there are severial issues (water is solid, no boats, lighting is wrong, etc)

Current focus is on getting original/classic content polished, then Kunark and Velious will follow.  Note that Kunark and Velious static objects and maps are already exporting.

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

Before building, set your Paths propertly in Configuration.cs.  There are 3 paths to set there

**Condition the EQ objects** 

Use EQWOWConverter and run the command "Condition Exported EQ Data"

 **Converting EQ texture assets into BLP**

Use BLPNG Converter.  Use option BLP > Choose Folder  (select the conditioned assets folder)

**Converting Everquest data into World of Warcraft data**

Use EQWOWConverter
- Run command "Convert EQ Data to WOW"

**Update related DBC files**

Use WDBX Editor and go into the exports directory and import the csv script files in \DBCUpdateScripts\.  Replace any values that exist, and there is no header row
(To do this, uncheck "Has Header Row?" and select "Update Existing" on the CSV settings pop-up on import)

**Package everything up**

Copy all of the output files from EQWOWConverter and the DBC changes into a MPQ and deploy.  Important: Make sure the max file count inside the MPQ is set to something quite large (65536) preferrably (In Ladik's MPQ Editor, Operations > Set Max File Count)

**Update the AzerothCore Database**

Run the scripts located into the exports directory subfolder \AzerothCoreSQLScripts\

**Regenerate map files for AzerothCore**

- Re-run the map_extractor, mmaps_generator, vmap4_assembler, vmap4_extractor (this is optional UNTIL creatures are implemented)

# Special Thanks
In no particular order...
- Dan Wilkins/Nick Gal/(others) for Lantern Extractor (https://github.com/LanternEQ/LanternExtractor) - This saved a lot of time trying to get EQ data exported
- The people behind https://wowdev.wiki - Navigating the WoW file formats would have been near impossible without this documentation
- WoW Modding Community Discord - For the one-off problem questions I've run into thus far (special callout to Aleist3r and Titi)
- Jarl Gullberg and team working on libwarcraft (https://github.com/WowDevTools/libwarcraft) - Whenever confused by elements outlined in wowdev.wiki, this code worked as a reference sanity check
