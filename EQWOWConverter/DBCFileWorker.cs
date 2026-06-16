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
        private AreaTableDBC areaTableDBC = new AreaTableDBC();
        private AreaTriggerDBC areaTriggerDBC = new AreaTriggerDBC();
        private CharStartOutfitDBC charStartOutfitDBC = new CharStartOutfitDBC();
        private CreatureDisplayInfoDBC creatureDisplayInfoDBC = new CreatureDisplayInfoDBC();
        private CreatureDisplayInfoExtraDBC creatureDisplayInfoExtraDBC = new CreatureDisplayInfoExtraDBC();
        private CreatureModelDataDBC creatureModelDataDBC = new CreatureModelDataDBC();
        private CreatureSoundDataDBC creatureSoundDataDBC = new CreatureSoundDataDBC();
        private FactionDBC factionDBC = new FactionDBC();
        private FactionTemplateDBC factionTemplateDBC = new FactionTemplateDBC();
        private FootstepTerrainLookupDBC footstepTerrainLookupDBC = new FootstepTerrainLookupDBC();
        private GameObjectDisplayInfoDBC gameObjectDisplayInfoDBC = new GameObjectDisplayInfoDBC();
        private ItemDBC itemDBC = new ItemDBC();
        private ItemDisplayInfoDBC itemDisplayInfoDBC = new ItemDisplayInfoDBC();
        private LFGDungeonGroupDBC lfgDungeonGroupDBC = new LFGDungeonGroupDBC();
        private LightDBC lightDBC = new LightDBC();
        private LightFloatBandDBC lightFloatBandDBC = new LightFloatBandDBC();
        private LightIntBandDBC lightIntBandDBC = new LightIntBandDBC();
        private LightParamsDBC lightParamsDBC = new LightParamsDBC();
        private LiquidTypeDBC liquidTypeDBC = new LiquidTypeDBC();
        private LoadingScreensDBC loadingScreensDBC = new LoadingScreensDBC();
        private MapDBC mapDBC = new MapDBC();
        private MapDifficultyDBC mapDifficultyDBC = new MapDifficultyDBC();
        private SkillLineDBC skillLineDBC = new SkillLineDBC();
        private SkillLineAbilityDBC skillLineAbilityDBC = new SkillLineAbilityDBC();
        private SkillRaceClassInfoDBC skillRaceClassInfoDBC = new SkillRaceClassInfoDBC();
        private SoundAmbienceDBC soundAmbienceDBC = new SoundAmbienceDBC();
        private SoundEntriesDBC soundEntriesDBC = new SoundEntriesDBC();
        private SpellDBC spellDBC = new SpellDBC();
        private SpellCastTimesDBC spellCastTimesDBC = new SpellCastTimesDBC();
        private SpellDurationDBC spellDurationDBC = new SpellDurationDBC();
        private SpellIconDBC spellIconDBC = new SpellIconDBC();
        private SpellItemEnchantmentDBC spellItemEnchantmentDBC = new SpellItemEnchantmentDBC();
        private SpellRadiusDBC spellRadiusDBC = new SpellRadiusDBC();
        private SpellRangeDBC spellRangeDBC = new SpellRangeDBC();
        private SpellVisualDBC spellVisualDBC = new SpellVisualDBC();
        private SpellVisualEffectNameDBC spellVisualEffectNameDBC = new SpellVisualEffectNameDBC();
        private SpellVisualKitDBC spellVisualKitDBC = new SpellVisualKitDBC();
        private SummonPropertiesDBC summonPropertiesDBC = new SummonPropertiesDBC();
        private TaxiPathDBC taxiPathDBC = new TaxiPathDBC();
        private TaxiPathNodeDBC taxiPathNodeDBC = new TaxiPathNodeDBC();
        private TotemCategoryDBC totemCategoryDBC = new TotemCategoryDBC();
        private TransportAnimationDBC transportAnimationDBC = new TransportAnimationDBC();
        private WorldMapAreaDBC worldMapAreaDBC = new WorldMapAreaDBC();
        private WorldSafeLocsDBC worldSafeLocsDBC = new WorldSafeLocsDBC();
        private WMOAreaTableDBC wmoAreaTableDBC = new WMOAreaTableDBC();
        private ZoneMusicDBC zoneMusicDBC = new ZoneMusicDBC();

        public void ExtractClientDBCFiles()
        {
            string wowExportPath = Configuration.PATH_EXPORT_FOLDER;

            Logger.WriteInfo("Extracting client DBC files...");

            // Make sure the patches folder is correct
            string wowPatchesFolder = Path.Combine(Configuration.PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER, "Data", Configuration.PATCH_LOCALIZATION_STRING);
            if (Directory.Exists(wowPatchesFolder) == false)
                throw new Exception("WoW client patches folder does not exist at '" + wowPatchesFolder + "', did you set PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER?");

            // Get a list of valid patch files (it's done this way to ensure sorting order is exactly right). Also ignore existing patch file
            string dataLocPatchNameNoExt = string.Concat("patch-", Configuration.PATCH_LOCALIZATION_STRING, "-", Configuration.PATCH_CLIENT_DATA_LOC_ID);
            List<string> patchFileNames = new List<string>();
            patchFileNames.Add(Path.Combine(wowPatchesFolder, string.Concat("locale-", Configuration.PATCH_LOCALIZATION_STRING, ".MPQ")));
            patchFileNames.Add(Path.Combine(wowPatchesFolder, string.Concat("patch-", Configuration.PATCH_LOCALIZATION_STRING, ".MPQ")));
            string[] existingPatchFiles = Directory.GetFiles(wowPatchesFolder, "patch-*-*.MPQ");
            foreach (string existingPatchName in existingPatchFiles)
                if (existingPatchName.Contains(dataLocPatchNameNoExt) == false)
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

        private void AddSpellDataBlock(SpellTemplate spellTemplate, List<SpellEffectBlock> spellEffectBlocks, int castTimeDBCID, bool isWorn)
        {
            if (spellEffectBlocks.Count == 0 || spellEffectBlocks[0].WOWSpellID <= 0)
                return;

            for (int i = 0; i < spellEffectBlocks.Count; i++)
            {
                SpellEffectBlock curEffectBlock = spellEffectBlocks[i];
                if (isWorn == false)
                {
                    // Don't hide the chain spells if there's an aura under the non-aura
                    bool hideFromDisplay = (i != 0) && (curEffectBlock.ForceVisibleSplitAura == false);
                    spellDBC.AddRow(curEffectBlock, spellTemplate.Description, spellTemplate, hideFromDisplay, spellTemplate.AuraDuration.IsInfinite, spellTemplate.PreventAuraClickOff,
                        curEffectBlock.SpellEffects[0].CalcEffectHighLevel, spellTemplate.IsToggleAura, castTimeDBCID);
                }
                else
                {
                    if (Configuration.ITEMS_SHOW_WORN_EFFECT_AURA_ICON == true)
                        spellDBC.AddRow(curEffectBlock, spellTemplate.AuraDescription, spellTemplate, i != 0, true, true,
                            curEffectBlock.SpellEffects[0].CalcEffectHighLevel, spellTemplate.IsToggleAura, castTimeDBCID);
                    else
                        spellDBC.AddRow(curEffectBlock, spellTemplate.AuraDescription, spellTemplate, true, true, true,
                            curEffectBlock.SpellEffects[0].CalcEffectHighLevel, spellTemplate.IsToggleAura, castTimeDBCID);
                }
            }

            // Skill-bound spells
            if (spellTemplate.SkillLine != 0)
                skillLineAbilityDBC.AddRow(SkillLineAbilityDBC.GenerateID(), spellTemplate, spellEffectBlocks[0].WOWSpellID);
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
            areaTableDBC.LoadFromDisk(dbcInputFolder, "AreaTable.dbc");
            areaTriggerDBC.LoadFromDisk(dbcInputFolder, "AreaTrigger.dbc");
            charStartOutfitDBC.LoadFromDisk(dbcInputFolder, "CharStartOutfit.dbc");
            creatureDisplayInfoDBC.LoadFromDisk(dbcInputFolder, "CreatureDisplayInfo.dbc");
            creatureDisplayInfoExtraDBC.LoadFromDisk(dbcInputFolder, "CreatureDisplayInfoExtra.dbc");
            creatureModelDataDBC.LoadFromDisk(dbcInputFolder, "CreatureModelData.dbc");
            creatureSoundDataDBC.LoadFromDisk(dbcInputFolder, "CreatureSoundData.dbc");
            factionDBC.LoadFromDisk(dbcInputFolder, "Faction.dbc");
            factionTemplateDBC.LoadFromDisk(dbcInputFolder, "FactionTemplate.dbc");
            footstepTerrainLookupDBC.LoadFromDisk(dbcInputFolder, "FootstepTerrainLookup.dbc");
            gameObjectDisplayInfoDBC.LoadFromDisk(dbcInputFolder, "GameObjectDisplayInfo.dbc");
            itemDBC.LoadFromDisk(dbcInputFolder, "Item.dbc");
            itemDisplayInfoDBC.LoadFromDisk(dbcInputFolder, "ItemDisplayInfo.dbc");
            lfgDungeonGroupDBC.LoadFromDisk(dbcInputFolder, "LFGDungeonGroup.dbc");
            lightDBC.LoadFromDisk(dbcInputFolder, "Light.dbc");
            lightFloatBandDBC.LoadFromDisk(dbcInputFolder, "LightFloatBand.dbc");
            lightIntBandDBC.LoadFromDisk(dbcInputFolder, "LightIntBand.dbc");
            lightParamsDBC.LoadFromDisk(dbcInputFolder, "LightParams.dbc");
            liquidTypeDBC.LoadFromDisk(dbcInputFolder, "LiquidType.dbc");
            loadingScreensDBC.LoadFromDisk(dbcInputFolder, "LoadingScreens.dbc");
            mapDBC.LoadFromDisk(dbcInputFolder, "Map.dbc");
            mapDifficultyDBC.LoadFromDisk(dbcInputFolder, "MapDifficulty.dbc");
            skillLineDBC.LoadFromDisk(dbcInputFolder, "SkillLine.dbc");
            skillLineAbilityDBC.LoadFromDisk(dbcInputFolder, "SkillLineAbility.dbc");
            skillRaceClassInfoDBC.LoadFromDisk(dbcInputFolder, "SkillRaceClassInfo.dbc");
            soundAmbienceDBC.LoadFromDisk(dbcInputFolder, "SoundAmbience.dbc");
            soundEntriesDBC.LoadFromDisk(dbcInputFolder, "SoundEntries.dbc");
            spellDBC.LoadFromDisk(dbcInputFolder, "Spell.dbc");
            spellCastTimesDBC.LoadFromDisk(dbcInputFolder, "SpellCastTimes.dbc");
            spellDurationDBC.LoadFromDisk(dbcInputFolder, "SpellDuration.dbc");
            spellIconDBC.LoadFromDisk(dbcInputFolder, "SpellIcon.dbc");
            spellItemEnchantmentDBC.LoadFromDisk(dbcInputFolder, "SpellItemEnchantment.dbc");
            spellRadiusDBC.LoadFromDisk(dbcInputFolder, "SpellRadius.dbc");
            spellRangeDBC.LoadFromDisk(dbcInputFolder, "SpellRange.dbc");
            spellVisualDBC.LoadFromDisk(dbcInputFolder, "SpellVisual.dbc");
            spellVisualEffectNameDBC.LoadFromDisk(dbcInputFolder, "SpellVisualEffectName.dbc");
            spellVisualKitDBC.LoadFromDisk(dbcInputFolder, "SpellVisualKit.dbc");
            summonPropertiesDBC.LoadFromDisk(dbcInputFolder, "SummonProperties.dbc");
            taxiPathDBC.LoadFromDisk(dbcInputFolder, "TaxiPath.dbc");
            taxiPathNodeDBC.LoadFromDisk(dbcInputFolder, "TaxiPathNode.dbc");
            totemCategoryDBC.LoadFromDisk(dbcInputFolder, "TotemCategory.dbc");
            transportAnimationDBC.LoadFromDisk(dbcInputFolder, "TransportAnimation.dbc");
            worldMapAreaDBC.LoadFromDisk(dbcInputFolder, "WorldMapArea.dbc");            
            worldSafeLocsDBC.LoadFromDisk(dbcInputFolder, "WorldSafeLocs.dbc");
            wmoAreaTableDBC.LoadFromDisk(dbcInputFolder, "WMOAreaTable.dbc");
            zoneMusicDBC.LoadFromDisk(dbcInputFolder, "ZoneMusic.dbc");

            // Liquid is common
            liquidTypeDBC.AddRows();

            // LoadingScreens is common
            loadingScreensDBC.AddRow(Configuration.DBCID_LOADINGSCREEN_ID_START, "EQAntonica", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQClassic.blp");
            loadingScreensDBC.AddRow(Configuration.DBCID_LOADINGSCREEN_ID_START + 1, "EQKunark", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQKunark.blp");
            loadingScreensDBC.AddRow(Configuration.DBCID_LOADINGSCREEN_ID_START + 2, "EQVelious", "Interface\\Glues\\LoadingScreens\\LoadingScreenEQVelious.blp");

            // Creatures
            Dictionary<string, int> creatureFootstepIDBySoundNames = new Dictionary<string, int>();
            int curCreatureFootstepID = Configuration.DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START;
            foreach (CreatureModelTemplate creatureModelTemplate in creatureModelTemplates)
            {
                creatureDisplayInfoDBC.AddRow(creatureModelTemplate.DBCCreatureDisplayID, creatureModelTemplate.DBCCreatureModelDataID);
                string relativeModelPath = "Creature\\Everquest\\" + creatureModelTemplate.GetCreatureModelFolderName() + "\\" + creatureModelTemplate.GenerateFileName() + ".mdx";
                creatureModelDataDBC.AddRow(creatureModelTemplate, relativeModelPath);
                if (creatureModelTemplate.Race.SoundWalkingName.Trim().Length > 0)
                    creatureSoundDataDBC.AddRow(creatureModelTemplate.DBCCreatureSoundDataID, creatureModelTemplate.Race, CreatureRace.FootstepIDBySoundName[creatureModelTemplate.Race.SoundWalkingName]);
            }
            string creatureSoundsDirectory = "Sound\\Creature\\Everquest";
            foreach (var soundByName in CreatureRace.SoundsBySoundNameAndDistance)
                foreach (var soundByDistance in soundByName.Value)
                    soundEntriesDBC.AddRow(soundByDistance.Value, soundByDistance.Value.Name, creatureSoundsDirectory);

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

            // Dungeon Finder Specific
            if (Configuration.DUNGEON_FINDER_ENABLED == true)
            {
                lfgDungeonGroupDBC.AddRow(Configuration.DBCID_LFGDUNGEONGROUP_DUNGEONS_ID, "EverQuest", Configuration.DBCID_LFGDUNGEONGROUP_DUNGEONS_ORDER_ID, false);
                lfgDungeonGroupDBC.AddRow(Configuration.DBCID_LFGDUNGEONGROUP_RAIDS_ID, "EverQuest Raid", Configuration.DBCID_LFGDUNGEONGROUP_RAIDS_ORDER_ID, true);
            }

            // Zone-specific records
            List<ZoneContinent> zoneContinents = ZoneContinent.GetZoneContinents();
            Dictionary<ZoneContinentType, ZoneContinent> zoneContinentsByContinentType = new Dictionary<ZoneContinentType, ZoneContinent>();
            foreach (ZoneContinent continent in zoneContinents)
            {
                string mapFolderName = string.Concat("EQ_", continent.ShortName);

                // Map
                mapDBC.AddRow(continent.DBCMapID, mapFolderName, continent.DescriptiveName, 0, 0);

                // World Map Area
                worldMapAreaDBC.AddRow(continent.DBCWorldMapAreaID, continent.DBCMapID, 0, mapFolderName, 1, -1, 1, -1, 0);
                zoneContinentsByContinentType.Add(continent.ContinentType, continent);
            }
            foreach (Zone zone in zones)
            {
                ZoneProperties zoneProperties = zone.ZoneProperties;

                // AreaTable
                areaTableDBC.AddRow(Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zoneProperties.DBCMapID, 0, zone.DefaultArea.AreaMusic, zone.DefaultArea.AreaAmbientSound, zone.DefaultArea.DisplayName, zoneProperties.IsRestingZoneWide, zone.DefaultArea.DoShowBreath);
                foreach (ZoneArea subArea in zoneProperties.SubZoneAreas)
                    areaTableDBC.AddRow(Convert.ToInt32(subArea.DBCAreaTableID), zoneProperties.DBCMapID, Convert.ToInt32(subArea.DBCParentAreaTableID), subArea.AreaMusic, subArea.AreaAmbientSound, subArea.DisplayName, zoneProperties.IsRestingZoneWide, subArea.DoShowBreath);

                // AreaTrigger
                foreach (ZonePropertiesZoneLineBox zoneLine in zoneProperties.ZoneLineBoxes)
                    areaTriggerDBC.AddRow(zoneLine.AreaTriggerID, zoneProperties.DBCMapID, zoneLine.BoxPosition.X, zoneLine.BoxPosition.Y,
                        zoneLine.BoxPosition.Z, zoneLine.BoxLength, zoneLine.BoxWidth, zoneLine.BoxHeight, zoneLine.BoxOrientation);

                // Light Tables
                AddLightData(zoneProperties.DBCMapID, zoneProperties.ZonewideEnvironmentProperties);
                foreach (ZoneEnvironmentSettings areaLightSettings in zoneProperties.AreaLightZoneEnvironmentProperties)
                    AddLightData(zoneProperties.DBCMapID, areaLightSettings);

                // Map
                mapDBC.AddRow(zoneProperties.DBCMapID, "EQ_" + zone.ShortName, zone.DescriptiveName, Convert.ToInt32(zone.DefaultArea.DBCAreaTableID), zone.LoadingScreenID);

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

                // World map (in-game display)
                if (Configuration.GENERATE_WORLDMAPS == true)
                {
                    string mapFolderName = string.Concat("EQ_", zoneProperties.ShortName);
                    int parentWorldMapID = 0;
                    if (zoneContinentsByContinentType.ContainsKey(zoneProperties.Continent) == true)
                        parentWorldMapID = zoneContinentsByContinentType[zoneProperties.Continent].DBCWorldMapAreaID;
                    worldMapAreaDBC.AddRow(zoneProperties.DBCWorldMapAreaID, zoneProperties.DBCMapID, Convert.ToInt32(zoneProperties.DefaultZoneArea.DBCAreaTableID), mapFolderName,
                        zoneProperties.DisplayMapMainLeft, zoneProperties.DisplayMapMainRight, zoneProperties.DisplayMapMainTop, zoneProperties.DisplayMapMainBottom, parentWorldMapID);
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

                    BoundingBox geoboxBounding = new BoundingBox(gameObjectModelsByNameAndOpenType[nameAndOpenType].InteractionBoundingBox);

                    // This is a bit of a hack right now to ensure that rotated objects are always interactable
                    float maxDistance = Math.Max(geoboxBounding.GetZDistance(), Math.Max(geoboxBounding.GetYDistance(), geoboxBounding.GetXDistance()));
                    geoboxBounding.ExpandToMinimumSize(maxDistance);

                    // Display info
                    string fileName = string.Concat(nameAndOpenType.Item1, "_", nameAndOpenType.Item2.ToString());
                    string relativeObjectFileName = Path.Combine("World", "Everquest", "GameObjects", fileName, fileName + ".mdx");
                    gameObjectDisplayInfoDBC.AddRow(gameObjectDisplayInfoIDsByModelNameAndOpenType[nameAndOpenType],
                        relativeObjectFileName.ToLower(),
                        geoboxBounding,
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
                        itemDBC.AddRow(itemTemplate, itemTemplate.WOWEntryID, itemTemplate.ItemDisplayInfo);
                    else
                    {
                        SpellTemplate spellTemplate = spellTemplatesByEQID[itemTemplate.EQScrollSpellID];
                        itemTemplate.ClassID = 9;
                        itemTemplate.SubClassID = 0;
                        foreach (var scrollPropertiesByClassType in spellTemplate.LearnScrollPropertiesByClassType)
                            itemDBC.AddRow(itemTemplate, scrollPropertiesByClassType.Value.WOWItemTemplateID, itemTemplate.ItemDisplayInfo);
                    }
                }
                else
                {
                    // Regular items (2 versions, one for creatures and one for players)
                    itemDBC.AddRow(itemTemplate, itemTemplate.WOWEntryID, itemTemplate.ItemDisplayInfo);
                    if (itemTemplate.ItemDisplayInfoForCreatureEquip != null)
                        itemDBC.AddRow(itemTemplate, itemTemplate.WOWEntryIDForNPCEquip, itemTemplate.ItemDisplayInfoForCreatureEquip);

                    // Sometimes a third if it's a starter item
                    if (itemTemplate.StarterVersionItemTemplateID != -1)
                        itemDBC.AddRow(itemTemplate, itemTemplate.StarterVersionItemTemplateID, itemTemplate.ItemDisplayInfo);
                }
            }
            foreach (ItemDisplayInfo itemDisplayInfo in ItemDisplayInfo.ItemDisplayInfos)
                itemDisplayInfoDBC.AddRow(itemDisplayInfo);
            // This is hard coded for IT159 (Celestial Fists / Monk Epic)
            spellVisualDBC.AddRow(ItemDisplayInfo.IT159SpellVisualID, 0, 0, 0, ItemDisplayInfo.IT159SpellVisualStateKitID);
            spellVisualKitDBC.AddRow(ItemDisplayInfo.IT159SpellVisualStateKitID, -1, 0, 0, 0, 0, ItemDisplayInfo.IT159SpellVisualEffectNameID);
            spellVisualEffectNameDBC.AddRow(ItemDisplayInfo.IT159SpellVisualEffectNameID, "EQ Celestial Fists", ItemDisplayInfo.IT159RelativeFileName);

            // SkillLine
            Dictionary<SpellEQSkillCategory, int> skillLineIDsBySkillCategory = SkillLineDBC.GetAllSkillLineIDsBySkillCategory();
            foreach (var skillLineIDBySkillCategory in skillLineIDsBySkillCategory)
            {
                string name = skillLineIDBySkillCategory.Key.ToString();
                skillLineDBC.AddRow(skillLineIDBySkillCategory.Value, name, 1);
                skillRaceClassInfoDBC.AddRow(skillLineIDBySkillCategory.Value, new List<ClassWOWType>() { ClassWOWType.All });
            }

            // Skills
            if (Configuration.PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES == true)
            {
                List<ClassWOWType> wowClassTypes = new List<ClassWOWType>();
                wowClassTypes.Add(ClassWOWType.All);
                skillRaceClassInfoDBC.AddRow(433, wowClassTypes);
            }
            if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES == true)
            {
                List<ClassWOWType> leatherClasses = PlayerClassMapping.GetWOWClassesEligibleForArmorType(ItemWOWArmorSubclassType.Leather).ToList();
                skillRaceClassInfoDBC.AddRow(414, leatherClasses);
                List<ClassWOWType> mailClasses = PlayerClassMapping.GetWOWClassesEligibleForArmorType(ItemWOWArmorSubclassType.Mail).ToList();
                skillRaceClassInfoDBC.AddRow(413, mailClasses);
                List<ClassWOWType> plateClasses = PlayerClassMapping.GetWOWClassesEligibleForArmorType(ItemWOWArmorSubclassType.Plate).ToList();
                skillRaceClassInfoDBC.AddRow(293, plateClasses);
            }
            if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES == true)
            {
                List<ClassWOWType> axeOneHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.AxeOneHand).ToList();
                skillRaceClassInfoDBC.AddRow(44, axeOneHandClasses);
                List<ClassWOWType> axeTwoHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.AxeTwoHand).ToList();
                skillRaceClassInfoDBC.AddRow(172, axeTwoHandClasses);
                List<ClassWOWType> maceOneHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.MaceOneHand).ToList();
                skillRaceClassInfoDBC.AddRow(54, maceOneHandClasses);
                List<ClassWOWType> maceTwoHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.MaceTwoHand).ToList();
                skillRaceClassInfoDBC.AddRow(160, maceTwoHandClasses);
                List<ClassWOWType> polearmClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Polearm).ToList();
                skillRaceClassInfoDBC.AddRow(229, polearmClasses);
                List<ClassWOWType> swordOneHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.SwordOneHand).ToList();
                skillRaceClassInfoDBC.AddRow(43, swordOneHandClasses);
                List<ClassWOWType> swordTwoHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.SwordTwoHand).ToList();
                skillRaceClassInfoDBC.AddRow(55, swordTwoHandClasses);
                List<ClassWOWType> staffClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Staff).ToList();
                skillRaceClassInfoDBC.AddRow(136, staffClasses);
                List<ClassWOWType> fistWeaponClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.FistWeapon).ToList();
                skillRaceClassInfoDBC.AddRow(473, fistWeaponClasses);
                List<ClassWOWType> daggerClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Dagger).ToList();
                skillRaceClassInfoDBC.AddRow(173, daggerClasses);
            }
            if (Configuration.PLAYER_SKILL_ENABLE_BOWS_ON_ALL_APPROPRIATE_EQ_ALIGNED_CLASSES == true)
            {
                List<ClassWOWType> bowClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Bow).ToList();
                skillRaceClassInfoDBC.AddRow(45, bowClasses);
            }

            // Spells
            for (int i = 0; i < 23; i++)
                spellIconDBC.AddSpellIconRow(i);
            for (int i = 0; i < 751; i++)
                spellIconDBC.AddItemIconRow(i);
            foreach (SpellTemplate spellTemplate in spellTemplates)
            {
                // Block-specific data
                AddSpellDataBlock(spellTemplate, spellTemplate.GroupedBaseSpellEffectBlocksForOutput, spellTemplate.SpellCastTimeDBCID, false);
                AddSpellDataBlock(spellTemplate, spellTemplate.GroupedWornSpellEffectBlocksForOutput, 1, true);
                AddSpellDataBlock(spellTemplate, spellTemplate.GroupedGoodProcSpellEffectBlocksForOutput, 1, false);
                for (int i = 0; i < spellTemplate.GroupedClickySpellEffectBlocksForOutputBySpellParameters.Count; i++)
                    AddSpellDataBlock(spellTemplate, spellTemplate.GroupedClickySpellEffectBlocksForOutputBySpellParameters[i], spellTemplate.ClickySpellParatemers[i].SpellCastTimeDBCID, false);

                // Add the enchantment, if there is one
                if (spellTemplate.WeaponSpellItemEnchantmentDBCID != 0)
                {
                    spellItemEnchantmentDBC.AddRowForRogueWeaponProc(spellTemplate.WeaponSpellItemEnchantmentDBCID, spellTemplate.WeaponItemEnchantProcSpellID,
                        Configuration.SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE, spellTemplate.WeaponItemEnchantSpellName);
                }

                // Pets
                if (Configuration.SPELL_EFFECT_SUMMON_PETS_USE_EQ_LEVEL_AND_BEHAVIOR == true)
                    if (spellTemplate.SummonSpellPet != null)
                        summonPropertiesDBC.AddRow(spellTemplate.SummonPropertiesDBCID, spellTemplate.SummonSpellPet);
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
                if (objectModel.Properties.ParticleEmitters.Count > 0)
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
                    mapDBC.AddRow(Convert.ToInt32(curTransportShip.MapID), curTransportShip.MeshName, curTransportShip.Name, 0, Configuration.DBCID_LOADINGSCREEN_ID_START);
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
                    gameObjectDisplayInfoDBC.AddRow(m2ByGameObjectID.Key, relativeObjectFileName.ToLower(), m2ByGameObjectID.Value.ObjectModel.InteractionBoundingBox);
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
                    gameObjectDisplayInfoDBC.AddRow(m2ByGameObjectID.Key, relativeObjectFileName.ToLower(), m2ByGameObjectID.Value.ObjectModel.InteractionBoundingBox, openSoundEntryID, closeSoundEntryID);
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
            int curTotemCategoryID = Configuration.TRADESKILL_TOTEM_CATEGORY_START;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX), "Toolbox", curTotemCategoryID, 1);
            curTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING), "Sewing Kit", curTotemCategoryID, 1);
            curTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING), "Jeweler's Kit", curTotemCategoryID, 1);
            curTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY), "Medicine Bag", curTotemCategoryID, 1);
            curTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING), "Fletching Kit", curTotemCategoryID, 1);
            Dictionary<string, UInt32> tradeskillTotems = TradeskillRecipe.GetTotemIDsByItemName();
            int curTradeskillTotemCategoryMaskValue = 1;
            int curTradeskillTotemCategoryMaskCount = 0;
            foreach (var tradeskillTotemData in tradeskillTotems)
            {
                totemCategoryDBC.AddRow(tradeskillTotemData.Value, tradeskillTotemData.Key, curTotemCategoryID, curTradeskillTotemCategoryMaskValue);
                curTradeskillTotemCategoryMaskValue *= 2;
                curTradeskillTotemCategoryMaskCount++;
                if (curTradeskillTotemCategoryMaskCount > 10)
                {
                    curTradeskillTotemCategoryMaskValue = 1;
                    curTradeskillTotemCategoryMaskCount = 0;
                    curTotemCategoryID++;
                }
            }

            // Bard instrument totems
            curTotemCategoryID++;
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_WIND), "Wind Instrument", curTotemCategoryID, 0x1);
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_STRING), "String Instrument", curTotemCategoryID, 0x2);
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_BRASS), "Brass Instrument", curTotemCategoryID, 0x4);
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_PERCUSSION), "Percussion Instrument", curTotemCategoryID, 0x8);
            totemCategoryDBC.AddRow(Convert.ToUInt32(Configuration.ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_ALL), "All Instruments", curTotemCategoryID, 0xF);

            // Save the files
            areaTableDBC.SaveToDisk(dbcOutputClientFolder);
            areaTableDBC.SaveToDisk(dbcOutputServerFolder);
            areaTriggerDBC.SaveToDisk(dbcOutputClientFolder);
            areaTriggerDBC.SaveToDisk(dbcOutputServerFolder);
            charStartOutfitDBC.SaveToDisk(dbcOutputClientFolder);
            charStartOutfitDBC.SaveToDisk(dbcOutputServerFolder);
            creatureDisplayInfoDBC.SaveToDisk(dbcOutputClientFolder);
            creatureDisplayInfoDBC.SaveToDisk(dbcOutputServerFolder);
            creatureDisplayInfoExtraDBC.SaveToDisk(dbcOutputClientFolder);
            creatureDisplayInfoExtraDBC.SaveToDisk(dbcOutputServerFolder);
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
            lfgDungeonGroupDBC.SaveToDisk(dbcOutputClientFolder);
            lfgDungeonGroupDBC.SaveToDisk(dbcOutputServerFolder);
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
            mapDBC.SaveToDisk(dbcOutputClientFolder);
            mapDBC.SaveToDisk(dbcOutputServerFolder);
            mapDifficultyDBC.SaveToDisk(dbcOutputClientFolder);
            mapDifficultyDBC.SaveToDisk(dbcOutputServerFolder);
            skillLineDBC.SaveToDisk(dbcOutputClientFolder);
            skillLineDBC.SaveToDisk(dbcOutputServerFolder);
            skillLineAbilityDBC.SaveToDisk(dbcOutputClientFolder);
            skillLineAbilityDBC.SaveToDisk(dbcOutputServerFolder);
            skillRaceClassInfoDBC.SaveToDisk(dbcOutputClientFolder);
            skillRaceClassInfoDBC.SaveToDisk(dbcOutputServerFolder);
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
            summonPropertiesDBC.SaveToDisk(dbcOutputClientFolder);
            summonPropertiesDBC.SaveToDisk(dbcOutputServerFolder);
            taxiPathDBC.SaveToDisk(dbcOutputClientFolder);
            taxiPathDBC.SaveToDisk(dbcOutputServerFolder);
            taxiPathNodeDBC.SaveToDisk(dbcOutputClientFolder);
            taxiPathNodeDBC.SaveToDisk(dbcOutputServerFolder);
            totemCategoryDBC.SaveToDisk(dbcOutputClientFolder);
            totemCategoryDBC.SaveToDisk(dbcOutputServerFolder);
            transportAnimationDBC.SaveToDisk(dbcOutputClientFolder);
            transportAnimationDBC.SaveToDisk(dbcOutputServerFolder);
            worldMapAreaDBC.SaveToDisk(dbcOutputClientFolder);
            worldMapAreaDBC.SaveToDisk(dbcOutputServerFolder);
            worldSafeLocsDBC.SaveToDisk(dbcOutputClientFolder);
            worldSafeLocsDBC.SaveToDisk(dbcOutputServerFolder);
            wmoAreaTableDBC.SaveToDisk(dbcOutputClientFolder);
            wmoAreaTableDBC.SaveToDisk(dbcOutputServerFolder);
            zoneMusicDBC.SaveToDisk(dbcOutputClientFolder);
            zoneMusicDBC.SaveToDisk(dbcOutputServerFolder);

            Logger.WriteDebug("Creating DBC Files complete");
        }

        private void AddLightData(int mapID, ZoneEnvironmentSettings zoneEnvironmentSettings)
        {
            lightDBC.AddRow(mapID, zoneEnvironmentSettings);

            lightFloatBandDBC.AddRows(zoneEnvironmentSettings.ParamatersClearWeather);
            lightFloatBandDBC.AddRows(zoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
            lightFloatBandDBC.AddRows(zoneEnvironmentSettings.ParamatersStormyWeather);
            lightFloatBandDBC.AddRows(zoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);

            lightIntBandDBC.AddRows(zoneEnvironmentSettings.ParamatersClearWeather);
            lightIntBandDBC.AddRows(zoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
            lightIntBandDBC.AddRows(zoneEnvironmentSettings.ParamatersStormyWeather);
            lightIntBandDBC.AddRows(zoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);

            lightParamsDBC.AddRow(zoneEnvironmentSettings.ParamatersClearWeather);
            lightParamsDBC.AddRow(zoneEnvironmentSettings.ParamatersClearWeatherUnderwater);
            lightParamsDBC.AddRow(zoneEnvironmentSettings.ParamatersStormyWeather);
            lightParamsDBC.AddRow(zoneEnvironmentSettings.ParamatersStormyWeatherUnderwater);
        }
    }
}
