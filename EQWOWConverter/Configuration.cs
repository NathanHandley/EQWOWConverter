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

namespace EQWOWConverter
{
    internal class Configuration
    {
        // ====================================================================
        // Core
        // ====================================================================
        // Plays a beep sound when the generate completes if set to true
        public static readonly bool CORE_CONSOLE_BEEP_ON_COMPLETE = true;

        // If true, the generator will run in multithreading mode
        public static readonly bool CORE_ENABLE_MULTITHREADING = true;
        public static readonly int CORE_ZONEGEN_THREAD_COUNT = 4;

        // ====================================================================
        // Logging
        // ====================================================================
        // Level of logs to write to the console and log file.
        // 1 = Error, 2 = Info, 3 = Debug
        public static readonly int LOGGING_CONSOLE_MIN_LEVEL = 2;
        public static readonly int LOGGING_FILE_MIN_LEVEL = 3;

        // ====================================================================
        // Paths and Files
        // ====================================================================
        // Location of the installed everquest trilogy
        public static readonly string PATH_EQTRILOGY_FOLDER = "E:\\Development\\EQWOW-Reference\\EverQuestTrilogy";

        // Location of eq data exports before conditioning (from LaternExtractor)
        public static readonly string PATH_EQEXPORTSRAW_FOLDER = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQClientExportRaw";

        // Location of your enUS World of Warcaft client, where your wow.exe is (such as C:\WorldOfWarcraft)
        public static readonly string PATH_WOW_ENUS_CLIENT_FOLDER = "E:\\Development\\azerothcore-wotlk\\Client\\";

        // Where the "conditioned" eq data export files go
        public static readonly string PATH_EQEXPORTSCONDITIONED_FOLDER = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQClientExportConditioned";

        // Where the intermediate generated WOW files go
        public static readonly string PATH_EXPORT_FOLDER = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\WOWExports";

        // The root of the tools directory (included in this source repository)
        public static readonly string PATH_TOOLS_FOLDER = "E:\\Development\\EQWOW\\Tools";

        // The root of the assets directory (included in this source repository)
        public static readonly string PATH_ASSETS_FOLDER = "E:\\Development\\EQWOW\\Assets";

        // Name of the newely generated patch file, without the extension. Note: Will be deleted when extracting DBC data, so make sure it's the last and new
        public static readonly string PATH_PATCH_NEW_FILE_NAME_NO_EXT = "patch-enUS-5";

        // Location of where the server DBC files would be deployed to if set to deploy
        public static readonly string PATH_DEPLOY_SERVER_DBC_FILES_FOLDER = "E:\\Development\\azerothcore-wotlk\\Build\\bin\\RelWithDebInfo\\data\\dbc";

        // ====================================================================
        // Deployment Rules
        // ====================================================================
        // If true, deploy the client file (patch mpq) after building it
        public static readonly bool DEPLOY_CLIENT_FILES = true;

        // If true, deploy to the server files/data after building
        public static readonly bool DEPLOY_SERVER_FILES = true;

        // If true, deploy to the SQL to the server
        // Note: May not work on remote servers (not tested)
        public static readonly bool DEPLOY_SERVER_SQL = true;

        // If true, DBC files are extracted every time.
        public static readonly bool EXTRACT_DBC_FILES = true;

        // If true and when deploying client files, clear the cache
        public static readonly bool DEPLOY_CLEAR_CACHE_ON_CLIENT_DEPLOY = true;

        // If deploying to SQL, you need to set these to something real that points to your databases
        public static readonly string DEPLOY_SQL_CONNECTION_STRING_CHARACTERS = "Server=127.0.0.1;Database=acore_characters;Uid=root;Pwd=rootpass;";
        public static readonly string DEPLOY_SQL_CONNECTION_STRING_WORLD = "Server=127.0.0.1;Database=acore_world;Uid=root;Pwd=rootpass;";

        // ====================================================================
        // Generator Rules
        // ====================================================================
        // The value EQ vertices multiply by when translated into WOW vertices
        // A WORLD_SCALE value of 0.25 seems to be 1:1 with EQ.  0.28 allows humans and 0.4 allows taurens to enter rivervale bank door
        public static readonly float GENERATE_WORLD_SCALE = 0.29f;
        public static readonly float GENERATE_CREATURE_SCALE = 0.255f;
        public static readonly float GENERATE_EQUIPMENT_HELD_SCALE = 0.35f;

        // Identifier for what subset of expansion data to work with.  0 = Classic, 1 = Kunark, 2 = Velious
        public static readonly int GENERATE_EQ_EXPANSION_ID_GENERAL = 2;
        public static readonly int GENERATE_EQ_EXPANSION_ID_ZONES = 0;
        public static readonly int GENERATE_EQ_EXPANSION_ID_TRANSPORTS = 0;
        public static readonly int GENERATE_EQ_EXPANSION_ID_TRADESKILLS = 0;
        public static readonly int GENERATE_EQ_EXPANSION_ID_EQUIPMENT_GRAPHICS = 0;

        // If true, then objects are generated
        public static readonly bool GENERATE_OBJECTS = true;

        // If true, then creatures are generated
        public static readonly bool GENERATE_CREATURES_AND_SPAWNS = true;

        // If true, then item armor player graphics are generated (and if set in [PATH_ASSETS_FOLDER]\CustomTextures\item\texturecomponents)
        public static readonly bool GENERATE_PLAYER_ARMOR_GRAPHICS = true;

        // If true, transports (ships, ferries) will be generated
        public static readonly bool GENERATE_TRANSPORTS = true;

        // If true, quests are generated
        public static readonly bool GENERATE_QUESTS = true;

        // If this has any zone short names in it, the ouput of the generator will perform an update only for these zones. If there is no previously
        // built patch mpq, it will be forced to do a complete build first.  Note that if any zones are entered in here, ONLY those zones
        // will load and work properly
        public static readonly List<string> GENERATE_ONLY_LISTED_ZONE_SHORTNAMES = new List<string>() { };

        // An extra amount to add to the boundary boxes when generating wow assets from EQ.  Needed to handle rounding.
        public static readonly float GENERATE_ADDED_BOUNDARY_AMOUNT = 0.01f;

        // How many insert rows to restrict in a SQL output file
        public static readonly int GENERATE_SQL_FILE_BATCH_SIZE = 50000;
        public static readonly int GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE = 5000;

        // How many file names to batch up when converting 
        public static readonly int GENERATE_BLPCONVERTBATCHSIZE = 50;

        // What edge buffer to add when doing floating point month
        public static readonly float GENERATE_FLOAT_EPSILON = 0.001f;

        // If true, SQL files will be generated in a way where they will have a unique ID to force an update if ran by azerothcore, regardless of changes
        public static readonly bool GENERATE_FORCE_SQL_UPDATES = true;

        //=====================================================================
        // Player
        //=====================================================================
        // If true, new players created will use the everquest start locations defined in PlayerClassRaceProperties
        // WARNING: This will DELETE the existing start locations in WoW zones, so be certain you want this enabled before deploying
        public static readonly bool PLAYER_USE_EQ_START_LOCATION = true;

        // If true, players will start with an EQ item loadout instead of a WOW item loadout
        public static readonly bool PLAYER_USE_EQ_START_ITEMS = true;

        // If true, this will also add a hearthstone if using EQ items
        public static readonly bool PLAYER_ADD_HEARTHSTONE_IF_USE_EQ_START_ITEMS = true;

        //=====================================================================
        // Zone General
        //=====================================================================
        // If this is set to false, any static graphics (like dirt, etc) are not rendered.  Only set to false for debugging
        public static readonly bool ZONE_SHOW_STATIC_GEOMETRY = true;

        // Maximum number of faces that fit into a render WMO group before it subdivides (max is due to various variable limits)
        public static readonly int ZONE_MAX_FACES_PER_WMOGROUP = 21840;

        // Maximum size of any zone-to-material-object creation along the X and Y axis
        public static readonly float ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE = 325.0f;

        // How much bigger to make the box which causes the shadow in a shadowbox
        public static readonly float ZONE_SHADOW_BOX_ADDED_SIZE = 50f;

        // If true, allow collision with world model objects. This will also impact music and ambient sounds, since they align to areas which require collision detection
        public static readonly bool ZONE_COLLISION_ENABLED = true;

        // When zone geometry gets broken into cuboids, this is the max side length of the area
        public static readonly int ZONE_SPLIT_COLLISION_CUBOID_MAX_EDGE_LEGNTH = 1200;

        // If set to 'true', show a box where the music zones are. This is for debugging only.  Only works when collision is enabled
        public static readonly bool ZONE_DRAW_COLLIDABLE_SUB_AREAS_AS_BOXES = false;

        // Maxinum number of triangle faces that can be in any zone-to-material-object
        public static readonly int ZONE_MAX_FACES_PER_ZONE_MATERIAL_OBJECT = 2000;

        // BSP tree nodes will stop subdividing when this many (or less) triangles are found
        public static readonly UInt16 ZONE_BTREE_MIN_SPLIT_SIZE = 10;

        // BSP tree nodes won't operate on bounding boxes smaller than this (X + Y + Z lengths)
        public static readonly float ZONE_BTREE_MIN_BOX_SIZE_TOTAL = 5f;

        // BSP tree nodes won't generate deeper than this many iterations
        public static readonly int ZONE_BTREE_MAX_NODE_GEN_DEPTH = 50;

        // Which ID to use if a graveyard isn't mapped for a zone.  13 is in east commons next to EC tunnel.
        public static readonly int ZONE_DEFAULT_GRAVEYARD_ID = 13;

        // ID for the creature template for the spirit healer.
        public static readonly int ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID = 6491;

        //=====================================================================
        // Liquid
        //=====================================================================
        // If this is true, it will show the true surface line of water and not just the material from EQ.  This should only be used
        // for debugging as it very visually unpleasant
        public static readonly bool LIQUID_SHOW_TRUE_SURFACE = false;

        // How much 'height' to add to liquid surface, helps with rendering the waves
        public static readonly float LIQUID_SURFACE_ADD_Z_HEIGHT = 0.001f;

        // How much to 'walk' the x value when generating an irregular quad of liquid for each plane
        public static readonly float LIQUID_QUADGEN_EDGE_WALK_SIZE = 0.2f;

        // How much to overlap the planes when generating an irregular quad of liquid
        public static readonly float LIQUID_QUADGEN_PLANE_OVERLAP_SIZE = 0.0001f;

        //=====================================================================
        // Lighting and Coloring
        //=====================================================================
        // If true, light instances are enabled.  They don't work at this time, so leave false
        public static readonly bool LIGHT_INSTANCES_ENABLED = false;

        // If true, light instances are rendered as torches.  Use for debugging only, and typically leave false
        public static readonly bool LIGHT_INSTANCES_DRAWN_AS_TORCHES = false;

        // Sets the modifier to add to the attenuation to define the start, calculated by multiplying this value to it
        public static readonly float LIGHT_INSTANCE_ATTENUATION_START_PROPORTION = 0.25f;

        // How much of the EQ original vertex color to apply to surfaces
        public static readonly double LIGHT_DEFAULT_VERTEX_COLOR_INTENSITY = 0.2;

        // Default ambience to apply to indoor areas (sets to r g and b). To have colors pop more like in EQ, set it lower like 96 or so.
        public static readonly byte LIGHT_DEFAULT_INDOOR_AMBIENCE = 165;

        // Amonut of glow to add to outdoor areas (ranges are 0-1)
        public static readonly float LIGHT_OUTSIDE_GLOW_CLEAR_WEATHER = 0.4f;
        public static readonly float LIGHT_OUTSIDE_GLOW_STORMY_WEATHER = 0.2f;
        public static readonly float LIGHT_OUTSIDE_GLOW_UNDERWATER = 0.6f;

        // Brightness of outdoor areas based on time
        public static readonly byte LIGHT_OUTSIDE_AMBIENT_TIME_0 = 58; // Midnight
        public static readonly byte LIGHT_OUTSIDE_AMBIENT_TIME_3 = 58; // 3 AM
        public static readonly byte LIGHT_OUTSIDE_AMBIENT_TIME_6 = 164; // 6 AM
        public static readonly byte LIGHT_OUTSIDE_AMBIENT_TIME_12 = 239; // Noon - Must always be the most bright
        public static readonly byte LIGHT_OUTSIDE_AMBIENT_TIME_21 = 192; // 9 PM
        public static readonly byte LIGHT_OUTSIDE_AMBIENT_TIME_22 = 58; // 10 PM

        //=====================================================================
        // Audio
        //=====================================================================       
        // Set which soundfont to use in the Tools/soundfont folder.  Alternate option is synthusr_samplefix.sf2
        public static readonly string AUDIO_SOUNDFONT_FILE_NAME = "AweROMGM.sf2";

        // If set to true, some audio tracks are swapped vs the original tracks.  Make it false if you want a more classic-like experience
        public static readonly bool AUDIO_USE_ALTERNATE_TRACKS = false;

        // How much to increase the music sound when converted from EverQuest
        public static readonly decimal AUDIO_MUSIC_CONVERSION_GAIN_AMOUNT = 1;

        // Mod / multiplier to volumes (multiplies the volume by this value)
        // NOTE: Sound Instance volumes can be found in Sound.GetVolume
        public static readonly float AUDIO_AMBIENT_SOUND_VOLUME_MOD = 1f;
        public static readonly float AUDIO_SOUNDINSTANCE_VOLUME_MOD = 1f; // Also game objects
        public static readonly float AUDIO_MUSIC_VOLUME_MOD = 1f;

        // If this is 'true', draw any sound instances in a zone as a little box
        public static readonly bool AUDIO_SOUNDINSTANCE_DRAW_AS_BOX = false;

        // The radius of a sound instance is multiplied by this to get the min distance, which is the range which the sound is 100% volume
        public static readonly float AUDIO_SOUNDINSTANCE_3D_MIN_DISTANCE_MOD = 0.4f;
        public static readonly float AUDIO_SOUNDINSTANCE_2D_MIN_DISTANCE_MOD = 0.8f;

        // Size of the box when rendering a sound instance (Note: It's 1/2 the in-game size)
        public static readonly float AUDIO_SOUNDINSTANCE_RENDEROBJECT_BOX_SIZE = 1f;

        // Name of the object material that is used when rendering the soundinstance object
        public static readonly string AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME = "akafloorgrate";

        // Volume of creature sound effects like attacks and being hit
        public static readonly float AUDIO_CREATURE_SOUND_VOLUME = 0.3f;

        //=====================================================================
        // Objects
        //=====================================================================
        // The minimum size of a bounding box for a static doodad.  Bigger means it can be seen further away
        public static readonly float OBJECT_STATIC_MIN_BOUNDING_BOX_SIZE = 25.1f;

        // For ladders, this is how far to extend out the steppable area in front and back of it (value is before world scaling)
        public static readonly float OBJECT_STATIC_LADDER_EXTEND_DISTANCE = 1.0f;

        // How much space between each step of a ladder along the Z axis (value is before world scaling)
        public static readonly float OBJECT_STATIC_LADDER_STEP_DISTANCE = 0.5f;

        //=====================================================================
        // Creatures
        //=====================================================================
        // This is the percent of the idle time that a 'fidget' occurs (1-100)
        // Note: There are two fidget animations, 1/2 this number applies to each fidget
        public static readonly int CREATURE_FIDGET_TIME_PERCENT = 20;

        // If true, additional data is added in the creature name for easy debugging. Default to false.
        public static readonly bool CREATURE_ADD_DEBUG_VALUES_TO_NAME = false;

        // Stat modifiers for creatures
        // - "MIN" and "MAX" are applied after all other calculations
        public static readonly float CREATURE_STAT_MOD_HP_ADD = 0.5f;
        public static readonly float CREATURE_STAT_MOD_HP_MIN = 0.8f;
        public static readonly float CREATURE_STAT_MOD_HP_MAX_NORMAL = 2f;
        public static readonly float CREATURE_STAT_MOD_HP_MAX_RARE = 2f;
        public static readonly float CREATURE_STAT_MOD_HP_SET_ELITE = 20f;
        public static readonly float CREATURE_STAT_MOD_HP_SET_ELITERARE = 80f;
        public static readonly float CREATURE_STAT_MOD_HP_SET_BOSS = 200f;
        public static readonly float CREATURE_STAT_MOD_AVGDMG_ADD = 0.2f;
        public static readonly float CREATURE_STAT_MOD_AVGDMG_MIN = 0.8f;
        public static readonly float CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL = 1.5f;
        public static readonly float CREATURE_STAT_MOD_AVGDMG_MAX_RARE = 1.5f;
        public static readonly float CREATURE_STAT_MOD_AVGDMG_SET_ELITE = 8f;
        public static readonly float CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE = 10f;
        public static readonly float CREATURE_STAT_MOD_AVGDMG_SET_BOSS = 18f;

        // The value to name the everquest parent reputation item as
        public static readonly string CREATURE_FACTION_ROOT_NAME = "EverQuest";

        // The default faction values to use if none can be mapped.  Using the 'neutral' record for now.
        public static readonly int CREATURE_FACTION_TEMPLATE_DEFAULT = 2302;
        public static readonly int CREATURE_FACTION_DEFAULT = 1200;

        // For any quest or merchant NPCs that aren't aligned to a raisable or lowerable faction, they will be mapped to this.  Default is Norrath Settlers.
        public static readonly int CREATURE_FACTION_TEMPLATE_NEUTRAL = 2302;
        public static readonly int CREATURE_FACTION_TEMPLATE_NEUTRAL_INTERACTIVE = 2313;

        // If set to true, all factions will show up for EverQuest in the faction list immediately
        public static readonly bool CREATURE_FACTION_SHOW_ALL = true;

        // What to multiple the EverQuest reputation rewards by.  WOW is approx 20-30x that of EQ in band.
        public static readonly int CREATURE_REP_REWARD_MULTIPLIER = 20;

        // Values for creatures without a default detection/agro range (note: This is NOT scaled by WORLD_SCALE)
        public static readonly float CREATURE_DEFAULT_DETECTION_RANGE = 20f;

        // ID for the class trainer menu text (328 exists already and is just "Greetings, $n")
        public static readonly int CREATURE_CLASS_TRAINER_NPC_TEXT_ID = 328;

        // ID for the class trainer menu broadcast texts 
        public static readonly int CREATURE_CLASS_TRAINER_TRAIN_BROADCAST_TEXT_ID = 2548; // Pre-exists, "I would like to train."
        public static readonly int CREATURE_CLASS_TRAINER_UNLEARN_BROADCAST_TEXT_ID = 62295; // Pre-exists, "I wish to unlearn my talents."
        public static readonly int CREATURE_CLASS_TRAINER_DUALTALENT_BROADCAST_TEXT_ID = 33762; // Pre-exists, "I wish to know about Dual Talent Specialization."

        // IDs for menus specific to class trainer
        public static readonly int CREATURE_CLASS_TRAINER_UNLEARN_MENU_ID = 4461; // Pre-exists
        public static readonly int CREATURE_CLASS_TRAINER_DUALTALENT_MENU_ID = 10371; // Pre-exists

        //=====================================================================
        // Items
        //=====================================================================
        // If true, this uses alternate stats for items that have been tweaked for balance reasons
        public static readonly bool ITEMS_USE_ALTERNATE_STATS = true;
        
        // This is how much is reduced from the weapon delay of EQ weapons, value is 0 - 1;
        public static readonly float ITEMS_WEAPON_DELAY_REDUCTION_AMT = 0.2f;

        // If this is true, then weapons and armor that allow all normally-aligned classes to be
        //  classified as 'all'.  For example: Bronze Breastplate allows all plate classes, so
        //  it will have the classes allowed list set to 'any'
        public static readonly bool ITEMS_ALLOW_ALL_CLASSES_ON_GENERIC_EQUIP = true;

        // This is how much 'weight' the lower stat has when converting EQ to WoW stats, with 
        //  values closer to 1 leaning towards the lower stat, and further from 1 leaning towards
        //  the higher stat.  Don't make it less than 1.  
        public static readonly float ITEMS_STATS_LOW_BIAS_WEIGHT = 2.5f;

        // How much to multiple the slot size of a bag in EQ.  EQ allows for 2x the number bags of WOW (not including starter)
        public static readonly int ITEMS_BAG_SLOT_MULTIPLIER = 2;

        //=====================================================================
        // Quests
        //=====================================================================
        // How many milliseconds to display a text block from an NPC on quest events
        public static readonly int QUESTS_TEXT_DURATION_IN_MS = 10000;

        // This is the icon ID that is used for quest rewards that contain more than one random item
        // The ID here is the icon ID as defined by X in "INV_EQ_X.blp"
        public static readonly int QUESTS_ITEMS_REWARD_CONTAINER_ICON_ID = 57;

        //=====================================================================
        // Spells
        //=====================================================================
        // IDs for special spells that need an exact match of ID between this and mod-everquest
        public static readonly int SPELLS_GATE_SPELLDBC_ID = 86900;
        public static readonly int SPELLS_BINDSELF_SPELLDBC_ID = 86901;
        public static readonly int SPELLS_BINDANY_SPELLDBC_ID = 86902;
        public static readonly int SPELLS_DAYPHASE_SPELLDBC_ID = 86903;
        public static readonly int SPELLS_NIGHTPHASE_SPELLDBC_ID = 86904;

        // These are the levels in which casters and melee can learn gate and bind. Setting to 0 or lower means
        // that can never be learned by that group.  If both melee and casters can learn bind, then a self-only
        // version of bind is learned.  If gate tether is enabled then the player can return to their gate point
        // by clicking off the buff (within 30 minutes)
        public static readonly int SPELLS_GATE_CASTER_LEARN_LEVEL = 1; // Set to 4 or 6 to be more like EQ
        public static readonly int SPELLS_GATE_MELEE_LEARN_LEVEL = 1; // Set to -1 to be like EQ (melee can't learn it)
        public static readonly int SPELLS_GATE_SPELL_LEARN_COST = 100;
        public static readonly int SPELLS_BIND_CASTER_LEARN_LEVEL = 1; // Set to 12 or 14 to be more like EQ
        public static readonly int SPELLS_BIND_MELEE_LEARN_LEVEL = 1; // Set to -1 to be like EQ (melee can't learn it)
        public static readonly int SPELLS_BIND_SPELL_LEARN_COST = 100;
        public static readonly bool SPELLS_GATE_TETHER_ENABLED = true;

        //=====================================================================
        // Tradeskills
        //=====================================================================
        // How much to multiply EQ skill requirements by to reach the same for WoW on conversion
        public static readonly float TRADESKILLS_CONVERSION_MOD = 1.3432f;

        // Max distance between Grey -> Green -> Yellow -> Red steps
        public static readonly int TRADESKILLS_SKILL_TIER_DISTANCE_LOW = 10;
        public static readonly int TRADESKILLS_SKILL_TIER_DISTANCE_HIGH = 25;

        // The skill level of a tradeskill will be priced closest to the value for that WOW skill level
        public static readonly int TRADESKILL_LEARN_COST_AT_1 = 10;
        public static readonly int TRADESKILL_LEARN_COST_AT_50 = 500;
        public static readonly int TRADESKILL_LEARN_COST_AT_100 = 1000;
        public static readonly int TRADESKILL_LEARN_COST_AT_200 = 2850;
        public static readonly int TRADESKILL_LEARN_COST_AT_300 = 12500;
        public static readonly int TRADESKILL_LEARN_COST_AT_450 = 150000;

        // How long every tradeskill will take in milliseconds
        public static readonly int TRADESKILL_CAST_TIME_IN_MS = 5000;

        // This is the icon ID that is used for tradeskill results that contain more than one item
        // The ID here is the icon ID as defined by X in "INV_EQ_X.blp"
        public static readonly int TRADESKILL_MULTI_ITEMS_CONTAINER_ICON_ID = 57;

        // Tradeskill items that need a totem in TotemCategory.dbc will align under this
        public static readonly int TRADESKILL_TOTEM_CATEGORY_START = 30;

        // ID for TotemCategory.dbc for specific tradeskills
        public static readonly int TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING = 210;
        public static readonly int TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING = 211;
        public static readonly int TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING = 212;
        public static readonly int TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY = 213;

        //=====================================================================
        // Transports
        //=====================================================================
        public static readonly float TRANSPORT_PAUSE_MULTIPLIER = 0.5f; // Pause as in 'stop at a port'. 1 will be EQ-like
        public static readonly int TRANSPORT_MOVE_SPEED = 30; // Most boats are 30 in WoW, but a value of around 9 is EQ-like
        public static readonly int TRANSPORT_ACCELERATION = 1;

        // ====================================================================
        // WOW DBC/File IDs
        // ====================================================================
        // IDs for AreaBit used in AreaTable, should be unique (max of 4095)
        public static readonly int DBCID_AREATABLE_AREABIT_BLOCK_1_START = 3092;
        public static readonly int DBCID_AREATABLE_AREABIT_BLOCK_1_END = 3172;
        public static readonly int DBCID_AREATABLE_AREABIT_BLOCK_2_START = 3462;
        public static readonly int DBCID_AREATABLE_AREABIT_BLOCK_2_END = 3616;
        public static readonly int DBCID_AREATABLE_AREABIT_BLOCK_3_START = 3800;
        public static readonly int DBCID_AREATABLE_AREABIT_BLOCK_3_END = 4095;

        // Identifies Area rows in AreaTable.dbc
        public static readonly UInt32 DBCID_AREATABLE_ID_START = 5100;

        // IDs for AreaTrigger.DBC. These will be generated in ascending order by MapID, and referenced in SQL scripts
        // for teleports as well any other area-based triggers
        public static readonly int DBCID_AREATRIGGER_ID_START = 6500;

        // IDs for CreatureDisplayInfo.dbc
        public static readonly int DBCID_CREATUREDISPLAYINFO_ID_START = 34000;
        public static readonly int DBCID_CREATUREDISPLAYINFO_ID_END = 40000;

        // IDs for CreatureModelData.dbc
        public static readonly int DBCID_CREATUREMODELDATA_ID_START = 3500;
        public static readonly int DBCID_CREATUREMODELDATA_ID_END = 5000;

        // IDs for CreatureSoundData.dbc
        public static readonly int DBCID_CREATURESOUNDDATA_ID_START = 3300;

        // IDs for FootstepTerrainLookup.dbc
        public static readonly int DBCID_FOOTSTEPTERRAINLOOKUP_ID_START = 600;
        public static readonly int DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START = 250;

        // IDs for rows inside GameObjectDisplayInfo.dbc
        public static readonly int DBCID_GAMEOBJECTDISPLAYINFO_ID_START = 11000;

        // Start ID for item display info
        public static readonly int DBCID_ITEMDISPLAYINFO_START = 86000;

        // Identifies the Light.DBC row, used for environmental properties
        public static readonly int DBCID_LIGHT_ID_START = 3500;

        // Identifies the LightParams.dbc, used for detailed values related to a Light.DBC row
        public static readonly int DBCID_LIGHTPARAMS_ID_START = 1050;

        // IDs for the loading screen
        public static readonly int DBCID_LOADINGSCREEN_ID_START = 255;

        // Identifies Maps in Map.dbc and MapDifficulty.dbc
        // Note: This value is hard coded in /WorldData/ZoneProperties.csv and /TransportShips.csv, so you cannot change only this value
        public static readonly int DBCID_MAP_ID_START = 750;
        public static readonly int DBCID_MAP_ID_END = 899;

        // ID for general/shared skill line holding EverQuest alteration abilities like
        //  gate found in SkillLine and SkillLineAbility.
        // NOTE BUG: Class trainers filter out most skill lines, so using "Defense" which should work for all
        public static readonly int DBCID_SKILLLINE_ALTERATION_ID = 95;

        // ID for skill line abilities found in SkillLineAbility.dbc
        public static readonly int DBCID_SKILLLINEABILITY_ID_START = 25000;

        // ID for sounds found in SoundEntries.dbc
        public static readonly int DBCID_SOUNDENTRIES_ID_START = 22000;

        // ID for sounds found in SoundAmbience.dbc
        public static readonly int DBCID_SOUNDAMBIENCE_ID_START = 600;

        // ID for spells found in Spell.dbc
        // - Manually created spells reserve IDs from 86900 to 86999.  See "Spells"
        // - Recipes reserve IDs 87000 to 91221
        public static readonly int DBCID_SPELL_ID_START = 87000;

        // ID for spellcasttimes.dbc
        public static readonly int DBCID_SPELLCASTTIME_ID_START = 215;

        // ID for spellicon.dbc
        public static readonly int DBCID_SPELLICON_ID_START = 4400;

        // ID for TaxiPath.dbc
        public static readonly int DBCID_TAXIPATH_ID_START = 2000;

        // ID for TaxiPathNode.dbc
        public static readonly int DBCID_TAXIPATHNODE_ID_START = 48000;

        // ID for TotemCategory.dbc
        // NOTE: Categories 210-219 are also reserved, but used for tradeskill containers
        public static readonly UInt32 DBCID_TOTEMCATEGORY_ID_START = 220;

        // ID for TransportAnimation.dbc
        public static readonly int DBCID_TRANSPORTANIMATION_ID_START = 180000;

        // IDs for WorldSafeLocs.dbc
        public static readonly int DBCID_WORLDSAFELOCS_ID_START = 1800;
        public static readonly int DBCID_WORLDSAFELOCS_ID_END = 2000;

        // Specific rows in WMOAreaTable.dbc
        public static readonly int DBCID_WMOAREATABLE_ID_START = 52000;

        // Identifies WMO Roots.  Found in WMOAreaTable.dbc and AreaTable.dbc
        public static readonly UInt32 DBCID_WMOAREATABLE_WMOID_START = 7000;

        // Identifies WMO Groups. Found in WMOAreaTable.dbc and the .wmo files
        public static readonly UInt32 DBCID_WMOAREATABLE_WMOGROUPID_START = 30000;

        // ID for music in ZoneMusic.dbc, and how many IDs to reserve on a per-zone basis
        public static readonly int DBCID_ZONEMUSIC_START = 700;

        // ====================================================================
        // SQL IDs
        // ====================================================================
        // Start and end IDs for broadcast_text sql records
        public static readonly int SQL_BROADCASTTEXT_ID_START = 80000;
        public static readonly int SQL_BROADCASTTEXT_ID_END = 99999;

        // Record identifier for the creature sql table, need at least 31k
        public static readonly int SQL_CREATURE_GUID_LOW = 310000;
        public static readonly int SQL_CREATURE_GUID_HIGH = 399999;

        // Record identifier for the creature template SQL table, Need just under 11k
        public static readonly int SQL_CREATURETEMPLATE_ENTRY_LOW = 45000;
        public static readonly int SQL_CREATURETEMPLATE_ENTRY_HIGH = 60000;

        // Start GUIDs for gameobjects
        public static readonly int SQL_GAMEOBJECT_GUID_ID_START = 310000;
        public static readonly int SQL_GAMEOBJECT_GUID_ID_END = 319999;

        // IDs for game_event records
        public static readonly int SQL_GAMEEVENT_ID_DAY = 125;
        public static readonly int SQL_GAMEEVENT_ID_NIGHT = 126;

        // Start and end IDs for gameobject_template rows
        public static readonly int SQL_GAMEOBJECTTEMPLATE_ID_START = 270000;
        public static readonly int SQL_GAMEOBJECTTEMPLATE_ID_END = 279999;

        // Start row for `game_tele` records. (~2000-2400)
        public static readonly int SQL_GAMETELE_ROWID_START = 2000;
        public static readonly int SQL_GAMETELE_ROWID_END = 2400;

        // Start and end IDs for custom gossip menu records
        public static readonly int SQL_GOSSIPMENU_MENUID_START = 62000;
        public static readonly int SQL_GOSSIPMENU_MENUID_END = 69999;

        // Start and end IDs for template entries
        public static readonly int SQL_ITEM_TEMPLATE_ENTRY_START = 85000;
        public static readonly int SQL_ITEM_TEMPLATE_ENTRY_END = 120000;
        public static readonly int SQL_ITEM_TEMPLATE_RANDOM_ITEM_CONTAINER_START_ID = 116000;

        // Stand and end IDs for npc_text sql records
        public static readonly int SQL_NPCTEXT_ID_START = 80000;
        public static readonly int SQL_NPCTEXT_ID_END = 99999;

        // Start ID for npc_trainer entries
        public static readonly int SQL_NPCTRAINER_ID_START = 210000;
        public static readonly int SQL_NPCTRAINER_ID_END = 211000;

        // Start ID for pool_template data rows (reserve 40k records)
        public static readonly int SQL_POOL_TEMPLATE_ID_START = 110000;
        public static readonly int SQL_POOL_TEMPLATE_ID_END = 150000;

        // Start and end IDs for quest template data rows
        // The 'shift' value is the value to add for the repeatable versions
        public static readonly int SQL_QUEST_TEMPLATE_ID_START = 30000;
        public static readonly int SQL_QUEST_TEMPLATE_ID_END = 40000;
        public static readonly int SQL_QUEST_TEMPLATE_ID_REPEATABLE_SHIFT = 5000;

        // Start and end IDs for transports
        public static readonly int SQL_TRANSPORTS_GUID_START = 21;
        public static readonly int SQL_TRANSPORTS_GUID_END = 41;
    }
}
