//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using EQWOWConverter.Common;
using EQWOWConverter.Creatures;
using EQWOWConverter.GameObjects;
using EQWOWConverter.Items;
using EQWOWConverter.ObjectModels;
using EQWOWConverter.Player;
using EQWOWConverter.Spells;
using EQWOWConverter.Tradeskills;
using EQWOWConverter.Transports;
using EQWOWConverter.WOWFiles;
using EQWOWConverter.Zones;
using System.Text;

namespace EQWOWConverter
{
    internal class DBCFileWorker
    {
        public void ExtractClientDBCFiles()
        {
            string wowExportPath = Configuration.PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Extracting client DBC files...");

            // Make sure the patches folder is correct
            string wowPatchesFolder = Path.Combine(Configuration.PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER, "Data", "enUS");
            if (Directory.Exists(wowPatchesFolder) == false)
                throw new Exception("WoW client patches folder does not exist at '" + wowPatchesFolder + "', did you set PATH_WOW_ENUS_CLIENT_FOLDER?");

            // Get a list of valid patch files (it's done this way to ensure sorting order is exactly right). Also ignore existing patch file
            List<string> patchFileNames = new List<string>();
            patchFileNames.Add(Path.Combine(wowPatchesFolder, "patch-enUS.MPQ"));
            string[] existingPatchFiles = Directory.GetFiles(wowPatchesFolder, "patch-*-*.MPQ");
            foreach (string existingPatchName in existingPatchFiles)
                if (existingPatchName.Contains(Configuration.PATH_CLIENT_PATCH_FILE_NAME_NO_EXT) == false)
                    patchFileNames.Add(existingPatchName);

            // Make sure all of the files are not locked
            foreach (string patchFileName in patchFileNames)
                if (FileTool.IsFileLocked(patchFileName))
                    throw new Exception("Patch file named '" + patchFileName + "' was locked and in use by another application");

            // Clear out any previously extracted DBC files
            Logger.WriteDebug("Deleting previously extracted DBC files");
            string exportedDBCFolder = Path.Combine(wowExportPath, "ExportedDBCFiles");
            FileTool.CreateBlankDirectory(exportedDBCFolder, false);

            // Generate a script to extract the DBC files
            Logger.WriteDebug("Generating script to extract DBC files");
            string workingGeneratedScriptsFolder = Path.Combine(wowExportPath, "GeneratedWorkingScripts");
            FileTool.CreateBlankDirectory(workingGeneratedScriptsFolder, true);
            StringBuilder dbcExtractScriptText = new StringBuilder();
            foreach (string patchFileName in patchFileNames)
                dbcExtractScriptText.AppendLine("extract \"" + patchFileName + "\" DBFilesClient\\* \"" + exportedDBCFolder + "\"");
            string dbcExtractionScriptFileName = Path.Combine(workingGeneratedScriptsFolder, "dbcextract.txt");
            using (var dbcExtractionScriptFile = new StreamWriter(dbcExtractionScriptFileName))
                dbcExtractionScriptFile.WriteLine(dbcExtractScriptText.ToString());

            // Extract the files using the script
            Logger.WriteDebug("Extracting DBC files");
            string mpqEditorFullPath = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "ladikmpqeditor", "MPQEditor.exe");
            if (File.Exists(mpqEditorFullPath) == false)
                throw new Exception("Failed to extract DBC files. '" + mpqEditorFullPath + "' does not exist. (Be sure to set your Configuration.PATH_TOOLS_FOLDER properly)");
            string args = "console \"" + dbcExtractionScriptFileName + "\"";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments = args;
            process.StartInfo.FileName = mpqEditorFullPath;
            process.Start();
            process.WaitForExit();

            Logger.WriteDebug("Extracting client DBC files complete");
        }

        public void CreateDBCFiles(List<Zone> zones, List<CreatureModelTemplate> creatureModelTemplates, List<SpellTemplate> spellTemplates)
        {
            string wowExportPath = Configuration.PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Creating DBC Files...");

            // Make sure there is a folder of files
            string dbcInputFolder = Path.Combine(wowExportPath, "ExportedDBCFiles");
            if (Directory.Exists(dbcInputFolder) == false)
            {
                Logger.WriteError("Failed to create DBC files, as '" + dbcInputFolder + "' did not exist");
                return;
            }

            // Clear prior folders
            string dbcOutputClientFolder = Path.Combine(wowExportPath, "MPQReady", "DBFilesClient");
            if (Directory.Exists(dbcOutputClientFolder) == true)
                Directory.Delete(dbcOutputClientFolder, true);
            Directory.CreateDirectory(dbcOutputClientFolder);
            string dbcOutputServerFolder = Path.Combine(wowExportPath, "DBCFilesServer");
            if (Directory.Exists(dbcOutputServerFolder) == true)
                Directory.Delete(dbcOutputServerFolder, true);
            Directory.CreateDirectory(dbcOutputServerFolder);

            // Load the files
            AreaTableDBC areaTableDBC = new AreaTableDBC();
            areaTableDBC.LoadFromDisk(dbcInputFolder, "AreaTable.dbc");
            AreaTriggerDBC areaTriggerDBC = new AreaTriggerDBC();
            areaTriggerDBC.LoadFromDisk(dbcInputFolder, "AreaTrigger.dbc");
            CharStartOutfitDBC charStartOutfitDBC = new CharStartOutfitDBC();
            if (Configuration.PLAYER_USE_EQ_START_ITEMS == true)
                charStartOutfitDBC.LoadFromDisk(dbcInputFolder, "CharStartOutfit.dbc");
            CreatureDisplayInfoDBC creatureDisplayInfoDBC = new CreatureDisplayInfoDBC();
            creatureDisplayInfoDBC.LoadFromDisk(dbcInputFolder, "CreatureDisplayInfo.dbc");
            CreatureModelDataDBC creatureModelDataDBC = new CreatureModelDataDBC();
            creatureModelDataDBC.LoadFromDisk(dbcInputFolder, "CreatureModelData.dbc");
            CreatureSoundDataDBC creatureSoundDataDBC = new CreatureSoundDataDBC();
            creatureSoundDataDBC.LoadFromDisk(dbcInputFolder, "CreatureSoundData.dbc");
            FactionDBC factionDBC = new FactionDBC();
            factionDBC.LoadFromDisk(dbcInputFolder, "Faction.dbc");
            FactionTemplateDBC factionTemplateDBC = new FactionTemplateDBC();
            factionTemplateDBC.LoadFromDisk(dbcInputFolder, "FactionTemplate.dbc");
            FootstepTerrainLookupDBC footstepTerrainLookupDBC = new FootstepTerrainLookupDBC();
            footstepTerrainLookupDBC.LoadFromDisk(dbcInputFolder, "FootstepTerrainLookup.dbc");
            GameObjectDisplayInfoDBC gameObjectDisplayInfoDBC = new GameObjectDisplayInfoDBC();
            gameObjectDisplayInfoDBC.LoadFromDisk(dbcInputFolder, "GameObjectDisplayInfo.dbc");
            ItemDBC itemDBC = new ItemDBC();
            itemDBC.LoadFromDisk(dbcInputFolder, "Item.dbc");
            ItemDisplayInfoDBC itemDisplayInfoDBC = new ItemDisplayInfoDBC();
            itemDisplayInfoDBC.LoadFromDisk(dbcInputFolder, "ItemDisplayInfo.dbc");
            LightDBC lightDBC = new LightDBC();
            lightDBC.LoadFromDisk(dbcInputFolder, "Light.dbc");
            LightFloatBandDBC lightFloatBandDBC = new LightFloatBandDBC();
            lightFloatBandDBC.LoadFromDisk(dbcInputFolder, "LightFloatBand.dbc");
            LightIntBandDBC lightIntBandDBC = new LightIntBandDBC();
            lightIntBandDBC.LoadFromDisk(dbcInputFolder, "LightIntBand.dbc");
            LightParamsDBC lightParamsDBC = new LightParamsDBC();
            lightParamsDBC.LoadFromDisk(dbcInputFolder, "LightParams.dbc");
            LiquidTypeDBC liquidTypeDBC = new LiquidTypeDBC();
            liquidTypeDBC.LoadFromDisk(dbcInputFolder, "LiquidType.dbc");
            LoadingScreensDBC loadingScreensDBC = new LoadingScreensDBC();
            loadingScreensDBC.LoadFromDisk(dbcInputFolder, "LoadingScreens.dbc");
            MapDBC mapDBCClient = new MapDBC();
            mapDBCClient.LoadFromDisk(dbcInputFolder, "Map.dbc");
            MapDBC mapDBCServer = new MapDBC();
            mapDBCServer.LoadFromDisk(dbcInputFolder, "Map.dbc");
            MapDifficultyDBC mapDifficultyDBC = new MapDifficultyDBC();
            mapDifficultyDBC.LoadFromDisk(dbcInputFolder, "MapDifficulty.dbc");
            //SkillLineDBC skillLineDBC = new SkillLineDBC();
            //skillLineDBC.LoadFromDisk(dbcInputFolder, "SkillLine.dbc");
            SkillLineAbilityDBC skillLineAbilityDBC = new SkillLineAbilityDBC();
            skillLineAbilityDBC.LoadFromDisk(dbcInputFolder, "SkillLineAbility.dbc");
            SoundAmbienceDBC soundAmbienceDBC = new SoundAmbienceDBC();
            soundAmbienceDBC.LoadFromDisk(dbcInputFolder, "SoundAmbience.dbc");
            SoundEntriesDBC soundEntriesDBC = new SoundEntriesDBC();
            soundEntriesDBC.LoadFromDisk(dbcInputFolder, "SoundEntries.dbc");
            SpellDBC spellDBC = new SpellDBC();
            spellDBC.LoadFromDisk(dbcInputFolder, "Spell.dbc");
            SpellCastTimesDBC spellCastTimesDBC = new SpellCastTimesDBC();
            spellCastTimesDBC.LoadFromDisk(dbcInputFolder, "SpellCastTimes.dbc");
            SpellDurationDBC spellDurationDBC = new SpellDurationDBC();
            spellDurationDBC.LoadFromDisk(dbcInputFolder, "SpellDuration.dbc");
            SpellIconDBC spellIconDBC = new SpellIconDBC();
            spellIconDBC.LoadFromDisk(dbcInputFolder, "SpellIcon.dbc");
            SpellItemEnchantmentDBC spellItemEnchantmentDBC = new SpellItemEnchantmentDBC();
            spellItemEnchantmentDBC.LoadFromDisk(dbcInputFolder, "SpellItemEnchantment.dbc");
            SpellRadiusDBC spellRadiusDBC = new SpellRadiusDBC();
            spellRadiusDBC.LoadFromDisk(dbcInputFolder, "SpellRadius.dbc");
            SpellRangeDBC spellRangeDBC = new SpellRangeDBC();
            spellRangeDBC.LoadFromDisk(dbcInputFolder, "SpellRange.dbc");
            SpellVisualDBC spellVisualDBC = new SpellVisualDBC();
            spellVisualDBC.LoadFromDisk(dbcInputFolder, "SpellVisual.dbc");
            SpellVisualEffectNameDBC spellVisualEffectNameDBC = new SpellVisualEffectNameDBC();
            spellVisualEffectNameDBC.LoadFromDisk(dbcInputFolder, "SpellVisualEffectName.dbc");
            SpellVisualKitDBC spellVisualKitDBC = new SpellVisualKitDBC();
            spellVisualKitDBC.LoadFromDisk(dbcInputFolder, "SpellVisualKit.dbc");
            TaxiPathDBC taxiPathDBC = new TaxiPathDBC();
            taxiPathDBC.LoadFromDisk(dbcInputFolder, "TaxiPath.dbc");
            TaxiPathNodeDBC taxiPathNodeDBC = new TaxiPathNodeDBC();
            taxiPathNodeDBC.LoadFromDisk(dbcInputFolder, "TaxiPathNode.dbc");
            TotemCategoryDBC totemCategoryDBC = new TotemCategoryDBC();
            totemCategoryDBC.LoadFromDisk(dbcInputFolder, "TotemCategory.dbc");
            TransportAnimationDBC transportAnimationDBC = new TransportAnimationDBC();
            transportAnimationDBC.LoadFromDisk(dbcInputFolder, "TransportAnimation.dbc");
            WorldSafeLocsDBC worldSafeLocsDBC = new WorldSafeLocsDBC();
            worldSafeLocsDBC.LoadFromDisk(dbcInputFolder, "WorldSafeLocs.dbc");
            WMOAreaTableDBC wmoAreaTableDBC = new WMOAreaTableDBC();
            wmoAreaTableDBC.LoadFromDisk(dbcInputFolder, "WMOAreaTable.dbc");
            ZoneMusicDBC zoneMusicDBC = new ZoneMusicDBC();
            zoneMusicDBC.LoadFromDisk(dbcInputFolder, "ZoneMusic.dbc");

            // Save common light parameters
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeather);
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeatherUnderwater);
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeather);
            lightFloatBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeatherUnderwater);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeather);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeatherUnderwater);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeather);
            lightIntBandDBC.AddRows(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeatherUnderwater);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeather);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersClearWeatherUnderwater);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeather);
            lightParamsDBC.AddRow(ZoneProperties.CommonOutdoorEnvironmentProperties.ParamatersStormyWeatherUnderwater);

            // Liquid is common
            liquidTypeDBC.AddRows();

            // LoadingScreens is common
            loadingScreensDBC.AddRow(Configuration.DBCID_LOADINGSCREEN_ID_START, "EQAntonica", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQClassic.blp");
            loadingScreensDBC.AddRow(Configuration.DBCID_LOADINGSCREEN_ID_START + 1, "EQKunark", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQKunark.blp");
            loadingScreensDBC.AddRow(Configuration.DBCID_LOADINGSCREEN_ID_START + 2, "EQVelious", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQVelious.blp");

            // Creatures sounds
            Dictionary<string, int> creatureFootstepIDBySoundNames = new Dictionary<string, int>();
            int curCreatureFootstepID = Configuration.DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START;
            foreach (CreatureModelTemplate creatureModelTemplate in creatureModelTemplates)
            {
                creatureDisplayInfoDBC.AddRow(creatureModelTemplate.DBCCreatureDisplayID, creatureModelTemplate.DBCCreatureModelDataID);
                string relativeModelPath = "Creature\\Everquest\\" + creatureModelTemplate.GetCreatureModelFolderName() + "\\" + creatureModelTemplate.GenerateFileName() + ".mdx";
                creatureModelDataDBC.AddRow(creatureModelTemplate, relativeModelPath);
                creatureSoundDataDBC.AddRow(creatureModelTemplate.DBCCreatureSoundDataID, creatureModelTemplate.Race, CreatureRace.FootstepIDBySoundName[creatureModelTemplate.Race.SoundWalkingName]);
            }
            string creatureSoundsDirectory = "Sound\\Creature\\Everquest";
            foreach (var sound in CreatureRace.SoundsBySoundName)
                soundEntriesDBC.AddRow(sound.Value, sound.Value.Name, creatureSoundsDirectory);

            // Faction
            foreach (CreatureFaction creatureFaction in CreatureFaction.GetCreatureFactionsByFactionID().Values)
            {
                factionDBC.AddRow(creatureFaction);
                if (creatureFaction.Name != Configuration.CREATURE_FACTION_ROOT_NAME)
                    factionTemplateDBC.AddRow(creatureFaction);
            }

            // Footstep Terrain Lookup (for creatures)
            foreach (var footstepIDBySoundID in CreatureRace.FootstepIDBySoundID)
                footstepTerrainLookupDBC.AddRow(footstepIDBySoundID.Value, footstepIDBySoundID.Key);

            // Zone-specific records
            foreach (Zone zone in zones)
            {
                ZoneProperties zoneProperties = zone.ZoneProperties;

                // AreaTable
                areaTableDBC.AddRow(Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zoneProperties.DBCMapID, 0, zone.DefaultArea.AreaMusic, zone.DefaultArea.AreaAmbientSound, zone.DefaultArea.DisplayName, zoneProperties.IsRestingZoneWide);
                foreach (ZoneArea subArea in zoneProperties.ZoneAreas)
                    areaTableDBC.AddRow(Convert.ToInt32(subArea.DBCAreaTableID), zoneProperties.DBCMapID, Convert.ToInt32(subArea.DBCParentAreaTableID), subArea.AreaMusic, subArea.AreaAmbientSound, subArea.DisplayName, zoneProperties.IsRestingZoneWide);

                // AreaTrigger
                foreach (ZonePropertiesZoneLineBox zoneLine in zoneProperties.ZoneLineBoxes)
                    areaTriggerDBC.AddRow(zoneLine.AreaTriggerID, zoneProperties.DBCMapID, zoneLine.BoxPosition.X, zoneLine.BoxPosition.Y,
                        zoneLine.BoxPosition.Z, zoneLine.BoxLength, zoneLine.BoxWidth, zoneLine.BoxHeight, zoneLine.BoxOrientation);

                // Light Tables
                if (zoneProperties.CustomZonewideEnvironmentProperties != null)
                {
                    ZoneEnvironmentSettings curZoneEnvironmentSettings = zoneProperties.CustomZonewideEnvironmentProperties;

                    lightDBC.AddRow(zoneProperties.DBCMapID, curZoneEnvironmentSettings);

                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeather);
                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeather);
                    lightFloatBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);

                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeather);
                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeather);
                    lightIntBandDBC.AddRows(curZoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);

                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersClearWeather);
                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersStormyWeather);
                    lightParamsDBC.AddRow(curZoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);
                }
                else
                {
                    lightDBC.AddRow(zoneProperties.DBCMapID, ZoneProperties.CommonOutdoorEnvironmentProperties);
                }

                // Map
                mapDBCClient.AddRow(zoneProperties.DBCMapID, "EQ_" + zone.ShortName, zone.DescriptiveName, 0, zone.LoadingScreenID);
                mapDBCServer.AddRow(zoneProperties.DBCMapID, "EQ_" + zone.ShortName, zone.DescriptiveName, Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zone.LoadingScreenID);

                // Map Difficulty
                mapDifficultyDBC.AddRow(zoneProperties.DBCMapID, zoneProperties.DBCMapDifficultyID);

                // Sound Ambience
                foreach (ZoneAreaAmbientSound zoneAreaAmbient in zone.ZoneAreaAmbientSounds)
                {
                    int soundIDDay = 0;
                    if (zoneAreaAmbient.DaySound != null)
                        soundIDDay = zoneAreaAmbient.DaySound.DBCID;
                    int soundIDNight = 0;
                    if (zoneAreaAmbient.NightSound != null)
                        soundIDNight = zoneAreaAmbient.NightSound.DBCID;
                    soundAmbienceDBC.AddRow(zoneAreaAmbient.DBCID, soundIDDay, soundIDNight);
                }

                // SoundEntries
                string musicDirectory = "Sound\\Music\\Everquest\\" + zoneProperties.ShortName;
                foreach (var sound in zone.MusicSoundsByFileNameNoExt)
                {
                    string fileNameWithExt = sound.Value.AudioFileNameNoExt + ".mp3";
                    soundEntriesDBC.AddRow(sound.Value, fileNameWithExt, musicDirectory);
                }
                string ambientSoundsDirectory = "Sound\\Ambience\\Everquest";
                foreach (var sound in zone.AmbientSoundsByFileNameNoExt)
                {
                    string fileNameWithExt = sound.Value.AudioFileNameNoExt + ".wav";
                    soundEntriesDBC.AddRow(sound.Value, fileNameWithExt, ambientSoundsDirectory);
                }
                foreach (SoundInstance soundInstance in zone.SoundInstances)
                {
                    if (soundInstance.Sound == null)
                        Logger.WriteError("Could not create SoundEntry.dbc record for sound instance '" + soundInstance.SoundFileNameDayNoExt + "' since the Sound was null");
                    else
                    {
                        string fileNameWithExt = soundInstance.Sound.AudioFileNameNoExt + ".wav";
                        soundEntriesDBC.AddRow(soundInstance.Sound, fileNameWithExt, ambientSoundsDirectory);
                    }
                }

                // WMOAreaTable (Header than groups)
                wmoAreaTableDBC.AddRow(Convert.ToInt32(zoneProperties.DBCWMOID), Convert.ToInt32(-1), 0, Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zone.DescriptiveName); // Header record
                foreach (ZoneModelObject wmo in zone.ZoneObjectModels)
                    wmoAreaTableDBC.AddRow(Convert.ToInt32(zoneProperties.DBCWMOID), Convert.ToInt32(wmo.WMOGroupID), 0, Convert.ToInt32(wmo.AreaTableID), wmo.DisplayName);

                // ZoneMusic
                foreach (ZoneAreaMusic zoneMusic in zone.ZoneAreaMusics)
                {
                    int soundIDDay = 0;
                    if (zoneMusic.DaySound != null)
                        soundIDDay = zoneMusic.DaySound.DBCID;
                    int soundIDNight = 0;
                    if (zoneMusic.NightSound != null)
                        soundIDNight = zoneMusic.NightSound.DBCID;
                    zoneMusicDBC.AddRow(zoneMusic.DBCID, zoneMusic.Name, soundIDDay, soundIDNight);
                }
            }

            // GameObjects
            if (Configuration.GENERATE_OBJECTS == true)
            {
                Dictionary<(string, GameObjectOpenType), int> gameObjectDisplayInfoIDsByModelNameAndOpenType = GameObject.GetGameObjectDisplayInfoIDsByModelNameAndOpenType();
                Dictionary<(string, GameObjectOpenType), ObjectModel> gameObjectModelsByNameAndOpenType = GameObject.GetNonDoodadObjectModelsByNameAndOpenType();
                Dictionary<(string, GameObjectOpenType), Sound> openSoundsByModelNameAndOpenType = GameObject.OpenSoundsByModelNameAndOpenType;
                Dictionary<(string, GameObjectOpenType), Sound> closeSoundsByModelNameAndOpenType = GameObject.CloseSoundsByModelNameAndOpenType;
                foreach (ValueTuple<string, GameObjectOpenType> nameAndOpenType in gameObjectDisplayInfoIDsByModelNameAndOpenType.Keys)
                {
                    // Sounds
                    int openSoundEntryID = 0;
                    int closeSoundEntryID = 0;
                    if (openSoundsByModelNameAndOpenType.ContainsKey(nameAndOpenType) == true)
                    {
                        openSoundEntryID = openSoundsByModelNameAndOpenType[nameAndOpenType].DBCID;
                        closeSoundEntryID = closeSoundsByModelNameAndOpenType[nameAndOpenType].DBCID;
                    }

                    // Display info
                    string fileName = string.Concat(nameAndOpenType.Item1, "_", nameAndOpenType.Item2.ToString());
                    string relativeObjectFileName = Path.Combine("World", "Everquest", "GameObjects", fileName, fileName + ".mdx");
                    gameObjectDisplayInfoDBC.AddRow(gameObjectDisplayInfoIDsByModelNameAndOpenType[nameAndOpenType],
                        relativeObjectFileName.ToLower(),
                        gameObjectModelsByNameAndOpenType[nameAndOpenType].VisibilityBoundingBox,
                        openSoundEntryID, closeSoundEntryID);
                }
                string soundDirectoryRelative = Path.Combine("Sound", "GameObjects");
                foreach (Sound sound in GameObject.AllSoundsBySoundName.Values)
                    soundEntriesDBC.AddRow(sound, sound.AudioFileNameNoExt + ".wav", soundDirectoryRelative);
            }

            // Graveyards
            Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();
            foreach (ZonePropertiesGraveyard graveyard in ZonePropertiesGraveyard.GetAllGraveyards())
            {
                if (zonePropertiesByShortName.ContainsKey(graveyard.LocationShortName) == true)
                {
                    int mapID = zonePropertiesByShortName[graveyard.LocationShortName].DBCMapID;
                    worldSafeLocsDBC.AddRow(graveyard, mapID);
                }
            }

            // Character start data
            // Must come before "Item Data"
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntry = ItemTemplate.GetItemTemplatesByWOWEntryID();
            if (Configuration.PLAYER_USE_EQ_START_ITEMS == true)
            {
                // Create the non-eq items to be used
                ItemTemplate itemHearthstone = new ItemTemplate(6948, ItemWOWInventoryType.NoEquip);
                itemHearthstone.IsGivenAsStartItem = true;
                itemHearthstone.IsExistingItemAlready = true;
                ItemTemplate itemTotem = new ItemTemplate(46978, ItemWOWInventoryType.NoEquip);
                itemTotem.IsGivenAsStartItem = true;
                itemTotem.IsExistingItemAlready = true;

                // Populate for all combinations, all races
                foreach (var classRaceProperties in PlayerClassRaceProperties.GetClassRacePropertiesByRaceAndClassID())
                {
                    // Grab all of the items
                    List<ItemTemplate> startingItems = new List<ItemTemplate>();
                    foreach (int itemID in classRaceProperties.Value.StartItemIDs)
                    {
                        if (itemID == 46978)
                            startingItems.Add(itemTotem);
                        else if (itemTemplatesByWOWEntry.ContainsKey(itemID) == false)
                            Logger.WriteError(string.Concat("Failed to pull startup item with wow entry id '", itemID, "' since it did not exist"));
                        else
                            startingItems.Add(itemTemplatesByWOWEntry[itemID]);
                    }

                    // Add the hearthstone if configured to do so
                    if (Configuration.PLAYER_ADD_HEARTHSTONE_IF_USE_EQ_START_ITEMS == true)
                        startingItems.Add(itemHearthstone);

                    // Add the rows
                    charStartOutfitDBC.AddRowsForSexes(Convert.ToByte(classRaceProperties.Value.RaceID), Convert.ToByte(classRaceProperties.Value.ClassID), startingItems);
                }
            }

            // Item data
            // Note: Must come AFTER character start data
            Dictionary<int, SpellTemplate> spellTemplatesByEQID = SpellTemplate.GetSpellTemplatesByEQID();
            foreach (ItemTemplate itemTemplate in itemTemplatesByWOWEntry.Values)
            {
                if (itemTemplate.IsExistingItemAlready == true)
                    continue;
                if (itemTemplate.DoesTeachSpell == true && itemTemplate.EQScrollSpellID != 0)
                {
                    // Spell scrolls get multiplied out by classes
                    if (Configuration.SPELLS_LEARNABLE_FROM_ITEMS_ENABLED == false || spellTemplatesByEQID.ContainsKey(itemTemplate.EQScrollSpellID) == false)
                        itemDBC.AddRow(itemTemplate, itemTemplate.WOWEntryID);
                    else
                    {
                        SpellTemplate spellTemplate = spellTemplatesByEQID[itemTemplate.EQScrollSpellID];
                        itemTemplate.ClassID = 9;
                        itemTemplate.SubClassID = 0;
                        foreach (var scrollPropertiesByClassType in spellTemplate.LearnScrollPropertiesByClassType)
                            itemDBC.AddRow(itemTemplate, scrollPropertiesByClassType.Value.WOWItemTemplateID);
                    }
                }
                else
                    itemDBC.AddRow(itemTemplate, itemTemplate.WOWEntryID);
            }
            foreach (ItemDisplayInfo itemDisplayInfo in ItemDisplayInfo.ItemDisplayInfos)
                itemDisplayInfoDBC.AddRow(itemDisplayInfo);

            // SkillLine
            //skillLineDBC.AddRow(Configuration.DBCID_SKILLLINE_ALTERATION_ID, "Alteration");            

            // Spells
            for (int i = 0; i < 23; i++)
                spellIconDBC.AddSpellIconRow(i);
            for (int i = 0; i < 751; i++)
                spellIconDBC.AddItemIconRow(i);
            foreach (SpellTemplate spellTemplate in spellTemplates)
            {
                // Effects max at three, so need to loop for them
                for (int i = 0; i < spellTemplate.GroupedBaseSpellEffectBlocksForOutput.Count; i++)
                {
                    SpellEffectBlock curEffectBlock = spellTemplate.GroupedBaseSpellEffectBlocksForOutput[i];
                    spellDBC.AddRow(curEffectBlock, spellTemplate.Description, spellTemplate, i != 0, spellTemplate.AuraDuration.IsInfinite, false, curEffectBlock.SpellEffects[0].CalcEffectHighLevel);

                    // Worn effects get their own copy too
                    if (spellTemplate.WOWSpellIDWorn > 0)
                    {
                        SpellEffectBlock curWornEffectBlock = spellTemplate.GroupedWornSpellEffectBlocksForOutput[i];
                        if (Configuration.ITEMS_SHOW_WORN_EFFECT_AURA_ICON == true)
                            spellDBC.AddRow(curWornEffectBlock, spellTemplate.AuraDescription, spellTemplate, i != 0, true, true, curWornEffectBlock.SpellEffects[0].CalcEffectHighLevel);
                        else
                            spellDBC.AddRow(curWornEffectBlock, spellTemplate.AuraDescription, spellTemplate, true, true, true, curWornEffectBlock.SpellEffects[0].CalcEffectHighLevel);
                    }
                }

                // Add the enchantment, if there is one
                if (spellTemplate.WeaponSpellItemEnchantmentDBCID != 0)
                {
                    spellItemEnchantmentDBC.AddRowForRogueWeaponProc(spellTemplate.WeaponSpellItemEnchantmentDBCID, spellTemplate.WeaponItemEnchantProcSpellID,
                        Configuration.SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE, spellTemplate.WeaponItemEnchantSpellName);
                }

                if (spellTemplate.SkillLine != 0)
                    skillLineAbilityDBC.AddRow(SkillLineAbilityDBC.GenerateID(), spellTemplate);
            }
            foreach (var spellCastTimeDBCIDByCastTime in SpellTemplate.SpellCastTimeDBCIDsByCastTime)
                spellCastTimesDBC.AddRow(spellCastTimeDBCIDByCastTime.Value, spellCastTimeDBCIDByCastTime.Key);
            foreach (var spellRangeDBCIDByRange in SpellTemplate.SpellRangeDBCIDsBySpellRange)
                spellRangeDBC.AddRow(spellRangeDBCIDByRange.Value, spellRangeDBCIDByRange.Key);
            foreach (var spellRadiusDBCIDByRadius in SpellTemplate.SpellRadiusDBCIDsBySpellRadius)
                spellRadiusDBC.AddRow(spellRadiusDBCIDByRadius.Value, spellRadiusDBCIDByRadius.Key);
            foreach (var soundByFileNameNoExt in SpellVisual.SoundsByFileNameNoExt)
            {
                string soundDirectoryRelative = Path.Combine("Sound", "Spells", "Everquest");
                soundEntriesDBC.AddRow(soundByFileNameNoExt.Value, soundByFileNameNoExt.Key + ".wav", soundDirectoryRelative);
            }
            foreach (SpellVisual spellVisual in SpellVisual.GetAllSpellVisuals())
            {
                spellVisualDBC.AddRow(spellVisual);
                for (int i = 0; i < 3; i++)
                {
                    int headEffectID = spellVisual.GetVisualIDForAttachLocationStage(SpellEmitterModelAttachLocationType.Head, (SpellVisualStageType)i);
                    int chestEffectID = spellVisual.GetVisualIDForAttachLocationStage(SpellEmitterModelAttachLocationType.Chest, (SpellVisualStageType)i);
                    int baseEffectID = spellVisual.GetVisualIDForAttachLocationStage(SpellEmitterModelAttachLocationType.Feet, (SpellVisualStageType)i);
                    int handEffectID = spellVisual.GetVisualIDForAttachLocationStage(SpellEmitterModelAttachLocationType.Hands, (SpellVisualStageType)i);
                    spellVisualKitDBC.AddRow(spellVisual, (SpellVisualStageType)i, headEffectID, chestEffectID, baseEffectID, handEffectID);
                }
            }
            foreach (ObjectModel objectModel in SpellVisual.GetAllEmitterObjectModels())
            {
                if (objectModel.Properties.SingleSpriteSpellParticleEmitters.Count > 0)
                {
                    string relativeMPQPath = Path.Combine("SPELLS", "Everquest", objectModel.Name, string.Concat(objectModel.Name, ".mdx"));
                    spellVisualEffectNameDBC.AddRow(objectModel.Properties.SpellVisualEffectNameDBCID, objectModel.Name, relativeMPQPath);
                }
            }
            // Row for auras (will be overwritten later by mod_everquest_spell
            spellDurationDBC.AddRow(Configuration.DBCID_SPELLDURATION_AURA_ID, -1);

            // Transports
            if (Configuration.GENERATE_TRANSPORTS == true)
            {
                foreach (var transportWMOByID in TransportShip.TransportShipWMOsByGameObjectDisplayInfoID)
                {
                    gameObjectDisplayInfoDBC.AddRow(transportWMOByID.Key, transportWMOByID.Value.RootFileRelativePathWithFileName.ToLower(), transportWMOByID.Value.BoundingBox);
                    wmoAreaTableDBC.AddRow(Convert.ToInt32(transportWMOByID.Value.Zone.ZoneProperties.DBCWMOID), Convert.ToInt32(-1), 0, 0, transportWMOByID.Value.Zone.DescriptiveName); // Header record
                    foreach (ZoneModelObject wmo in transportWMOByID.Value.Zone.ZoneObjectModels)
                        wmoAreaTableDBC.AddRow(Convert.ToInt32(transportWMOByID.Value.Zone.ZoneProperties.DBCWMOID), Convert.ToInt32(wmo.WMOGroupID), 0, 0, wmo.DisplayName);
                }
                Dictionary<string, int> mapIDsByShortName = new Dictionary<string, int>();
                foreach (Zone zone in zones)
                    mapIDsByShortName.Add(zone.ShortName.ToLower().Trim(), zone.ZoneProperties.DBCMapID);
                HashSet<int> validTransportGroupIDs = new HashSet<int>();
                foreach (TransportShip curTransportShip in TransportShip.GetAllTransportShips())
                {
                    // Only add this transport ship if the full path is zones that are loaded
                    bool zonesAreLoaded = true;
                    foreach (string touchedZone in curTransportShip.GetTouchedZonesSplitOut())
                    {
                        if (mapIDsByShortName.ContainsKey(touchedZone.ToLower().Trim()) == false)
                        {
                            zonesAreLoaded = false;
                            Logger.WriteDebug("Skipping transport ship since zone '" + touchedZone + "' isn't being converted");
                            break;
                        }
                    }
                    if (zonesAreLoaded == false)
                        continue;
                    validTransportGroupIDs.Add(curTransportShip.PathGroupID);

                    // TODO: Make loading screen configurable
                    mapDBCClient.AddRow(Convert.ToInt32(curTransportShip.MapID), curTransportShip.MeshName, curTransportShip.Name, 0, Configuration.DBCID_LOADINGSCREEN_ID_START);
                    mapDBCServer.AddRow(Convert.ToInt32(curTransportShip.MapID), curTransportShip.MeshName, curTransportShip.Name, 0, Configuration.DBCID_LOADINGSCREEN_ID_START);
                    taxiPathDBC.AddRow(curTransportShip.TaxiPathID);
                }
                foreach (TransportShipPathNode shipNode in TransportShipPathNode.GetAllPathNodesSorted())
                {
                    if (shipNode.WOWPathID == 0)
                        continue;
                    if (validTransportGroupIDs.Contains(shipNode.PathGroup) == false)
                        continue;
                    int mapID = mapIDsByShortName[shipNode.MapShortName.ToLower().Trim()];
                    taxiPathNodeDBC.AddRow(shipNode.WOWPathID, shipNode.StepNumber, mapID, shipNode.XPosition, shipNode.YPosition,
                        shipNode.ZPosition, shipNode.PauseTimeInSec);
                }
                foreach (var m2ByGameObjectID in TransportLift.ObjectModelM2ByMeshGameObjectDisplayID)
                {
                    string relativeObjectFileName = Path.Combine("World", "Everquest", "Transports", m2ByGameObjectID.Value.ObjectModel.Name, m2ByGameObjectID.Value.ObjectModel.Name + ".mdx");
                    gameObjectDisplayInfoDBC.AddRow(m2ByGameObjectID.Key, relativeObjectFileName.ToLower(), m2ByGameObjectID.Value.ObjectModel.VisibilityBoundingBox);
                }
                foreach (var m2ByGameObjectID in TransportLiftTrigger.ObjectModelM2ByMeshGameObjectDisplayID)
                {
                    string relativeObjectFileName = Path.Combine("World", "Everquest", "Transports", m2ByGameObjectID.Value.ObjectModel.Name, m2ByGameObjectID.Value.ObjectModel.Name + ".mdx");
                    int openSoundEntryID = 0;
                    if (m2ByGameObjectID.Value.ObjectModel.SoundsByAnimationType.ContainsKey(AnimationType.Open) == true)
                        openSoundEntryID = m2ByGameObjectID.Value.ObjectModel.SoundsByAnimationType[AnimationType.Open].DBCID;
                    int closeSoundEntryID = 0;
                    if (m2ByGameObjectID.Value.ObjectModel.SoundsByAnimationType.ContainsKey(AnimationType.Close) == true)
                        closeSoundEntryID = m2ByGameObjectID.Value.ObjectModel.SoundsByAnimationType[AnimationType.Close].DBCID;
                    gameObjectDisplayInfoDBC.AddRow(m2ByGameObjectID.Key, relativeObjectFileName.ToLower(), m2ByGameObjectID.Value.ObjectModel.VisibilityBoundingBox, openSoundEntryID, closeSoundEntryID);
                }
                foreach (TransportLiftPathNode pathNode in TransportLiftPathNode.GetAllPathNodesSorted())
                {
                    // Only add nodes for zones that are loaded
                    if (mapIDsByShortName.ContainsKey(pathNode.ZoneShortName.ToLower()) == false)
                        continue;
                    transportAnimationDBC.AddRow(pathNode.GameObjectTemplateEntryID, pathNode.TimestampInMS, pathNode.XPositionOffset, pathNode.YPositionOffset, pathNode.ZPositionOffset, pathNode.AnimationSequenceID);
                }
                string soundDirectoryRelative = Path.Combine("Sound", "EQTransports");
                foreach (Sound sound in TransportLiftTrigger.AllSoundsBySoundName.Values)
                    soundEntriesDBC.AddRow(sound, sound.AudioFileNameNoExt + ".wav", soundDirectoryRelative);
            }

            // Tradeskills
            int curTradeskillTotemCategoryID = Configuration.TRADESKILL_TOTEM_CATEGORY_START;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX), "Toolbox", curTradeskillTotemCategoryID, 1);
            curTradeskillTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING), "Sewing Kit", curTradeskillTotemCategoryID, 1);
            curTradeskillTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING), "Jeweler's Kit", curTradeskillTotemCategoryID, 1);
            curTradeskillTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY), "Medicine Bag", curTradeskillTotemCategoryID, 1);
            curTradeskillTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING), "Fletching Kit", curTradeskillTotemCategoryID, 1);
            Dictionary<string, UInt32> tradeskillTotems = TradeskillRecipe.GetTotemIDsByItemName();
            int curTradeskillTotemCategoryMaskValue = 1;
            int curTradeskillTotemCategoryMaskCount = 0;
            foreach (var tradeskillTotemData in tradeskillTotems)
            {
                totemCategoryDBC.AddRow(tradeskillTotemData.Value, tradeskillTotemData.Key, curTradeskillTotemCategoryID, curTradeskillTotemCategoryMaskValue);
                curTradeskillTotemCategoryMaskValue *= 2;
                curTradeskillTotemCategoryMaskCount++;
                if (curTradeskillTotemCategoryMaskCount > 10)
                {
                    curTradeskillTotemCategoryMaskValue = 1;
                    curTradeskillTotemCategoryMaskCount = 0;
                    curTradeskillTotemCategoryID++;
                }
            }

            // Save the files
            areaTableDBC.SaveToDisk(dbcOutputClientFolder);
            areaTableDBC.SaveToDisk(dbcOutputServerFolder);
            areaTriggerDBC.SaveToDisk(dbcOutputClientFolder);
            areaTriggerDBC.SaveToDisk(dbcOutputServerFolder);
            if (Configuration.PLAYER_USE_EQ_START_ITEMS == true)
            {
                charStartOutfitDBC.SaveToDisk(dbcOutputClientFolder);
                charStartOutfitDBC.SaveToDisk(dbcOutputServerFolder);
            }
            creatureDisplayInfoDBC.SaveToDisk(dbcOutputClientFolder);
            creatureDisplayInfoDBC.SaveToDisk(dbcOutputServerFolder);
            creatureModelDataDBC.SaveToDisk(dbcOutputClientFolder);
            creatureModelDataDBC.SaveToDisk(dbcOutputServerFolder);
            creatureSoundDataDBC.SaveToDisk(dbcOutputClientFolder);
            creatureSoundDataDBC.SaveToDisk(dbcOutputServerFolder);
            factionDBC.SaveToDisk(dbcOutputClientFolder);
            factionDBC.SaveToDisk(dbcOutputServerFolder);
            factionTemplateDBC.SaveToDisk(dbcOutputClientFolder);
            factionTemplateDBC.SaveToDisk(dbcOutputServerFolder);
            footstepTerrainLookupDBC.SaveToDisk(dbcOutputClientFolder);
            footstepTerrainLookupDBC.SaveToDisk(dbcOutputServerFolder);
            gameObjectDisplayInfoDBC.SaveToDisk(dbcOutputClientFolder);
            gameObjectDisplayInfoDBC.SaveToDisk(dbcOutputServerFolder);
            itemDBC.SaveToDisk(dbcOutputClientFolder);
            itemDBC.SaveToDisk(dbcOutputServerFolder);
            itemDisplayInfoDBC.SaveToDisk(dbcOutputClientFolder);
            itemDisplayInfoDBC.SaveToDisk(dbcOutputServerFolder);
            lightDBC.SaveToDisk(dbcOutputClientFolder);
            lightDBC.SaveToDisk(dbcOutputServerFolder);
            lightFloatBandDBC.SaveToDisk(dbcOutputClientFolder);
            lightFloatBandDBC.SaveToDisk(dbcOutputServerFolder);
            lightIntBandDBC.SaveToDisk(dbcOutputClientFolder);
            lightIntBandDBC.SaveToDisk(dbcOutputServerFolder);
            lightParamsDBC.SaveToDisk(dbcOutputClientFolder);
            lightParamsDBC.SaveToDisk(dbcOutputServerFolder);
            liquidTypeDBC.SaveToDisk(dbcOutputClientFolder);
            liquidTypeDBC.SaveToDisk(dbcOutputServerFolder);
            loadingScreensDBC.SaveToDisk(dbcOutputClientFolder);
            loadingScreensDBC.SaveToDisk(dbcOutputServerFolder);
            mapDBCClient.SaveToDisk(dbcOutputClientFolder);
            mapDBCServer.SaveToDisk(dbcOutputServerFolder);
            mapDifficultyDBC.SaveToDisk(dbcOutputClientFolder);
            mapDifficultyDBC.SaveToDisk(dbcOutputServerFolder);
            //skillLineDBC.SaveToDisk(dbcOutputClientFolder);
            //skillLineDBC.SaveToDisk(dbcOutputServerFolder);
            skillLineAbilityDBC.SaveToDisk(dbcOutputClientFolder);
            skillLineAbilityDBC.SaveToDisk(dbcOutputServerFolder);
            soundAmbienceDBC.SaveToDisk(dbcOutputClientFolder);
            soundAmbienceDBC.SaveToDisk(dbcOutputServerFolder);
            soundEntriesDBC.SaveToDisk(dbcOutputClientFolder);
            soundEntriesDBC.SaveToDisk(dbcOutputServerFolder);
            spellDBC.SaveToDisk(dbcOutputClientFolder);
            spellDBC.SaveToDisk(dbcOutputServerFolder);
            spellCastTimesDBC.SaveToDisk(dbcOutputClientFolder);
            spellCastTimesDBC.SaveToDisk(dbcOutputServerFolder);
            spellDurationDBC.SaveToDisk(dbcOutputClientFolder);
            spellDurationDBC.SaveToDisk(dbcOutputServerFolder);
            spellIconDBC.SaveToDisk(dbcOutputClientFolder);
            spellIconDBC.SaveToDisk(dbcOutputServerFolder);
            spellItemEnchantmentDBC.SaveToDisk(dbcOutputClientFolder);
            spellItemEnchantmentDBC.SaveToDisk(dbcOutputServerFolder);
            spellRadiusDBC.SaveToDisk(dbcOutputClientFolder);
            spellRadiusDBC.SaveToDisk(dbcOutputServerFolder);
            spellRangeDBC.SaveToDisk(dbcOutputClientFolder);
            spellRangeDBC.SaveToDisk(dbcOutputServerFolder);
            spellVisualDBC.SaveToDisk(dbcOutputClientFolder);
            spellVisualDBC.SaveToDisk(dbcOutputServerFolder);
            spellVisualEffectNameDBC.SaveToDisk(dbcOutputClientFolder);
            spellVisualEffectNameDBC.SaveToDisk(dbcOutputServerFolder);
            spellVisualKitDBC.SaveToDisk(dbcOutputClientFolder);
            spellVisualKitDBC.SaveToDisk(dbcOutputServerFolder);
            taxiPathDBC.SaveToDisk(dbcOutputClientFolder);
            taxiPathDBC.SaveToDisk(dbcOutputServerFolder);
            taxiPathNodeDBC.SaveToDisk(dbcOutputClientFolder);
            taxiPathNodeDBC.SaveToDisk(dbcOutputServerFolder);
            totemCategoryDBC.SaveToDisk(dbcOutputClientFolder);
            totemCategoryDBC.SaveToDisk(dbcOutputServerFolder);
            transportAnimationDBC.SaveToDisk(dbcOutputClientFolder);
            transportAnimationDBC.SaveToDisk(dbcOutputServerFolder);
            worldSafeLocsDBC.SaveToDisk(dbcOutputClientFolder);
            worldSafeLocsDBC.SaveToDisk(dbcOutputServerFolder);
            wmoAreaTableDBC.SaveToDisk(dbcOutputClientFolder);
            wmoAreaTableDBC.SaveToDisk(dbcOutputServerFolder);
            zoneMusicDBC.SaveToDisk(dbcOutputClientFolder);
            zoneMusicDBC.SaveToDisk(dbcOutputServerFolder);

            Logger.WriteDebug("Creating DBC Files complete");
        }
    }
}
