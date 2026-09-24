# EQ to WOW Converter
Converts the EverQuest assets from the original game into World of Warcraft 3.3.5

<img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/WestFreeportInterior.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Nagafen.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/KelethinHighFog.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Neriak.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/OnABoat.jpg?raw=true" width="300"/><img src="https://github.com/NathanHandley/EQWOWConverter/blob/Screenshots/Mistmoore.jpg?raw=true" width="300"/>

To see video references, go here: [YouTube Playlist](https://youtube.com/playlist?list=PLB5LJGQjmSQTqbmGEf3WAZc_VvI_ogf3g&si=B7HPIb46PemFglAB)

# Requirements
- Windows build environment
- AzerothCore based WoW 3.3.5 server (last through this changeset: https://github.com/azerothcore/azerothcore-wotlk/commit/fe6913668821601b07e13366716db2e9dfb1701d)
- Install the "mod-everquest" mod into AzerothCore (https://github.com/NathanHandley/mod-everquest)
- Installed unpatched client of the EverQuest Trilogy
- Installed 3.3.5a WoW client, US version (which must be patched to not check for MD5 signatures, similar to other modded projects) 
- If you want armor textures, install a texture pack.  One can be found here: https://github.com/NathanHandley/EQWOWConverter-HumTexturePack

# Instructions (if building from source)
1. Build and run the application once in order to generate a configuration.txt file.
2. Open the configuration.txt file and set the desired values in sections 1, 2, and 3 (near the top)
3. (Optional) Install a texture pack, such as one here: https://github.com/NathanHandley/EQWOWConverter-HumTexturePack
4. Build and use EQWOWConverter and run the command "Perform Everything"
5. (Optional) Deploy your files manually if you did not set up automatic deployment in the configuration
6. Regenerate map/vmap files for the server per AzerothCore instructions

Note: If you have a working install, you typically only need to select option 6 to keep up to date on future builds

# Deploying the Files
Automatic Deployment **Highly Recommended**:
1. Set the configuration.txt deployment settings in "2. Deployment Settings"
2. Build and let it deploy for you next time you run it

Manual Deployment (Alternate):
1. Run the .sql files located in <PATH_WORKING_FOLDER>/WOWExports/SQLScripts against your databases, with /Characters/ against your characters database and /World/ against your world database
2. Copy contents of <PATH_WORKING_FOLDER>/WOWExports/DBCFilesServer into your server's dbc files location (typically located in the "data" folder during AzerothCore setup)
3. Place the (*.mpq) patch file inside <PATH_WORKING_FOLDER>/WOWExports into the (<PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER>/Data/) and (<PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER>/Data/enUS/) folders. (note that enUS may be different depending on your locale)
4. Copy the AddOn from your <PATH_WORKING_FOLDER>/AddOnsReady into the (<PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER>/Interface/AddOns/) folder.

# Special Thanks
In no particular order...
- Dan Wilkins/Nick Gal/(others) for Lantern Extractor (https://github.com/LanternEQ/LanternExtractor) - This saved a lot of time trying to get EQ data exported
- Also the community behind Lantern Extractor on their Discord.  Special callouts to Kicnlag, Silvae, Wiz, and Eldrich.
- The people behind https://wowdev.wiki - Navigating the WoW file formats would have been near impossible without this documentation
- WoW Modding Community Discord - For the one-off problem questions I've run into thus far (special callout to Aleist3r, Titi, Soup Aura, Stoneharry)
- Jarl Gullberg and team working on libwarcraft (https://github.com/WowDevTools/libwarcraft) - Whenever confused by elements outlined in wowdev.wiki, this code worked as a reference sanity check

# Eulogy of the Developer
(this is 100% human written, by me)

My name is Nathan Handley, and this project (EQWOWConverter) contains the last substantive hand-written code I've ever written.  It's not all hand-written code, but everything in 2024 (and before), most of 2025, and some of 2026+ is.

I started writing code in the 90s as a kid with the common dream of being a game programmer.  Over the decades I have written hundreds of apps and utilities in both a private and public capacity.  It's been my passion to write code and I've been defined by it, but not anymore.  I am the slow part of the chain now and there's no output justification to ever write any substantial code again by hand. I'll miss that inner developer and cherish the skills and ability to read and write code, but it's time to move on just like the binary and ASM developers did as higher level languages came out.  And in time, soon, we'll see the end of human-readable code.

In this repository (and mod-everquest) the code has slowly grown to be more AI-code.  Now in this case I had written a massive amount of code and structure prior to introducing AI in 2025 so the AI tools have been using my style and approach exactly to how I would write things.  But at time of writing this, my workflow has been "give detailed instructions and architecture layout, sometimes pre-frame the classes, and then let AI fill in the rest".  Unless it is anything lua, then in that case AI is writing it 100%.  Right now the longest part of my delivery is my instructions + review + human alignment cleanup + testing, not the actual code generation itself.  I actually think it would be a liability to write all the code now.

I'm leaving my name on the copyright since the majority of this code base is hand-written and what the AI has written has been under my direction and little-to-no code has remained as-is from an AI writing it (I groom/adjust everything to align to patterns/or and fix bugs).  I'm not sure what copyright should look like anymore, but at a minimum I want to prevent people from using this code in a closed source project without giving back to the community, so I'll leave it like it is.