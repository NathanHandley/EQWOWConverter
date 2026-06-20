//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using System.Text;

namespace EQWOWConverter
{
    internal class Configuration
    {
        // ====================================================================
        // Not Loaded From Configuration File
        // ====================================================================
        public static string CONFIGONLY_CONFIGURATION_FILE_NAME = "configuration.txt";

        // This is the version that the mod-everquest AzerothCore module needs to be compatible with
        public static int CONFIGONLY_CORE_MOD_VERSION = 20;

        // If true, all creatures and their waypoints will spawn as a default non-mobile object. This should only be
        // done for debugging reasons, as the game will not look or feel anything like it should
        public static bool CONFIGONLY_CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE = false;
        public static int CONFIGONLY_SQL_CREATURETEMPLATE_DEBUG_ENTRY_LOW = 300000; // Used for CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE
        public static int CONFIGONLY_SQL_CREATURETEMPLATE_DEBUG_ENTRY_HIGH = 2000000; // Used for CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE
        public static int CONFIGONLY_SQL_CREATURE_GUID_DEBUG_LOW = 2000000; // Used for CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE
        public static int CONFIGONLY_SQL_CREATURE_GUID_DEBUG_HIGH = 5060599; // Used for CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE

        // If this has any zone short names in it, the ouput of the generator will perform an update only for these zones. If there is no previously
        // built patch mpq, it will be forced to do a complete build first.  Note that if any zones are entered in here, ONLY those zones
        // will load and work properly
        public static List<string> CONFIGONLY_ONLY_LISTED_ZONE_SHORTNAMES = new List<string>() { };

        // ====================================================================
        // Paths and Files
        // ====================================================================
        // Location of the installed everquest trilogy client (this must have the eqgame.exe file in it)
        public static string PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER = "C:\\";

        // Location of the installed enUS version of World of Warcaft client (this must have the wow.exe in it)
        public static string PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER = "C:\\";

        // The root of the tools directory (comes with this source code in a folder)
        public static string PATH_TOOLS_FOLDER = "Tools";

        // The root of the assets directory (comes with this source code in a folder)
        public static string PATH_ASSETS_FOLDER = "Assets";

        // The root folder where temporary folders and file will be generated (ensure at least 10GB of space is available in this location)
        public static string PATH_WORKING_FOLDER = "Output";

        // ID to append to the end of the /Data/ patch file (such as the "4" in "patch-4.mpq). Make it uniquely new.
        public static string PATCH_CLIENT_DATA_ID = "4";

        // ID to append to the localized patch file in /Data/<locale> (such as the "5" in patch-enUS-5.mpq). Make it uniquely new.
        public static string PATCH_CLIENT_DATA_LOC_ID = "5";

        // What language to generate things as
        public static string PATCH_LOCALIZATION_STRING = "enUS";

        // ====================================================================
        // Deployment Rules
        // ====================================================================
        // If true, deploy the client file (patch mpq) after building it
        public static bool DEPLOY_CLIENT_FILES = true;

        // If true and when deploying client files, clear the cache (only relevant if you set DEPLOY_CLIENT_FILES to true, otherwise ignored)
        public static bool DEPLOY_CLEAR_CACHE_ON_CLIENT_DEPLOY = true;

        // If true, deploy to the server files/data after building
        public static bool DEPLOY_SERVER_FILES = true;

        // Location of where the server DBC files would be deployed to (only relevant if you set DEPLOY_SERVER_FILES to true, otherwise ignored)
        public static string DEPLOY_SERVER_DBC_FOLDER_LOCATION = "C:\\";

        // If true, deploy to the SQL to the server
        // Note: May not work on remote servers (not tested)
        public static bool DEPLOY_SERVER_SQL = true;

        // If deploying to SQL, you need to set these to something real that points to your databases (only relevant if you set DEPLOY_SERVER_SQL to true, otherwise ignored)
        public static string DEPLOY_SQL_CONNECTION_STRING_CHARACTERS = "Server=127.0.0.1;Database=acore_characters;Uid=root;Pwd=rootpass;";
        public static string DEPLOY_SQL_CONNECTION_STRING_WORLD = "Server=127.0.0.1;Database=acore_world;Uid=root;Pwd=rootpass;";

        // ====================================================================
        // Core
        // ====================================================================       
        // Plays a beep sound when the generate completes if set to true
        public static bool CORE_CONSOLE_BEEP_ON_COMPLETE = true;

        // If true, the conditioner & generator will run in multithreading mode
        public static bool CORE_ENABLE_MULTITHREADING = true;
        public static int CORE_ZONEGEN_THREAD_COUNT = 4;
        public static int CORE_PNGTOBLPCONVERSION_THREAD_COUNT = 8;
        public static int CORE_MUSICCONVERSION_THREAD_COUNT = 8;

        // ====================================================================
        // Logging
        // ====================================================================
        // Level of logs to write to the console and log file.
        // 1: Error, 2: Info, 3: Debug
        public static int LOGGING_CONSOLE_MIN_LEVEL = 2;
        public static int LOGGING_FILE_MIN_LEVEL = 2;

        // ====================================================================
        // Generator Rules
        // ====================================================================
        // The value EQ vertices multiply by when translated into WOW vertices
        // A WORLD_SCALE value of 0.25 seems to be 1:1 with EQ.  0.28 allows humans and 0.4 allows taurens to enter rivervale bank door
        public static float GENERATE_WORLD_SCALE = 0.29f;
        public static float GENERATE_CREATURE_SCALE = 0.255f;
        public static float GENERATE_EQUIPMENT_PLAYER_SCALE = 0.35f; // The size of equipment on players
        public static float GENERATE_EQUIPMENT_CREATURE_SCALE = 0.255f; // The size of equipment on creatures/npcs

        // Identifier for what subset of expansion data to work with.  0: Classic, 1: Kunark, 2: Velious
        public static int GENERATE_EQ_EXPANSION_ID_GENERAL = 2; // Not advisable to set this lower than 2
        public static int GENERATE_EQ_EXPANSION_ID_ZONES = 2;
        public static int GENERATE_EQ_EXPANSION_ID_TRANSPORTS = 2;
        public static int GENERATE_EQ_EXPANSION_ID_TRADESKILLS = 2;
        public static int GENERATE_EQ_EXPANSION_ID_EQUIPMENT_GRAPHICS = 1;

        // If true, DBC files are extracted every time.
        public static bool GENERATE_EXTRACT_DBC_FILES = true;

        // If true, then objects are generated
        public static bool GENERATE_OBJECTS = true;

        // If true, then creatures are generated
        public static bool GENERATE_CREATURES_AND_SPAWNS = true;

        // If true, then item armor player graphics are generated (and if set in [PATH_ASSETS_FOLDER]\CustomTextures\item\texturecomponents)
        public static bool GENERATE_PLAYER_ARMOR_GRAPHICS = true;

        // If true, transports (ships, ferries) will be generated
        public static bool GENERATE_TRANSPORTS = true;

        // If true, quests are generated
        public static bool GENERATE_QUESTS = true;

        // If true, generate and copy maps / minimaps
        public static bool GENERATE_WORLDMAPS = true;

        // An extra amount to add to the boundary boxes when generating wow assets from EQ.  Needed to handle rounding.
        public static float GENERATE_ADDED_BOUNDARY_AMOUNT = 0.01f;

        // How many insert rows to restrict in a SQL output file
        public static int GENERATE_SQL_FILE_BATCH_SIZE = 50000;
        public static int GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE = 5000;

        // How many file names to batch up when converting (must be greater or equal to 1)
        public static int GENERATE_BLPCONVERTBATCHSIZE = 50;

        // What edge buffer to add when doing floating point month
        public static float GENERATE_FLOAT_EPSILON = 0.001f;

        // If true, SQL files will be generated in a way where they will have a unique ID to force an update if ran by azerothcore, regardless of changes
        public static bool GENERATE_FORCE_SQL_UPDATES = true;

        // The minimum size that boundary boxes should be for any object models when output
        public static float GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE = 5f;

        // If true, guild banks will now appear. In some cases this will replace an existing banker, others will add a new guild bank NPC object
        public static bool GENERATE_ENABLE_GUILD_VAULTS = true;

        // If true, Priests of Discord (in Norrath) will teleport players to Azeroth, and Azeroth will have Priests of Discord to send players back to Norrath
        // Note that CreatureFactionClassAlignment.csv and CreatureFactionRaceAlignment.csv factor into Norrath destinations
        public static bool GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION = true;
        public static int GENERATE_ENABLE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID = 55813;

        // If false, equipment is balanced to max level 60 and original levels are used. If true, use adjusted levels and zones/equip is balanced to 80
        // with Classic through 60, Kunark through 70, and Velious through 80. Zones will also have a smoother level curve if set to true (NYI)
        public static bool GENERATE_REBALANCE_CONTENT_TO_LEVEL_80 = false;

        // If false, unobtainable items will not output to the database
        public static bool GENERATE_NON_PLAYER_OBTAINABLE_ITEMS = false;

        //=====================================================================
        // Player
        //=====================================================================
        // If true, new players created will use the everquest start locations defined in PlayerClassRaceProperties
        public static bool PLAYER_USE_EQ_START_LOCATION = false;

        // If true, players will start with an EQ item loadout instead of a WOW item loadout
        public static bool PLAYER_USE_EQ_START_ITEMS = false;

        // If true, this will also add a hearthstone if using EQ items
        public static bool PLAYER_ADD_HEARTHSTONE_IF_USE_EQ_START_ITEMS = true;

        // If true, players start with a bind and gate spell regardless of class (with no costs)
        public static bool PLAYER_ADD_CUSTOM_BIND_AND_GATE_ON_START = true;

        // These properties are for replacing the collision for many race models that otherwise wouldn't fit in most doorways (bigger than human male)
        // NOTE: This WILL change the camera-center value for any reduced models.
        public static bool PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED = true;
        // Default value here is max that allows all but Halfling doors to be entered by all, which seems to be just above Night Elf Female
        public static float PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX = 2.275f;

        // If true, all wow classes will gain access to the related skills from level 1, per class alignments in PlayerClassMappings.csv
        public static bool PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES = true;
        public static bool PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES = true;
        public static bool PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES = true;
        public static bool PLAYER_SKILL_ENABLE_BOWS_ON_ALL_APPROPRIATE_EQ_ALIGNED_CLASSES = true;

        // If true, DeathKnights will start at level 1 and not be locked to the starter area (and comes with runeforging)
        // Warning: This should only be done if you plan to use EQ start locations, otherwise you'll just be 'stuck' as a level 1 in a hard area
        public static bool PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES = false;

        //=====================================================================
        // Zone General
        //=====================================================================
        // If this is set to false, any static graphics (like dirt, etc) are not rendered.  Only set to false for debugging
        public static bool ZONE_SHOW_STATIC_GEOMETRY = true;

        // Maximum number of faces that fit into a render WMO group before it subdivides (max is due to various variable limits)
        public static int ZONE_MAX_FACES_PER_WMOGROUP = 21840;

        // Maximum size of any zone-to-material-object creation along the X and Y axis
        public static float ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE = 325.0f;

        // If true, some zones (like gfay and halas) will hide sunlight with a shadowbox
        public static bool ZONE_ALLOW_SUN_HIDING_WITH_SHADOWBOX_ENABLED = true;

        // How much bigger to make the box which causes the shadow in a shadowbox
        public static float ZONE_SHADOW_BOX_ADDED_SIZE = 50f;

        // If true, allow collision with world model objects. This will also impact music and ambient sounds, since they align to areas which require collision detection
        public static bool ZONE_COLLISION_ENABLED = true;

        // If true, this makes all visable geometry collidable. Should only be used for development/debugging purposes (like water coordinates).
        public static bool ZONE_ENABLE_COLLISION_ON_ALL_ZONE_RENDER_MATERIALS = false;

        // When zone geometry gets broken into cuboids, this is the max side length of the area
        public static int ZONE_SPLIT_COLLISION_CUBOID_MAX_EDGE_LEGNTH = 1200;

        // If set to 'true', show a box where the music zones are. This is for debugging only. Only works when collision is enabled
        public static bool ZONE_DRAW_COLLIDABLE_SUB_AREAS_AS_BOXES = false;

        // Maxinum number of triangle faces that can be in any zone-to-material-object
        public static int ZONE_MAX_FACES_PER_ZONE_MATERIAL_OBJECT = 2000;

        // BSP tree nodes will stop subdividing when this many (or less) triangles are found
        public static UInt16 ZONE_BTREE_MIN_SPLIT_SIZE = 10;

        // BSP tree nodes won't operate on bounding boxes smaller than this (X + Y + Z lengths)
        public static float ZONE_BTREE_MIN_BOX_SIZE_TOTAL = 5f;

        // BSP tree nodes won't generate deeper than this many iterations
        public static int ZONE_BTREE_MAX_NODE_GEN_DEPTH = 50;

        // Which ID to use if a graveyard isn't mapped for a zone.  13 is in east commons next to EC tunnel.
        public static int ZONE_DEFAULT_GRAVEYARD_ID = 13;

        // ID for the creature template for the spirit healer.
        public static int ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID = 6491;

        // If true, enable weather in zones.
        public static bool ZONE_WEATHER_ENABLED = true;

        // If true, characters can fly in the zones if they have a mount
        public static bool ZONE_FLYING_ALLOWED = true;

        //=====================================================================
        // Dungeons
        //=====================================================================
        // If true, dungeon finder can be used for special versions of EQ dungeons
        public static bool DUNGEON_FINDER_ENABLED = true;

        //=====================================================================
        // World Maps (and Minimaps)
        //=====================================================================
        // When true, many various proprties are changed to support generation of minimaps, such as 'baking' in animated textures
        // Leave this false unless generating maps in order to make minimaps, otherwise you'll have terrible visuals and performance
        public static bool WORLDMAP_DEBUG_GENERATION_MODE_ENABLED = false;

        // Borders on any maps that were generated, which is blank space and important to mark since coordinates in map space are calculated at generation
        public static int WORLDMAP_LEFT_BORDER_PIXEL_SIZE = 2;
        public static int WORLDMAP_RIGHT_BORDER_PIXEL_SIZE = 2;
        public static int WORLDMAP_TOP_BORDER_PIXEL_SIZE = 2;
        public static int WORLDMAP_BOTTOM_BORDER_PIXEL_SIZE = 2;

        // Controls showing suggested levels on the linked maps
        public static bool WORLDMAP_SHOW_SUGGESTED_LEVELS_ON_LINKED_MAPS = true;

        //=====================================================================
        // Events
        //=====================================================================
        // This value is used in generating end timestamps in things like game_event. At time of writing, the max value of
        // AzerothCore's game event time is based on MYSQL TIMESTAMP which caps at 2038-01-19 03:14:07
        public static int EVENTS_MAX_DATETIME_YEAR = 2037;

        // If true, all day or night creature spawn events will have their day/time normalized, and only
        // one event will be created for each.
        public static bool EVENTS_DO_NORMALIZE_GAME_EVENTS = true;
        public static int EVENTS_NORMALIZED_DAY_SPAWN_START_HOUR = 5;
        public static int EVENTS_NORMALIZED_DAY_SPAWN_LENGTH_IN_HOUR = 16;
        public static int EVENTS_NORMALIZED_NIGHT_SPAWN_START_HOUR = 21;
        public static int EVENTS_NORMALIZED_NIGHT_SPAWN_LENGTH_IN_HOUR = 8;

        //=====================================================================
        // Liquid
        //=====================================================================
        // If true, liquid is enabled
        public static bool LIQUID_ENABLED = true;

        // If this is true, it will show the true surface line of water and not just the material from EQ.  This should only be used
        // for debugging as it very visually unpleasant
        public static bool LIQUID_SHOW_TRUE_SURFACE = false;

        // How much 'height' to add to liquid surface, helps with rendering the waves
        public static float LIQUID_SURFACE_ADD_Z_HEIGHT = 0.001f;

        // How much to overlap the planes when generating an irregular quad of liquid
        public static float LIQUID_QUADGEN_PLANE_OVERLAP_SIZE = 0.0001f;

        //=====================================================================
        // Lighting and Coloring
        //=====================================================================
        // If true, light instances are enabled.  They don't work at this time, so leave false
        public static bool LIGHT_INSTANCES_ENABLED = false;

        // If true, light instances are rendered as torches.  Use for debugging only, and typically leave false
        public static bool LIGHT_INSTANCES_DRAWN_AS_TORCHES = false;

        // Sets the modifier to add to the attenuation to define the start, calculated by multiplying this value to it
        public static float LIGHT_INSTANCE_ATTENUATION_START_PROPORTION = 0.25f;

        // Amonut of glow to add to outdoor areas (ranges are 0-1)
        public static float LIGHT_OUTSIDE_GLOW_CLEAR_WEATHER = 0.4f;
        public static float LIGHT_OUTSIDE_GLOW_STORMY_WEATHER = 0.2f;
        public static float LIGHT_OUTSIDE_GLOW_UNDERWATER = 0.6f;

        // Brightness of outdoor areas based on time
        public static byte LIGHT_OUTSIDE_AMBIENT_TIME_0 = 58; // Midnight
        public static byte LIGHT_OUTSIDE_AMBIENT_TIME_3 = 58; // 3 AM
        public static byte LIGHT_OUTSIDE_AMBIENT_TIME_6 = 164; // 6 AM
        public static byte LIGHT_OUTSIDE_AMBIENT_TIME_12 = 239; // Noon - Must always be the most bright
        public static byte LIGHT_OUTSIDE_AMBIENT_TIME_21 = 192; // 9 PM
        public static byte LIGHT_OUTSIDE_AMBIENT_TIME_22 = 58; // 10 PM

        // Storm brightness
        public static float LIGHT_STORM_COLOR_MOD = 0.8f;

        //=====================================================================
        // Audio
        //=====================================================================       
        // Set which soundfont to use in the Tools/soundfont folder.  Alternate option is synthusr_samplefix.sf2
        public static string AUDIO_SOUNDFONT_FILE_NAME = "AweROMGM.sf2";

        // If set to true, some audio tracks are swapped vs the original tracks.  Make it false if you want a more classic-like experience
        public static bool AUDIO_USE_ALTERNATE_TRACKS = false;

        // How much to increase the music sound when converted from EverQuest
        public static decimal AUDIO_MUSIC_CONVERSION_GAIN_AMOUNT = 1;

        // If true, all of the music tracks will get converted, not just the ones that appear in zones
        public static bool AUDIO_MUSIC_FORCE_GENERATE_ALL_MUSIC_TRACKS = false;

        // Mod / multiplier to volumes (multiplies the volume by this value)
        // NOTE: Sound Instance volumes can be found in Sound.GetVolume
        public static float AUDIO_AMBIENT_SOUND_VOLUME_MOD = 1f;
        public static float AUDIO_SOUNDINSTANCE_VOLUME_MOD = 1f; // Also game objects
        public static float AUDIO_MUSIC_VOLUME_MOD = 1f;

        // If this is 'true', draw any sound instances in a zone as a little box
        public static bool AUDIO_SOUNDINSTANCE_DRAW_AS_BOX = false;

        // The radius of a sound instance is multiplied by this to get the min distance, which is the range which the sound is 100% volume
        public static float AUDIO_SOUNDINSTANCE_3D_MIN_DISTANCE_MOD = 0.4f;
        public static float AUDIO_SOUNDINSTANCE_2D_MIN_DISTANCE_MOD = 0.8f;

        // Size of the box when rendering a sound instance (Note: It's 1/2 the in-game size)
        public static float AUDIO_SOUNDINSTANCE_RENDEROBJECT_BOX_SIZE = 1f;

        // Name of the object material that is used when rendering the soundinstance object
        public static string AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME = "akafloorgrate";

        // Volume of creature sound effects like attacks and being hit
        public static float AUDIO_CREATURE_SOUND_VOLUME = 0.3f;

        // Creature sound radius (in CreatureRaces) are multilpied for this for the min distance, which is the range of max volume
        public static float AUDIO_CREATURE_MIN_DISTANCE_MOD = 0.5f;

        // Volume of spells and other effects
        public static float AUDIO_SPELL_SOUND_VOLUME = 0.3f;

        //=====================================================================
        // Objects
        //=====================================================================
        // For ladders, this is how far to extend out the steppable area in front and back of it (percentage of thickness)
        public static float OBJECT_STATIC_LADDER_EXTEND_DISTANCE = 0.29f;

        // How much space between each step of a ladder along the Z axis (true units)
        public static float OBJECT_STATIC_LADDER_STEP_DISTANCE = 0.145f;

        // How much the lower edge of a ladder step-down plane should be in proportion to its thickness.
        public static float OBJECT_STATIC_LADDER_STEP_DROP_DISTANCE_MOD = 1.07f;

        // How long (in ms) the open/close animation will be for game objects
        public static int OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS = 1000;
        public static int OBJECT_GAMEOBJECT_OPENCLOSE_SLEEPER_FIELD_ANIMATIONTIME_INMS = 5000;

        // How big of an area that a tradeskill focus item (forge, cooking fire) covers in effect
        public static int OBJECT_GAMEOBJECT_TRADESKILLFOCUS_EFFECT_AREA_MIN_SIZE = 5;

        // If true, custom mailboxes are put into the game as 'postmen'
        public static bool OBJECT_GAMEOBJECT_ENABLE_MAILBOXES = true;

        // If true, a fixed respawn timer will be used for 'ground objects', and if false then the EQ respawn timers will be used
        public static bool OBJECT_GAMEOBJECT_CHEST_USE_FIXED_RESPAWN_TIMER = true;

        // If OBJECT_GAMEOBJECT_CHEST_USE_FIXED_RESPAWN_TIMER is true, then this is how many seconds will elapse before the ground objects respawn
        public static int OBJECT_GAMEOBJECT_CHEST_FIXED_RESPAWN_TIME_IN_SEC = 120;

        // The starting ID for any material index that should be ignored from rendering
        public static int OBJECT_IGNORE_RENDER_MATERIAL_ID_START = 10000;

        //=====================================================================
        // Creatures
        //=====================================================================
        // This is the percent of the idle time that a 'fidget' occurs (1-100)
        // Note: There are two fidget animations, 1/2 this number applies to each fidget
        public static int CREATURE_FIDGET_TIME_PERCENT = 30;

        // Stat modifiers for creatures
        // - "MIN" and "MAX" are applied after all other calculations
        public static float CREATURE_STAT_MOD_HP_ADD = 1.5f;
        public static float CREATURE_STAT_MOD_HP_MIN = 1.5f;
        public static float CREATURE_STAT_MOD_HP_MAX_NORMAL = 4f;
        public static float CREATURE_STAT_MOD_HP_MAX_RARE = 5f;
        public static float CREATURE_STAT_MOD_HP_SET_ELITE = 20f;
        public static float CREATURE_STAT_MOD_HP_SET_ELITERARE = 80f;
        public static float CREATURE_STAT_MOD_HP_SET_BOSS = 200f;
        public static float CREATURE_STAT_MOD_AVGDMG_ADD = 0.6f;
        public static float CREATURE_STAT_MOD_AVGDMG_MIN = 1f;
        public static float CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL = 2.5f;
        public static float CREATURE_STAT_MOD_AVGDMG_MAX_RARE = 2.5f;
        public static float CREATURE_STAT_MOD_AVGDMG_SET_ELITE = 8f;
        public static float CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE = 10f;
        public static float CREATURE_STAT_MOD_AVGDMG_SET_BOSS = 18f;

        // If true, creature pets will have the same scaling as set above
        public static bool CREATURE_PET_ALLOW_STAT_MOD_SCALING = false;

        // The value to name the everquest parent reputation item as
        public static string CREATURE_FACTION_ROOT_NAME = "EverQuest";

        // The default faction values to use if none can be mapped.  Using the 'neutral' record for now.
        public static int CREATURE_FACTION_TEMPLATE_DEFAULT = 2302;
        public static int CREATURE_FACTION_DEFAULT = 1200;

        // For any quest or merchant NPCs that aren't aligned to a raisable or lowerable faction, they will be mapped to this.  Default is Norrath Settlers.
        public static int CREATURE_FACTION_TEMPLATE_NEUTRAL = 2302;
        public static int CREATURE_FACTION_TEMPLATE_NEUTRAL_INTERACTIVE = 2313;

        // If set to true, all factions will show up for EverQuest in the faction list immediately
        public static bool CREATURE_FACTION_SHOW_ALL = true;

        // What to multiple the EverQuest reputation rewards by.  WOW is approx 20-30x that of EQ in band.
        public static int CREATURE_REP_REWARD_MULTIPLIER = 20;

        // ID for the menu text (328 exists already and is just "Greetings, $n")
        public static int CREATURE_GOSSIP_NPC_TEXT_ID = 328;

        // ID for the menu broadcast texts 
        public static int CREATURE_GOSSIP_TRAIN_BROADCAST_TEXT_ID = 2548; // Pre-exists, "I would like to train."
        public static int CREATURE_GOSSIP_UNLEARN_BROADCAST_TEXT_ID = 62295; // Pre-exists, "I wish to unlearn my talents."
        public static int CREATURE_GOSSIP_DUALTALENT_BROADCAST_TEXT_ID = 33762; // Pre-exists, "I wish to know about Dual Talent Specialization."
        public static int CREATURE_GOSSIP_PURCHASE_BROADCAST_TEXT_ID = 3370; // Pre-exists, "I want to browse your goods."

        // IDs for menus specific to class trainer
        public static int CREATURE_CLASS_TRAINER_UNLEARN_MENU_ID = 4461; // Pre-exists
        public static int CREATURE_CLASS_TRAINER_DUALTALENT_MENU_ID = 10371; // Pre-exists

        // If true, riding trainers will be generated
        public static bool CREATURE_RIDING_TRAINERS_ENABLED = true;

        // If true, riding trainers will include flying mounts as well
        public static bool CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY = true;

        // Minimum amount of duration a creature buff buff needs to be in order to be cast out of combat
        public static int CREATURE_SPELL_OOC_BUFF_MIN_DURATION_IN_MS = 60000;

        // How long to wait initially before casting a buff, to stagger casting a bit
        public static int CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MIN_IN_MS = 500;
        public static int CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MAX_IN_MS = 5000;
        public static int CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_RANDOM_RANGE_ADD_IN_MS = 2000;

        // How much time to add the the max recast delay for combat spells so that there's a bit of variation
        public static float CREATURE_SPELL_COMBAT_RECAST_DELAY_MAX_ADD_MOD = 0.25f;

        // At what level of life a creature should cast a heal spell, if they have one
        public static int CREATURE_SPELL_COMBAT_HEAL_MIN_LIFE_PERCENT = 30;

        // Percent (0-100) of the normal mana regeneration rate that spell-casting creatures should have, with approximately 10% being more EQ like
        public static int CREATURE_MANA_REGEN_PERCENT = 10;

        // If "GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION" is true, this is the text
        // that displays when you talk to a Priest of Discord
        public static string CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT = "Ah, child of Azeroth. The Priests of Discord have torn the rift wide so that your wars and ours may bleed together. Where shall I send you to spread the Discord?";
        public static string CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT = "Hail, traveler of Norrath. I can only send you back through the rift to the land in Azeroth that calls calls to your blood. Let your arrival there spread confusion and chaos among your kin. Where does your heritage demand I deliver you?";
        public static string CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT = "Greetings. As much as I wish to help you sow Discord, you carry with you the recent echo of one of our portals. See me again once that echo fades, and I shall aid you.";

        // If true, any creature initial spawn location will instead be the first node in the path grid, but only for paths managed by the AzerothCore engine
        public static bool CREATURE_SPAWN_LOCATION_TAKEN_FROM_GRID_FOR_NON_CUSTOM_PATH = true;

        //=====================================================================
        // Items
        //=====================================================================
        // If true, this uses alternate stats for items that have been tweaked for balance reasons
        public static bool ITEMS_USE_ALTERNATE_STATS = true;
        
        // This is how much is reduced from the weapon delay of EQ weapons, value is 0 - 1;
        public static float ITEMS_WEAPON_DELAY_REDUCTION_AMT = 0.2f;

        // This is the base PPM (Procs Per Minute) used for weapon proc weapons
        public static float ITEMS_WEAPON_EFFECT_PPM_BASE_RATE = 2f;

        // If true, gear that has a worn effect will show as a buff on the character
        public static bool ITEMS_SHOW_WORN_EFFECT_AURA_ICON = true;

        // If true, any item that is clickable item that also has a spell will be replaced with a 
        // container item that contains both the equippable item as well as a non-equipable version
        // that can be clicked from inventory.  WOW doesn't let you click equipable spell items
        // from inventory
        public static bool ITEMS_CREATE_ESSENCE_ITEM_FOR_EQUIPEABLE_CLICK_SPELL_ITEMS = true;

        // This is how much 'weight' the lower stat has when converting EQ to WoW stats, with 
        //  values closer to 1 leaning towards the lower stat, and further from 1 leaning towards
        //  the higher stat.  Don't make it less than 1.  
        public static float ITEMS_STATS_LOW_BIAS_WEIGHT = 1f;

        // How much to multiple the slot size of a bag in EQ.  EQ allows for 2x the number bags of WOW (not including starter)
        public static int ITEMS_BAG_SLOT_MULTIPLIER = 2;

        // If true, weight reduction on bags will translate to additional slots
        // Note that the add percent rounds up to the next multiple of 2
        public static bool ITEMS_BAG_WEIGHT_REDUCTION_INCREASES_SLOTS_ENABLED = true;
        public static float ITEM_BAG_WEIGHT_REDUCTION_INCREASE_SLOTS_ADD_PER_PERCENT = 0.08f;

        // This is the icon ID that is used for multi-item containers that contain more than one item
        // The ID here is the icon ID as defined by X in "INV_EQ_X.blp"
        public static int ITEMS_MULTI_ITEMS_CONTAINER_ICON_ID = 57;

        // Arrows reuse existing WOW models, and the specific model is defined here
        public static string ITEM_ARROW_MODEL_NAME = "ArrowFlight_01.mdx";
        public static string ITEM_ARROW_TEXTURE_NAME = "Arrow_A_01Brown";

        // Spell ID for the visual effect from Monk's epic weapon (Celestial Fists)
        public static int ITEMS_MONK_EPIC_GLOVES_IT159_SPELL_ID = 86903;

        // Spell IDs for the +fishing effect of bait
        public static int ITEMS_FISHING_BAIT_POTENCY_TIER_1_SPELL_ID = 8087;
        public static int ITEMS_FISHING_BAIT_POTENCY_TIER_2_SPELL_ID = 8088;

        // ID for TotemCategory.dbc for bard instrument groups (any instrument of the matching type satisfies a song that requires it)
        public static int ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_WIND = 215;
        public static int ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_STRING = 216;
        public static int ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_BRASS = 217;
        public static int ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_PERCUSSION = 218;
        public static int ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_ALL = 219;

        //=====================================================================
        // Quests
        //=====================================================================
        // How many milliseconds to display a text block from an NPC on quest events
        public static int QUESTS_TEXT_DURATION_IN_MS = 10000;

        // This is the icon ID that is used for quest rewards that contain more than one random item
        // The ID here is the icon ID as defined by X in "INV_EQ_X.blp"
        public static int QUESTS_ITEMS_REWARD_CONTAINER_ICON_ID = 57;

        //=====================================================================
        // Spells
        //=====================================================================
        // If this is true, use the level as defined in everquest for summoned pets as well
        // as the control behavior.  Otherwise it will behave like a controllable pet and
        // is level aligned with the player
        // NOTE: Not currently working right, so leave false.  Lots of work left before this is good.
        public static bool SPELL_EFFECT_SUMMON_PETS_USE_EQ_LEVEL_AND_BEHAVIOR = false;

        // This is how high (WOW side) stats will be be scaled to.  This should almost always be
        // set to the server max level configuration as it just handles 'trickle up' stats
        public static int SPELL_EFFECT_CALC_STATS_FOR_MAX_LEVEL = 80;

        // If true, the player can return to their gate point by clicking off the buff (within 30 minutes)
        public static bool SPELLS_GATE_TETHER_ENABLED = true;

        // IDs for special spells that need an exact match of ID between this and mod-everquest
        public static int SPELLS_GATECUSTOM_SPELLDBC_ID = 86900;
        public static int SPELLS_BINDCUSTOM_SPELLDBC_ID = 86901;

        // How much to multiply the EQ range value for WoW
        public static float SPELLS_RANGE_MULTIPLIER = 0.3333f;

        // The most that a movement speed reduction can slow a target.  Should be above -100
        public static int SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE = -90;

        // Everquest has a 'tick' every 6 seconds, so buffs and debuffs should use this as a multiplier
        // Increase or decrease this to modify how long spells work for and, in effect, the damage they do
        // World of Warcraft typically uses a reduced amount and dummy targets start out of combat events at >5 seconds
        // It's highly advisable to use 3 or 2 since they are divisors of 6, but note that generally smaller has
        // rounding issues on the bottom end from a balance perspective
        public static int SPELL_PERIODIC_SECONDS_PER_TICK_EQ = 6;
        public static int SPELL_PERIODIC_SECONDS_PER_TICK_WOW = 3;

        // This is 'added time' in the periodic tick that comes from bard casters.  WOW 3.3.5 does not have
        // 'rolling dots', so without any kind of buffer there won't be a damage/heal 'tick' that occurs
        // on targets near the bard since the spell would get overridden right when a tick would occur
        public static int SPELL_PERIODIC_BARD_TICK_BUFFER_IN_MS = 50;

        // Bards can have this many songs playing at the same time.
        // - Set as 0 to disable this entirely, allowing all songs to play at once
        // - Set as 1 to have a more EQ like experience
        public static int SPELL_MAX_CONCURRENT_BARD_SONGS = 3;

        // This is the minimum allowable recovery time any spell can have, which any smaller will become zero
        // and only subjected to the global cooldown of 1.5 seconds.  This is only enforced on the raw spell
        // records and not the SpellTemplate, to ensure cast repeats are correct for creatures
        public static int SPELL_RECOVERY_TIME_MINIMUM_IN_MS = 3501;

        // The percent chance that a feign death spell cast fails
        public static int SPELL_FEIGN_DEATH_FAIL_CHANCE_PERCENT = 5;

        // If true, you can learn spells from items
        public static bool SPELLS_LEARNABLE_FROM_ITEMS_ENABLED = true;

        // All spell properties
        public static int SPELLS_EFFECT_EMITTER_LONGEST_SPELL_TIME_IN_MS = 16000;

        // How often weapon procs occur
        public static int SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE = 25;
        public static int SPELLS_ENCHANT_SPELL_IMBUE_PROC_CHANGE = 25;

        // How long rogue poisons stay on the weapons
        public static int SPELL_ENCHANT_ROGUE_POISON_ENCHANT_DURATION_ON_WEAPON_TIME_IN_SECONDS = 3600;

        // How long it takes to apply rogue poison
        public static int SPELL_ENCHANT_ROGUE_POISON_APPLY_TIME_IN_MS = 4000;

        // What to show when a rogue has a poison, 0 will disable it (and be more EQ like)
        public static int SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_APPLYING_VISUAL_ID = 1168;
        public static int SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_EFFECT_VISUAL_ID = 26;

        // Spell emitter particles
        public static float SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_SPELL = 0.05f;
        public static float SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_ENVIRONMENT = 0.15f;
        public static float SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_SPELL = 0.8f;
        public static float SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_ENVIRONMENT = 1f;
        public static int SPELLS_EFFECT_EMITTER_TARGET_DURATION_IN_MS = 5000;
        public static float SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_SPELL = 0.3048f;
        public static float SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_ENVIRONMENT = 0.3048f;
        public static float SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_SPELL = 0.75f;
        public static float SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_ENVIRONMENT = 1f;
        public static int SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MINIMUM = 25;
        public static int SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_DEFAULT = 25;
        public static int SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_DEFAULT = 25;
        public static float SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MOD = 4f;
        public static float SPELL_EFFECT_EMITTER_SPAWN_RATE_DISC_MOD = 1f;
        public static float SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_MOD = 1.25f;

        // Sprite List particles
        public static float SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MIN = 0.1f;
        public static float SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX = 10f;
        public static float SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX_EQ_VALUE = 25f; // Anything greater than 15 is treated as 15
        public static float SPELL_EFFECT_SPRITE_LIST_RADIUS_MOD = 0.3048f;
        public static float SPELL_EFFECT_SPRITE_LIST_ANIMATION_SCALE_MOD = 0.5f;
        public static int SPELL_EFFECT_SPRITE_LIST_ANIMATION_FRAME_DELAY_IN_MS = 60;
        public static int SPELL_EFFECT_SPRITE_LIST_MAX_NON_PREJECTILE_ANIM_TIME_IN_MS = 2500;
        public static float SPELL_EFFECT_SPRITE_LIST_PULSE_RANGE = 1.5f;
        public static float SPELL_EFFECT_SPRITE_LIST_CIRCULAR_SHIFT_MOD = 0.5f; // Various reports show this as 0.066 for EQ like (divide by 15) but that seems wrong
        public static float SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_HIGH = 3f;
        public static float SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_LOW = -1f;

        // BardTick visuals have their durations multiplied by this ammount.  It uses the
        // SPELL_PERIODIC_SECONDS_PER_TICK_WOW variable to calculate the amount.  So at 0.333,
        // a 6 second tick would be 2 seconds (EQ like) and 3 second tick would be 1 second
        public static float SPELL_EFFECT_BARD_TICK_VISUAL_DURATION_MOD_FROM_TICK = 0.333f;

        // If this is true, then when a bard song is cast then a tick is applied immediately on targets
        public static bool SPELL_EFFECT_BARD_ADDITIONAL_TICK_ON_CAST = true;

        // This is how much 'weight' the lower effect value has when converting EQ to WoW spell effects,
        //  with values closer to 1 leaning towards the lower effect, and further from 1 leaning towards
        //  the higher effect.  Don't make it less than 1.  
        public static float SPELL_EFFECT_VALUE_LOW_BIAS_WEIGHT = 1.7f;

        // If true, the damage formula will honor spell level based values, otherwise it'll use maximum
        public static bool SPELL_EFFECT_USE_DYNAMIC_EFFECT_VALUES = true;
        public static bool SPELL_EFFECT_USE_DYNAMIC_AURA_DURATIONS = true;

        // Revive will give HP/MP instead of EXP on revive, so this is the multiplier to use for that
        public static int SPELL_EFFECT_REVIVE_EXPPCT_TO_HPMP_MULTIPLIER = 22;

        // Default time that a shrink/grow spell will last for
        public static int SPELL_MODEL_SIZE_CHANGE_EFFECT_DEFAULT_TIME_IN_MS = 1800000;

        // Values for the cooldown spells applied by Priests of Discord when you switch worlds, setting cooldown duration to 0 will disable it
        public static int SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID = 86902;
        public static int SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN = 30;

        // This is the default amount influence spell strength by spell power (when not overriden)
        public static float SPELL_DEFAULT_SPELL_POWER_INFLUENCE_PERCENT = 0.33f;

        // Summoner dummy spell ID used to prevent creatures from summoning more creatures
        public static int SPELL_SUMMON_CASTER_AURA_SPELL_ID = 86905;

        // Hidden passive aura used to reduce creature mana regeneration (see CREATURE_MANA_REGEN_PERCENT)
        public static int SPELL_CREATURE_REDUCED_MANA_REGEN_SPELL_ID = 86907;

        // This is the ID used to detect invis and stealth, and already exists in AzerothCore
        public static int SPELL_CREATURE_SEE_INVIS_AND_STEALTH_SPELL_ID = 18950;

        // WoW invisibility group (InvisibilityType) reserved for EQ "invis vs undead" (0 = general invis, 1 should be unused)
        public static int SPELL_INVIS_VS_UNDEAD_INVIS_TYPE = 1;

        // Custom detect aura granted to everything that should see through "invis vs undead" (non-undead + see_invis_undead undead)
        public static int SPELL_CREATURE_INVIS_VS_UNDEAD_DETECT_SPELL_ID = 86913;

        // EQ has no "daze" snare when a creature melee-hits a player from behind so this can disable it (in EQ zones only)
        public static bool COMBAT_DAZE_IN_EQ_ZONES_ENABLED = true;

        //=====================================================================
        // Combat Skills (adjacent to spells)
        //=====================================================================
        // Bash skills in EQ are either from warrior/cleric/paladin/shadowknight or those that use warrior skills
        public static bool COMBATSKILL_BASH_ENABLED = true;
        public static bool COMBATSKILL_BASH_PLAYER_LEARNABLE = true;
        public static int COMBATSKILL_BASH_SPELL_ID = 86908;
        public static int COMBATSKILL_BASH_SPELL_ICON_EQ_ID = 11;
        public static int COMBATSKILL_BASH_CREATURE_MIN_LEVEL = 6;
        public static int COMBATSKILL_BASH_COOLDOWN_IN_MS = 8000;
        public static int COMBATSKILL_BASH_STUN_DURATION_IN_MS = 2000;
        public static int COMBATSKILL_BASH_BASE_DAMAGE = 4;
        public static float COMBATSKILL_BASH_DAMAGE_PER_LEVEL = 1.5f;
        public static int COMBATSKILL_BASH_RANGE = 5;
        // Bash Forbearance gives protection from bash to prevent stunlocking
        public static bool COMBATSKILL_BASH_FORBEARANCE_ENABLED = true;
        public static int COMBATSKILL_BASH_FORBEARANCE_SPELL_ID = 86906;
        public static int COMBATSKILL_BASH_FORBEARANCE_SPELL_ICON_EQ_ID = 18;
        public static int COMBATSKILL_BASH_FORBEARANCE_DURATION_IN_MS = 5000;

        // Slam is an Ogre/Troll/Barb racial in EQ (a Bash-equivalent that needs no shield). It shares a cooldown with Bash.
        // Granted to players by race (PlayerClassRaceProperties.HasSlam), available from level 1.
        public static bool COMBATSKILL_SLAM_ENABLED = true;
        public static bool COMBATSKILL_SLAM_PLAYER_LEARNABLE = true;
        public static int COMBATSKILL_SLAM_SPELL_ID = 86911;
        public static int COMBATSKILL_SLAM_SPELL_ICON_EQ_ID = 9;
        public static int COMBATSKILL_SLAM_STUN_DURATION_IN_MS = 2000;
        public static int COMBATSKILL_SLAM_BASE_DAMAGE = 4;
        public static float COMBATSKILL_SLAM_DAMAGE_PER_LEVEL = 1.5f;
        public static int COMBATSKILL_SLAM_RANGE = 5;

        // Harm Touch is a shadowknight ability (a long-cooldown direct damage "touch"). Also granted to player Death Knights. HP is ~2.5x higher in WoW
        public static bool COMBATSKILL_HARMTOUCH_ENABLED = true;
        public static bool COMBATSKILL_HARMTOUCH_PLAYER_LEARNABLE = true;
        public static int COMBATSKILL_HARMTOUCH_SPELL_ID = 86909;
        public static int COMBATSKILL_HARMTOUCH_SPELL_ICON_EQ_ID = 3;
        public static int COMBATSKILL_HARMTOUCH_CREATURE_MIN_LEVEL = 1;
        public static int COMBATSKILL_HARMTOUCH_COOLDOWN_IN_MS = 2400000;
        public static int COMBATSKILL_HARMTOUCH_CREATURE_INITIAL_DELAY_IN_MS = 1000;
        public static int COMBATSKILL_HARMTOUCH_BASE_DAMAGE = 50; // 20 is EQ normal, but HP is 2.5x higher in WoW (generally)
        public static float COMBATSKILL_HARMTOUCH_DAMAGE_PER_LEVEL = 20.0f; // 8 is EQ normal, but HP is 2.5x higher in WoW (generally)
        public static int COMBATSKILL_HARMTOUCH_RANGE = 30;

        // Lay on Hands is a paladin ability (a long-cooldown large self heal used when badly hurt). HP is ~2.5x higher in WoW
        public static bool COMBATSKILL_LAYONHANDS_ENABLED = true;
        public static int COMBATSKILL_LAYONHANDS_SPELL_ID = 86910;
        public static int COMBATSKILL_LAYONHANDS_SPELL_ICON_EQ_ID = 10;
        public static int COMBATSKILL_LAYONHANDS_CREATURE_MIN_LEVEL = 1;
        public static int COMBATSKILL_LAYONHANDS_COOLDOWN_IN_MS = 2400000;
        public static int COMBATSKILL_LAYONHANDS_HEALTH_TRIGGER_PCT = 20;
        public static int COMBATSKILL_LAYONHANDS_BASE_HEAL = 125; // 50  is EQ normal, but HP is 2.5x higher in WoW (generally)
        public static float COMBATSKILL_LAYONHANDS_HEAL_PER_LEVEL = 75.0f; // 30 is EQ normal, but HP is 2.5x higher in WoW (generally)

        // Feign Death is a monk ability (a self ability that drops aggro by playing dead, instant like in TAKP)
        public static bool COMBATSKILL_FEIGNDEATH_ENABLED = true;
        public static bool COMBATSKILL_FEIGNDEATH_PLAYER_LEARNABLE = true;
        public static int COMBATSKILL_FEIGNDEATH_SPELL_ID = 86912;
        public static int COMBATSKILL_FEIGNDEATH_SPELL_ICON_EQ_ID = 7;
        public static int COMBATSKILL_FEIGNDEATH_FAIL_CHANCE_PERCENT = 5;
        public static int COMBATSKILL_FEIGNDEATH_COOLDOWN_IN_MS = 1500;

        //=====================================================================
        // Fishing
        //=====================================================================
        // How much to multiply the EQ fish catching skill requirement for WOW
        public static float FISHING_SKILL_CONVERSION_MOD_60 = 0.8772f;
        public static float FISHING_SKILL_CONVERSION_MOD_80 = 1.3432f;

        //=====================================================================
        // Forage
        //=====================================================================
        // Which eq spell icon to use for the Forage skill. Can be a value between 0-22
        public static int FORAGE_SPELL_ICON_EQ_ID = 16;

        // Spell id for the forage spell
        public static int FORAGE_SPELL_TEMPLATE_ID = 86904;

        //=====================================================================
        // Tradeskills
        //=====================================================================
        // How much to multiply EQ skill requirements by to reach the same for WoW on conversion
        public static float TRADESKILLS_CONVERSION_MOD_60 = 0.8772f;
        public static float TRADESKILLS_CONVERSION_MOD_80 = 1.3432f;

        // Max distance between Grey -> Green -> Yellow -> Red steps
        public static int TRADESKILLS_SKILL_TIER_DISTANCE_LOW = 10;
        public static int TRADESKILLS_SKILL_TIER_DISTANCE_HIGH = 25;

        // The skill level of a tradeskill will be priced closest to the value for that WOW skill level
        public static int TRADESKILL_LEARN_COST_AT_1 = 10;
        public static int TRADESKILL_LEARN_COST_AT_50 = 500;
        public static int TRADESKILL_LEARN_COST_AT_100 = 1000;
        public static int TRADESKILL_LEARN_COST_AT_200 = 2850;
        public static int TRADESKILL_LEARN_COST_AT_300 = 12500;
        public static int TRADESKILL_LEARN_COST_AT_450 = 150000;

        // How long every tradeskill will take in milliseconds
        public static int TRADESKILL_CAST_TIME_IN_MS = 5000;

        // Tradeskill items that need a totem in TotemCategory.dbc will align under this
        public static int TRADESKILL_TOTEM_CATEGORY_START = 30;

        // ID for TotemCategory.dbc for specific tradeskills
        public static int TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING = 210;
        public static int TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX = 211;
        public static int TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING = 212;
        public static int TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY = 213;
        public static int TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING = 214;

        //=====================================================================
        // Transports
        //=====================================================================
        public static float TRANSPORT_PAUSE_MULTIPLIER = 0.5f; // Pause as in 'stop at a port'. 1 will be EQ-like
        public static int TRANSPORT_MOVE_SPEED = 30; // Most boats are 30 in WoW, but a value of around 9 is EQ-like
        public static int TRANSPORT_ACCELERATION = 1;
        public static bool TRANSPORT_ALLOW_FIXED_SPEEDS = true; // If true, allows "fixed_speed" to override TRANSPORT_MOVE_SPEED

        // ====================================================================
        // WOW DBC/File IDs
        // ====================================================================
        // IDs for AreaBit used in AreaTable, should be unique (max of 4095)
        public static int DBCID_AREATABLE_AREABIT_BLOCK_1_START = 3092;
        public static int DBCID_AREATABLE_AREABIT_BLOCK_1_END = 3172;
        public static int DBCID_AREATABLE_AREABIT_BLOCK_2_START = 3462;
        public static int DBCID_AREATABLE_AREABIT_BLOCK_2_END = 3616;
        public static int DBCID_AREATABLE_AREABIT_BLOCK_3_START = 3800;
        public static int DBCID_AREATABLE_AREABIT_BLOCK_3_END = 4095;

        // Identifies Area rows in AreaTable.dbc
        // Note: These are hardcoded in ZoneSubAreas.csv and ZoneProperties.csv
        public static UInt32 DBCID_AREATABLE_ID_START = 5100;
        public static UInt32 DBCID_AREATABLE_ID_END = 6000;

        // IDs for AreaTrigger.DBC. These will be generated in ascending order by MapID, and referenced in SQL scripts
        // for teleports as well any other area-based triggers. Defined in ZoneLineBoxes.csv
        public static int DBCID_AREATRIGGER_ID_START = 6500;
        public static int DBCID_AREATRIGGER_ID_END = 8500;

        // IDs for CreatureDisplayInfo.dbc
        public static int DBCID_CREATUREDISPLAYINFO_ID_START = 34000;
        public static int DBCID_CREATUREDISPLAYINFO_ID_END = 40000;

        // IDs for CreatureDisplayInfoExtra.dbc
        public static int DBCID_CREATUREDISPLAYINFOEXTRA_ID_START = 23000;
        public static int DBCID_CREATUREDISPLAYINFOEXTRA_ID_END = 23010;

        // IDs for CreatureModelData.dbc
        public static int DBCID_CREATUREMODELDATA_ID_START = 3500;
        public static int DBCID_CREATUREMODELDATA_ID_END = 5000;

        // IDs for CreatureSoundData.dbc
        public static int DBCID_CREATURESOUNDDATA_ID_START = 3300;

        // IDs for FootstepTerrainLookup.dbc
        public static int DBCID_FOOTSTEPTERRAINLOOKUP_ID_START = 600;
        public static int DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START = 250;

        // IDs for rows inside GameObjectDisplayInfo.dbc
        public static int DBCID_GAMEOBJECTDISPLAYINFO_ID_START = 11000;

        // Start ID for item display info
        public static int DBCID_ITEMDISPLAYINFO_START = 86000;

        // IDs for LFGDungeonGroup.dbc
        public static int DBCID_LFGDUNGEONGROUP_DUNGEONS_ID = 15;
        public static int DBCID_LFGDUNGEONGROUP_DUNGEONS_ORDER_ID = 6;
        public static int DBCID_LFGDUNGEONGROUP_RAIDS_ID = 15;
        public static int DBCID_LFGDUNGEONGROUP_RAIDS_ORDER_ID = 22;

        // Start ID for LFGDungeons.dbc
        public static int DBCID_LFGDUNGEONS_ID_START = 300;

        // Identifies the Light.DBC row, used for environmental properties
        public static int DBCID_LIGHT_ID_START = 3500;

        // Identifies the LightParams.dbc, used for detailed values related to a Light.DBC row
        public static int DBCID_LIGHTPARAMS_ID_START = 1050;

        // IDs for the loading screen
        public static int DBCID_LOADINGSCREEN_ID_START = 255;

        // Identifies Maps in Map.dbc and MapDifficulty.dbc
        // Note: This value is hard coded in /WorldData/ZoneProperties.csv, TransportShips.csv, and ZoneDisplayMapContinents.csv, so you cannot change only this value
        // Values from 901+ are dungeon finder
        public static int DBCID_MAP_ID_START = 750;
        public static int DBCID_MAP_ID_END = 975;

        // ID for SkillLine.dbc
        public static int DBCID_SKILLLINE_ID_START = 810;

        // ID for skill line abilities found in SkillLineAbility.dbc
        public static int DBCID_SKILLLINEABILITY_ID_START = 25000;

        // ID for SkillRaceClassInfo.dbc
        public static int DBCID_SKILLRACECLASSINFO_ID_START = 1010;

        // ID for sounds found in SoundEntries.dbc
        public static int DBCID_SOUNDENTRIES_ID_START = 22000;

        // ID for sounds found in SoundAmbience.dbc
        public static int DBCID_SOUNDAMBIENCE_ID_START = 600;

        // ID for spells found in Spell.dbc
        // - Manually created spells reserve IDs from 86900 to 86999 and all are defined in the config
        // - Recipes reserve IDs 87000 to 91362
        // - Converted spells IDs start at 92000 and base spells range to 95826,
        // - SpellIDs 96000 - 96049 reserved for 'worn' effects (effects that always take effect when worn)
        // - SpellIDs 96100 - 96199 reserved for 'coat' effects that come from rogue poisons, triggering another spell
        // - SpellIDs 96200 - 97656 reserved for 'clicky' effects (defined in ItemTemplates.csv under clickeffect_wow)
        // - SpellIDs 97700 - 97715 reserved for 'good clicky' effects (defined in SpellTemplate.csv under wow_good_proc_id)
        // - SpellIDs 98000+ (or as defined below) used for 'generated spell IDs'
        public static int DBCID_SPELL_ID_START = 86900;
        public static int DBCID_SPELL_ID_GENERATED_START = 98000;
        public static int DBCID_SPELL_ID_END = 110000;

        // ID for spellcasttimes.dbc
        public static int DBCID_SPELLCASTTIME_ID_START = 215;

        // ID for spellcategory.dbc
        public static int DBCID_SPELLCATEGORY_ID_START = 1300;

        // ID for spellduration.dbc
        public static int DBCID_SPELLDURATION_AURA_ID = 610;

        // Stand ID for spell item ennchantments
        public static int DBCID_SPELLITEMENCHANTMENT_ID_START = 4000;

        // ID for spellicon.dbc
        public static int DBCID_SPELLICON_ID_START = 4400;

        // ID for spellradius.dbc
        public static int DBCID_SPELLRADIUS_ID_START = 70;

        // ID for SpellRange.dbc
        public static int DBCID_SPELLRANGE_ID_START = 190;

        // IDs for the SpellVisual line of DBC files
        public static int DBCID_SPELLVISUAL_ID_START = 17000;
        public static int DBCID_SPELLVISUALKIT_ID_START = 16000;
        public static int DBCID_SPELLVISUALEFFECTNAME_ID_START = 7200;

        // IDs for SummonProperties.dbc
        public static int DBCID_SUMMONPROPERTIES_ID_START = 3100;

        // ID for TaxiPath.dbc
        public static int DBCID_TAXIPATH_ID_START = 2000;

        // ID for TaxiPathNode.dbc
        public static int DBCID_TAXIPATHNODE_ID_START = 48000;

        // ID for TotemCategory.dbc
        // NOTE: Categories 210-219 are also reserved, but used for tradeskill containers
        public static UInt32 DBCID_TOTEMCATEGORY_ID_START = 220;

        // ID for TransportAnimation.dbc
        public static int DBCID_TRANSPORTANIMATION_ID_START = 180000;

        // IDs for WorldSafeLocs.dbc, defined in ZoneGraveyards.csv
        public static int DBCID_WORLDSAFELOCS_ID_START = 1800;
        public static int DBCID_WORLDSAFELOCS_ID_END = 2000;

        // Specific rows in WMOAreaTable.dbc
        public static int DBCID_WMOAREATABLE_ID_START = 52000;

        // Identifies WMO Groups. Found in WMOAreaTable.dbc and the .wmo files
        public static UInt32 DBCID_WMOAREATABLE_WMOGROUPID_START = 30000;

        // ID for music in ZoneMusic.dbc, and how many IDs to reserve on a per-zone basis
        public static int DBCID_ZONEMUSIC_START = 700;

        // ====================================================================
        // SQL IDs
        // ====================================================================
        // Start and end IDs for broadcast_text sql records
        public static int SQL_BROADCASTTEXT_ID_START = 80000;
        public static int SQL_BROADCASTTEXT_ID_END = 99999;

        // Record identifier for the creature sql table, need at least 31k
        public static int SQL_CREATURE_GUID_LOW = 310000;
        public static int SQL_CREATURE_GUID_HIGH = 399999;

        // Record identifier for for creature_immunities
        public static int SQL_CREATUREIMMUNITIES_ID_START = 4000;
        public static int SQL_CREATUREIMMUNITIES_ID_END = 4100;

        // Record identifier for the creature template SQL table
        public static int SQL_CREATURETEMPLATE_ENTRY_LOW = 45000;
        public static int SQL_CREATURETEMPLATE_ENTRY_HIGH = 60000;
        public static int SQL_CREATURETEMPLATE_GENERATED_START_ID = 56000;

        // Start GUIDs for gameobjects
        public static int SQL_GAMEOBJECT_GUID_ID_START = 310000;
        public static int SQL_GAMEOBJECT_GUID_ID_END = 319999;

        // IDs for events
        public static int SQL_GAME_EVENTS_ID_START = 200;
        public static int SQL_GAME_EVENTS_ID_END = 225;

        // Start and end IDs for gameobject_template rows
        // - GameObjects.csv owns rows 270000 - 274999
        // - TransportLifts.csv and TransportLiftTriggers.csv own rows 279900 - 279999
        public static int SQL_GAMEOBJECTTEMPLATE_ID_START = 270000;
        public static int SQL_GAMEOBJECTTEMPLATE_ID_END = 279999;
        public static int SQL_GAMEOBJECTTEMPLATE_SHIP_ID_START = 279970;
        public static int SQL_GAMEOBJECTTEMPLATE_SHIP_ID_END = 279989;

        // Start row for `game_tele` records. (~2000-2400)
        public static int SQL_GAMETELE_ROWID_START = 2000;
        public static int SQL_GAMETELE_ROWID_END = 2400;

        // Start and end IDs for custom gossip menu records
        public static int SQL_GOSSIPMENU_MENUID_START = 62000;
        public static int SQL_GOSSIPMENU_MENUID_END = 69999;

        // Start and end IDs for template entries
        // - Starter Version IDs range 110451 - 110499
        // - Class-Specific scroll IDs range 110500 - 112887
        // - Equipped Click Bag IDs range 113000 - 113932
        // - Equipped Click Essence IDs range 114000 - 114932
        // - Quest Template multi-item reward containers IDs range 116000 - 116200
        // - Tradeskill multi-item creation containers IDs range 117000 - 117349
        // - NPC-worn items have IDs 120000 - 145000
        public static int SQL_ITEM_TEMPLATE_ENTRY_START = 85000;
        public static int SQL_ITEM_TEMPLATE_ENTRY_END = 145000;

        // Start and end IDs for npc_text sql records
        public static int SQL_NPCTEXT_ID_START = 80000;
        public static int SQL_NPCTEXT_ID_END = 99999;

        // Start and end IDs for page_text SQL records
        public static int SQL_PAGETEXT_ID_START = 3800;
        public static int SQL_PAGETEXT_ID_END = 4800;

        // Start ID for pet_name_generation entries
        public static int SQL_PETNAMEGENERATION_ID_START = 400;

        // Start and end ID for pool_template data rows (reserve 40k records)
        public static int SQL_POOL_TEMPLATE_ID_START = 110000;
        public static int SQL_POOL_TEMPLATE_ID_END = 150000;

        // Start and end IDs for quest template data rows
        // The 'shift' value is the value to add for the repeatable versions
        public static int SQL_QUEST_TEMPLATE_ID_START = 30000;
        public static int SQL_QUEST_TEMPLATE_ID_END = 40000;
        public static int SQL_QUEST_TEMPLATE_ID_REPEATABLE_SHIFT = 5000;

        // Stard and end IDs for reference_loot_template.entry
        public static int SQL_REFERENCE_LOOT_TEMPLATE_ID_START = 60000;
        public static int SQL_REFERENCE_LOOT_TEMPLATE_ID_END = 60999;

        // Start and end IDs for spell groups
        public static int SQL_SPELL_GROUP_ID_START = 1500;
        public static int SQL_SPELL_GROUP_ID_FOR_BARD_AURA_START = 1750;
        public static int SQL_SPELL_GROUP_ID_END = 1999;

        // Start and end IDs for trainers
        public static int SQL_TRAINER_ID_START = 151;
        public static int SQL_TRAINER_ID_END = 176;

        // Start and end IDs for transports
        public static int SQL_TRANSPORTS_GUID_START = 21;
        public static int SQL_TRANSPORTS_GUID_END = 41;

        // DO NOT CHANGE THESE VALUES UNLESS YOU KNOW WHAT YOU ARE DOING
        // They are temporary values used for configuration purposes, resolved at runtime
        public static string PATH_EQEXPORTSRAW_FOLDER { get => Path.Combine(PATH_WORKING_FOLDER, "EQClientExportRaw"); }
        public static string PATH_EQEXPORTSCONDITIONED_FOLDER { get => Path.Combine(PATH_WORKING_FOLDER, "EQClientExportConditioned"); }
        public static string PATH_EXPORT_FOLDER { get => Path.Combine(PATH_WORKING_FOLDER, "WOWExports"); }

        public static List<string> SplitIntoFixedLines(string text, int maxLength = 80)
        {
            List<string> lines = new List<string>();
            if (string.IsNullOrEmpty(text) == true)
                return lines;

            StringBuilder sb = new StringBuilder();
            int currentLength = 0;
            string[] words = text.Trim().Split(' ');
            foreach (string word in words)
            {
                if (string.IsNullOrEmpty(word) == true)
                    continue;

                int wordLength = word.Length;
                int addedLength = wordLength + (currentLength > 0 ? 1 : 0);
                if (currentLength + addedLength > maxLength)
                {
                    if (currentLength > 0)
                    {
                        lines.Add(sb.ToString());
                        sb.Clear();
                        currentLength = 0;
                    }
                    if (wordLength > maxLength)
                    {
                        lines.Add(word);
                        currentLength = 0;
                        continue;
                    }
                }
                if (currentLength > 0)
                {
                    sb.Append(' ');
                    currentLength++;
                }

                sb.Append(word);
                currentLength += wordLength;
            }
            if (currentLength > 0)
                lines.Add(sb.ToString());

            return lines;
        }

        public static T ReadVariableFromConfigString<T>(string configVariableName, Dictionary<string, string> configValuesByVariableName, T defaultValue)
        {
            if (configValuesByVariableName.ContainsKey(configVariableName) == false)
            {
                Logger.WriteDebug("No variable found in config value with name '", configVariableName, "', so using default value");
                return defaultValue;
            }
            if (string.IsNullOrWhiteSpace(configValuesByVariableName[configVariableName]) == true)
                return defaultValue;
            try
            {
                return (T)Convert.ChangeType(configValuesByVariableName[configVariableName].Trim(), typeof(T));
            }
            catch
            {
                Logger.WriteError("Error parsing config value '", configValuesByVariableName[configVariableName], "' for variable '", configVariableName, "', so using default");
                return defaultValue;
            }
        }

        public static void OutputTextLineToConfig(string textLine)
        {
            File.AppendAllText(CONFIGONLY_CONFIGURATION_FILE_NAME, String.Concat(textLine, Environment.NewLine));
        }

        public static void OutputBlankLineToConfig()
        {
            File.AppendAllText(CONFIGONLY_CONFIGURATION_FILE_NAME, Environment.NewLine);
        }

        public static void OutputVariableToConfig<T>(string configVariableName, T value, string comment, bool addBlankLineAfter = true)
        {
            StringBuilder outputContentSB = new StringBuilder();

            List<string> restrictedComment = SplitIntoFixedLines(comment, 110);
            foreach (string commentPart in restrictedComment)
            {
                outputContentSB.Append("# ");
                outputContentSB.AppendLine(commentPart);
            }
            outputContentSB.Append(configVariableName);
            outputContentSB.Append(" = ");
            outputContentSB.Append(value);
            outputContentSB.AppendLine(string.Empty);
            if (addBlankLineAfter == true)
                outputContentSB.AppendLine(string.Empty);
            File.AppendAllText(CONFIGONLY_CONFIGURATION_FILE_NAME, outputContentSB.ToString());
        }

        public static void SaveConfiguration()
        {
            if (File.Exists(CONFIGONLY_CONFIGURATION_FILE_NAME) == true)
                File.Delete(CONFIGONLY_CONFIGURATION_FILE_NAME);

            OutputTextLineToConfig("# +---------------------------------------------------------------------------+");
            OutputTextLineToConfig("# | 1. Manditory Path Settings (Set these before it will work)                |");
            OutputTextLineToConfig("# +---------------------------------------------------------------------------+");
            OutputBlankLineToConfig();
            OutputVariableToConfig("PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER", FileTool.RemoveRelativePathIfExists(PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER), "Location of the installed everquest trilogy client (this must have the eqgame.exe file in it)", true);
            OutputVariableToConfig("PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER", FileTool.RemoveRelativePathIfExists(PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER), "Location of the installed enUS version of World of Warcaft client (this must have the wow.exe in it)", true);
            OutputTextLineToConfig("# +---------------------------------------------------------------------------+");
            OutputTextLineToConfig("# | 2. Deployment Settings (Highly suggested to set to make install easier)   |");
            OutputTextLineToConfig("# +---------------------------------------------------------------------------+");
            OutputBlankLineToConfig();
            OutputVariableToConfig("DEPLOY_CLIENT_FILES", DEPLOY_CLIENT_FILES, "If true, deploy the client file (patch mpq) after building it");
            OutputVariableToConfig("DEPLOY_SERVER_FILES", DEPLOY_SERVER_FILES, "If true, deploy to the server files/data after building");
            OutputVariableToConfig("DEPLOY_SERVER_DBC_FOLDER_LOCATION", FileTool.RemoveRelativePathIfExists(DEPLOY_SERVER_DBC_FOLDER_LOCATION), "Location of where the server DBC files would be deployed to (only relevant if you set DEPLOY_SERVER_FILES to true, otherwise ignored)");
            OutputVariableToConfig("DEPLOY_SERVER_SQL", DEPLOY_SERVER_SQL, "If true, deploy to the SQL to the server");
            OutputVariableToConfig("DEPLOY_SQL_CONNECTION_STRING_CHARACTERS", DEPLOY_SQL_CONNECTION_STRING_CHARACTERS, "If deploying to SQL, you need to set these to something real that points to your databases (only relevant if you set DEPLOY_SERVER_SQL to true, otherwise ignored)", false);
            OutputVariableToConfig("DEPLOY_SQL_CONNECTION_STRING_WORLD", DEPLOY_SQL_CONNECTION_STRING_WORLD, "");
            OutputTextLineToConfig("# +---------------------------------------------------------------------------+");
            OutputTextLineToConfig("# | 3. Enhancements / Customizations (Settings that alter the world from EQ)  |");
            OutputTextLineToConfig("# +---------------------------------------------------------------------------+");
            OutputBlankLineToConfig();
            OutputTextLineToConfig("# If false, equipment is balanced to max level 60 and original levels are used. If true, use adjusted levels and zones/equip is balanced to 80");
            OutputVariableToConfig("GENERATE_REBALANCE_CONTENT_TO_LEVEL_80", GENERATE_REBALANCE_CONTENT_TO_LEVEL_80, "with Classic through 60, Kunark through 70, and Velious through 80. Zones will also have a smoother level curve if set to true.");
            OutputVariableToConfig("PLAYER_USE_EQ_START_LOCATION", PLAYER_USE_EQ_START_LOCATION, "If true, new players created will use the everquest start locations defined in PlayerClassRaceProperties");
            OutputVariableToConfig("PLAYER_USE_EQ_START_ITEMS", PLAYER_USE_EQ_START_ITEMS, "If true, players will start with an EQ item loadout instead of a WOW item loadout");
            OutputVariableToConfig("PLAYER_ADD_HEARTHSTONE_IF_USE_EQ_START_ITEMS", PLAYER_ADD_HEARTHSTONE_IF_USE_EQ_START_ITEMS, "If true, this will also add a hearthstone if using EQ items");
            OutputVariableToConfig("PLAYER_ADD_CUSTOM_BIND_AND_GATE_ON_START", PLAYER_ADD_CUSTOM_BIND_AND_GATE_ON_START, "If true, players start with a bind and gate spell regardless of class (with no costs)");
            OutputTextLineToConfig("# If true, DeathKnights will start at level 1 and not be locked to the starter area (and comes with runeforging)");
            OutputVariableToConfig("PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES", PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES, "Warning: This should only be done if you plan to use EQ start locations, otherwise you'll just be 'stuck' as a level 1 in a hard area");
            OutputVariableToConfig("PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES", PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES, "If true, all wow classes will gain access to the related skills from level 1, per class alignments in PlayerClassMappings.csv", false);
            OutputVariableToConfig("PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES", PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES, "", false);
            OutputVariableToConfig("PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES", PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES, "", false);
            OutputVariableToConfig("PLAYER_SKILL_ENABLE_BOWS_ON_ALL_APPROPRIATE_EQ_ALIGNED_CLASSES", PLAYER_SKILL_ENABLE_BOWS_ON_ALL_APPROPRIATE_EQ_ALIGNED_CLASSES, "");
            OutputVariableToConfig("DUNGEON_FINDER_ENABLED", DUNGEON_FINDER_ENABLED, "If true, dungeon finder can be used for special versions of EQ dungeons");
            OutputTextLineToConfig("# If true, Priests of Discord (in Norrath) will teleport players to Azeroth, and Azeroth will have Priests of Discord to send players back to Norrath");
            OutputVariableToConfig("GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION", GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION, "Note that CreatureFactionClassAlignment.csv and CreatureFactionRaceAlignment.csv factor into Norrath destinations", false);
            OutputVariableToConfig("GENERATE_ENABLE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID", GENERATE_ENABLE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID, "");
            OutputVariableToConfig("GENERATE_ENABLE_GUILD_VAULTS", GENERATE_ENABLE_GUILD_VAULTS, "If true, guild banks will now appear. In some cases this will replace an existing banker, others will add a new guild bank NPC object");
            OutputVariableToConfig("OBJECT_GAMEOBJECT_ENABLE_MAILBOXES", OBJECT_GAMEOBJECT_ENABLE_MAILBOXES, "If true, custom mailboxes are put into the game as 'postmen'");
            OutputVariableToConfig("OBJECT_GAMEOBJECT_CHEST_USE_FIXED_RESPAWN_TIMER", OBJECT_GAMEOBJECT_CHEST_USE_FIXED_RESPAWN_TIMER, "If true, a fixed respawn timer will be used for 'ground objects', and if false then the EQ respawn timers will be used");
            OutputVariableToConfig("OBJECT_GAMEOBJECT_CHEST_FIXED_RESPAWN_TIME_IN_SEC", OBJECT_GAMEOBJECT_CHEST_FIXED_RESPAWN_TIME_IN_SEC, "If OBJECT_GAMEOBJECT_CHEST_USE_FIXED_RESPAWN_TIMER is true, then this is how many seconds will elapse before the ground objects respawn");
            OutputVariableToConfig("AUDIO_USE_ALTERNATE_TRACKS", AUDIO_USE_ALTERNATE_TRACKS, "If set to true, some audio tracks are swapped vs the original tracks.  Make it false if you want a more classic-like experience");
            OutputVariableToConfig("SPELL_EFFECT_SUMMON_PETS_USE_EQ_LEVEL_AND_BEHAVIOR", SPELL_EFFECT_SUMMON_PETS_USE_EQ_LEVEL_AND_BEHAVIOR, "If this is true, use the level as defined in everquest for summoned pets as well as the control behavior. (Highly advisable to leave False)");
            OutputVariableToConfig("SPELLS_GATE_TETHER_ENABLED", SPELLS_GATE_TETHER_ENABLED, "If true, the player can return to their gate point by clicking off the buff (within 30 minutes)");
            OutputVariableToConfig("SPELL_MAX_CONCURRENT_BARD_SONGS", SPELL_MAX_CONCURRENT_BARD_SONGS, "Bards can have this many songs playing at the same time.");
            OutputVariableToConfig("SPELL_PERIODIC_SECONDS_PER_TICK_EQ", SPELL_PERIODIC_SECONDS_PER_TICK_EQ, "Everquest has a 'tick' every 6 seconds, so buffs and debuffs should use this as a multiplier (WoW typically has 3)");
            OutputVariableToConfig("CREATURE_RIDING_TRAINERS_ENABLED", CREATURE_RIDING_TRAINERS_ENABLED, "If true, riding trainers will be generated");
            OutputVariableToConfig("CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY", CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY, "If true, riding trainers will include flying mounts as well");
            OutputVariableToConfig("ZONE_FLYING_ALLOWED", ZONE_FLYING_ALLOWED, "If true, characters can fly in the zones if they have a mount");
            OutputVariableToConfig("EVENTS_DO_NORMALIZE_GAME_EVENTS", EVENTS_DO_NORMALIZE_GAME_EVENTS, "If true, all day or night creature spawn events will have their day/time normalized, and only one event will be created for each. Config variables to tweak this further can be found further below");
            OutputVariableToConfig("ITEMS_BAG_WEIGHT_REDUCTION_INCREASES_SLOTS_ENABLED", ITEMS_BAG_WEIGHT_REDUCTION_INCREASES_SLOTS_ENABLED, "If true, weight reduction on bags will translate to additional slots");
            OutputVariableToConfig("PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED", PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED, "These properties are for replacing the collision for many race models that otherwise wouldn't fit in most doorways (bigger than human male)");
            OutputVariableToConfig("PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX", PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX, "Default value here is max that allows all but Halfling doors to be entered by all, which seems to be just above Night Elf Female");
            OutputTextLineToConfig("# +---------------------------------------------------------------------------+");
            OutputTextLineToConfig("# | Other Settings (Tuning or Debugging, typically ignore)                    |");
            OutputTextLineToConfig("# +---------------------------------------------------------------------------+");
            OutputBlankLineToConfig();
            OutputVariableToConfig("PATH_TOOLS_FOLDER", FileTool.RemoveRelativePathIfExists(PATH_TOOLS_FOLDER), "The root of the tools directory (comes with this source code in a folder)");
            OutputVariableToConfig("PATH_ASSETS_FOLDER", FileTool.RemoveRelativePathIfExists(PATH_ASSETS_FOLDER), "The root of the assets directory (comes with this source code in a folder)");
            OutputVariableToConfig("PATH_WORKING_FOLDER", FileTool.RemoveRelativePathIfExists(PATH_WORKING_FOLDER), "The root folder where temporary folders and file will be generated (ensure at least 10GB of space is available in this location)");
            OutputVariableToConfig("PATCH_CLIENT_DATA_ID", PATCH_CLIENT_DATA_ID, "ID to append to the end of the /Data/ patch file (such as the \"4\" in \"patch-4.mpq). Make it uniquely new.");
            OutputVariableToConfig("PATCH_CLIENT_DATA_LOC_ID", PATCH_CLIENT_DATA_LOC_ID, "ID to append to the localized patch file in /Data/<locale> (such as the \"5\" in patch-enUS-5.mpq). Make it uniquely new.");
            OutputVariableToConfig("PATCH_LOCALIZATION_STRING", PATCH_LOCALIZATION_STRING, "What language to generate things as");
            OutputVariableToConfig("DEPLOY_CLEAR_CACHE_ON_CLIENT_DEPLOY", DEPLOY_CLEAR_CACHE_ON_CLIENT_DEPLOY, "If true and when deploying client files, clear the cache (only relevant if you set DEPLOY_CLIENT_FILES to true, otherwise ignored)");
            OutputVariableToConfig("CORE_CONSOLE_BEEP_ON_COMPLETE", CORE_CONSOLE_BEEP_ON_COMPLETE, "Plays a beep sound when the generate completes if set to true");
            OutputVariableToConfig("CORE_ENABLE_MULTITHREADING", CORE_ENABLE_MULTITHREADING, "If true, the conditioner & generator will run in multithreading mode", false);
            OutputVariableToConfig("CORE_ZONEGEN_THREAD_COUNT", CORE_ZONEGEN_THREAD_COUNT, "", false);
            OutputVariableToConfig("CORE_PNGTOBLPCONVERSION_THREAD_COUNT", CORE_PNGTOBLPCONVERSION_THREAD_COUNT, "", false);
            OutputVariableToConfig("CORE_MUSICCONVERSION_THREAD_COUNT", CORE_MUSICCONVERSION_THREAD_COUNT, "");
            OutputVariableToConfig("LOGGING_CONSOLE_MIN_LEVEL", LOGGING_CONSOLE_MIN_LEVEL, "Level of logs to write to the console and log file. 1: Error, 2: Info, 3: Debug", false);
            OutputVariableToConfig("LOGGING_FILE_MIN_LEVEL", LOGGING_FILE_MIN_LEVEL, "");
            OutputVariableToConfig("GENERATE_WORLD_SCALE", GENERATE_WORLD_SCALE, "The value EQ vertices multiply by when translated into WOW vertices. A WORLD_SCALE value of 0.25 seems to be 1:1 with EQ.  0.28 allows humans and 0.4 allows taurens to enter rivervale bank door", false);
            OutputVariableToConfig("GENERATE_CREATURE_SCALE", GENERATE_CREATURE_SCALE, "", false);
            OutputVariableToConfig("GENERATE_EQUIPMENT_PLAYER_SCALE", GENERATE_EQUIPMENT_PLAYER_SCALE, "The size of equipment on players", false);
            OutputVariableToConfig("GENERATE_EQUIPMENT_CREATURE_SCALE", GENERATE_EQUIPMENT_CREATURE_SCALE, "The size of equipment on creatures/npcs");
            OutputVariableToConfig("GENERATE_EQ_EXPANSION_ID_GENERAL", GENERATE_EQ_EXPANSION_ID_GENERAL, "Identifier for what subset of expansion data to work with.  0: Classic, 1: Kunark, 2: Velious.", false);
            OutputVariableToConfig("GENERATE_EQ_EXPANSION_ID_ZONES", GENERATE_EQ_EXPANSION_ID_ZONES, "", false);
            OutputVariableToConfig("GENERATE_EQ_EXPANSION_ID_TRANSPORTS", GENERATE_EQ_EXPANSION_ID_TRANSPORTS, "", false);
            OutputVariableToConfig("GENERATE_EQ_EXPANSION_ID_TRADESKILLS", GENERATE_EQ_EXPANSION_ID_TRADESKILLS, "", false);
            OutputVariableToConfig("GENERATE_EQ_EXPANSION_ID_EQUIPMENT_GRAPHICS", GENERATE_EQ_EXPANSION_ID_EQUIPMENT_GRAPHICS, "");
            OutputVariableToConfig("GENERATE_EXTRACT_DBC_FILES", GENERATE_EXTRACT_DBC_FILES, "If true, DBC files are extracted every time.");
            OutputVariableToConfig("GENERATE_OBJECTS", GENERATE_OBJECTS, "If true, then objects are generated");
            OutputVariableToConfig("GENERATE_CREATURES_AND_SPAWNS", GENERATE_CREATURES_AND_SPAWNS, "If true, then creatures are generated");
            OutputVariableToConfig("GENERATE_PLAYER_ARMOR_GRAPHICS", GENERATE_PLAYER_ARMOR_GRAPHICS, "If true, then item armor player graphics are generated (and if set in [PATH_ASSETS_FOLDER]\\CustomTextures\\item\\texturecomponents)");
            OutputVariableToConfig("GENERATE_TRANSPORTS", GENERATE_TRANSPORTS, "If true, transports (ships, ferries) will be generated");
            OutputVariableToConfig("GENERATE_QUESTS", GENERATE_QUESTS, "If true, quests are generated");
            OutputVariableToConfig("GENERATE_WORLDMAPS", GENERATE_WORLDMAPS, "If true, generate and copy maps / minimaps");
            OutputVariableToConfig("GENERATE_ADDED_BOUNDARY_AMOUNT", GENERATE_ADDED_BOUNDARY_AMOUNT, "An extra amount to add to the boundary boxes when generating wow assets from EQ.  Needed to handle rounding.");
            OutputVariableToConfig("GENERATE_SQL_FILE_BATCH_SIZE", GENERATE_SQL_FILE_BATCH_SIZE, "How many insert rows to restrict in a SQL output file", false);
            OutputVariableToConfig("GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE", GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE, "");
            OutputVariableToConfig("GENERATE_BLPCONVERTBATCHSIZE", GENERATE_BLPCONVERTBATCHSIZE, "How many file names to batch up when converting (must be greater or equal to 1)");
            OutputVariableToConfig("GENERATE_FLOAT_EPSILON", GENERATE_FLOAT_EPSILON, "What edge buffer to add when doing floating point month");
            OutputVariableToConfig("GENERATE_FORCE_SQL_UPDATES", GENERATE_FORCE_SQL_UPDATES, "If true, SQL files will be generated in a way where they will have a unique ID to force an update if ran by azerothcore, regardless of changes");
            OutputVariableToConfig("GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE", GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE, "The minimum size that boundary boxes should be for any object models when output");
            OutputVariableToConfig("GENERATE_NON_PLAYER_OBTAINABLE_ITEMS", GENERATE_NON_PLAYER_OBTAINABLE_ITEMS, "If false, unobtainable items will not output to the database");
            OutputVariableToConfig("ZONE_SHOW_STATIC_GEOMETRY", ZONE_SHOW_STATIC_GEOMETRY, "If this is set to false, any static graphics (like dirt, etc) are not rendered.  Only set to false for debugging");
            OutputVariableToConfig("ZONE_MAX_FACES_PER_WMOGROUP", ZONE_MAX_FACES_PER_WMOGROUP, "Maximum number of faces that fit into a render WMO group before it subdivides (max is due to various variable limits)");
            OutputVariableToConfig("ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE", ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE, "Maximum size of any zone-to-material-object creation along the X and Y axis");
            OutputVariableToConfig("ZONE_ALLOW_SUN_HIDING_WITH_SHADOWBOX_ENABLED", ZONE_ALLOW_SUN_HIDING_WITH_SHADOWBOX_ENABLED, "If true, some zones (like gfay and halas) will hide sunlight with a shadowbox");
            OutputVariableToConfig("ZONE_SHADOW_BOX_ADDED_SIZE", ZONE_SHADOW_BOX_ADDED_SIZE, "How much bigger to make the box which causes the shadow in a shadowbox");
            OutputVariableToConfig("ZONE_COLLISION_ENABLED", ZONE_COLLISION_ENABLED, "If true, allow collision with world model objects. This will also impact music and ambient sounds, since they align to areas which require collision detection");
            OutputVariableToConfig("ZONE_ENABLE_COLLISION_ON_ALL_ZONE_RENDER_MATERIALS", ZONE_ENABLE_COLLISION_ON_ALL_ZONE_RENDER_MATERIALS, "If true, this makes all visable geometry collidable. Should only be used for development/debugging purposes (like water coordinates).");
            OutputVariableToConfig("ZONE_SPLIT_COLLISION_CUBOID_MAX_EDGE_LEGNTH", ZONE_SPLIT_COLLISION_CUBOID_MAX_EDGE_LEGNTH, "When zone geometry gets broken into cuboids, this is the max side length of the area");
            OutputVariableToConfig("ZONE_DRAW_COLLIDABLE_SUB_AREAS_AS_BOXES", ZONE_DRAW_COLLIDABLE_SUB_AREAS_AS_BOXES, "If set to 'true', show a box where the music zones are. This is for debugging only. Only works when collision is enabled");
            OutputVariableToConfig("ZONE_MAX_FACES_PER_ZONE_MATERIAL_OBJECT", ZONE_MAX_FACES_PER_ZONE_MATERIAL_OBJECT, "Maxinum number of triangle faces that can be in any zone-to-material-object");
            OutputVariableToConfig("ZONE_BTREE_MIN_SPLIT_SIZE", ZONE_BTREE_MIN_SPLIT_SIZE, "BSP tree nodes will stop subdividing when this many (or less) triangles are found");
            OutputVariableToConfig("ZONE_BTREE_MIN_BOX_SIZE_TOTAL", ZONE_BTREE_MIN_BOX_SIZE_TOTAL, "BSP tree nodes won't operate on bounding boxes smaller than this (X + Y + Z lengths)");
            OutputVariableToConfig("ZONE_BTREE_MAX_NODE_GEN_DEPTH", ZONE_BTREE_MAX_NODE_GEN_DEPTH, "BSP tree nodes won't generate deeper than this many iterations");
            OutputVariableToConfig("ZONE_DEFAULT_GRAVEYARD_ID", ZONE_DEFAULT_GRAVEYARD_ID, "Which ID to use if a graveyard isn't mapped for a zone.  13 is in east commons next to EC tunnel.");
            OutputVariableToConfig("ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID", ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID, "ID for the creature template for the spirit healer.");
            OutputVariableToConfig("ZONE_WEATHER_ENABLED", ZONE_WEATHER_ENABLED, "If true, enable weather in zones.");
            OutputVariableToConfig("WORLDMAP_DEBUG_GENERATION_MODE_ENABLED", WORLDMAP_DEBUG_GENERATION_MODE_ENABLED, "When true, many various proprties are changed to support generation of minimaps, such as 'baking' in animated textures");
            OutputVariableToConfig("WORLDMAP_LEFT_BORDER_PIXEL_SIZE", WORLDMAP_LEFT_BORDER_PIXEL_SIZE, "Borders on any maps that were generated, which is blank space and important to mark since coordinates in map space are calculated at generation", false);
            OutputVariableToConfig("WORLDMAP_RIGHT_BORDER_PIXEL_SIZE", WORLDMAP_RIGHT_BORDER_PIXEL_SIZE, "", false);
            OutputVariableToConfig("WORLDMAP_TOP_BORDER_PIXEL_SIZE", WORLDMAP_TOP_BORDER_PIXEL_SIZE, "", false);
            OutputVariableToConfig("WORLDMAP_BOTTOM_BORDER_PIXEL_SIZE", WORLDMAP_BOTTOM_BORDER_PIXEL_SIZE, "");
            OutputVariableToConfig("WORLDMAP_SHOW_SUGGESTED_LEVELS_ON_LINKED_MAPS", WORLDMAP_SHOW_SUGGESTED_LEVELS_ON_LINKED_MAPS, "Controls showing suggested levels on the linked maps");
            OutputVariableToConfig("EVENTS_MAX_DATETIME_YEAR", EVENTS_MAX_DATETIME_YEAR, "This value is used in generating end timestamps in things like game_event. At time of writing, the max value of AzerothCore's game event time is based on MYSQL TIMESTAMP which caps at 2038-01-19 03:14:07");
            OutputVariableToConfig("EVENTS_NORMALIZED_DAY_SPAWN_START_HOUR", EVENTS_NORMALIZED_DAY_SPAWN_START_HOUR, "If true, all day or night creature spawn events will have their day/time normalized, and only one event will be created for each.", false);
            OutputVariableToConfig("EVENTS_NORMALIZED_DAY_SPAWN_LENGTH_IN_HOUR", EVENTS_NORMALIZED_DAY_SPAWN_LENGTH_IN_HOUR, "", false);
            OutputVariableToConfig("EVENTS_NORMALIZED_NIGHT_SPAWN_START_HOUR", EVENTS_NORMALIZED_NIGHT_SPAWN_START_HOUR, "", false);
            OutputVariableToConfig("EVENTS_NORMALIZED_NIGHT_SPAWN_LENGTH_IN_HOUR", EVENTS_NORMALIZED_NIGHT_SPAWN_LENGTH_IN_HOUR, "");
            OutputVariableToConfig("LIQUID_ENABLED", LIQUID_ENABLED, "If true, liquid is enabled");
            OutputVariableToConfig("LIQUID_SHOW_TRUE_SURFACE", LIQUID_SHOW_TRUE_SURFACE, "If this is true, it will show the true surface line of water and not just the material from EQ.  This should only be used for debugging as it very visually unpleasant");
            OutputVariableToConfig("LIQUID_SURFACE_ADD_Z_HEIGHT", LIQUID_SURFACE_ADD_Z_HEIGHT, "How much 'height' to add to liquid surface, helps with rendering the waves");
            OutputVariableToConfig("LIQUID_QUADGEN_PLANE_OVERLAP_SIZE", LIQUID_QUADGEN_PLANE_OVERLAP_SIZE, "How much to overlap the planes when generating an irregular quad of liquid");
            OutputVariableToConfig("LIGHT_INSTANCES_ENABLED", LIGHT_INSTANCES_ENABLED, "If true, light instances are enabled.  They don't work at this time, so leave false");
            OutputVariableToConfig("LIGHT_INSTANCES_DRAWN_AS_TORCHES", LIGHT_INSTANCES_DRAWN_AS_TORCHES, "If true, light instances are rendered as torches.  Use for debugging only, and typically leave false");
            OutputVariableToConfig("LIGHT_INSTANCE_ATTENUATION_START_PROPORTION", LIGHT_INSTANCE_ATTENUATION_START_PROPORTION, "Sets the modifier to add to the attenuation to define the start, calculated by multiplying this value to it");
            OutputVariableToConfig("LIGHT_OUTSIDE_GLOW_CLEAR_WEATHER", LIGHT_OUTSIDE_GLOW_CLEAR_WEATHER, "Amonut of glow to add to outdoor areas (ranges are 0-1)");
            OutputVariableToConfig("LIGHT_OUTSIDE_GLOW_STORMY_WEATHER", LIGHT_OUTSIDE_GLOW_STORMY_WEATHER, "Amonut of glow to add to outdoor areas (ranges are 0-1)");
            OutputVariableToConfig("LIGHT_OUTSIDE_GLOW_UNDERWATER", LIGHT_OUTSIDE_GLOW_UNDERWATER, "Amonut of glow to add to outdoor areas (ranges are 0-1)");
            OutputVariableToConfig("LIGHT_OUTSIDE_AMBIENT_TIME_0", LIGHT_OUTSIDE_AMBIENT_TIME_0, "Brightness of outdoor areas based on time", false);
            OutputVariableToConfig("LIGHT_OUTSIDE_AMBIENT_TIME_3", LIGHT_OUTSIDE_AMBIENT_TIME_3, "", false);
            OutputVariableToConfig("LIGHT_OUTSIDE_AMBIENT_TIME_6", LIGHT_OUTSIDE_AMBIENT_TIME_6, "", false);
            OutputVariableToConfig("LIGHT_OUTSIDE_AMBIENT_TIME_12", LIGHT_OUTSIDE_AMBIENT_TIME_12, "", false);
            OutputVariableToConfig("LIGHT_OUTSIDE_AMBIENT_TIME_21", LIGHT_OUTSIDE_AMBIENT_TIME_21, "", false);
            OutputVariableToConfig("LIGHT_OUTSIDE_AMBIENT_TIME_22", LIGHT_OUTSIDE_AMBIENT_TIME_22, "");
            OutputVariableToConfig("LIGHT_STORM_COLOR_MOD", LIGHT_STORM_COLOR_MOD, "Storm brightness");
            OutputVariableToConfig("AUDIO_SOUNDFONT_FILE_NAME", AUDIO_SOUNDFONT_FILE_NAME, "Set which soundfont to use in the Tools/soundfont folder.  Alternate option is synthusr_samplefix.sf2");
            OutputVariableToConfig("AUDIO_MUSIC_CONVERSION_GAIN_AMOUNT", AUDIO_MUSIC_CONVERSION_GAIN_AMOUNT, "How much to increase the music sound when converted from EverQuest");
            OutputVariableToConfig("AUDIO_MUSIC_FORCE_GENERATE_ALL_MUSIC_TRACKS", AUDIO_MUSIC_FORCE_GENERATE_ALL_MUSIC_TRACKS, "If true, all of the music tracks will get converted, not just the ones that appear in zones");
            OutputVariableToConfig("AUDIO_AMBIENT_SOUND_VOLUME_MOD", AUDIO_AMBIENT_SOUND_VOLUME_MOD, "Mod / multiplier to volumes (multiplies the volume by this value)", false);
            OutputVariableToConfig("AUDIO_SOUNDINSTANCE_VOLUME_MOD", AUDIO_SOUNDINSTANCE_VOLUME_MOD, "", false);
            OutputVariableToConfig("AUDIO_MUSIC_VOLUME_MOD", AUDIO_MUSIC_VOLUME_MOD, "");
            OutputVariableToConfig("AUDIO_SOUNDINSTANCE_DRAW_AS_BOX", AUDIO_SOUNDINSTANCE_DRAW_AS_BOX, "If this is 'true', draw any sound instances in a zone as a little box");
            OutputVariableToConfig("AUDIO_SOUNDINSTANCE_3D_MIN_DISTANCE_MOD", AUDIO_SOUNDINSTANCE_3D_MIN_DISTANCE_MOD, "The radius of a sound instance is multiplied by this to get the min distance, which is the range which the sound is 100% volume", false);
            OutputVariableToConfig("AUDIO_SOUNDINSTANCE_2D_MIN_DISTANCE_MOD", AUDIO_SOUNDINSTANCE_2D_MIN_DISTANCE_MOD, "");
            OutputVariableToConfig("AUDIO_SOUNDINSTANCE_RENDEROBJECT_BOX_SIZE", AUDIO_SOUNDINSTANCE_RENDEROBJECT_BOX_SIZE, "Size of the box when rendering a sound instance (Note: It's 1/2 the in-game size)");
            OutputVariableToConfig("AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME", AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME, "Name of the object material that is used when rendering the soundinstance object");
            OutputVariableToConfig("AUDIO_CREATURE_SOUND_VOLUME", AUDIO_CREATURE_SOUND_VOLUME, "Volume of creature sound effects like attacks and being hit");
            OutputVariableToConfig("AUDIO_CREATURE_MIN_DISTANCE_MOD", AUDIO_CREATURE_MIN_DISTANCE_MOD, "Creature sound radius (in CreatureRaces) are multilpied for this for the min distance, which is the range of max volume");
            OutputVariableToConfig("AUDIO_SPELL_SOUND_VOLUME", AUDIO_SPELL_SOUND_VOLUME, "Volume of spells and other effects");
            OutputVariableToConfig("OBJECT_STATIC_LADDER_EXTEND_DISTANCE", OBJECT_STATIC_LADDER_EXTEND_DISTANCE, "For ladders, this is how far to extend out the steppable area in front and back of it (percentage of thickness)");
            OutputVariableToConfig("OBJECT_STATIC_LADDER_STEP_DISTANCE", OBJECT_STATIC_LADDER_STEP_DISTANCE, "How much space between each step of a ladder along the Z axis (true units)");
            OutputVariableToConfig("OBJECT_STATIC_LADDER_STEP_DROP_DISTANCE_MOD", OBJECT_STATIC_LADDER_STEP_DROP_DISTANCE_MOD, "How much the lower edge of a ladder step-down plane should be in proportion to its thickness.");
            OutputVariableToConfig("OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS", OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS, "How long (in ms) the open/close animation will be for game objects", false);
            OutputVariableToConfig("OBJECT_GAMEOBJECT_OPENCLOSE_SLEEPER_FIELD_ANIMATIONTIME_INMS", OBJECT_GAMEOBJECT_OPENCLOSE_SLEEPER_FIELD_ANIMATIONTIME_INMS, "");
            OutputVariableToConfig("OBJECT_GAMEOBJECT_TRADESKILLFOCUS_EFFECT_AREA_MIN_SIZE", OBJECT_GAMEOBJECT_TRADESKILLFOCUS_EFFECT_AREA_MIN_SIZE, "How big of an area that a tradeskill focus item (forge, cooking fire) covers in effect");
            OutputVariableToConfig("OBJECT_IGNORE_RENDER_MATERIAL_ID_START", OBJECT_IGNORE_RENDER_MATERIAL_ID_START, "The starting ID for any material index that should be ignored from rendering");
            OutputVariableToConfig("CREATURE_FIDGET_TIME_PERCENT", CREATURE_FIDGET_TIME_PERCENT, "This is the percent of the idle time that a 'fidget' occurs (1-100)");
            OutputVariableToConfig("CREATURE_STAT_MOD_HP_ADD", CREATURE_STAT_MOD_HP_ADD, "Stat modifiers for creatures - \"MIN\" and \"MAX\" are applied after all other calculations", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_HP_MIN", CREATURE_STAT_MOD_HP_MIN, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_HP_MAX_NORMAL", CREATURE_STAT_MOD_HP_MAX_NORMAL, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_HP_MAX_RARE", CREATURE_STAT_MOD_HP_MAX_RARE, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_HP_SET_ELITE", CREATURE_STAT_MOD_HP_SET_ELITE, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_HP_SET_ELITERARE", CREATURE_STAT_MOD_HP_SET_ELITERARE, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_HP_SET_BOSS", CREATURE_STAT_MOD_HP_SET_BOSS, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_AVGDMG_ADD", CREATURE_STAT_MOD_AVGDMG_ADD, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_AVGDMG_MIN", CREATURE_STAT_MOD_AVGDMG_MIN, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL", CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_AVGDMG_MAX_RARE", CREATURE_STAT_MOD_AVGDMG_MAX_RARE, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_AVGDMG_SET_ELITE", CREATURE_STAT_MOD_AVGDMG_SET_ELITE, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE", CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE, "", false);
            OutputVariableToConfig("CREATURE_STAT_MOD_AVGDMG_SET_BOSS", CREATURE_STAT_MOD_AVGDMG_SET_BOSS, "");
            OutputVariableToConfig("CREATURE_PET_ALLOW_STAT_MOD_SCALING", CREATURE_PET_ALLOW_STAT_MOD_SCALING, "If true, creature pets will have the same scaling as set above");
            OutputVariableToConfig("CREATURE_FACTION_ROOT_NAME", CREATURE_FACTION_ROOT_NAME, "The value to name the everquest parent reputation item as");
            OutputVariableToConfig("CREATURE_FACTION_TEMPLATE_DEFAULT", CREATURE_FACTION_TEMPLATE_DEFAULT, "The default faction values to use if none can be mapped.  Using the 'neutral' record for now.", false);
            OutputVariableToConfig("CREATURE_FACTION_DEFAULT", CREATURE_FACTION_DEFAULT, "");
            OutputVariableToConfig("CREATURE_FACTION_TEMPLATE_NEUTRAL", CREATURE_FACTION_TEMPLATE_NEUTRAL, "For any quest or merchant NPCs that aren't aligned to a raisable or lowerable faction, they will be mapped to this.  Default is Norrath Settlers.", false);
            OutputVariableToConfig("CREATURE_FACTION_TEMPLATE_NEUTRAL_INTERACTIVE", CREATURE_FACTION_TEMPLATE_NEUTRAL_INTERACTIVE, "");
            OutputVariableToConfig("CREATURE_FACTION_SHOW_ALL", CREATURE_FACTION_SHOW_ALL, "If set to true, all factions will show up for EverQuest in the faction list immediately");
            OutputVariableToConfig("CREATURE_REP_REWARD_MULTIPLIER", CREATURE_REP_REWARD_MULTIPLIER, "What to multiple the EverQuest reputation rewards by.  WOW is approx 20-30x that of EQ in band.");
            OutputVariableToConfig("CREATURE_GOSSIP_NPC_TEXT_ID", CREATURE_GOSSIP_NPC_TEXT_ID, "ID for the menu text (328 exists already and is just \"Greetings, $n\")");
            OutputTextLineToConfig("# ID for the menu broadcast texts");
            OutputVariableToConfig("CREATURE_GOSSIP_TRAIN_BROADCAST_TEXT_ID", CREATURE_GOSSIP_TRAIN_BROADCAST_TEXT_ID, "Pre-exists, \"I would like to train.\"", false);
            OutputVariableToConfig("CREATURE_GOSSIP_UNLEARN_BROADCAST_TEXT_ID", CREATURE_GOSSIP_UNLEARN_BROADCAST_TEXT_ID, "Pre-exists, \"I wish to unlearn my talents.\"", false);
            OutputVariableToConfig("CREATURE_GOSSIP_DUALTALENT_BROADCAST_TEXT_ID", CREATURE_GOSSIP_DUALTALENT_BROADCAST_TEXT_ID, "Pre-exists, \"I wish to know about Dual Talent Specialization.\"", false);
            OutputVariableToConfig("CREATURE_GOSSIP_PURCHASE_BROADCAST_TEXT_ID", CREATURE_GOSSIP_PURCHASE_BROADCAST_TEXT_ID, "Pre-exists, \"I want to browse your goods.\"");
            OutputVariableToConfig("CREATURE_CLASS_TRAINER_UNLEARN_MENU_ID", CREATURE_CLASS_TRAINER_UNLEARN_MENU_ID, "IDs for menus specific to class trainer", false);
            OutputVariableToConfig("CREATURE_CLASS_TRAINER_DUALTALENT_MENU_ID", CREATURE_CLASS_TRAINER_DUALTALENT_MENU_ID, "");
            OutputVariableToConfig("CREATURE_SPELL_OOC_BUFF_MIN_DURATION_IN_MS", CREATURE_SPELL_OOC_BUFF_MIN_DURATION_IN_MS, "Minimum amount of duration a creature buff buff needs to be in order to be cast out of combat");
            OutputVariableToConfig("CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MIN_IN_MS", CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MIN_IN_MS, "How long to wait initially before casting a buff, to stagger casting a bit", false);
            OutputVariableToConfig("CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MAX_IN_MS", CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MAX_IN_MS, "", false);
            OutputVariableToConfig("CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_RANDOM_RANGE_ADD_IN_MS", CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_RANDOM_RANGE_ADD_IN_MS, "");
            OutputVariableToConfig("CREATURE_SPELL_COMBAT_RECAST_DELAY_MAX_ADD_MOD", CREATURE_SPELL_COMBAT_RECAST_DELAY_MAX_ADD_MOD, "How much time to add the the max recast delay for combat spells so that there's a bit of variation");
            OutputVariableToConfig("CREATURE_SPELL_COMBAT_HEAL_MIN_LIFE_PERCENT", CREATURE_SPELL_COMBAT_HEAL_MIN_LIFE_PERCENT, "At what level of life a creature should cast a heal spell, if they have one");
            OutputVariableToConfig("CREATURE_MANA_REGEN_PERCENT ", CREATURE_MANA_REGEN_PERCENT, "Percent (0-100) of the normal mana regeneration rate that spell-casting creatures should have, with approximately 10% being more EQ like");
            OutputTextLineToConfig("# If \"GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION\" is true, this is the text");
            OutputVariableToConfig("CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT", CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT, "that displays when you talk to a Priest of Discord", false);
            OutputVariableToConfig("CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT", CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT, "");
            OutputVariableToConfig("CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT", CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT, "");
            OutputVariableToConfig("CREATURE_SPAWN_LOCATION_TAKEN_FROM_GRID_FOR_NON_CUSTOM_PATH", CREATURE_SPAWN_LOCATION_TAKEN_FROM_GRID_FOR_NON_CUSTOM_PATH, "If true, any creature initial spawn location will instead be the first node in the path grid, but only for paths managed by the AzerothCore engine");
            OutputVariableToConfig("ITEMS_USE_ALTERNATE_STATS", ITEMS_USE_ALTERNATE_STATS, "If true, this uses alternate stats for items that have been tweaked for balance reasons");
            OutputVariableToConfig("ITEMS_WEAPON_DELAY_REDUCTION_AMT", ITEMS_WEAPON_DELAY_REDUCTION_AMT, "This is how much is reduced from the weapon delay of EQ weapons, value is 0 - 1;");
            OutputVariableToConfig("ITEMS_WEAPON_EFFECT_PPM_BASE_RATE", ITEMS_WEAPON_EFFECT_PPM_BASE_RATE, "This is the base PPM (Procs Per Minute) used for weapon proc weapons");
            OutputVariableToConfig("ITEMS_SHOW_WORN_EFFECT_AURA_ICON", ITEMS_SHOW_WORN_EFFECT_AURA_ICON, "If true, gear that has a worn effect will show as a buff on the character");
            OutputVariableToConfig("ITEMS_CREATE_ESSENCE_ITEM_FOR_EQUIPEABLE_CLICK_SPELL_ITEMS", ITEMS_CREATE_ESSENCE_ITEM_FOR_EQUIPEABLE_CLICK_SPELL_ITEMS, "If true, any item that is clickable item that also has a spell will be replaced with a container item that contains both the equippable item as well as a non-equipable version that can be clicked from inventory.");
            OutputVariableToConfig("ITEMS_STATS_LOW_BIAS_WEIGHT", ITEMS_STATS_LOW_BIAS_WEIGHT, "This is how much 'weight' the lower stat has when converting EQ to WoW stats, with values closer to 1 leaning towards the lower stat, and further from 1 leaning towards the higher stat.");
            OutputVariableToConfig("ITEMS_BAG_SLOT_MULTIPLIER", ITEMS_BAG_SLOT_MULTIPLIER, "How much to multiple the slot size of a bag in EQ.  EQ allows for 2x the number bags of WOW (not including starter)");
            OutputVariableToConfig("ITEM_BAG_WEIGHT_REDUCTION_INCREASE_SLOTS_ADD_PER_PERCENT", ITEM_BAG_WEIGHT_REDUCTION_INCREASE_SLOTS_ADD_PER_PERCENT, "When ITEMS_BAG_WEIGHT_REDUCTION_INCREASES_SLOTS_ENABLED is true, this is how much to increase bag size by");
            OutputVariableToConfig("ITEMS_MULTI_ITEMS_CONTAINER_ICON_ID", ITEMS_MULTI_ITEMS_CONTAINER_ICON_ID, "This is the icon ID that is used for multi-item containers that contain more than one item");
            OutputVariableToConfig("ITEM_ARROW_MODEL_NAME", ITEM_ARROW_MODEL_NAME, "Arrows reuse existing WOW models, and the specific model is defined here", false);
            OutputVariableToConfig("ITEM_ARROW_TEXTURE_NAME", ITEM_ARROW_TEXTURE_NAME, "");
            OutputVariableToConfig("ITEMS_FISHING_BAIT_POTENCY_TIER_1_SPELL_ID", ITEMS_FISHING_BAIT_POTENCY_TIER_1_SPELL_ID, "Spell IDs for the +fishing effect of bait", false);
            OutputVariableToConfig("ITEMS_FISHING_BAIT_POTENCY_TIER_2_SPELL_ID", ITEMS_FISHING_BAIT_POTENCY_TIER_2_SPELL_ID, "");
            OutputVariableToConfig("ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_WIND", ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_WIND, "ID for TotemCategory.dbc for bard instrument groups", false);
            OutputVariableToConfig("ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_STRING", ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_STRING, "", false);
            OutputVariableToConfig("ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_BRASS", ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_BRASS, "", false);
            OutputVariableToConfig("ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_PERCUSSION", ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_PERCUSSION, "", false);
            OutputVariableToConfig("ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_ALL", ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_ALL, "", false);
            OutputVariableToConfig("QUESTS_TEXT_DURATION_IN_MS", QUESTS_TEXT_DURATION_IN_MS, "How many milliseconds to display a text block from an NPC on quest events");
            OutputVariableToConfig("ITEMS_MONK_EPIC_GLOVES_IT159_SPELL_ID", ITEMS_MONK_EPIC_GLOVES_IT159_SPELL_ID, "Spell ID for the visual effect from Monk's epic weapon (Celestial Fists)");
            OutputVariableToConfig("QUESTS_ITEMS_REWARD_CONTAINER_ICON_ID", QUESTS_ITEMS_REWARD_CONTAINER_ICON_ID, "This is the icon ID that is used for quest rewards that contain more than one random item");
            OutputVariableToConfig("SPELL_EFFECT_CALC_STATS_FOR_MAX_LEVEL", SPELL_EFFECT_CALC_STATS_FOR_MAX_LEVEL, "This is how high (WOW side) stats will be be scaled to.  This should almost always be set to the server max level configuration.");
            OutputVariableToConfig("SPELLS_GATECUSTOM_SPELLDBC_ID", SPELLS_GATECUSTOM_SPELLDBC_ID, "IDs for special spells that need an exact match of ID between this and mod-everquest");
            OutputVariableToConfig("SPELLS_BINDCUSTOM_SPELLDBC_ID", SPELLS_BINDCUSTOM_SPELLDBC_ID, "IDs for special spells that need an exact match of ID between this and mod-everquest");
            OutputVariableToConfig("SPELLS_RANGE_MULTIPLIER", SPELLS_RANGE_MULTIPLIER, "How much to multiply the EQ range value for WoW");
            OutputVariableToConfig("SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE", SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE, "The most that a movement speed reduction can slow a target.  Should be above -100");
            OutputVariableToConfig("SPELL_PERIODIC_SECONDS_PER_TICK_WOW", SPELL_PERIODIC_SECONDS_PER_TICK_WOW, "Everquest has a 'tick' every 6 seconds, so buffs and debuffs should use this as a multiplier");
            OutputVariableToConfig("SPELL_PERIODIC_BARD_TICK_BUFFER_IN_MS", SPELL_PERIODIC_BARD_TICK_BUFFER_IN_MS, "This is 'added time' in the periodic tick that comes from bard casters.");
            OutputVariableToConfig("SPELL_RECOVERY_TIME_MINIMUM_IN_MS", SPELL_RECOVERY_TIME_MINIMUM_IN_MS, "This is the minimum allowable recovery time any spell can have, which any smaller will become zero");
            OutputVariableToConfig("SPELL_FEIGN_DEATH_FAIL_CHANCE_PERCENT ", SPELL_FEIGN_DEATH_FAIL_CHANCE_PERCENT, "The percent chance that a feign death spell cast fails");
            OutputVariableToConfig("SPELLS_LEARNABLE_FROM_ITEMS_ENABLED", SPELLS_LEARNABLE_FROM_ITEMS_ENABLED, "If true, you can learn spells from items");
            OutputVariableToConfig("SPELLS_EFFECT_EMITTER_LONGEST_SPELL_TIME_IN_MS", SPELLS_EFFECT_EMITTER_LONGEST_SPELL_TIME_IN_MS, "All spell properties");
            OutputVariableToConfig("SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE", SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE, "How often weapon procs occur", false);
            OutputVariableToConfig("SPELLS_ENCHANT_SPELL_IMBUE_PROC_CHANGE", SPELLS_ENCHANT_SPELL_IMBUE_PROC_CHANGE, "");
            OutputVariableToConfig("SPELL_ENCHANT_ROGUE_POISON_ENCHANT_DURATION_ON_WEAPON_TIME_IN_SECONDS", SPELL_ENCHANT_ROGUE_POISON_ENCHANT_DURATION_ON_WEAPON_TIME_IN_SECONDS, "How long rogue poisons stay on the weapons");
            OutputVariableToConfig("SPELL_ENCHANT_ROGUE_POISON_APPLY_TIME_IN_MS", SPELL_ENCHANT_ROGUE_POISON_APPLY_TIME_IN_MS, "How long it takes to apply rogue poison");
            OutputVariableToConfig("SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_APPLYING_VISUAL_ID", SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_APPLYING_VISUAL_ID, "What to show when a rogue has a poison, 0 will disable it (and be more EQ like)", false);
            OutputVariableToConfig("SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_EFFECT_VISUAL_ID", SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_EFFECT_VISUAL_ID, "");
            OutputVariableToConfig("SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_SPELL", SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_SPELL, "Spell emitter particles",  false);
            OutputVariableToConfig("SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_ENVIRONMENT", SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_ENVIRONMENT, "", false);
            OutputVariableToConfig("SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_SPELL", SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_SPELL, "", false);
            OutputVariableToConfig("SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_ENVIRONMENT", SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_ENVIRONMENT, "", false);
            OutputVariableToConfig("SPELLS_EFFECT_EMITTER_TARGET_DURATION_IN_MS", SPELLS_EFFECT_EMITTER_TARGET_DURATION_IN_MS, "", false);
            OutputVariableToConfig("SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_SPELL", SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_SPELL, "", false);
            OutputVariableToConfig("SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_ENVIRONMENT", SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_ENVIRONMENT, "", false);
            OutputVariableToConfig("SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_SPELL", SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_SPELL, "", false);
            OutputVariableToConfig("SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_ENVIRONMENT", SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_ENVIRONMENT, "", false);
            OutputVariableToConfig("SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MINIMUM", SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MINIMUM, "", false);
            OutputVariableToConfig("SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_DEFAULT", SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_DEFAULT, "", false);
            OutputVariableToConfig("SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_DEFAULT", SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_DEFAULT, "", false);
            OutputVariableToConfig("SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MOD", SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MOD, "", false);
            OutputVariableToConfig("SPELL_EFFECT_EMITTER_SPAWN_RATE_DISC_MOD", SPELL_EFFECT_EMITTER_SPAWN_RATE_DISC_MOD, "", false);
            OutputVariableToConfig("SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_MOD", SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_MOD, "");
            OutputVariableToConfig("SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MIN", SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MIN, "Sprite List particles", false);
            OutputVariableToConfig("SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX", SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX, "", false);
            OutputVariableToConfig("SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX_EQ_VALUE", SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX_EQ_VALUE, "", false);
            OutputVariableToConfig("SPELL_EFFECT_SPRITE_LIST_RADIUS_MOD", SPELL_EFFECT_SPRITE_LIST_RADIUS_MOD, "", false);
            OutputVariableToConfig("SPELL_EFFECT_SPRITE_LIST_ANIMATION_SCALE_MOD", SPELL_EFFECT_SPRITE_LIST_ANIMATION_SCALE_MOD, "", false);
            OutputVariableToConfig("SPELL_EFFECT_SPRITE_LIST_ANIMATION_FRAME_DELAY_IN_MS", SPELL_EFFECT_SPRITE_LIST_ANIMATION_FRAME_DELAY_IN_MS, "", false);
            OutputVariableToConfig("SPELL_EFFECT_SPRITE_LIST_MAX_NON_PREJECTILE_ANIM_TIME_IN_MS", SPELL_EFFECT_SPRITE_LIST_MAX_NON_PREJECTILE_ANIM_TIME_IN_MS, "", false);
            OutputVariableToConfig("SPELL_EFFECT_SPRITE_LIST_PULSE_RANGE", SPELL_EFFECT_SPRITE_LIST_PULSE_RANGE, "", false);
            OutputVariableToConfig("SPELL_EFFECT_SPRITE_LIST_CIRCULAR_SHIFT_MOD", SPELL_EFFECT_SPRITE_LIST_CIRCULAR_SHIFT_MOD, "", false);
            OutputVariableToConfig("SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_HIGH", SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_HIGH, "", false);
            OutputVariableToConfig("SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_LOW", SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_LOW, "");
            OutputVariableToConfig("SPELL_EFFECT_BARD_TICK_VISUAL_DURATION_MOD_FROM_TICK", SPELL_EFFECT_BARD_TICK_VISUAL_DURATION_MOD_FROM_TICK, "BardTick visuals have their durations multiplied by this ammount.");
            OutputVariableToConfig("SPELL_EFFECT_BARD_ADDITIONAL_TICK_ON_CAST", SPELL_EFFECT_BARD_ADDITIONAL_TICK_ON_CAST, "If this is true, then when a bard song is cast then a tick is applied immediately on targets");
            OutputVariableToConfig("SPELL_EFFECT_VALUE_LOW_BIAS_WEIGHT", SPELL_EFFECT_VALUE_LOW_BIAS_WEIGHT, "This is how much 'weight' the lower effect value has when converting EQ to WoW spell effects");
            OutputVariableToConfig("SPELL_EFFECT_USE_DYNAMIC_EFFECT_VALUES", SPELL_EFFECT_USE_DYNAMIC_EFFECT_VALUES, "If true, the damage formula will honor spell level based values, otherwise it'll use maximum", false);
            OutputVariableToConfig("SPELL_EFFECT_USE_DYNAMIC_AURA_DURATIONS", SPELL_EFFECT_USE_DYNAMIC_AURA_DURATIONS, "");
            OutputVariableToConfig("SPELL_EFFECT_REVIVE_EXPPCT_TO_HPMP_MULTIPLIER", SPELL_EFFECT_REVIVE_EXPPCT_TO_HPMP_MULTIPLIER, "Revive will give HP/MP instead of EXP on revive, so this is the multiplier to use for that");
            OutputVariableToConfig("SPELL_MODEL_SIZE_CHANGE_EFFECT_DEFAULT_TIME_IN_MS", SPELL_MODEL_SIZE_CHANGE_EFFECT_DEFAULT_TIME_IN_MS, "Default time that a shrink/grow spell will last for");
            OutputVariableToConfig("SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID", SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, "Values for the cooldown spells applied by Priests of Discord when you switch worlds, setting cooldown duration to 0 will disable it", false);
            OutputVariableToConfig("SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN", SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN, "");
            OutputVariableToConfig("SPELL_DEFAULT_SPELL_POWER_INFLUENCE_PERCENT", SPELL_DEFAULT_SPELL_POWER_INFLUENCE_PERCENT, "This is the default amount influence spell strength by spell power (when not overriden)");
            OutputVariableToConfig("SPELL_SUMMON_CASTER_AURA_SPELL_ID", SPELL_SUMMON_CASTER_AURA_SPELL_ID, "Summoner dummy spell ID used to prevent creatures from summoning more creatures");
            OutputVariableToConfig("SPELL_CREATURE_REDUCED_MANA_REGEN_SPELL_ID", SPELL_CREATURE_REDUCED_MANA_REGEN_SPELL_ID, "Hidden passive aura used to reduce creature mana regeneration (see CREATURE_MANA_REGEN_PERCENT)");
            OutputVariableToConfig("SPELL_CREATURE_SEE_INVIS_AND_STEALTH_SPELL_ID", SPELL_CREATURE_SEE_INVIS_AND_STEALTH_SPELL_ID, "This is the ID used to detect invis and stealth, and already exists in AzerothCore");
            OutputVariableToConfig("SPELL_INVIS_VS_UNDEAD_INVIS_TYPE", SPELL_INVIS_VS_UNDEAD_INVIS_TYPE, "WoW invisibility group (InvisibilityType) reserved for EQ 'invis vs undead' (0 = general invis, 1 should be unused)");
            OutputVariableToConfig("SPELL_CREATURE_INVIS_VS_UNDEAD_DETECT_SPELL_ID", SPELL_CREATURE_INVIS_VS_UNDEAD_DETECT_SPELL_ID, "Custom detect aura granted to everything that should see through 'invis vs undead' (non-undead + see_invis_undead undead)");
            OutputVariableToConfig("COMBAT_DAZE_IN_EQ_ZONES_ENABLED", COMBAT_DAZE_IN_EQ_ZONES_ENABLED, "EQ has no \"daze\" snare when a creature melee-hits a player from behind so this can disable it (in EQ zones only)");
            OutputVariableToConfig("COMBATSKILL_BASH_ENABLED", COMBATSKILL_BASH_ENABLED, "Bash skills in EQ are either from warrior/cleric/paladin/shadowknight or those that use warrior skills", false);
            OutputVariableToConfig("COMBATSKILL_BASH_PLAYER_LEARNABLE", COMBATSKILL_BASH_PLAYER_LEARNABLE, "Whether classes that have Bash learn it as players (from level 1)", false);
            OutputVariableToConfig("COMBATSKILL_BASH_SPELL_ID", COMBATSKILL_BASH_SPELL_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_BASH_SPELL_ICON_EQ_ID", COMBATSKILL_BASH_SPELL_ICON_EQ_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_BASH_CREATURE_MIN_LEVEL", COMBATSKILL_BASH_CREATURE_MIN_LEVEL, "", false);
            OutputVariableToConfig("COMBATSKILL_BASH_COOLDOWN_IN_MS", COMBATSKILL_BASH_COOLDOWN_IN_MS, "", false);
            OutputVariableToConfig("COMBATSKILL_BASH_STUN_DURATION_IN_MS", COMBATSKILL_BASH_STUN_DURATION_IN_MS, "", false);
            OutputVariableToConfig("COMBATSKILL_BASH_BASE_DAMAGE", COMBATSKILL_BASH_BASE_DAMAGE, "", false);
            OutputVariableToConfig("COMBATSKILL_BASH_DAMAGE_PER_LEVEL", COMBATSKILL_BASH_DAMAGE_PER_LEVEL, "", false);
            OutputVariableToConfig("COMBATSKILL_BASH_RANGE", COMBATSKILL_BASH_RANGE, "");
            OutputVariableToConfig("COMBATSKILL_BASH_FORBEARANCE_ENABLED", COMBATSKILL_BASH_FORBEARANCE_ENABLED, "Bash Forbearance: a target struck by Bash briefly cannot be affected by Bash again", false);
            OutputVariableToConfig("COMBATSKILL_BASH_FORBEARANCE_SPELL_ID", COMBATSKILL_BASH_FORBEARANCE_SPELL_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_BASH_FORBEARANCE_SPELL_ICON_EQ_ID", COMBATSKILL_BASH_FORBEARANCE_SPELL_ICON_EQ_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_BASH_FORBEARANCE_DURATION_IN_MS", COMBATSKILL_BASH_FORBEARANCE_DURATION_IN_MS, "");
            OutputTextLineToConfig("# Slam is an Ogre/Troll/Barb racial in EQ (a Bash-equivalent that needs no shield). It shares a cooldown with Bash.");
            OutputVariableToConfig("COMBATSKILL_SLAM_ENABLED", COMBATSKILL_SLAM_ENABLED, "Granted to players by race (PlayerClassRaceProperties.HasSlam), available from level 1.", false);
            OutputVariableToConfig("COMBATSKILL_SLAM_PLAYER_LEARNABLE", COMBATSKILL_SLAM_PLAYER_LEARNABLE, "Whether races that have Slam learn it as players (from level 1)", false);
            OutputVariableToConfig("COMBATSKILL_SLAM_SPELL_ID", COMBATSKILL_SLAM_SPELL_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_SLAM_SPELL_ICON_EQ_ID", COMBATSKILL_SLAM_SPELL_ICON_EQ_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_SLAM_STUN_DURATION_IN_MS", COMBATSKILL_SLAM_STUN_DURATION_IN_MS, "", false);
            OutputVariableToConfig("COMBATSKILL_SLAM_BASE_DAMAGE", COMBATSKILL_SLAM_BASE_DAMAGE, "", false);
            OutputVariableToConfig("COMBATSKILL_SLAM_DAMAGE_PER_LEVEL", COMBATSKILL_SLAM_DAMAGE_PER_LEVEL, "", false);
            OutputVariableToConfig("COMBATSKILL_SLAM_RANGE", COMBATSKILL_SLAM_RANGE, "", false);
            OutputVariableToConfig("COMBATSKILL_HARMTOUCH_ENABLED", COMBATSKILL_HARMTOUCH_ENABLED, "Harm Touch is a shadowknight ability (a long-cooldown direct damage \"touch\"). Also granted to player Death Knights. HP is ~2.5x higher in WoW", false);
            OutputVariableToConfig("COMBATSKILL_HARMTOUCH_PLAYER_LEARNABLE", COMBATSKILL_HARMTOUCH_PLAYER_LEARNABLE, "", false);
            OutputVariableToConfig("COMBATSKILL_HARMTOUCH_SPELL_ID", COMBATSKILL_HARMTOUCH_SPELL_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_HARMTOUCH_SPELL_ICON_EQ_ID", COMBATSKILL_HARMTOUCH_SPELL_ICON_EQ_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_HARMTOUCH_CREATURE_MIN_LEVEL", COMBATSKILL_HARMTOUCH_CREATURE_MIN_LEVEL, "", false);
            OutputVariableToConfig("COMBATSKILL_HARMTOUCH_COOLDOWN_IN_MS", COMBATSKILL_HARMTOUCH_COOLDOWN_IN_MS, "", false);
            OutputVariableToConfig("COMBATSKILL_HARMTOUCH_CREATURE_INITIAL_DELAY_IN_MS", COMBATSKILL_HARMTOUCH_CREATURE_INITIAL_DELAY_IN_MS, "", false);
            OutputVariableToConfig("COMBATSKILL_HARMTOUCH_BASE_DAMAGE", COMBATSKILL_HARMTOUCH_BASE_DAMAGE, "", false);
            OutputVariableToConfig("COMBATSKILL_HARMTOUCH_DAMAGE_PER_LEVEL", COMBATSKILL_HARMTOUCH_DAMAGE_PER_LEVEL, "", false);
            OutputVariableToConfig("COMBATSKILL_HARMTOUCH_RANGE", COMBATSKILL_HARMTOUCH_RANGE, "");
            OutputVariableToConfig("COMBATSKILL_LAYONHANDS_ENABLED", COMBATSKILL_LAYONHANDS_ENABLED, "Lay on Hands is a paladin ability (a long-cooldown large self heal used when badly hurt). HP is ~2.5x higher in WoW", false);
            OutputVariableToConfig("COMBATSKILL_LAYONHANDS_SPELL_ID", COMBATSKILL_LAYONHANDS_SPELL_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_LAYONHANDS_SPELL_ICON_EQ_ID", COMBATSKILL_LAYONHANDS_SPELL_ICON_EQ_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_LAYONHANDS_CREATURE_MIN_LEVEL", COMBATSKILL_LAYONHANDS_CREATURE_MIN_LEVEL, "", false);
            OutputVariableToConfig("COMBATSKILL_LAYONHANDS_COOLDOWN_IN_MS", COMBATSKILL_LAYONHANDS_COOLDOWN_IN_MS, "", false);
            OutputVariableToConfig("COMBATSKILL_LAYONHANDS_HEALTH_TRIGGER_PCT", COMBATSKILL_LAYONHANDS_HEALTH_TRIGGER_PCT, "", false);
            OutputVariableToConfig("COMBATSKILL_LAYONHANDS_BASE_HEAL", COMBATSKILL_LAYONHANDS_BASE_HEAL, "", false);
            OutputVariableToConfig("COMBATSKILL_LAYONHANDS_HEAL_PER_LEVEL", COMBATSKILL_LAYONHANDS_HEAL_PER_LEVEL, "");
            OutputVariableToConfig("COMBATSKILL_FEIGNDEATH_ENABLED", COMBATSKILL_FEIGNDEATH_ENABLED, "Feign Death is a monk ability (a self ability that drops aggro by playing dead, instant like in TAKP)", false);
            OutputVariableToConfig("COMBATSKILL_FEIGNDEATH_PLAYER_LEARNABLE", COMBATSKILL_FEIGNDEATH_PLAYER_LEARNABLE, "Whether classes that have Feign Death learn it as players (from level 1)", false);
            OutputVariableToConfig("COMBATSKILL_FEIGNDEATH_SPELL_ID", COMBATSKILL_FEIGNDEATH_SPELL_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_FEIGNDEATH_SPELL_ICON_EQ_ID", COMBATSKILL_FEIGNDEATH_SPELL_ICON_EQ_ID, "", false);
            OutputVariableToConfig("COMBATSKILL_FEIGNDEATH_FAIL_CHANCE_PERCENT", COMBATSKILL_FEIGNDEATH_FAIL_CHANCE_PERCENT, "", false);
            OutputVariableToConfig("COMBATSKILL_FEIGNDEATH_COOLDOWN_IN_MS", COMBATSKILL_FEIGNDEATH_COOLDOWN_IN_MS, "");
            OutputVariableToConfig("FISHING_SKILL_CONVERSION_MOD_60", FISHING_SKILL_CONVERSION_MOD_60, "How much to multiply the EQ fish catching skill requirement for WOW", false);
            OutputVariableToConfig("FISHING_SKILL_CONVERSION_MOD_80", FISHING_SKILL_CONVERSION_MOD_80, "");
            OutputVariableToConfig("FORAGE_SPELL_ICON_EQ_ID", FORAGE_SPELL_ICON_EQ_ID, "Which eq spell icon to use for the Forage skill. Can be a value between 0-22");
            OutputVariableToConfig("FORAGE_SPELL_TEMPLATE_ID", FORAGE_SPELL_TEMPLATE_ID, "Spell id for the forage spell");
            OutputVariableToConfig("TRADESKILLS_CONVERSION_MOD_60", TRADESKILLS_CONVERSION_MOD_60, "How much to multiply EQ skill requirements by to reach the same for WoW on conversion", false);
            OutputVariableToConfig("TRADESKILLS_CONVERSION_MOD_80", TRADESKILLS_CONVERSION_MOD_80, "");
            OutputVariableToConfig("TRADESKILLS_SKILL_TIER_DISTANCE_LOW", TRADESKILLS_SKILL_TIER_DISTANCE_LOW, "Max distance between Grey -> Green -> Yellow -> Red steps", false);
            OutputVariableToConfig("TRADESKILLS_SKILL_TIER_DISTANCE_HIGH", TRADESKILLS_SKILL_TIER_DISTANCE_HIGH, "");
            OutputVariableToConfig("TRADESKILL_LEARN_COST_AT_1", TRADESKILL_LEARN_COST_AT_1, "The skill level of a tradeskill will be priced closest to the value for that WOW skill level", false);
            OutputVariableToConfig("TRADESKILL_LEARN_COST_AT_50", TRADESKILL_LEARN_COST_AT_50, "", false);
            OutputVariableToConfig("TRADESKILL_LEARN_COST_AT_100", TRADESKILL_LEARN_COST_AT_100, "", false);
            OutputVariableToConfig("TRADESKILL_LEARN_COST_AT_200", TRADESKILL_LEARN_COST_AT_200, "", false);
            OutputVariableToConfig("TRADESKILL_LEARN_COST_AT_300", TRADESKILL_LEARN_COST_AT_300, "", false);
            OutputVariableToConfig("TRADESKILL_LEARN_COST_AT_450", TRADESKILL_LEARN_COST_AT_450, "");
            OutputVariableToConfig("TRADESKILL_CAST_TIME_IN_MS", TRADESKILL_CAST_TIME_IN_MS, "How long every tradeskill will take in milliseconds");
            OutputVariableToConfig("TRADESKILL_TOTEM_CATEGORY_START", TRADESKILL_TOTEM_CATEGORY_START, "Tradeskill items that need a totem in TotemCategory.dbc will align under this");
            OutputVariableToConfig("TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING", TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING, "ID for TotemCategory.dbc for specific tradeskills", false);
            OutputVariableToConfig("TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX", TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX, "", false);
            OutputVariableToConfig("TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING", TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING, "", false);
            OutputVariableToConfig("TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY", TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY, "", false);
            OutputVariableToConfig("TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING", TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING, "");
            OutputVariableToConfig("TRANSPORT_PAUSE_MULTIPLIER", TRANSPORT_PAUSE_MULTIPLIER, "Pause as in 'stop at a port'. 1 will be EQ-like");
            OutputVariableToConfig("TRANSPORT_MOVE_SPEED", TRANSPORT_MOVE_SPEED, "Most boats are 30 in WoW, but a value of around 9 is EQ-like");
            OutputVariableToConfig("TRANSPORT_ACCELERATION", TRANSPORT_ACCELERATION, "Most boats are 30 in WoW, but a value of around 9 is EQ-like");
            OutputVariableToConfig("TRANSPORT_ALLOW_FIXED_SPEEDS", TRANSPORT_ALLOW_FIXED_SPEEDS, "If true, allows \"fixed_speed\" to override TRANSPORT_MOVE_SPEED");
            OutputTextLineToConfig("# +---------------------------------------------------------------------------+");
            OutputTextLineToConfig("# | DBC and SQL IDs (Only change these if you know what you're doing)         |");
            OutputTextLineToConfig("# | - Typically only changed if other custom content uses the IDs             |");
            OutputTextLineToConfig("# +---------------------------------------------------------------------------+");
            OutputBlankLineToConfig();
            OutputVariableToConfig("DBCID_AREATABLE_AREABIT_BLOCK_1_START", DBCID_AREATABLE_AREABIT_BLOCK_1_START, "IDs for AreaBit used in AreaTable, should be unique (max of 4095)", false);
            OutputVariableToConfig("DBCID_AREATABLE_AREABIT_BLOCK_1_END", DBCID_AREATABLE_AREABIT_BLOCK_1_END, "", false);
            OutputVariableToConfig("DBCID_AREATABLE_AREABIT_BLOCK_2_START", DBCID_AREATABLE_AREABIT_BLOCK_2_START, "", false);
            OutputVariableToConfig("DBCID_AREATABLE_AREABIT_BLOCK_2_END", DBCID_AREATABLE_AREABIT_BLOCK_2_END, "", false);
            OutputVariableToConfig("DBCID_AREATABLE_AREABIT_BLOCK_3_START", DBCID_AREATABLE_AREABIT_BLOCK_3_START, "", false);
            OutputVariableToConfig("DBCID_AREATABLE_AREABIT_BLOCK_3_END", DBCID_AREATABLE_AREABIT_BLOCK_3_END, "");
            OutputVariableToConfig("DBCID_AREATABLE_ID_START", DBCID_AREATABLE_ID_START, "Identifies Area rows in AreaTable.dbc. Note these are hardcoded in ZoneSubAreas.csv and ZoneProperties.csv", false);
            OutputVariableToConfig("DBCID_AREATABLE_ID_END", DBCID_AREATABLE_ID_END, "");
            OutputVariableToConfig("DBCID_AREATRIGGER_ID_START", DBCID_AREATRIGGER_ID_START, "IDs for AreaTrigger.DBC. Defined in ZoneLineBoxes.csv", false);
            OutputVariableToConfig("DBCID_AREATRIGGER_ID_END", DBCID_AREATRIGGER_ID_END, "");
            OutputVariableToConfig("DBCID_CREATUREDISPLAYINFO_ID_START", DBCID_CREATUREDISPLAYINFO_ID_START, "IDs for CreatureDisplayInfo.dbc", false);
            OutputVariableToConfig("DBCID_CREATUREDISPLAYINFO_ID_END", DBCID_CREATUREDISPLAYINFO_ID_END, "");
            OutputVariableToConfig("DBCID_CREATUREDISPLAYINFOEXTRA_ID_START", DBCID_CREATUREDISPLAYINFOEXTRA_ID_START, "IDs for CreatureDisplayInfoExtra.dbc", false);
            OutputVariableToConfig("DBCID_CREATUREDISPLAYINFOEXTRA_ID_END", DBCID_CREATUREDISPLAYINFOEXTRA_ID_END, "");
            OutputVariableToConfig("DBCID_CREATUREMODELDATA_ID_START", DBCID_CREATUREMODELDATA_ID_START, "IDs for CreatureModelData.dbc", false);
            OutputVariableToConfig("DBCID_CREATUREMODELDATA_ID_END", DBCID_CREATUREMODELDATA_ID_END, "");
            OutputVariableToConfig("DBCID_CREATURESOUNDDATA_ID_START", DBCID_CREATURESOUNDDATA_ID_START, "IDs for CreatureSoundData.dbc");
            OutputVariableToConfig("DBCID_FOOTSTEPTERRAINLOOKUP_ID_START", DBCID_FOOTSTEPTERRAINLOOKUP_ID_START, "IDs for FootstepTerrainLookup.dbc", false);
            OutputVariableToConfig("DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START", DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START, "");
            OutputVariableToConfig("DBCID_GAMEOBJECTDISPLAYINFO_ID_START", DBCID_GAMEOBJECTDISPLAYINFO_ID_START, "IDs for rows inside GameObjectDisplayInfo.dbc");
            OutputVariableToConfig("DBCID_LFGDUNGEONGROUP_DUNGEONS_ID", DBCID_LFGDUNGEONGROUP_DUNGEONS_ID, "IDs for LFGDungeonGroup.dbc", false);
            OutputVariableToConfig("DBCID_LFGDUNGEONGROUP_DUNGEONS_ORDER_ID", DBCID_LFGDUNGEONGROUP_DUNGEONS_ORDER_ID, "", false);
            OutputVariableToConfig("DBCID_LFGDUNGEONGROUP_RAIDS_ID", DBCID_LFGDUNGEONGROUP_RAIDS_ID, "", false);
            OutputVariableToConfig("DBCID_LFGDUNGEONGROUP_RAIDS_ORDER_ID", DBCID_LFGDUNGEONGROUP_RAIDS_ORDER_ID, "");
            OutputVariableToConfig("DBCID_LFGDUNGEONS_ID_START", DBCID_LFGDUNGEONS_ID_START, "Start ID for LFGDungeons.dbc") ;
            OutputVariableToConfig("DBCID_ITEMDISPLAYINFO_START", DBCID_ITEMDISPLAYINFO_START, "Start ID for item display info");
            OutputVariableToConfig("DBCID_LIGHT_ID_START", DBCID_LIGHT_ID_START, "Identifies the Light.DBC row, used for environmental properties");
            OutputVariableToConfig("DBCID_LIGHTPARAMS_ID_START", DBCID_LIGHTPARAMS_ID_START, "Identifies the LightParams.dbc, used for detailed values related to a Light.DBC row");
            OutputVariableToConfig("DBCID_LOADINGSCREEN_ID_START", DBCID_LOADINGSCREEN_ID_START, "IDs for the loading screen");
            OutputTextLineToConfig("# Identifies Maps in Map.dbc and MapDifficulty.dbc. This value is hard coded in /WorldData/ZoneProperties.csv, TransportShips.csv, and ZoneDisplayMapContinents.csv, so you cannot change only this value. ");
            OutputVariableToConfig("DBCID_MAP_ID_START", DBCID_MAP_ID_START, "Values from 901+ are dungeon finder", false);
            OutputVariableToConfig("DBCID_MAP_ID_END", DBCID_MAP_ID_END, "");
            OutputVariableToConfig("DBCID_SKILLLINE_ID_START", DBCID_SKILLLINE_ID_START, "ID for SkillLine.dbc");
            OutputVariableToConfig("DBCID_SKILLLINEABILITY_ID_START", DBCID_SKILLLINEABILITY_ID_START, "ID for skill line abilities found in SkillLineAbility.dbc");
            OutputVariableToConfig("DBCID_SKILLRACECLASSINFO_ID_START", DBCID_SKILLRACECLASSINFO_ID_START, "ID for SkillRaceClassInfo.dbc");
            OutputVariableToConfig("DBCID_SOUNDENTRIES_ID_START", DBCID_SOUNDENTRIES_ID_START, "ID for sounds found in SoundEntries.dbc");
            OutputVariableToConfig("DBCID_SOUNDAMBIENCE_ID_START", DBCID_SOUNDAMBIENCE_ID_START, "ID for sounds found in SoundAmbience.dbc");
            OutputTextLineToConfig("# ID for spells found in Spell.dbc");
            OutputTextLineToConfig("# - Manually created spells reserve IDs from 86900 to 86999.  See \"Spells\"");
            OutputTextLineToConfig("# - Recipes reserve IDs 87000 to 91362");
            OutputTextLineToConfig("# - Converted spells IDs start at 92000 and base spells range to 95826");
            OutputTextLineToConfig("# - SpellIDs 96000 - 96049 reserved for 'worn' effects (effects that always take effect when worn)");
            OutputTextLineToConfig("# - SpellIDs 96100 - 96199 reserved for 'coat' effects that come from rogue poisons, triggering another spell");
            OutputTextLineToConfig("# - SpellIDs 96200 - 97656 reserved for 'clicky' effects (defined in ItemTemplates.csv under clickeffect_wow)");
            OutputTextLineToConfig("# - SpellIDs 97700 - 97715 reserved for 'good clicky' effects (defined in SpellTemplate.csv under wow_good_proc_id)");
            OutputTextLineToConfig("# - SpellIDs 98000+ (or as defined below) used for 'generated spell IDs'");
            OutputVariableToConfig("DBCID_SPELL_ID_START", DBCID_SPELL_ID_START, "", false);
            OutputVariableToConfig("DBCID_SPELL_ID_GENERATED_START", DBCID_SPELL_ID_GENERATED_START, "", false);
            OutputVariableToConfig("DBCID_SPELL_ID_END", DBCID_SPELL_ID_END, "");
            OutputVariableToConfig("DBCID_SPELLCASTTIME_ID_START", DBCID_SPELLCASTTIME_ID_START, "ID for spellcasttimes.dbc");
            OutputVariableToConfig("DBCID_SPELLCATEGORY_ID_START", DBCID_SPELLCATEGORY_ID_START, "ID for spellcategory.dbc");
            OutputVariableToConfig("DBCID_SPELLDURATION_AURA_ID", DBCID_SPELLDURATION_AURA_ID, "ID for spellduration.dbc");
            OutputVariableToConfig("DBCID_SPELLITEMENCHANTMENT_ID_START", DBCID_SPELLITEMENCHANTMENT_ID_START, "Stand ID for spell item ennchantments");
            OutputVariableToConfig("DBCID_SPELLICON_ID_START", DBCID_SPELLICON_ID_START, "ID for spellicon.dbc");
            OutputVariableToConfig("DBCID_SPELLRADIUS_ID_START", DBCID_SPELLRADIUS_ID_START, "ID for spellradius.dbc");
            OutputVariableToConfig("DBCID_SPELLRANGE_ID_START", DBCID_SPELLRANGE_ID_START, "ID for SpellRange.dbc");
            OutputVariableToConfig("DBCID_SPELLVISUAL_ID_START", DBCID_SPELLVISUAL_ID_START, "IDs for the SpellVisual line of DBC files", false);
            OutputVariableToConfig("DBCID_SPELLVISUALKIT_ID_START", DBCID_SPELLVISUALKIT_ID_START, "", false);
            OutputVariableToConfig("DBCID_SPELLVISUALEFFECTNAME_ID_START", DBCID_SPELLVISUALEFFECTNAME_ID_START, "");
            OutputVariableToConfig("DBCID_SUMMONPROPERTIES_ID_START", DBCID_SUMMONPROPERTIES_ID_START, "IDs for SummonProperties.dbc");
            OutputVariableToConfig("DBCID_TAXIPATH_ID_START", DBCID_TAXIPATH_ID_START, "ID for TaxiPath.dbc");
            OutputVariableToConfig("DBCID_TAXIPATHNODE_ID_START", DBCID_TAXIPATHNODE_ID_START, "ID for TaxiPathNode.dbc");
            OutputVariableToConfig("DBCID_TOTEMCATEGORY_ID_START", DBCID_TOTEMCATEGORY_ID_START, "ID for TotemCategory.dbc");
            OutputVariableToConfig("DBCID_TRANSPORTANIMATION_ID_START", DBCID_TRANSPORTANIMATION_ID_START, "ID for TransportAnimation.dbc");
            OutputVariableToConfig("DBCID_WORLDSAFELOCS_ID_START", DBCID_WORLDSAFELOCS_ID_START, "IDs for WorldSafeLocs.dbc, defined in ZoneGraveyards.csv", false);
            OutputVariableToConfig("DBCID_WORLDSAFELOCS_ID_END", DBCID_WORLDSAFELOCS_ID_END, "");
            OutputVariableToConfig("DBCID_WMOAREATABLE_ID_START", DBCID_WMOAREATABLE_ID_START, "Specific rows in WMOAreaTable.dbc");
            OutputVariableToConfig("DBCID_WMOAREATABLE_WMOGROUPID_START", DBCID_WMOAREATABLE_WMOGROUPID_START, "Identifies WMO Groups.");
            OutputVariableToConfig("DBCID_ZONEMUSIC_START", DBCID_ZONEMUSIC_START, "ID for music in ZoneMusic.dbc, and how many IDs to reserve on a per-zone basis");
            OutputVariableToConfig("SQL_BROADCASTTEXT_ID_START", SQL_BROADCASTTEXT_ID_START, "Start and end IDs for broadcast_text sql records", false);
            OutputVariableToConfig("SQL_BROADCASTTEXT_ID_END", SQL_BROADCASTTEXT_ID_END, "");
            OutputVariableToConfig("SQL_CREATURE_GUID_LOW", SQL_CREATURE_GUID_LOW, "Record identifier for the creature sql table, need at least 31k", false);
            OutputVariableToConfig("SQL_CREATURE_GUID_HIGH", SQL_CREATURE_GUID_HIGH, "");
            OutputVariableToConfig("SQL_CREATUREIMMUNITIES_ID_START", SQL_CREATUREIMMUNITIES_ID_START, "Record identifier for for creature_immunities", false);
            OutputVariableToConfig("SQL_CREATUREIMMUNITIES_ID_END", SQL_CREATUREIMMUNITIES_ID_END, "");
            OutputVariableToConfig("SQL_CREATURETEMPLATE_ENTRY_LOW", SQL_CREATURETEMPLATE_ENTRY_LOW, "Record identifier for the creature template SQL table", false);
            OutputVariableToConfig("SQL_CREATURETEMPLATE_ENTRY_HIGH", SQL_CREATURETEMPLATE_ENTRY_HIGH, "", false);
            OutputVariableToConfig("SQL_CREATURETEMPLATE_GENERATED_START_ID", SQL_CREATURETEMPLATE_GENERATED_START_ID, "", false);
            OutputVariableToConfig("SQL_GAMEOBJECT_GUID_ID_START", SQL_GAMEOBJECT_GUID_ID_START, "Start GUIDs for gameobjects", false);
            OutputVariableToConfig("SQL_GAMEOBJECT_GUID_ID_END", SQL_GAMEOBJECT_GUID_ID_END, "");
            OutputVariableToConfig("SQL_GAME_EVENTS_ID_START", SQL_GAME_EVENTS_ID_START, "IDs for events", false);
            OutputVariableToConfig("SQL_GAME_EVENTS_ID_END", SQL_GAME_EVENTS_ID_END, "");
            OutputTextLineToConfig("# Start and end IDs for gameobject_template rows");
            OutputTextLineToConfig("# - GameObjects.csv owns rows 270000 - 274999");
            OutputTextLineToConfig("# - TransportLifts.csv and TransportLiftTriggers.csv own rows 279900 - 279999");
            OutputVariableToConfig("SQL_GAMEOBJECTTEMPLATE_ID_START", SQL_GAMEOBJECTTEMPLATE_ID_START, "", false);
            OutputVariableToConfig("SQL_GAMEOBJECTTEMPLATE_ID_END", SQL_GAMEOBJECTTEMPLATE_ID_END, "", false);
            OutputVariableToConfig("SQL_GAMEOBJECTTEMPLATE_SHIP_ID_START", SQL_GAMEOBJECTTEMPLATE_SHIP_ID_START, "", false);
            OutputVariableToConfig("SQL_GAMEOBJECTTEMPLATE_SHIP_ID_END", SQL_GAMEOBJECTTEMPLATE_SHIP_ID_END, "");
            OutputVariableToConfig("SQL_GAMETELE_ROWID_START", SQL_GAMETELE_ROWID_START, "Start row for `game_tele` records. (~2000-2400)", false);
            OutputVariableToConfig("SQL_GAMETELE_ROWID_END", SQL_GAMETELE_ROWID_END, "");
            OutputVariableToConfig("SQL_GOSSIPMENU_MENUID_START", SQL_GOSSIPMENU_MENUID_START, "Start and end IDs for custom gossip menu records", false);
            OutputVariableToConfig("SQL_GOSSIPMENU_MENUID_END", SQL_GOSSIPMENU_MENUID_END, "");
            OutputTextLineToConfig("# Start and end ID for template entries, which are entered into various .csv files");
            OutputTextLineToConfig("# - Class-Specific scroll IDs range 110500 - 112887");
            OutputTextLineToConfig("# - Equipped Click Bag IDs range 113000 - 113932");
            OutputTextLineToConfig("# - Equipped Click Essence IDs range 114000 - 114932");
            OutputTextLineToConfig("# - Quest Template multi-item reward containers IDs range 116000 - 116200");
            OutputTextLineToConfig("# - Tradeskill multi-item creation containers IDs range 117000 - 117217");
            OutputTextLineToConfig("# - NPC-worn items have IDs 120000 - 145000");
            OutputVariableToConfig("SQL_ITEM_TEMPLATE_ENTRY_START", SQL_ITEM_TEMPLATE_ENTRY_START, "", false);
            OutputVariableToConfig("SQL_ITEM_TEMPLATE_ENTRY_END", SQL_ITEM_TEMPLATE_ENTRY_END, "", false);
            OutputVariableToConfig("SQL_NPCTEXT_ID_START", SQL_NPCTEXT_ID_START, "Start and end IDs for npc_text sql records", false);
            OutputVariableToConfig("SQL_NPCTEXT_ID_END", SQL_NPCTEXT_ID_END, "");
            OutputVariableToConfig("SQL_PAGETEXT_ID_START", SQL_PAGETEXT_ID_START, "Start and end IDs for page_text SQL records", false);
            OutputVariableToConfig("SQL_PAGETEXT_ID_END", SQL_PAGETEXT_ID_END, "");
            OutputVariableToConfig("SQL_PETNAMEGENERATION_ID_START", SQL_PETNAMEGENERATION_ID_START, "Start ID for pet_name_generation entries");
            OutputVariableToConfig("SQL_POOL_TEMPLATE_ID_START", SQL_POOL_TEMPLATE_ID_START, "Start and end ID for pool_template data rows (reserve 40k records)", false);
            OutputVariableToConfig("SQL_POOL_TEMPLATE_ID_END", SQL_POOL_TEMPLATE_ID_END, "");
            OutputVariableToConfig("SQL_QUEST_TEMPLATE_ID_START", SQL_QUEST_TEMPLATE_ID_START, "Start and end IDs for quest template data rows", false);
            OutputVariableToConfig("SQL_QUEST_TEMPLATE_ID_END", SQL_QUEST_TEMPLATE_ID_END, "", false);
            OutputVariableToConfig("SQL_QUEST_TEMPLATE_ID_REPEATABLE_SHIFT", SQL_QUEST_TEMPLATE_ID_REPEATABLE_SHIFT, "");
            OutputVariableToConfig("SQL_REFERENCE_LOOT_TEMPLATE_ID_START", SQL_REFERENCE_LOOT_TEMPLATE_ID_START, "Stard and end IDs for reference_loot_template.entry", false);
            OutputVariableToConfig("SQL_REFERENCE_LOOT_TEMPLATE_ID_END", SQL_REFERENCE_LOOT_TEMPLATE_ID_END, "");
            OutputVariableToConfig("SQL_SPELL_GROUP_ID_START", SQL_SPELL_GROUP_ID_START, "Start and end IDs for spell groups", false);
            OutputVariableToConfig("SQL_SPELL_GROUP_ID_FOR_BARD_AURA_START", SQL_SPELL_GROUP_ID_FOR_BARD_AURA_START, "", false);
            OutputVariableToConfig("SQL_SPELL_GROUP_ID_END", SQL_SPELL_GROUP_ID_END, "");
            OutputVariableToConfig("SQL_TRAINER_ID_START", SQL_TRAINER_ID_START, "Start and end IDs for trainers", false);
            OutputVariableToConfig("SQL_TRAINER_ID_END", SQL_TRAINER_ID_END, "");
            OutputVariableToConfig("SQL_TRANSPORTS_GUID_START", SQL_TRANSPORTS_GUID_START, "Start and end IDs for transports", false);
            OutputVariableToConfig("SQL_TRANSPORTS_GUID_END", SQL_TRANSPORTS_GUID_END, "");
        }

        public static void LoadConfiguration()
        {
            // Pull the configs off disk
            Dictionary<string, string> configValuesByVariableName = new Dictionary<string, string>();
            List<string> configRows = FileTool.ReadAllStringLinesFromFile(CONFIGONLY_CONFIGURATION_FILE_NAME, false, true);
            for (int i = 0; i < configRows.Count; i++)
            {
                // Ignore anything past a # and blank rows
                string row = configRows[i].Split("#")[0].Trim();
                if (row.Length == 0)
                    continue;

                // Some strings have an equals symbol in the value, splitting only on first equals
                int firstEqualsIndex = row.IndexOf('=');
                if (firstEqualsIndex == -1)
                    continue;
                string variableName = row.Substring(0, firstEqualsIndex).Trim();
                string variableValue = row.Substring(firstEqualsIndex + 1).Trim();
                configValuesByVariableName.Add(variableName.ToUpper(), variableValue);
            }

            PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER = FileTool.CleanPath(ReadVariableFromConfigString("PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER", configValuesByVariableName, PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER));
            PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER = FileTool.CleanPath(ReadVariableFromConfigString("PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER", configValuesByVariableName, PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER));
            PATH_TOOLS_FOLDER = FileTool.CleanPath(ReadVariableFromConfigString("PATH_TOOLS_FOLDER", configValuesByVariableName, PATH_TOOLS_FOLDER));
            PATH_ASSETS_FOLDER = FileTool.CleanPath(ReadVariableFromConfigString("PATH_ASSETS_FOLDER", configValuesByVariableName, PATH_ASSETS_FOLDER));
            PATH_WORKING_FOLDER = FileTool.CleanPath(ReadVariableFromConfigString("PATH_WORKING_FOLDER", configValuesByVariableName, PATH_WORKING_FOLDER));
            PATCH_CLIENT_DATA_ID = ReadVariableFromConfigString("PATCH_CLIENT_DATA_ID", configValuesByVariableName, PATCH_CLIENT_DATA_ID);
            PATCH_CLIENT_DATA_LOC_ID = ReadVariableFromConfigString("PATCH_CLIENT_DATA_LOC_ID", configValuesByVariableName, PATCH_CLIENT_DATA_LOC_ID);
            PATCH_LOCALIZATION_STRING = ReadVariableFromConfigString("PATCH_LOCALIZATION_STRING", configValuesByVariableName, PATCH_LOCALIZATION_STRING);

            DEPLOY_CLIENT_FILES = ReadVariableFromConfigString("DEPLOY_CLIENT_FILES", configValuesByVariableName, DEPLOY_CLIENT_FILES);
            DEPLOY_CLEAR_CACHE_ON_CLIENT_DEPLOY = ReadVariableFromConfigString("DEPLOY_CLEAR_CACHE_ON_CLIENT_DEPLOY", configValuesByVariableName, DEPLOY_CLEAR_CACHE_ON_CLIENT_DEPLOY);
            DEPLOY_SERVER_FILES = ReadVariableFromConfigString("DEPLOY_SERVER_FILES", configValuesByVariableName, DEPLOY_SERVER_FILES);
            DEPLOY_SERVER_DBC_FOLDER_LOCATION = FileTool.CleanPath(ReadVariableFromConfigString("DEPLOY_SERVER_DBC_FOLDER_LOCATION", configValuesByVariableName, DEPLOY_SERVER_DBC_FOLDER_LOCATION));
            DEPLOY_SERVER_SQL = ReadVariableFromConfigString("DEPLOY_SERVER_SQL", configValuesByVariableName, DEPLOY_SERVER_SQL);
            DEPLOY_SQL_CONNECTION_STRING_CHARACTERS = ReadVariableFromConfigString("DEPLOY_SQL_CONNECTION_STRING_CHARACTERS", configValuesByVariableName, DEPLOY_SQL_CONNECTION_STRING_CHARACTERS);
            DEPLOY_SQL_CONNECTION_STRING_WORLD = ReadVariableFromConfigString("DEPLOY_SQL_CONNECTION_STRING_WORLD", configValuesByVariableName, DEPLOY_SQL_CONNECTION_STRING_WORLD);

            CORE_CONSOLE_BEEP_ON_COMPLETE = ReadVariableFromConfigString("CORE_CONSOLE_BEEP_ON_COMPLETE", configValuesByVariableName, CORE_CONSOLE_BEEP_ON_COMPLETE);
            CORE_ENABLE_MULTITHREADING = ReadVariableFromConfigString("CORE_ENABLE_MULTITHREADING", configValuesByVariableName, CORE_ENABLE_MULTITHREADING);
            CORE_ZONEGEN_THREAD_COUNT = ReadVariableFromConfigString("CORE_ZONEGEN_THREAD_COUNT", configValuesByVariableName, CORE_ZONEGEN_THREAD_COUNT);
            CORE_PNGTOBLPCONVERSION_THREAD_COUNT = ReadVariableFromConfigString("CORE_PNGTOBLPCONVERSION_THREAD_COUNT", configValuesByVariableName, CORE_PNGTOBLPCONVERSION_THREAD_COUNT);
            CORE_MUSICCONVERSION_THREAD_COUNT = ReadVariableFromConfigString("CORE_MUSICCONVERSION_THREAD_COUNT", configValuesByVariableName, CORE_MUSICCONVERSION_THREAD_COUNT);

            LOGGING_CONSOLE_MIN_LEVEL = ReadVariableFromConfigString("LOGGING_CONSOLE_MIN_LEVEL", configValuesByVariableName, LOGGING_CONSOLE_MIN_LEVEL);
            LOGGING_FILE_MIN_LEVEL = ReadVariableFromConfigString("LOGGING_FILE_MIN_LEVEL", configValuesByVariableName, LOGGING_FILE_MIN_LEVEL);

            GENERATE_WORLD_SCALE = ReadVariableFromConfigString("GENERATE_WORLD_SCALE", configValuesByVariableName, GENERATE_WORLD_SCALE);
            GENERATE_CREATURE_SCALE = ReadVariableFromConfigString("GENERATE_CREATURE_SCALE", configValuesByVariableName, GENERATE_CREATURE_SCALE);
            GENERATE_EQUIPMENT_PLAYER_SCALE = ReadVariableFromConfigString("GENERATE_EQUIPMENT_PLAYER_SCALE", configValuesByVariableName, GENERATE_EQUIPMENT_PLAYER_SCALE);
            GENERATE_EQUIPMENT_CREATURE_SCALE = ReadVariableFromConfigString("GENERATE_EQUIPMENT_CREATURE_SCALE", configValuesByVariableName, GENERATE_EQUIPMENT_CREATURE_SCALE);

            GENERATE_EQ_EXPANSION_ID_GENERAL = ReadVariableFromConfigString("GENERATE_EQ_EXPANSION_ID_GENERAL", configValuesByVariableName, GENERATE_EQ_EXPANSION_ID_GENERAL);
            GENERATE_EQ_EXPANSION_ID_ZONES = ReadVariableFromConfigString("GENERATE_EQ_EXPANSION_ID_ZONES", configValuesByVariableName, GENERATE_EQ_EXPANSION_ID_ZONES);
            GENERATE_EQ_EXPANSION_ID_TRANSPORTS = ReadVariableFromConfigString("GENERATE_EQ_EXPANSION_ID_TRANSPORTS", configValuesByVariableName, GENERATE_EQ_EXPANSION_ID_TRANSPORTS);
            GENERATE_EQ_EXPANSION_ID_TRADESKILLS = ReadVariableFromConfigString("GENERATE_EQ_EXPANSION_ID_TRADESKILLS", configValuesByVariableName, GENERATE_EQ_EXPANSION_ID_TRADESKILLS);
            GENERATE_EQ_EXPANSION_ID_EQUIPMENT_GRAPHICS = ReadVariableFromConfigString("GENERATE_EQ_EXPANSION_ID_EQUIPMENT_GRAPHICS", configValuesByVariableName, GENERATE_EQ_EXPANSION_ID_EQUIPMENT_GRAPHICS);

            GENERATE_EXTRACT_DBC_FILES = ReadVariableFromConfigString("GENERATE_EXTRACT_DBC_FILES", configValuesByVariableName, GENERATE_EXTRACT_DBC_FILES);
            GENERATE_OBJECTS = ReadVariableFromConfigString("GENERATE_OBJECTS", configValuesByVariableName, GENERATE_OBJECTS);
            GENERATE_CREATURES_AND_SPAWNS = ReadVariableFromConfigString("GENERATE_CREATURES_AND_SPAWNS", configValuesByVariableName, GENERATE_CREATURES_AND_SPAWNS);
            GENERATE_PLAYER_ARMOR_GRAPHICS = ReadVariableFromConfigString("GENERATE_PLAYER_ARMOR_GRAPHICS", configValuesByVariableName, GENERATE_PLAYER_ARMOR_GRAPHICS);
            GENERATE_TRANSPORTS = ReadVariableFromConfigString("GENERATE_TRANSPORTS", configValuesByVariableName, GENERATE_TRANSPORTS);
            GENERATE_QUESTS = ReadVariableFromConfigString("GENERATE_QUESTS", configValuesByVariableName, GENERATE_QUESTS);
            GENERATE_WORLDMAPS = ReadVariableFromConfigString("GENERATE_WORLDMAPS", configValuesByVariableName, GENERATE_WORLDMAPS);

            GENERATE_ADDED_BOUNDARY_AMOUNT = ReadVariableFromConfigString("GENERATE_ADDED_BOUNDARY_AMOUNT", configValuesByVariableName, GENERATE_ADDED_BOUNDARY_AMOUNT);
            GENERATE_SQL_FILE_BATCH_SIZE = ReadVariableFromConfigString("GENERATE_SQL_FILE_BATCH_SIZE", configValuesByVariableName, GENERATE_SQL_FILE_BATCH_SIZE);
            GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE = ReadVariableFromConfigString("GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE", configValuesByVariableName, GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE);
            GENERATE_BLPCONVERTBATCHSIZE = ReadVariableFromConfigString("GENERATE_BLPCONVERTBATCHSIZE", configValuesByVariableName, GENERATE_BLPCONVERTBATCHSIZE);
            GENERATE_FLOAT_EPSILON = ReadVariableFromConfigString("GENERATE_FLOAT_EPSILON", configValuesByVariableName, GENERATE_FLOAT_EPSILON);
            GENERATE_FORCE_SQL_UPDATES = ReadVariableFromConfigString("GENERATE_FORCE_SQL_UPDATES", configValuesByVariableName, GENERATE_FORCE_SQL_UPDATES);
            GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE = ReadVariableFromConfigString("GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE", configValuesByVariableName, GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
            GENERATE_ENABLE_GUILD_VAULTS = ReadVariableFromConfigString("GENERATE_ENABLE_GUILD_VAULTS", configValuesByVariableName, GENERATE_ENABLE_GUILD_VAULTS);
            GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION = ReadVariableFromConfigString("GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION", configValuesByVariableName, GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION);
            GENERATE_ENABLE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID = ReadVariableFromConfigString("GENERATE_ENABLE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID", configValuesByVariableName, GENERATE_ENABLE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID);
            GENERATE_REBALANCE_CONTENT_TO_LEVEL_80 = ReadVariableFromConfigString("GENERATE_REBALANCE_CONTENT_TO_LEVEL_80", configValuesByVariableName, GENERATE_REBALANCE_CONTENT_TO_LEVEL_80);
            GENERATE_NON_PLAYER_OBTAINABLE_ITEMS = ReadVariableFromConfigString("GENERATE_NON_PLAYER_OBTAINABLE_ITEMS", configValuesByVariableName, GENERATE_NON_PLAYER_OBTAINABLE_ITEMS);

            PLAYER_USE_EQ_START_LOCATION = ReadVariableFromConfigString("PLAYER_USE_EQ_START_LOCATION", configValuesByVariableName, PLAYER_USE_EQ_START_LOCATION);
            PLAYER_USE_EQ_START_ITEMS = ReadVariableFromConfigString("PLAYER_USE_EQ_START_ITEMS", configValuesByVariableName, PLAYER_USE_EQ_START_ITEMS);
            PLAYER_ADD_HEARTHSTONE_IF_USE_EQ_START_ITEMS = ReadVariableFromConfigString("PLAYER_ADD_HEARTHSTONE_IF_USE_EQ_START_ITEMS", configValuesByVariableName, PLAYER_ADD_HEARTHSTONE_IF_USE_EQ_START_ITEMS);
            PLAYER_ADD_CUSTOM_BIND_AND_GATE_ON_START = ReadVariableFromConfigString("PLAYER_ADD_CUSTOM_BIND_AND_GATE_ON_START", configValuesByVariableName, PLAYER_ADD_CUSTOM_BIND_AND_GATE_ON_START);
            PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED = ReadVariableFromConfigString("PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED", configValuesByVariableName, PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED);
            PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX = ReadVariableFromConfigString("PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX", configValuesByVariableName, PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX);
            PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES = ReadVariableFromConfigString("PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES", configValuesByVariableName, PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES);
            PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES = ReadVariableFromConfigString("PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES", configValuesByVariableName, PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES);
            PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES = ReadVariableFromConfigString("PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES", configValuesByVariableName, PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES);
            PLAYER_SKILL_ENABLE_BOWS_ON_ALL_APPROPRIATE_EQ_ALIGNED_CLASSES = ReadVariableFromConfigString("PLAYER_SKILL_ENABLE_BOWS_ON_ALL_APPROPRIATE_EQ_ALIGNED_CLASSES", configValuesByVariableName, PLAYER_SKILL_ENABLE_BOWS_ON_ALL_APPROPRIATE_EQ_ALIGNED_CLASSES);
            PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES = ReadVariableFromConfigString("PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES", configValuesByVariableName, PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES);

            ZONE_SHOW_STATIC_GEOMETRY = ReadVariableFromConfigString("ZONE_SHOW_STATIC_GEOMETRY", configValuesByVariableName, ZONE_SHOW_STATIC_GEOMETRY);
            ZONE_MAX_FACES_PER_WMOGROUP = ReadVariableFromConfigString("ZONE_MAX_FACES_PER_WMOGROUP", configValuesByVariableName, ZONE_MAX_FACES_PER_WMOGROUP);
            ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE = ReadVariableFromConfigString("ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE", configValuesByVariableName, ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE);
            ZONE_ALLOW_SUN_HIDING_WITH_SHADOWBOX_ENABLED = ReadVariableFromConfigString("ZONE_ALLOW_SUN_HIDING_WITH_SHADOWBOX_ENABLED", configValuesByVariableName, ZONE_ALLOW_SUN_HIDING_WITH_SHADOWBOX_ENABLED);
            ZONE_SHADOW_BOX_ADDED_SIZE = ReadVariableFromConfigString("ZONE_SHADOW_BOX_ADDED_SIZE", configValuesByVariableName, ZONE_SHADOW_BOX_ADDED_SIZE);
            ZONE_COLLISION_ENABLED = ReadVariableFromConfigString("ZONE_COLLISION_ENABLED", configValuesByVariableName, ZONE_COLLISION_ENABLED);
            ZONE_ENABLE_COLLISION_ON_ALL_ZONE_RENDER_MATERIALS = ReadVariableFromConfigString("ZONE_ENABLE_COLLISION_ON_ALL_ZONE_RENDER_MATERIALS", configValuesByVariableName, ZONE_ENABLE_COLLISION_ON_ALL_ZONE_RENDER_MATERIALS);
            ZONE_SPLIT_COLLISION_CUBOID_MAX_EDGE_LEGNTH = ReadVariableFromConfigString("ZONE_SPLIT_COLLISION_CUBOID_MAX_EDGE_LEGNTH", configValuesByVariableName, ZONE_SPLIT_COLLISION_CUBOID_MAX_EDGE_LEGNTH);
            ZONE_DRAW_COLLIDABLE_SUB_AREAS_AS_BOXES = ReadVariableFromConfigString("ZONE_DRAW_COLLIDABLE_SUB_AREAS_AS_BOXES", configValuesByVariableName, ZONE_DRAW_COLLIDABLE_SUB_AREAS_AS_BOXES);
            ZONE_MAX_FACES_PER_ZONE_MATERIAL_OBJECT = ReadVariableFromConfigString("ZONE_MAX_FACES_PER_ZONE_MATERIAL_OBJECT", configValuesByVariableName, ZONE_MAX_FACES_PER_ZONE_MATERIAL_OBJECT);
            ZONE_BTREE_MIN_SPLIT_SIZE = ReadVariableFromConfigString("ZONE_BTREE_MIN_SPLIT_SIZE", configValuesByVariableName, ZONE_BTREE_MIN_SPLIT_SIZE);
            ZONE_BTREE_MIN_BOX_SIZE_TOTAL = ReadVariableFromConfigString("ZONE_BTREE_MIN_BOX_SIZE_TOTAL", configValuesByVariableName, ZONE_BTREE_MIN_BOX_SIZE_TOTAL);
            ZONE_BTREE_MAX_NODE_GEN_DEPTH = ReadVariableFromConfigString("ZONE_BTREE_MAX_NODE_GEN_DEPTH", configValuesByVariableName, ZONE_BTREE_MAX_NODE_GEN_DEPTH);
            ZONE_DEFAULT_GRAVEYARD_ID = ReadVariableFromConfigString("ZONE_DEFAULT_GRAVEYARD_ID", configValuesByVariableName, ZONE_DEFAULT_GRAVEYARD_ID);
            ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID = ReadVariableFromConfigString("ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID", configValuesByVariableName, ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID);
            ZONE_WEATHER_ENABLED = ReadVariableFromConfigString("ZONE_WEATHER_ENABLED", configValuesByVariableName, ZONE_WEATHER_ENABLED);
            ZONE_FLYING_ALLOWED = ReadVariableFromConfigString("ZONE_FLYING_ALLOWED", configValuesByVariableName, ZONE_FLYING_ALLOWED);

            DUNGEON_FINDER_ENABLED = ReadVariableFromConfigString("DUNGEON_FINDER_ENABLED", configValuesByVariableName, DUNGEON_FINDER_ENABLED);

            WORLDMAP_DEBUG_GENERATION_MODE_ENABLED = ReadVariableFromConfigString("WORLDMAP_DEBUG_GENERATION_MODE_ENABLED", configValuesByVariableName, WORLDMAP_DEBUG_GENERATION_MODE_ENABLED);
            WORLDMAP_LEFT_BORDER_PIXEL_SIZE = ReadVariableFromConfigString("WORLDMAP_LEFT_BORDER_PIXEL_SIZE", configValuesByVariableName, WORLDMAP_LEFT_BORDER_PIXEL_SIZE);
            WORLDMAP_RIGHT_BORDER_PIXEL_SIZE = ReadVariableFromConfigString("WORLDMAP_RIGHT_BORDER_PIXEL_SIZE", configValuesByVariableName, WORLDMAP_RIGHT_BORDER_PIXEL_SIZE);
            WORLDMAP_TOP_BORDER_PIXEL_SIZE = ReadVariableFromConfigString("WORLDMAP_TOP_BORDER_PIXEL_SIZE", configValuesByVariableName, WORLDMAP_TOP_BORDER_PIXEL_SIZE);
            WORLDMAP_BOTTOM_BORDER_PIXEL_SIZE = ReadVariableFromConfigString("WORLDMAP_BOTTOM_BORDER_PIXEL_SIZE", configValuesByVariableName, WORLDMAP_BOTTOM_BORDER_PIXEL_SIZE);
            WORLDMAP_SHOW_SUGGESTED_LEVELS_ON_LINKED_MAPS = ReadVariableFromConfigString("WORLDMAP_SHOW_SUGGESTED_LEVELS_ON_LINKED_MAPS", configValuesByVariableName, WORLDMAP_SHOW_SUGGESTED_LEVELS_ON_LINKED_MAPS);

            EVENTS_MAX_DATETIME_YEAR = ReadVariableFromConfigString("EVENTS_MAX_DATETIME_YEAR", configValuesByVariableName, EVENTS_MAX_DATETIME_YEAR);
            EVENTS_DO_NORMALIZE_GAME_EVENTS = ReadVariableFromConfigString("EVENTS_DO_NORMALIZE_GAME_EVENTS", configValuesByVariableName, EVENTS_DO_NORMALIZE_GAME_EVENTS);
            EVENTS_NORMALIZED_DAY_SPAWN_START_HOUR = ReadVariableFromConfigString("EVENTS_NORMALIZED_DAY_SPAWN_START_HOUR", configValuesByVariableName, EVENTS_NORMALIZED_DAY_SPAWN_START_HOUR);
            EVENTS_NORMALIZED_DAY_SPAWN_LENGTH_IN_HOUR = ReadVariableFromConfigString("EVENTS_NORMALIZED_DAY_SPAWN_LENGTH_IN_HOUR", configValuesByVariableName, EVENTS_NORMALIZED_DAY_SPAWN_LENGTH_IN_HOUR);
            EVENTS_NORMALIZED_NIGHT_SPAWN_START_HOUR = ReadVariableFromConfigString("EVENTS_NORMALIZED_NIGHT_SPAWN_START_HOUR", configValuesByVariableName, EVENTS_NORMALIZED_NIGHT_SPAWN_START_HOUR);
            EVENTS_NORMALIZED_NIGHT_SPAWN_LENGTH_IN_HOUR = ReadVariableFromConfigString("EVENTS_NORMALIZED_NIGHT_SPAWN_LENGTH_IN_HOUR", configValuesByVariableName, EVENTS_NORMALIZED_NIGHT_SPAWN_LENGTH_IN_HOUR);

            LIQUID_ENABLED = ReadVariableFromConfigString("LIQUID_ENABLED", configValuesByVariableName, LIQUID_ENABLED);
            LIQUID_SHOW_TRUE_SURFACE = ReadVariableFromConfigString("LIQUID_SHOW_TRUE_SURFACE", configValuesByVariableName, LIQUID_SHOW_TRUE_SURFACE);
            LIQUID_SURFACE_ADD_Z_HEIGHT = ReadVariableFromConfigString("LIQUID_SURFACE_ADD_Z_HEIGHT", configValuesByVariableName, LIQUID_SURFACE_ADD_Z_HEIGHT);
            LIQUID_QUADGEN_PLANE_OVERLAP_SIZE = ReadVariableFromConfigString("LIQUID_QUADGEN_PLANE_OVERLAP_SIZE", configValuesByVariableName, LIQUID_QUADGEN_PLANE_OVERLAP_SIZE);

            LIGHT_INSTANCES_ENABLED = ReadVariableFromConfigString("LIGHT_INSTANCES_ENABLED", configValuesByVariableName, LIGHT_INSTANCES_ENABLED);
            LIGHT_INSTANCES_DRAWN_AS_TORCHES = ReadVariableFromConfigString("LIGHT_INSTANCES_DRAWN_AS_TORCHES", configValuesByVariableName, LIGHT_INSTANCES_DRAWN_AS_TORCHES);
            LIGHT_INSTANCE_ATTENUATION_START_PROPORTION = ReadVariableFromConfigString("LIGHT_INSTANCE_ATTENUATION_START_PROPORTION", configValuesByVariableName, LIGHT_INSTANCE_ATTENUATION_START_PROPORTION);
            LIGHT_OUTSIDE_GLOW_CLEAR_WEATHER = ReadVariableFromConfigString("LIGHT_OUTSIDE_GLOW_CLEAR_WEATHER", configValuesByVariableName, LIGHT_OUTSIDE_GLOW_CLEAR_WEATHER);
            LIGHT_OUTSIDE_GLOW_STORMY_WEATHER = ReadVariableFromConfigString("LIGHT_OUTSIDE_GLOW_STORMY_WEATHER", configValuesByVariableName, LIGHT_OUTSIDE_GLOW_STORMY_WEATHER);
            LIGHT_OUTSIDE_GLOW_UNDERWATER = ReadVariableFromConfigString("LIGHT_OUTSIDE_GLOW_UNDERWATER", configValuesByVariableName, LIGHT_OUTSIDE_GLOW_UNDERWATER);

            LIGHT_OUTSIDE_AMBIENT_TIME_0 = ReadVariableFromConfigString("LIGHT_OUTSIDE_AMBIENT_TIME_0", configValuesByVariableName, LIGHT_OUTSIDE_AMBIENT_TIME_0);
            LIGHT_OUTSIDE_AMBIENT_TIME_3 = ReadVariableFromConfigString("LIGHT_OUTSIDE_AMBIENT_TIME_3", configValuesByVariableName, LIGHT_OUTSIDE_AMBIENT_TIME_3);
            LIGHT_OUTSIDE_AMBIENT_TIME_6 = ReadVariableFromConfigString("LIGHT_OUTSIDE_AMBIENT_TIME_6", configValuesByVariableName, LIGHT_OUTSIDE_AMBIENT_TIME_6);
            LIGHT_OUTSIDE_AMBIENT_TIME_12 = ReadVariableFromConfigString("LIGHT_OUTSIDE_AMBIENT_TIME_12", configValuesByVariableName, LIGHT_OUTSIDE_AMBIENT_TIME_12);
            LIGHT_OUTSIDE_AMBIENT_TIME_21 = ReadVariableFromConfigString("LIGHT_OUTSIDE_AMBIENT_TIME_21", configValuesByVariableName, LIGHT_OUTSIDE_AMBIENT_TIME_21);
            LIGHT_OUTSIDE_AMBIENT_TIME_22 = ReadVariableFromConfigString("LIGHT_OUTSIDE_AMBIENT_TIME_22", configValuesByVariableName, LIGHT_OUTSIDE_AMBIENT_TIME_22);
            LIGHT_STORM_COLOR_MOD = ReadVariableFromConfigString("LIGHT_STORM_COLOR_MOD", configValuesByVariableName, LIGHT_STORM_COLOR_MOD);

            AUDIO_SOUNDFONT_FILE_NAME = ReadVariableFromConfigString("AUDIO_SOUNDFONT_FILE_NAME", configValuesByVariableName, AUDIO_SOUNDFONT_FILE_NAME);
            AUDIO_USE_ALTERNATE_TRACKS = ReadVariableFromConfigString("AUDIO_USE_ALTERNATE_TRACKS", configValuesByVariableName, AUDIO_USE_ALTERNATE_TRACKS);
            AUDIO_MUSIC_CONVERSION_GAIN_AMOUNT = ReadVariableFromConfigString("AUDIO_MUSIC_CONVERSION_GAIN_AMOUNT", configValuesByVariableName, AUDIO_MUSIC_CONVERSION_GAIN_AMOUNT);
            AUDIO_MUSIC_FORCE_GENERATE_ALL_MUSIC_TRACKS = ReadVariableFromConfigString("AUDIO_MUSIC_FORCE_GENERATE_ALL_MUSIC_TRACKS", configValuesByVariableName, AUDIO_MUSIC_FORCE_GENERATE_ALL_MUSIC_TRACKS);
            AUDIO_AMBIENT_SOUND_VOLUME_MOD = ReadVariableFromConfigString("AUDIO_AMBIENT_SOUND_VOLUME_MOD", configValuesByVariableName, AUDIO_AMBIENT_SOUND_VOLUME_MOD);
            AUDIO_SOUNDINSTANCE_VOLUME_MOD = ReadVariableFromConfigString("AUDIO_SOUNDINSTANCE_VOLUME_MOD", configValuesByVariableName, AUDIO_SOUNDINSTANCE_VOLUME_MOD);
            AUDIO_MUSIC_VOLUME_MOD = ReadVariableFromConfigString("AUDIO_MUSIC_VOLUME_MOD", configValuesByVariableName, AUDIO_MUSIC_VOLUME_MOD);
            AUDIO_SOUNDINSTANCE_DRAW_AS_BOX = ReadVariableFromConfigString("AUDIO_SOUNDINSTANCE_DRAW_AS_BOX", configValuesByVariableName, AUDIO_SOUNDINSTANCE_DRAW_AS_BOX);
            AUDIO_SOUNDINSTANCE_3D_MIN_DISTANCE_MOD = ReadVariableFromConfigString("AUDIO_SOUNDINSTANCE_3D_MIN_DISTANCE_MOD", configValuesByVariableName, AUDIO_SOUNDINSTANCE_3D_MIN_DISTANCE_MOD);
            AUDIO_SOUNDINSTANCE_2D_MIN_DISTANCE_MOD = ReadVariableFromConfigString("AUDIO_SOUNDINSTANCE_2D_MIN_DISTANCE_MOD", configValuesByVariableName, AUDIO_SOUNDINSTANCE_2D_MIN_DISTANCE_MOD);
            AUDIO_SOUNDINSTANCE_RENDEROBJECT_BOX_SIZE = ReadVariableFromConfigString("AUDIO_SOUNDINSTANCE_RENDEROBJECT_BOX_SIZE", configValuesByVariableName, AUDIO_SOUNDINSTANCE_RENDEROBJECT_BOX_SIZE);
            AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME = ReadVariableFromConfigString("AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME", configValuesByVariableName, AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME);
            AUDIO_CREATURE_SOUND_VOLUME = ReadVariableFromConfigString("AUDIO_CREATURE_SOUND_VOLUME", configValuesByVariableName, AUDIO_CREATURE_SOUND_VOLUME);
            AUDIO_CREATURE_MIN_DISTANCE_MOD = ReadVariableFromConfigString("AUDIO_CREATURE_MIN_DISTANCE_MOD", configValuesByVariableName, AUDIO_CREATURE_MIN_DISTANCE_MOD);
            AUDIO_SPELL_SOUND_VOLUME = ReadVariableFromConfigString("AUDIO_SPELL_SOUND_VOLUME", configValuesByVariableName, AUDIO_SPELL_SOUND_VOLUME);

            OBJECT_STATIC_LADDER_EXTEND_DISTANCE = ReadVariableFromConfigString("OBJECT_STATIC_LADDER_EXTEND_DISTANCE", configValuesByVariableName, OBJECT_STATIC_LADDER_EXTEND_DISTANCE);
            OBJECT_STATIC_LADDER_STEP_DISTANCE = ReadVariableFromConfigString("OBJECT_STATIC_LADDER_STEP_DISTANCE", configValuesByVariableName, OBJECT_STATIC_LADDER_STEP_DISTANCE);
            OBJECT_STATIC_LADDER_STEP_DROP_DISTANCE_MOD = ReadVariableFromConfigString("OBJECT_STATIC_LADDER_STEP_DROP_DISTANCE_MOD", configValuesByVariableName, OBJECT_STATIC_LADDER_STEP_DROP_DISTANCE_MOD);
            OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS = ReadVariableFromConfigString("OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS", configValuesByVariableName, OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS);
            OBJECT_GAMEOBJECT_OPENCLOSE_SLEEPER_FIELD_ANIMATIONTIME_INMS = ReadVariableFromConfigString("OBJECT_GAMEOBJECT_OPENCLOSE_SLEEPER_FIELD_ANIMATIONTIME_INMS", configValuesByVariableName, OBJECT_GAMEOBJECT_OPENCLOSE_SLEEPER_FIELD_ANIMATIONTIME_INMS);
            OBJECT_GAMEOBJECT_TRADESKILLFOCUS_EFFECT_AREA_MIN_SIZE = ReadVariableFromConfigString("OBJECT_GAMEOBJECT_TRADESKILLFOCUS_EFFECT_AREA_MIN_SIZE", configValuesByVariableName, OBJECT_GAMEOBJECT_TRADESKILLFOCUS_EFFECT_AREA_MIN_SIZE);
            OBJECT_GAMEOBJECT_ENABLE_MAILBOXES = ReadVariableFromConfigString("OBJECT_GAMEOBJECT_ENABLE_MAILBOXES", configValuesByVariableName, OBJECT_GAMEOBJECT_ENABLE_MAILBOXES);
            OBJECT_GAMEOBJECT_CHEST_USE_FIXED_RESPAWN_TIMER = ReadVariableFromConfigString("OBJECT_GAMEOBJECT_CHEST_USE_FIXED_RESPAWN_TIMER", configValuesByVariableName, OBJECT_GAMEOBJECT_CHEST_USE_FIXED_RESPAWN_TIMER);
            OBJECT_GAMEOBJECT_CHEST_FIXED_RESPAWN_TIME_IN_SEC = ReadVariableFromConfigString("OBJECT_GAMEOBJECT_CHEST_FIXED_RESPAWN_TIME_IN_SEC", configValuesByVariableName, OBJECT_GAMEOBJECT_CHEST_FIXED_RESPAWN_TIME_IN_SEC);
            OBJECT_IGNORE_RENDER_MATERIAL_ID_START = ReadVariableFromConfigString("OBJECT_IGNORE_RENDER_MATERIAL_ID_START", configValuesByVariableName, OBJECT_IGNORE_RENDER_MATERIAL_ID_START);

            CREATURE_FIDGET_TIME_PERCENT = ReadVariableFromConfigString("CREATURE_FIDGET_TIME_PERCENT", configValuesByVariableName, CREATURE_FIDGET_TIME_PERCENT);
            CREATURE_STAT_MOD_HP_ADD = ReadVariableFromConfigString("CREATURE_STAT_MOD_HP_ADD", configValuesByVariableName, CREATURE_STAT_MOD_HP_ADD);
            CREATURE_STAT_MOD_HP_MIN = ReadVariableFromConfigString("CREATURE_STAT_MOD_HP_MIN", configValuesByVariableName, CREATURE_STAT_MOD_HP_MIN);
            CREATURE_STAT_MOD_HP_MAX_NORMAL = ReadVariableFromConfigString("CREATURE_STAT_MOD_HP_MAX_NORMAL", configValuesByVariableName, CREATURE_STAT_MOD_HP_MAX_NORMAL);
            CREATURE_STAT_MOD_HP_MAX_RARE = ReadVariableFromConfigString("CREATURE_STAT_MOD_HP_MAX_RARE", configValuesByVariableName, CREATURE_STAT_MOD_HP_MAX_RARE);
            CREATURE_STAT_MOD_HP_SET_ELITE = ReadVariableFromConfigString("CREATURE_STAT_MOD_HP_SET_ELITE", configValuesByVariableName, CREATURE_STAT_MOD_HP_SET_ELITE);
            CREATURE_STAT_MOD_HP_SET_ELITERARE = ReadVariableFromConfigString("CREATURE_STAT_MOD_HP_SET_ELITERARE", configValuesByVariableName, CREATURE_STAT_MOD_HP_SET_ELITERARE);
            CREATURE_STAT_MOD_HP_SET_BOSS = ReadVariableFromConfigString("CREATURE_STAT_MOD_HP_SET_BOSS", configValuesByVariableName, CREATURE_STAT_MOD_HP_SET_BOSS);
            CREATURE_STAT_MOD_AVGDMG_ADD = ReadVariableFromConfigString("CREATURE_STAT_MOD_AVGDMG_ADD", configValuesByVariableName, CREATURE_STAT_MOD_AVGDMG_ADD);
            CREATURE_STAT_MOD_AVGDMG_MIN = ReadVariableFromConfigString("CREATURE_STAT_MOD_AVGDMG_MIN", configValuesByVariableName, CREATURE_STAT_MOD_AVGDMG_MIN);
            CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL = ReadVariableFromConfigString("CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL", configValuesByVariableName, CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL);
            CREATURE_STAT_MOD_AVGDMG_MAX_RARE = ReadVariableFromConfigString("CREATURE_STAT_MOD_AVGDMG_MAX_RARE", configValuesByVariableName, CREATURE_STAT_MOD_AVGDMG_MAX_RARE);
            CREATURE_STAT_MOD_AVGDMG_SET_ELITE = ReadVariableFromConfigString("CREATURE_STAT_MOD_AVGDMG_SET_ELITE", configValuesByVariableName, CREATURE_STAT_MOD_AVGDMG_SET_ELITE);
            CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE = ReadVariableFromConfigString("CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE", configValuesByVariableName, CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE);
            CREATURE_STAT_MOD_AVGDMG_SET_BOSS = ReadVariableFromConfigString("CREATURE_STAT_MOD_AVGDMG_SET_BOSS", configValuesByVariableName, CREATURE_STAT_MOD_AVGDMG_SET_BOSS);
            CREATURE_PET_ALLOW_STAT_MOD_SCALING = ReadVariableFromConfigString("CREATURE_PET_ALLOW_STAT_MOD_SCALING", configValuesByVariableName, CREATURE_PET_ALLOW_STAT_MOD_SCALING);
            CREATURE_FACTION_ROOT_NAME = ReadVariableFromConfigString("CREATURE_FACTION_ROOT_NAME", configValuesByVariableName, CREATURE_FACTION_ROOT_NAME);
            CREATURE_FACTION_TEMPLATE_DEFAULT = ReadVariableFromConfigString("CREATURE_FACTION_TEMPLATE_DEFAULT", configValuesByVariableName, CREATURE_FACTION_TEMPLATE_DEFAULT);
            CREATURE_FACTION_DEFAULT = ReadVariableFromConfigString("CREATURE_FACTION_DEFAULT", configValuesByVariableName, CREATURE_FACTION_DEFAULT);
            CREATURE_FACTION_TEMPLATE_NEUTRAL = ReadVariableFromConfigString("CREATURE_FACTION_TEMPLATE_NEUTRAL", configValuesByVariableName, CREATURE_FACTION_TEMPLATE_NEUTRAL);
            CREATURE_FACTION_TEMPLATE_NEUTRAL_INTERACTIVE = ReadVariableFromConfigString("CREATURE_FACTION_TEMPLATE_NEUTRAL_INTERACTIVE", configValuesByVariableName, CREATURE_FACTION_TEMPLATE_NEUTRAL_INTERACTIVE);
            CREATURE_FACTION_SHOW_ALL = ReadVariableFromConfigString("CREATURE_FACTION_SHOW_ALL", configValuesByVariableName, CREATURE_FACTION_SHOW_ALL);
            CREATURE_REP_REWARD_MULTIPLIER = ReadVariableFromConfigString("CREATURE_REP_REWARD_MULTIPLIER", configValuesByVariableName, CREATURE_REP_REWARD_MULTIPLIER);
            CREATURE_GOSSIP_NPC_TEXT_ID = ReadVariableFromConfigString("CREATURE_GOSSIP_NPC_TEXT_ID", configValuesByVariableName, CREATURE_GOSSIP_NPC_TEXT_ID);
            CREATURE_GOSSIP_TRAIN_BROADCAST_TEXT_ID = ReadVariableFromConfigString("CREATURE_GOSSIP_TRAIN_BROADCAST_TEXT_ID", configValuesByVariableName, CREATURE_GOSSIP_TRAIN_BROADCAST_TEXT_ID);
            CREATURE_GOSSIP_UNLEARN_BROADCAST_TEXT_ID = ReadVariableFromConfigString("CREATURE_GOSSIP_UNLEARN_BROADCAST_TEXT_ID", configValuesByVariableName, CREATURE_GOSSIP_UNLEARN_BROADCAST_TEXT_ID);
            CREATURE_GOSSIP_DUALTALENT_BROADCAST_TEXT_ID = ReadVariableFromConfigString("CREATURE_GOSSIP_DUALTALENT_BROADCAST_TEXT_ID", configValuesByVariableName, CREATURE_GOSSIP_DUALTALENT_BROADCAST_TEXT_ID);
            CREATURE_GOSSIP_PURCHASE_BROADCAST_TEXT_ID = ReadVariableFromConfigString("CREATURE_GOSSIP_PURCHASE_BROADCAST_TEXT_ID", configValuesByVariableName, CREATURE_GOSSIP_PURCHASE_BROADCAST_TEXT_ID);
            CREATURE_CLASS_TRAINER_UNLEARN_MENU_ID = ReadVariableFromConfigString("CREATURE_CLASS_TRAINER_UNLEARN_MENU_ID", configValuesByVariableName, CREATURE_CLASS_TRAINER_UNLEARN_MENU_ID);
            CREATURE_CLASS_TRAINER_DUALTALENT_MENU_ID = ReadVariableFromConfigString("CREATURE_CLASS_TRAINER_DUALTALENT_MENU_ID", configValuesByVariableName, CREATURE_CLASS_TRAINER_DUALTALENT_MENU_ID);
            CREATURE_RIDING_TRAINERS_ENABLED = ReadVariableFromConfigString("CREATURE_RIDING_TRAINERS_ENABLED", configValuesByVariableName, CREATURE_RIDING_TRAINERS_ENABLED);
            CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY = ReadVariableFromConfigString("CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY", configValuesByVariableName, CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY);
            CREATURE_SPELL_OOC_BUFF_MIN_DURATION_IN_MS = ReadVariableFromConfigString("CREATURE_SPELL_OOC_BUFF_MIN_DURATION_IN_MS", configValuesByVariableName, CREATURE_SPELL_OOC_BUFF_MIN_DURATION_IN_MS);
            CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MIN_IN_MS = ReadVariableFromConfigString("CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MIN_IN_MS", configValuesByVariableName, CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MIN_IN_MS);
            CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MAX_IN_MS = ReadVariableFromConfigString("CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MAX_IN_MS", configValuesByVariableName, CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MAX_IN_MS);
            CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_RANDOM_RANGE_ADD_IN_MS = ReadVariableFromConfigString("CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_RANDOM_RANGE_ADD_IN_MS", configValuesByVariableName, CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_RANDOM_RANGE_ADD_IN_MS);
            CREATURE_SPELL_COMBAT_RECAST_DELAY_MAX_ADD_MOD = ReadVariableFromConfigString("CREATURE_SPELL_COMBAT_RECAST_DELAY_MAX_ADD_MOD", configValuesByVariableName, CREATURE_SPELL_COMBAT_RECAST_DELAY_MAX_ADD_MOD);
            CREATURE_SPELL_COMBAT_HEAL_MIN_LIFE_PERCENT = ReadVariableFromConfigString("CREATURE_SPELL_COMBAT_HEAL_MIN_LIFE_PERCENT", configValuesByVariableName, CREATURE_SPELL_COMBAT_HEAL_MIN_LIFE_PERCENT);
            CREATURE_MANA_REGEN_PERCENT = ReadVariableFromConfigString("CREATURE_MANA_REGEN_PERCENT", configValuesByVariableName, CREATURE_MANA_REGEN_PERCENT);
            CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT = ReadVariableFromConfigString("CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT", configValuesByVariableName, CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT);
            CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT = ReadVariableFromConfigString("CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT", configValuesByVariableName, CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT);
            CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT = ReadVariableFromConfigString("CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT", configValuesByVariableName, CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT);
            CREATURE_SPAWN_LOCATION_TAKEN_FROM_GRID_FOR_NON_CUSTOM_PATH = ReadVariableFromConfigString("CREATURE_SPAWN_LOCATION_TAKEN_FROM_GRID_FOR_NON_CUSTOM_PATH", configValuesByVariableName, CREATURE_SPAWN_LOCATION_TAKEN_FROM_GRID_FOR_NON_CUSTOM_PATH);

            ITEMS_USE_ALTERNATE_STATS = ReadVariableFromConfigString("ITEMS_USE_ALTERNATE_STATS", configValuesByVariableName, ITEMS_USE_ALTERNATE_STATS);
            ITEMS_WEAPON_DELAY_REDUCTION_AMT = ReadVariableFromConfigString("ITEMS_WEAPON_DELAY_REDUCTION_AMT", configValuesByVariableName, ITEMS_WEAPON_DELAY_REDUCTION_AMT);
            ITEMS_WEAPON_EFFECT_PPM_BASE_RATE = ReadVariableFromConfigString("ITEMS_WEAPON_EFFECT_PPM_BASE_RATE", configValuesByVariableName, ITEMS_WEAPON_EFFECT_PPM_BASE_RATE);
            ITEMS_SHOW_WORN_EFFECT_AURA_ICON = ReadVariableFromConfigString("ITEMS_SHOW_WORN_EFFECT_AURA_ICON", configValuesByVariableName, ITEMS_SHOW_WORN_EFFECT_AURA_ICON);
            ITEMS_CREATE_ESSENCE_ITEM_FOR_EQUIPEABLE_CLICK_SPELL_ITEMS = ReadVariableFromConfigString("ITEMS_CREATE_ESSENCE_ITEM_FOR_EQUIPEABLE_CLICK_SPELL_ITEMS", configValuesByVariableName, ITEMS_CREATE_ESSENCE_ITEM_FOR_EQUIPEABLE_CLICK_SPELL_ITEMS);
            ITEMS_STATS_LOW_BIAS_WEIGHT = ReadVariableFromConfigString("ITEMS_STATS_LOW_BIAS_WEIGHT", configValuesByVariableName, ITEMS_STATS_LOW_BIAS_WEIGHT);
            ITEMS_BAG_SLOT_MULTIPLIER = ReadVariableFromConfigString("ITEMS_BAG_SLOT_MULTIPLIER", configValuesByVariableName, ITEMS_BAG_SLOT_MULTIPLIER);
            ITEMS_BAG_WEIGHT_REDUCTION_INCREASES_SLOTS_ENABLED = ReadVariableFromConfigString("ITEMS_BAG_WEIGHT_REDUCTION_INCREASES_SLOTS_ENABLED", configValuesByVariableName, ITEMS_BAG_WEIGHT_REDUCTION_INCREASES_SLOTS_ENABLED);
            ITEM_BAG_WEIGHT_REDUCTION_INCREASE_SLOTS_ADD_PER_PERCENT = ReadVariableFromConfigString("ITEM_BAG_WEIGHT_REDUCTION_INCREASE_SLOTS_ADD_PER_PERCENT", configValuesByVariableName, ITEM_BAG_WEIGHT_REDUCTION_INCREASE_SLOTS_ADD_PER_PERCENT);
            ITEMS_MULTI_ITEMS_CONTAINER_ICON_ID = ReadVariableFromConfigString("ITEMS_MULTI_ITEMS_CONTAINER_ICON_ID", configValuesByVariableName, ITEMS_MULTI_ITEMS_CONTAINER_ICON_ID);
            ITEM_ARROW_MODEL_NAME = ReadVariableFromConfigString("ITEM_ARROW_MODEL_NAME", configValuesByVariableName, ITEM_ARROW_MODEL_NAME);
            ITEM_ARROW_TEXTURE_NAME = ReadVariableFromConfigString("ITEM_ARROW_TEXTURE_NAME", configValuesByVariableName, ITEM_ARROW_TEXTURE_NAME);
            ITEMS_MONK_EPIC_GLOVES_IT159_SPELL_ID = ReadVariableFromConfigString("ITEMS_MONK_EPIC_GLOVES_IT159_SPELL_ID", configValuesByVariableName, ITEMS_MONK_EPIC_GLOVES_IT159_SPELL_ID);
            ITEMS_FISHING_BAIT_POTENCY_TIER_1_SPELL_ID = ReadVariableFromConfigString("ITEMS_FISHING_BAIT_POTENCY_TIER_1_SPELL_ID", configValuesByVariableName, ITEMS_FISHING_BAIT_POTENCY_TIER_1_SPELL_ID);
            ITEMS_FISHING_BAIT_POTENCY_TIER_2_SPELL_ID = ReadVariableFromConfigString("ITEMS_FISHING_BAIT_POTENCY_TIER_2_SPELL_ID", configValuesByVariableName, ITEMS_FISHING_BAIT_POTENCY_TIER_2_SPELL_ID);
            ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_WIND = ReadVariableFromConfigString("ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_WIND", configValuesByVariableName, ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_WIND);
            ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_STRING = ReadVariableFromConfigString("ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_STRING", configValuesByVariableName, ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_STRING);
            ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_BRASS = ReadVariableFromConfigString("ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_BRASS", configValuesByVariableName, ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_BRASS);
            ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_PERCUSSION = ReadVariableFromConfigString("ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_PERCUSSION", configValuesByVariableName, ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_PERCUSSION);
            ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_ALL = ReadVariableFromConfigString("ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_ALL", configValuesByVariableName, ITEM_INSTRUMENT_TOTEM_CATEGORY_DBCID_ALL);

            QUESTS_TEXT_DURATION_IN_MS = ReadVariableFromConfigString("QUESTS_TEXT_DURATION_IN_MS", configValuesByVariableName, QUESTS_TEXT_DURATION_IN_MS);
            QUESTS_ITEMS_REWARD_CONTAINER_ICON_ID = ReadVariableFromConfigString("QUESTS_ITEMS_REWARD_CONTAINER_ICON_ID", configValuesByVariableName, QUESTS_ITEMS_REWARD_CONTAINER_ICON_ID);

            SPELL_EFFECT_SUMMON_PETS_USE_EQ_LEVEL_AND_BEHAVIOR = ReadVariableFromConfigString("SPELL_EFFECT_SUMMON_PETS_USE_EQ_LEVEL_AND_BEHAVIOR", configValuesByVariableName, SPELL_EFFECT_SUMMON_PETS_USE_EQ_LEVEL_AND_BEHAVIOR);
            SPELL_EFFECT_CALC_STATS_FOR_MAX_LEVEL = ReadVariableFromConfigString("SPELL_EFFECT_CALC_STATS_FOR_MAX_LEVEL", configValuesByVariableName, SPELL_EFFECT_CALC_STATS_FOR_MAX_LEVEL);
            SPELLS_GATE_TETHER_ENABLED = ReadVariableFromConfigString("SPELLS_GATE_TETHER_ENABLED", configValuesByVariableName, SPELLS_GATE_TETHER_ENABLED);
            SPELLS_GATECUSTOM_SPELLDBC_ID = ReadVariableFromConfigString("SPELLS_GATECUSTOM_SPELLDBC_ID", configValuesByVariableName, SPELLS_GATECUSTOM_SPELLDBC_ID);
            SPELLS_BINDCUSTOM_SPELLDBC_ID = ReadVariableFromConfigString("SPELLS_BINDCUSTOM_SPELLDBC_ID", configValuesByVariableName, SPELLS_BINDCUSTOM_SPELLDBC_ID);
            SPELLS_RANGE_MULTIPLIER = ReadVariableFromConfigString("SPELLS_RANGE_MULTIPLIER", configValuesByVariableName, SPELLS_RANGE_MULTIPLIER);
            SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE = ReadVariableFromConfigString("SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE", configValuesByVariableName, SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE);
            SPELL_PERIODIC_SECONDS_PER_TICK_EQ = ReadVariableFromConfigString("SPELL_PERIODIC_SECONDS_PER_TICK_EQ", configValuesByVariableName, SPELL_PERIODIC_SECONDS_PER_TICK_EQ);
            SPELL_PERIODIC_SECONDS_PER_TICK_WOW = ReadVariableFromConfigString("SPELL_PERIODIC_SECONDS_PER_TICK_WOW", configValuesByVariableName, SPELL_PERIODIC_SECONDS_PER_TICK_WOW);
            SPELL_PERIODIC_BARD_TICK_BUFFER_IN_MS = ReadVariableFromConfigString("SPELL_PERIODIC_BARD_TICK_BUFFER_IN_MS", configValuesByVariableName, SPELL_PERIODIC_BARD_TICK_BUFFER_IN_MS);
            SPELL_MAX_CONCURRENT_BARD_SONGS = ReadVariableFromConfigString("SPELL_MAX_CONCURRENT_BARD_SONGS", configValuesByVariableName, SPELL_MAX_CONCURRENT_BARD_SONGS);
            SPELL_RECOVERY_TIME_MINIMUM_IN_MS = ReadVariableFromConfigString("SPELL_RECOVERY_TIME_MINIMUM_IN_MS", configValuesByVariableName, SPELL_RECOVERY_TIME_MINIMUM_IN_MS);
            SPELL_FEIGN_DEATH_FAIL_CHANCE_PERCENT = ReadVariableFromConfigString("SPELL_FEIGN_DEATH_FAIL_CHANCE_PERCENT", configValuesByVariableName, SPELL_FEIGN_DEATH_FAIL_CHANCE_PERCENT);
            SPELLS_LEARNABLE_FROM_ITEMS_ENABLED = ReadVariableFromConfigString("SPELLS_LEARNABLE_FROM_ITEMS_ENABLED", configValuesByVariableName, SPELLS_LEARNABLE_FROM_ITEMS_ENABLED);
            SPELLS_EFFECT_EMITTER_LONGEST_SPELL_TIME_IN_MS = ReadVariableFromConfigString("SPELLS_EFFECT_EMITTER_LONGEST_SPELL_TIME_IN_MS", configValuesByVariableName, SPELLS_EFFECT_EMITTER_LONGEST_SPELL_TIME_IN_MS);
            SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE = ReadVariableFromConfigString("SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE", configValuesByVariableName, SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE);
            SPELLS_ENCHANT_SPELL_IMBUE_PROC_CHANGE = ReadVariableFromConfigString("SPELLS_ENCHANT_SPELL_IMBUE_PROC_CHANGE", configValuesByVariableName, SPELLS_ENCHANT_SPELL_IMBUE_PROC_CHANGE);
            SPELL_ENCHANT_ROGUE_POISON_ENCHANT_DURATION_ON_WEAPON_TIME_IN_SECONDS = ReadVariableFromConfigString("SPELL_ENCHANT_ROGUE_POISON_ENCHANT_DURATION_ON_WEAPON_TIME_IN_SECONDS", configValuesByVariableName, SPELL_ENCHANT_ROGUE_POISON_ENCHANT_DURATION_ON_WEAPON_TIME_IN_SECONDS);
            SPELL_ENCHANT_ROGUE_POISON_APPLY_TIME_IN_MS = ReadVariableFromConfigString("SPELL_ENCHANT_ROGUE_POISON_APPLY_TIME_IN_MS", configValuesByVariableName, SPELL_ENCHANT_ROGUE_POISON_APPLY_TIME_IN_MS);
            SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_APPLYING_VISUAL_ID = ReadVariableFromConfigString("SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_APPLYING_VISUAL_ID", configValuesByVariableName, SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_APPLYING_VISUAL_ID);
            SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_EFFECT_VISUAL_ID = ReadVariableFromConfigString("SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_EFFECT_VISUAL_ID", configValuesByVariableName, SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_EFFECT_VISUAL_ID);
            SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_SPELL = ReadVariableFromConfigString("SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_SPELL", configValuesByVariableName, SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_SPELL);
            SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_SPELL = ReadVariableFromConfigString("SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_ENVIRONMENT", configValuesByVariableName, SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN_ENVIRONMENT);
            SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_SPELL = ReadVariableFromConfigString("SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_SPELL", configValuesByVariableName, SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_SPELL);
            SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_SPELL = ReadVariableFromConfigString("SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_ENVIRONMENT", configValuesByVariableName, SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX_ENVIRONMENT);
            SPELLS_EFFECT_EMITTER_TARGET_DURATION_IN_MS = ReadVariableFromConfigString("SPELLS_EFFECT_EMITTER_TARGET_DURATION_IN_MS", configValuesByVariableName, SPELLS_EFFECT_EMITTER_TARGET_DURATION_IN_MS);
            SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_SPELL = ReadVariableFromConfigString("SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_SPELL", configValuesByVariableName, SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_SPELL);
            SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_ENVIRONMENT = ReadVariableFromConfigString("SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_ENVIRONMENT", configValuesByVariableName, SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD_ENVIRONMENT);
            SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_SPELL = ReadVariableFromConfigString("SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_SPELL", configValuesByVariableName, SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_SPELL);
            SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_ENVIRONMENT = ReadVariableFromConfigString("SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_ENVIRONMENT", configValuesByVariableName, SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD_ENVIRONMENT);
            SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MINIMUM = ReadVariableFromConfigString("SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MINIMUM", configValuesByVariableName, SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MINIMUM);
            SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_DEFAULT = ReadVariableFromConfigString("SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_DEFAULT", configValuesByVariableName, SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_DEFAULT);
            SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_DEFAULT = ReadVariableFromConfigString("SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_DEFAULT", configValuesByVariableName, SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_DEFAULT);
            SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MOD = ReadVariableFromConfigString("SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MOD", configValuesByVariableName, SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MOD);
            SPELL_EFFECT_EMITTER_SPAWN_RATE_DISC_MOD = ReadVariableFromConfigString("SPELL_EFFECT_EMITTER_SPAWN_RATE_DISC_MOD", configValuesByVariableName, SPELL_EFFECT_EMITTER_SPAWN_RATE_DISC_MOD);
            SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_MOD = ReadVariableFromConfigString("SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_MOD", configValuesByVariableName, SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_MOD);
            SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MIN = ReadVariableFromConfigString("SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MIN", configValuesByVariableName, SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MIN);
            SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX = ReadVariableFromConfigString("SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX", configValuesByVariableName, SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX);
            SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX_EQ_VALUE = ReadVariableFromConfigString("SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX_EQ_VALUE", configValuesByVariableName, SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX_EQ_VALUE);
            SPELL_EFFECT_SPRITE_LIST_RADIUS_MOD = ReadVariableFromConfigString("SPELL_EFFECT_SPRITE_LIST_RADIUS_MOD", configValuesByVariableName, SPELL_EFFECT_SPRITE_LIST_RADIUS_MOD);
            SPELL_EFFECT_SPRITE_LIST_ANIMATION_SCALE_MOD = ReadVariableFromConfigString("SPELL_EFFECT_SPRITE_LIST_ANIMATION_SCALE_MOD", configValuesByVariableName, SPELL_EFFECT_SPRITE_LIST_ANIMATION_SCALE_MOD);
            SPELL_EFFECT_SPRITE_LIST_ANIMATION_FRAME_DELAY_IN_MS = ReadVariableFromConfigString("SPELL_EFFECT_SPRITE_LIST_ANIMATION_FRAME_DELAY_IN_MS", configValuesByVariableName, SPELL_EFFECT_SPRITE_LIST_ANIMATION_FRAME_DELAY_IN_MS);
            SPELL_EFFECT_SPRITE_LIST_MAX_NON_PREJECTILE_ANIM_TIME_IN_MS = ReadVariableFromConfigString("SPELL_EFFECT_SPRITE_LIST_MAX_NON_PREJECTILE_ANIM_TIME_IN_MS", configValuesByVariableName, SPELL_EFFECT_SPRITE_LIST_MAX_NON_PREJECTILE_ANIM_TIME_IN_MS);
            SPELL_EFFECT_SPRITE_LIST_PULSE_RANGE = ReadVariableFromConfigString("SPELL_EFFECT_SPRITE_LIST_PULSE_RANGE", configValuesByVariableName, SPELL_EFFECT_SPRITE_LIST_PULSE_RANGE);
            SPELL_EFFECT_SPRITE_LIST_CIRCULAR_SHIFT_MOD = ReadVariableFromConfigString("SPELL_EFFECT_SPRITE_LIST_CIRCULAR_SHIFT_MOD", configValuesByVariableName, SPELL_EFFECT_SPRITE_LIST_CIRCULAR_SHIFT_MOD);
            SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_HIGH = ReadVariableFromConfigString("SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_HIGH", configValuesByVariableName, SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_HIGH);
            SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_LOW = ReadVariableFromConfigString("SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_LOW", configValuesByVariableName, SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_LOW);
            SPELL_EFFECT_BARD_TICK_VISUAL_DURATION_MOD_FROM_TICK = ReadVariableFromConfigString("SPELL_EFFECT_BARD_TICK_VISUAL_DURATION_MOD_FROM_TICK", configValuesByVariableName, SPELL_EFFECT_BARD_TICK_VISUAL_DURATION_MOD_FROM_TICK);
            SPELL_EFFECT_BARD_ADDITIONAL_TICK_ON_CAST = ReadVariableFromConfigString("SPELL_EFFECT_BARD_ADDITIONAL_TICK_ON_CAST", configValuesByVariableName, SPELL_EFFECT_BARD_ADDITIONAL_TICK_ON_CAST);
            SPELL_EFFECT_VALUE_LOW_BIAS_WEIGHT = ReadVariableFromConfigString("SPELL_EFFECT_VALUE_LOW_BIAS_WEIGHT", configValuesByVariableName, SPELL_EFFECT_VALUE_LOW_BIAS_WEIGHT);
            SPELL_EFFECT_USE_DYNAMIC_EFFECT_VALUES = ReadVariableFromConfigString("SPELL_EFFECT_USE_DYNAMIC_EFFECT_VALUES", configValuesByVariableName, SPELL_EFFECT_USE_DYNAMIC_EFFECT_VALUES);
            SPELL_EFFECT_USE_DYNAMIC_AURA_DURATIONS = ReadVariableFromConfigString("SPELL_EFFECT_USE_DYNAMIC_AURA_DURATIONS", configValuesByVariableName, SPELL_EFFECT_USE_DYNAMIC_AURA_DURATIONS);
            SPELL_EFFECT_REVIVE_EXPPCT_TO_HPMP_MULTIPLIER = ReadVariableFromConfigString("SPELL_EFFECT_REVIVE_EXPPCT_TO_HPMP_MULTIPLIER", configValuesByVariableName, SPELL_EFFECT_REVIVE_EXPPCT_TO_HPMP_MULTIPLIER);
            SPELL_MODEL_SIZE_CHANGE_EFFECT_DEFAULT_TIME_IN_MS = ReadVariableFromConfigString("SPELL_MODEL_SIZE_CHANGE_EFFECT_DEFAULT_TIME_IN_MS", configValuesByVariableName, SPELL_MODEL_SIZE_CHANGE_EFFECT_DEFAULT_TIME_IN_MS);
            SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID = ReadVariableFromConfigString("SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID", configValuesByVariableName, SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID);
            SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN = ReadVariableFromConfigString("SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN", configValuesByVariableName, SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN);
            SPELL_DEFAULT_SPELL_POWER_INFLUENCE_PERCENT = ReadVariableFromConfigString("SPELL_DEFAULT_SPELL_POWER_INFLUENCE_PERCENT", configValuesByVariableName, SPELL_DEFAULT_SPELL_POWER_INFLUENCE_PERCENT);
            SPELL_SUMMON_CASTER_AURA_SPELL_ID = ReadVariableFromConfigString("SPELL_SUMMON_CASTER_AURA_SPELL_ID", configValuesByVariableName, SPELL_SUMMON_CASTER_AURA_SPELL_ID);
            SPELL_CREATURE_REDUCED_MANA_REGEN_SPELL_ID = ReadVariableFromConfigString("SPELL_CREATURE_REDUCED_MANA_REGEN_SPELL_ID", configValuesByVariableName, SPELL_CREATURE_REDUCED_MANA_REGEN_SPELL_ID);
            SPELL_CREATURE_SEE_INVIS_AND_STEALTH_SPELL_ID = ReadVariableFromConfigString("SPELL_CREATURE_SEE_INVIS_AND_STEALTH_SPELL_ID", configValuesByVariableName, SPELL_CREATURE_SEE_INVIS_AND_STEALTH_SPELL_ID);
            SPELL_INVIS_VS_UNDEAD_INVIS_TYPE = ReadVariableFromConfigString("SPELL_INVIS_VS_UNDEAD_INVIS_TYPE", configValuesByVariableName, SPELL_INVIS_VS_UNDEAD_INVIS_TYPE);
            SPELL_CREATURE_INVIS_VS_UNDEAD_DETECT_SPELL_ID = ReadVariableFromConfigString("SPELL_CREATURE_INVIS_VS_UNDEAD_DETECT_SPELL_ID", configValuesByVariableName, SPELL_CREATURE_INVIS_VS_UNDEAD_DETECT_SPELL_ID);
            COMBAT_DAZE_IN_EQ_ZONES_ENABLED = ReadVariableFromConfigString("COMBAT_DAZE_IN_EQ_ZONES_ENABLED", configValuesByVariableName, COMBAT_DAZE_IN_EQ_ZONES_ENABLED);
            COMBATSKILL_BASH_ENABLED = ReadVariableFromConfigString("COMBATSKILL_BASH_ENABLED", configValuesByVariableName, COMBATSKILL_BASH_ENABLED);
            COMBATSKILL_BASH_PLAYER_LEARNABLE = ReadVariableFromConfigString("COMBATSKILL_BASH_PLAYER_LEARNABLE", configValuesByVariableName, COMBATSKILL_BASH_PLAYER_LEARNABLE);
            COMBATSKILL_BASH_SPELL_ID = ReadVariableFromConfigString("COMBATSKILL_BASH_SPELL_ID", configValuesByVariableName, COMBATSKILL_BASH_SPELL_ID);
            COMBATSKILL_BASH_SPELL_ICON_EQ_ID = ReadVariableFromConfigString("COMBATSKILL_BASH_SPELL_ICON_EQ_ID", configValuesByVariableName, COMBATSKILL_BASH_SPELL_ICON_EQ_ID);
            COMBATSKILL_BASH_CREATURE_MIN_LEVEL = ReadVariableFromConfigString("COMBATSKILL_BASH_CREATURE_MIN_LEVEL", configValuesByVariableName, COMBATSKILL_BASH_CREATURE_MIN_LEVEL);
            COMBATSKILL_BASH_COOLDOWN_IN_MS = ReadVariableFromConfigString("COMBATSKILL_BASH_COOLDOWN_IN_MS", configValuesByVariableName, COMBATSKILL_BASH_COOLDOWN_IN_MS);
            COMBATSKILL_BASH_STUN_DURATION_IN_MS = ReadVariableFromConfigString("COMBATSKILL_BASH_STUN_DURATION_IN_MS", configValuesByVariableName, COMBATSKILL_BASH_STUN_DURATION_IN_MS);
            COMBATSKILL_BASH_BASE_DAMAGE = ReadVariableFromConfigString("COMBATSKILL_BASH_BASE_DAMAGE", configValuesByVariableName, COMBATSKILL_BASH_BASE_DAMAGE);
            COMBATSKILL_BASH_DAMAGE_PER_LEVEL = ReadVariableFromConfigString("COMBATSKILL_BASH_DAMAGE_PER_LEVEL", configValuesByVariableName, COMBATSKILL_BASH_DAMAGE_PER_LEVEL);
            COMBATSKILL_BASH_RANGE = ReadVariableFromConfigString("COMBATSKILL_BASH_RANGE", configValuesByVariableName, COMBATSKILL_BASH_RANGE);
            COMBATSKILL_BASH_FORBEARANCE_ENABLED = ReadVariableFromConfigString("COMBATSKILL_BASH_FORBEARANCE_ENABLED", configValuesByVariableName, COMBATSKILL_BASH_FORBEARANCE_ENABLED);
            COMBATSKILL_BASH_FORBEARANCE_SPELL_ID = ReadVariableFromConfigString("COMBATSKILL_BASH_FORBEARANCE_SPELL_ID", configValuesByVariableName, COMBATSKILL_BASH_FORBEARANCE_SPELL_ID);
            COMBATSKILL_BASH_FORBEARANCE_SPELL_ICON_EQ_ID = ReadVariableFromConfigString("COMBATSKILL_BASH_FORBEARANCE_SPELL_ICON_EQ_ID", configValuesByVariableName, COMBATSKILL_BASH_FORBEARANCE_SPELL_ICON_EQ_ID);
            COMBATSKILL_BASH_FORBEARANCE_DURATION_IN_MS = ReadVariableFromConfigString("COMBATSKILL_BASH_FORBEARANCE_DURATION_IN_MS", configValuesByVariableName, COMBATSKILL_BASH_FORBEARANCE_DURATION_IN_MS);
            COMBATSKILL_SLAM_ENABLED = ReadVariableFromConfigString("COMBATSKILL_SLAM_ENABLED", configValuesByVariableName, COMBATSKILL_SLAM_ENABLED);
            COMBATSKILL_SLAM_PLAYER_LEARNABLE = ReadVariableFromConfigString("COMBATSKILL_SLAM_PLAYER_LEARNABLE", configValuesByVariableName, COMBATSKILL_SLAM_PLAYER_LEARNABLE);
            COMBATSKILL_SLAM_SPELL_ID = ReadVariableFromConfigString("COMBATSKILL_SLAM_SPELL_ID", configValuesByVariableName, COMBATSKILL_SLAM_SPELL_ID);
            COMBATSKILL_SLAM_SPELL_ICON_EQ_ID = ReadVariableFromConfigString("COMBATSKILL_SLAM_SPELL_ICON_EQ_ID", configValuesByVariableName, COMBATSKILL_SLAM_SPELL_ICON_EQ_ID);
            COMBATSKILL_SLAM_STUN_DURATION_IN_MS = ReadVariableFromConfigString("COMBATSKILL_SLAM_STUN_DURATION_IN_MS", configValuesByVariableName, COMBATSKILL_SLAM_STUN_DURATION_IN_MS);
            COMBATSKILL_SLAM_BASE_DAMAGE = ReadVariableFromConfigString("COMBATSKILL_SLAM_BASE_DAMAGE", configValuesByVariableName, COMBATSKILL_SLAM_BASE_DAMAGE);
            COMBATSKILL_SLAM_DAMAGE_PER_LEVEL = ReadVariableFromConfigString("COMBATSKILL_SLAM_DAMAGE_PER_LEVEL", configValuesByVariableName, COMBATSKILL_SLAM_DAMAGE_PER_LEVEL);
            COMBATSKILL_SLAM_RANGE = ReadVariableFromConfigString("COMBATSKILL_SLAM_RANGE", configValuesByVariableName, COMBATSKILL_SLAM_RANGE);
            COMBATSKILL_HARMTOUCH_ENABLED = ReadVariableFromConfigString("COMBATSKILL_HARMTOUCH_ENABLED", configValuesByVariableName, COMBATSKILL_HARMTOUCH_ENABLED);
            COMBATSKILL_HARMTOUCH_PLAYER_LEARNABLE = ReadVariableFromConfigString("COMBATSKILL_HARMTOUCH_PLAYER_LEARNABLE", configValuesByVariableName, COMBATSKILL_HARMTOUCH_PLAYER_LEARNABLE);
            COMBATSKILL_HARMTOUCH_SPELL_ID = ReadVariableFromConfigString("COMBATSKILL_HARMTOUCH_SPELL_ID", configValuesByVariableName, COMBATSKILL_HARMTOUCH_SPELL_ID);
            COMBATSKILL_HARMTOUCH_SPELL_ICON_EQ_ID = ReadVariableFromConfigString("COMBATSKILL_HARMTOUCH_SPELL_ICON_EQ_ID", configValuesByVariableName, COMBATSKILL_HARMTOUCH_SPELL_ICON_EQ_ID);
            COMBATSKILL_HARMTOUCH_CREATURE_MIN_LEVEL = ReadVariableFromConfigString("COMBATSKILL_HARMTOUCH_CREATURE_MIN_LEVEL", configValuesByVariableName, COMBATSKILL_HARMTOUCH_CREATURE_MIN_LEVEL);
            COMBATSKILL_HARMTOUCH_COOLDOWN_IN_MS = ReadVariableFromConfigString("COMBATSKILL_HARMTOUCH_COOLDOWN_IN_MS", configValuesByVariableName, COMBATSKILL_HARMTOUCH_COOLDOWN_IN_MS);
            COMBATSKILL_HARMTOUCH_CREATURE_INITIAL_DELAY_IN_MS = ReadVariableFromConfigString("COMBATSKILL_HARMTOUCH_CREATURE_INITIAL_DELAY_IN_MS", configValuesByVariableName, COMBATSKILL_HARMTOUCH_CREATURE_INITIAL_DELAY_IN_MS);
            COMBATSKILL_HARMTOUCH_BASE_DAMAGE = ReadVariableFromConfigString("COMBATSKILL_HARMTOUCH_BASE_DAMAGE", configValuesByVariableName, COMBATSKILL_HARMTOUCH_BASE_DAMAGE);
            COMBATSKILL_HARMTOUCH_DAMAGE_PER_LEVEL = ReadVariableFromConfigString("COMBATSKILL_HARMTOUCH_DAMAGE_PER_LEVEL", configValuesByVariableName, COMBATSKILL_HARMTOUCH_DAMAGE_PER_LEVEL);
            COMBATSKILL_HARMTOUCH_RANGE = ReadVariableFromConfigString("COMBATSKILL_HARMTOUCH_RANGE", configValuesByVariableName, COMBATSKILL_HARMTOUCH_RANGE);
            COMBATSKILL_LAYONHANDS_ENABLED = ReadVariableFromConfigString("COMBATSKILL_LAYONHANDS_ENABLED", configValuesByVariableName, COMBATSKILL_LAYONHANDS_ENABLED);
            COMBATSKILL_LAYONHANDS_SPELL_ID = ReadVariableFromConfigString("COMBATSKILL_LAYONHANDS_SPELL_ID", configValuesByVariableName, COMBATSKILL_LAYONHANDS_SPELL_ID);
            COMBATSKILL_LAYONHANDS_SPELL_ICON_EQ_ID = ReadVariableFromConfigString("COMBATSKILL_LAYONHANDS_SPELL_ICON_EQ_ID", configValuesByVariableName, COMBATSKILL_LAYONHANDS_SPELL_ICON_EQ_ID);
            COMBATSKILL_LAYONHANDS_CREATURE_MIN_LEVEL = ReadVariableFromConfigString("COMBATSKILL_LAYONHANDS_CREATURE_MIN_LEVEL", configValuesByVariableName, COMBATSKILL_LAYONHANDS_CREATURE_MIN_LEVEL);
            COMBATSKILL_LAYONHANDS_COOLDOWN_IN_MS = ReadVariableFromConfigString("COMBATSKILL_LAYONHANDS_COOLDOWN_IN_MS", configValuesByVariableName, COMBATSKILL_LAYONHANDS_COOLDOWN_IN_MS);
            COMBATSKILL_LAYONHANDS_HEALTH_TRIGGER_PCT = ReadVariableFromConfigString("COMBATSKILL_LAYONHANDS_HEALTH_TRIGGER_PCT", configValuesByVariableName, COMBATSKILL_LAYONHANDS_HEALTH_TRIGGER_PCT);
            COMBATSKILL_LAYONHANDS_BASE_HEAL = ReadVariableFromConfigString("COMBATSKILL_LAYONHANDS_BASE_HEAL", configValuesByVariableName, COMBATSKILL_LAYONHANDS_BASE_HEAL);
            COMBATSKILL_LAYONHANDS_HEAL_PER_LEVEL = ReadVariableFromConfigString("COMBATSKILL_LAYONHANDS_HEAL_PER_LEVEL", configValuesByVariableName, COMBATSKILL_LAYONHANDS_HEAL_PER_LEVEL);
            COMBATSKILL_FEIGNDEATH_ENABLED = ReadVariableFromConfigString("COMBATSKILL_FEIGNDEATH_ENABLED", configValuesByVariableName, COMBATSKILL_FEIGNDEATH_ENABLED);
            COMBATSKILL_FEIGNDEATH_PLAYER_LEARNABLE = ReadVariableFromConfigString("COMBATSKILL_FEIGNDEATH_PLAYER_LEARNABLE", configValuesByVariableName, COMBATSKILL_FEIGNDEATH_PLAYER_LEARNABLE);
            COMBATSKILL_FEIGNDEATH_SPELL_ID = ReadVariableFromConfigString("COMBATSKILL_FEIGNDEATH_SPELL_ID", configValuesByVariableName, COMBATSKILL_FEIGNDEATH_SPELL_ID);
            COMBATSKILL_FEIGNDEATH_SPELL_ICON_EQ_ID = ReadVariableFromConfigString("COMBATSKILL_FEIGNDEATH_SPELL_ICON_EQ_ID", configValuesByVariableName, COMBATSKILL_FEIGNDEATH_SPELL_ICON_EQ_ID);
            COMBATSKILL_FEIGNDEATH_FAIL_CHANCE_PERCENT = ReadVariableFromConfigString("COMBATSKILL_FEIGNDEATH_FAIL_CHANCE_PERCENT", configValuesByVariableName, COMBATSKILL_FEIGNDEATH_FAIL_CHANCE_PERCENT);
            COMBATSKILL_FEIGNDEATH_COOLDOWN_IN_MS = ReadVariableFromConfigString("COMBATSKILL_FEIGNDEATH_COOLDOWN_IN_MS", configValuesByVariableName, COMBATSKILL_FEIGNDEATH_COOLDOWN_IN_MS);
            FISHING_SKILL_CONVERSION_MOD_60 = ReadVariableFromConfigString("FISHING_SKILL_CONVERSION_MOD_60", configValuesByVariableName, FISHING_SKILL_CONVERSION_MOD_60);
            FISHING_SKILL_CONVERSION_MOD_80 = ReadVariableFromConfigString("FISHING_SKILL_CONVERSION_MOD_80", configValuesByVariableName, FISHING_SKILL_CONVERSION_MOD_80);
            FORAGE_SPELL_ICON_EQ_ID = ReadVariableFromConfigString("FORAGE_SPELL_ICON_EQ_ID", configValuesByVariableName, FORAGE_SPELL_ICON_EQ_ID);
            FORAGE_SPELL_TEMPLATE_ID = ReadVariableFromConfigString("FORAGE_SPELL_TEMPLATE_ID", configValuesByVariableName, FORAGE_SPELL_TEMPLATE_ID);
            TRADESKILLS_CONVERSION_MOD_60 = ReadVariableFromConfigString("TRADESKILLS_CONVERSION_MOD_60", configValuesByVariableName, TRADESKILLS_CONVERSION_MOD_60);
            TRADESKILLS_CONVERSION_MOD_80 = ReadVariableFromConfigString("TRADESKILLS_CONVERSION_MOD_80", configValuesByVariableName, TRADESKILLS_CONVERSION_MOD_80);
            TRADESKILLS_SKILL_TIER_DISTANCE_LOW = ReadVariableFromConfigString("TRADESKILLS_SKILL_TIER_DISTANCE_LOW", configValuesByVariableName, TRADESKILLS_SKILL_TIER_DISTANCE_LOW);
            TRADESKILLS_SKILL_TIER_DISTANCE_HIGH = ReadVariableFromConfigString("TRADESKILLS_SKILL_TIER_DISTANCE_HIGH", configValuesByVariableName, TRADESKILLS_SKILL_TIER_DISTANCE_HIGH);
            TRADESKILL_LEARN_COST_AT_1 = ReadVariableFromConfigString("TRADESKILL_LEARN_COST_AT_1", configValuesByVariableName, TRADESKILL_LEARN_COST_AT_1);
            TRADESKILL_LEARN_COST_AT_50 = ReadVariableFromConfigString("TRADESKILL_LEARN_COST_AT_50", configValuesByVariableName, TRADESKILL_LEARN_COST_AT_50);
            TRADESKILL_LEARN_COST_AT_100 = ReadVariableFromConfigString("TRADESKILL_LEARN_COST_AT_100", configValuesByVariableName, TRADESKILL_LEARN_COST_AT_100);
            TRADESKILL_LEARN_COST_AT_200 = ReadVariableFromConfigString("TRADESKILL_LEARN_COST_AT_200", configValuesByVariableName, TRADESKILL_LEARN_COST_AT_200);
            TRADESKILL_LEARN_COST_AT_300 = ReadVariableFromConfigString("TRADESKILL_LEARN_COST_AT_300", configValuesByVariableName, TRADESKILL_LEARN_COST_AT_300);
            TRADESKILL_LEARN_COST_AT_450 = ReadVariableFromConfigString("TRADESKILL_LEARN_COST_AT_450", configValuesByVariableName, TRADESKILL_LEARN_COST_AT_450);
            TRADESKILL_CAST_TIME_IN_MS = ReadVariableFromConfigString("TRADESKILL_CAST_TIME_IN_MS", configValuesByVariableName, TRADESKILL_CAST_TIME_IN_MS);
            TRADESKILL_TOTEM_CATEGORY_START = ReadVariableFromConfigString("TRADESKILL_TOTEM_CATEGORY_START", configValuesByVariableName, TRADESKILL_TOTEM_CATEGORY_START);
            TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING = ReadVariableFromConfigString("TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING", configValuesByVariableName, TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING);
            TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX = ReadVariableFromConfigString("TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX", configValuesByVariableName, TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX);
            TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING = ReadVariableFromConfigString("TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING", configValuesByVariableName, TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING);
            TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY = ReadVariableFromConfigString("TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY", configValuesByVariableName, TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY);
            TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING = ReadVariableFromConfigString("TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING", configValuesByVariableName, TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING);

            TRANSPORT_PAUSE_MULTIPLIER = ReadVariableFromConfigString("TRANSPORT_PAUSE_MULTIPLIER", configValuesByVariableName, TRANSPORT_PAUSE_MULTIPLIER);
            TRANSPORT_MOVE_SPEED = ReadVariableFromConfigString("TRANSPORT_MOVE_SPEED", configValuesByVariableName, TRANSPORT_MOVE_SPEED);
            TRANSPORT_ACCELERATION = ReadVariableFromConfigString("TRANSPORT_ACCELERATION", configValuesByVariableName, TRANSPORT_ACCELERATION);
            TRANSPORT_ALLOW_FIXED_SPEEDS = ReadVariableFromConfigString("TRANSPORT_ALLOW_FIXED_SPEEDS", configValuesByVariableName, TRANSPORT_ALLOW_FIXED_SPEEDS);

            DBCID_AREATABLE_AREABIT_BLOCK_1_START = ReadVariableFromConfigString("DBCID_AREATABLE_AREABIT_BLOCK_1_START", configValuesByVariableName, DBCID_AREATABLE_AREABIT_BLOCK_1_START);
            DBCID_AREATABLE_AREABIT_BLOCK_1_END = ReadVariableFromConfigString("DBCID_AREATABLE_AREABIT_BLOCK_1_END", configValuesByVariableName, DBCID_AREATABLE_AREABIT_BLOCK_1_END);
            DBCID_AREATABLE_AREABIT_BLOCK_2_START = ReadVariableFromConfigString("DBCID_AREATABLE_AREABIT_BLOCK_2_START", configValuesByVariableName, DBCID_AREATABLE_AREABIT_BLOCK_2_START);
            DBCID_AREATABLE_AREABIT_BLOCK_2_END = ReadVariableFromConfigString("DBCID_AREATABLE_AREABIT_BLOCK_2_END", configValuesByVariableName, DBCID_AREATABLE_AREABIT_BLOCK_2_END);
            DBCID_AREATABLE_AREABIT_BLOCK_3_START = ReadVariableFromConfigString("DBCID_AREATABLE_AREABIT_BLOCK_3_START", configValuesByVariableName, DBCID_AREATABLE_AREABIT_BLOCK_3_START);
            DBCID_AREATABLE_AREABIT_BLOCK_3_END = ReadVariableFromConfigString("DBCID_AREATABLE_AREABIT_BLOCK_3_END", configValuesByVariableName, DBCID_AREATABLE_AREABIT_BLOCK_3_END);
            DBCID_AREATABLE_ID_START = ReadVariableFromConfigString("DBCID_AREATABLE_ID_START", configValuesByVariableName, DBCID_AREATABLE_ID_START);
            DBCID_AREATABLE_ID_END = ReadVariableFromConfigString("DBCID_AREATABLE_ID_END", configValuesByVariableName, DBCID_AREATABLE_ID_END);
            DBCID_AREATRIGGER_ID_START = ReadVariableFromConfigString("DBCID_AREATRIGGER_ID_START", configValuesByVariableName, DBCID_AREATRIGGER_ID_START);
            DBCID_AREATRIGGER_ID_END = ReadVariableFromConfigString("DBCID_AREATRIGGER_ID_END", configValuesByVariableName, DBCID_AREATRIGGER_ID_END);
            DBCID_CREATUREDISPLAYINFO_ID_START = ReadVariableFromConfigString("DBCID_CREATUREDISPLAYINFO_ID_START", configValuesByVariableName, DBCID_CREATUREDISPLAYINFO_ID_START);
            DBCID_CREATUREDISPLAYINFO_ID_END = ReadVariableFromConfigString("DBCID_CREATUREDISPLAYINFO_ID_END", configValuesByVariableName, DBCID_CREATUREDISPLAYINFO_ID_END);
            DBCID_CREATUREDISPLAYINFOEXTRA_ID_START = ReadVariableFromConfigString("DBCID_CREATUREDISPLAYINFOEXTRA_ID_START", configValuesByVariableName, DBCID_CREATUREDISPLAYINFOEXTRA_ID_START);
            DBCID_CREATUREDISPLAYINFOEXTRA_ID_END = ReadVariableFromConfigString("DBCID_CREATUREDISPLAYINFOEXTRA_ID_END", configValuesByVariableName, DBCID_CREATUREDISPLAYINFOEXTRA_ID_END);
            DBCID_CREATUREMODELDATA_ID_START = ReadVariableFromConfigString("DBCID_CREATUREMODELDATA_ID_START", configValuesByVariableName, DBCID_CREATUREMODELDATA_ID_START);
            DBCID_CREATUREMODELDATA_ID_END = ReadVariableFromConfigString("DBCID_CREATUREMODELDATA_ID_END", configValuesByVariableName, DBCID_CREATUREMODELDATA_ID_END);
            DBCID_CREATURESOUNDDATA_ID_START = ReadVariableFromConfigString("DBCID_CREATURESOUNDDATA_ID_START", configValuesByVariableName, DBCID_CREATURESOUNDDATA_ID_START);
            DBCID_FOOTSTEPTERRAINLOOKUP_ID_START = ReadVariableFromConfigString("DBCID_FOOTSTEPTERRAINLOOKUP_ID_START", configValuesByVariableName, DBCID_FOOTSTEPTERRAINLOOKUP_ID_START);
            DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START = ReadVariableFromConfigString("DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START", configValuesByVariableName, DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START);
            DBCID_GAMEOBJECTDISPLAYINFO_ID_START = ReadVariableFromConfigString("DBCID_GAMEOBJECTDISPLAYINFO_ID_START", configValuesByVariableName, DBCID_GAMEOBJECTDISPLAYINFO_ID_START);
            DBCID_LFGDUNGEONGROUP_DUNGEONS_ID = ReadVariableFromConfigString("DBCID_LFGDUNGEONGROUP_DUNGEONS_ID", configValuesByVariableName, DBCID_LFGDUNGEONGROUP_DUNGEONS_ID);
            DBCID_LFGDUNGEONGROUP_DUNGEONS_ORDER_ID = ReadVariableFromConfigString("DBCID_LFGDUNGEONGROUP_DUNGEONS_ORDER_ID", configValuesByVariableName, DBCID_LFGDUNGEONGROUP_DUNGEONS_ORDER_ID);
            DBCID_LFGDUNGEONGROUP_RAIDS_ID = ReadVariableFromConfigString("DBCID_LFGDUNGEONGROUP_RAIDS_ID", configValuesByVariableName, DBCID_LFGDUNGEONGROUP_RAIDS_ID);
            DBCID_LFGDUNGEONGROUP_RAIDS_ORDER_ID = ReadVariableFromConfigString("DBCID_LFGDUNGEONGROUP_RAIDS_ORDER_ID", configValuesByVariableName, DBCID_LFGDUNGEONGROUP_RAIDS_ORDER_ID);
            DBCID_LFGDUNGEONS_ID_START = ReadVariableFromConfigString("DBCID_LFGDUNGEONS_ID_START", configValuesByVariableName, DBCID_LFGDUNGEONS_ID_START);
            DBCID_ITEMDISPLAYINFO_START = ReadVariableFromConfigString("DBCID_ITEMDISPLAYINFO_START", configValuesByVariableName, DBCID_ITEMDISPLAYINFO_START);
            DBCID_LIGHT_ID_START = ReadVariableFromConfigString("DBCID_LIGHT_ID_START", configValuesByVariableName, DBCID_LIGHT_ID_START);
            DBCID_LIGHTPARAMS_ID_START = ReadVariableFromConfigString("DBCID_LIGHTPARAMS_ID_START", configValuesByVariableName, DBCID_LIGHTPARAMS_ID_START);
            DBCID_LOADINGSCREEN_ID_START = ReadVariableFromConfigString("DBCID_LOADINGSCREEN_ID_START", configValuesByVariableName, DBCID_LOADINGSCREEN_ID_START);
            DBCID_MAP_ID_START = ReadVariableFromConfigString("DBCID_MAP_ID_START", configValuesByVariableName, DBCID_MAP_ID_START);
            DBCID_MAP_ID_END = ReadVariableFromConfigString("DBCID_MAP_ID_END", configValuesByVariableName, DBCID_MAP_ID_END);
            DBCID_SKILLLINE_ID_START = ReadVariableFromConfigString("DBCID_SKILLLINE_ID_START", configValuesByVariableName, DBCID_SKILLLINE_ID_START);
            DBCID_SKILLLINEABILITY_ID_START = ReadVariableFromConfigString("DBCID_SKILLLINEABILITY_ID_START", configValuesByVariableName, DBCID_SKILLLINEABILITY_ID_START);
            DBCID_SKILLRACECLASSINFO_ID_START = ReadVariableFromConfigString("DBCID_SKILLRACECLASSINFO_ID_START", configValuesByVariableName, DBCID_SKILLRACECLASSINFO_ID_START);
            DBCID_SOUNDENTRIES_ID_START = ReadVariableFromConfigString("DBCID_SOUNDENTRIES_ID_START", configValuesByVariableName, DBCID_SOUNDENTRIES_ID_START);
            DBCID_SOUNDAMBIENCE_ID_START = ReadVariableFromConfigString("DBCID_SOUNDAMBIENCE_ID_START", configValuesByVariableName, DBCID_SOUNDAMBIENCE_ID_START);
            DBCID_SPELL_ID_START = ReadVariableFromConfigString("DBCID_SPELL_ID_START", configValuesByVariableName, DBCID_SPELL_ID_START);
            DBCID_SPELL_ID_GENERATED_START = ReadVariableFromConfigString("DBCID_SPELL_ID_GENERATED_START", configValuesByVariableName, DBCID_SPELL_ID_GENERATED_START);
            DBCID_SPELL_ID_END = ReadVariableFromConfigString("DBCID_SPELL_ID_END", configValuesByVariableName, DBCID_SPELL_ID_END);
            DBCID_SPELLCASTTIME_ID_START = ReadVariableFromConfigString("DBCID_SPELLCASTTIME_ID_START", configValuesByVariableName, DBCID_SPELLCASTTIME_ID_START);
            DBCID_SPELLCATEGORY_ID_START = ReadVariableFromConfigString("DBCID_SPELLCATEGORY_ID_START", configValuesByVariableName, DBCID_SPELLCATEGORY_ID_START);
            DBCID_SPELLDURATION_AURA_ID = ReadVariableFromConfigString("DBCID_SPELLDURATION_AURA_ID", configValuesByVariableName, DBCID_SPELLDURATION_AURA_ID);
            DBCID_SPELLITEMENCHANTMENT_ID_START = ReadVariableFromConfigString("DBCID_SPELLITEMENCHANTMENT_ID_START", configValuesByVariableName, DBCID_SPELLITEMENCHANTMENT_ID_START);
            DBCID_SPELLICON_ID_START = ReadVariableFromConfigString("DBCID_SPELLICON_ID_START", configValuesByVariableName, DBCID_SPELLICON_ID_START);
            DBCID_SPELLRADIUS_ID_START = ReadVariableFromConfigString("DBCID_SPELLRADIUS_ID_START", configValuesByVariableName, DBCID_SPELLRADIUS_ID_START);
            DBCID_SPELLRANGE_ID_START = ReadVariableFromConfigString("DBCID_SPELLRANGE_ID_START", configValuesByVariableName, DBCID_SPELLRANGE_ID_START);
            DBCID_SPELLVISUAL_ID_START = ReadVariableFromConfigString("DBCID_SPELLVISUAL_ID_START", configValuesByVariableName, DBCID_SPELLVISUAL_ID_START);
            DBCID_SPELLVISUALKIT_ID_START = ReadVariableFromConfigString("DBCID_SPELLVISUALKIT_ID_START", configValuesByVariableName, DBCID_SPELLVISUALKIT_ID_START);
            DBCID_SPELLVISUALEFFECTNAME_ID_START = ReadVariableFromConfigString("DBCID_SPELLVISUALEFFECTNAME_ID_START", configValuesByVariableName, DBCID_SPELLVISUALEFFECTNAME_ID_START);
            DBCID_SUMMONPROPERTIES_ID_START = ReadVariableFromConfigString("DBCID_SUMMONPROPERTIES_ID_START", configValuesByVariableName, DBCID_SUMMONPROPERTIES_ID_START);
            DBCID_TAXIPATH_ID_START = ReadVariableFromConfigString("DBCID_TAXIPATH_ID_START", configValuesByVariableName, DBCID_TAXIPATH_ID_START);
            DBCID_TAXIPATHNODE_ID_START = ReadVariableFromConfigString("DBCID_TAXIPATHNODE_ID_START", configValuesByVariableName, DBCID_TAXIPATHNODE_ID_START);
            DBCID_TOTEMCATEGORY_ID_START = ReadVariableFromConfigString("DBCID_TOTEMCATEGORY_ID_START", configValuesByVariableName, DBCID_TOTEMCATEGORY_ID_START);
            DBCID_TRANSPORTANIMATION_ID_START = ReadVariableFromConfigString("DBCID_TRANSPORTANIMATION_ID_START", configValuesByVariableName, DBCID_TRANSPORTANIMATION_ID_START);
            DBCID_WORLDSAFELOCS_ID_START = ReadVariableFromConfigString("DBCID_WORLDSAFELOCS_ID_START", configValuesByVariableName, DBCID_WORLDSAFELOCS_ID_START);
            DBCID_WORLDSAFELOCS_ID_END = ReadVariableFromConfigString("DBCID_WORLDSAFELOCS_ID_END", configValuesByVariableName, DBCID_WORLDSAFELOCS_ID_END);
            DBCID_WMOAREATABLE_ID_START = ReadVariableFromConfigString("DBCID_WMOAREATABLE_ID_START", configValuesByVariableName, DBCID_WMOAREATABLE_ID_START);
            DBCID_WMOAREATABLE_WMOGROUPID_START = ReadVariableFromConfigString("DBCID_WMOAREATABLE_WMOGROUPID_START", configValuesByVariableName, DBCID_WMOAREATABLE_WMOGROUPID_START);
            DBCID_ZONEMUSIC_START = ReadVariableFromConfigString("DBCID_ZONEMUSIC_START", configValuesByVariableName, DBCID_ZONEMUSIC_START);

            SQL_BROADCASTTEXT_ID_START = ReadVariableFromConfigString("SQL_BROADCASTTEXT_ID_START", configValuesByVariableName, SQL_BROADCASTTEXT_ID_START);
            SQL_BROADCASTTEXT_ID_END = ReadVariableFromConfigString("SQL_BROADCASTTEXT_ID_END", configValuesByVariableName, SQL_BROADCASTTEXT_ID_END);
            SQL_CREATURE_GUID_LOW = ReadVariableFromConfigString("SQL_CREATURE_GUID_LOW", configValuesByVariableName, SQL_CREATURE_GUID_LOW);
            SQL_CREATURE_GUID_HIGH = ReadVariableFromConfigString("SQL_CREATURE_GUID_HIGH", configValuesByVariableName, SQL_CREATURE_GUID_HIGH);
            SQL_CREATUREIMMUNITIES_ID_START = ReadVariableFromConfigString("SQL_CREATURE_IMMUNITIES_ID_START", configValuesByVariableName, SQL_CREATUREIMMUNITIES_ID_START);
            SQL_CREATUREIMMUNITIES_ID_END = ReadVariableFromConfigString("SQL_CREATURE_IMMUNITIES_ID_END", configValuesByVariableName, SQL_CREATUREIMMUNITIES_ID_END);
            SQL_CREATURETEMPLATE_ENTRY_LOW = ReadVariableFromConfigString("SQL_CREATURETEMPLATE_ENTRY_LOW", configValuesByVariableName, SQL_CREATURETEMPLATE_ENTRY_LOW);
            SQL_CREATURETEMPLATE_ENTRY_HIGH = ReadVariableFromConfigString("SQL_CREATURETEMPLATE_ENTRY_HIGH", configValuesByVariableName, SQL_CREATURETEMPLATE_ENTRY_HIGH);
            SQL_CREATURETEMPLATE_GENERATED_START_ID = ReadVariableFromConfigString("SQL_CREATURETEMPLATE_GENERATED_START_ID", configValuesByVariableName, SQL_CREATURETEMPLATE_GENERATED_START_ID);
            SQL_GAMEOBJECT_GUID_ID_START = ReadVariableFromConfigString("SQL_GAMEOBJECT_GUID_ID_START", configValuesByVariableName, SQL_GAMEOBJECT_GUID_ID_START);
            SQL_GAMEOBJECT_GUID_ID_END = ReadVariableFromConfigString("SQL_GAMEOBJECT_GUID_ID_END", configValuesByVariableName, SQL_GAMEOBJECT_GUID_ID_END);
            SQL_GAME_EVENTS_ID_START = ReadVariableFromConfigString("SQL_GAME_EVENTS_ID_START", configValuesByVariableName, SQL_GAME_EVENTS_ID_START);
            SQL_GAME_EVENTS_ID_END = ReadVariableFromConfigString("SQL_GAME_EVENTS_ID_END", configValuesByVariableName, SQL_GAME_EVENTS_ID_END);
            SQL_GAMEOBJECTTEMPLATE_ID_START = ReadVariableFromConfigString("SQL_GAMEOBJECTTEMPLATE_ID_START", configValuesByVariableName, SQL_GAMEOBJECTTEMPLATE_ID_START);
            SQL_GAMEOBJECTTEMPLATE_ID_END = ReadVariableFromConfigString("SQL_GAMEOBJECTTEMPLATE_ID_END", configValuesByVariableName, SQL_GAMEOBJECTTEMPLATE_ID_END);
            SQL_GAMEOBJECTTEMPLATE_SHIP_ID_START = ReadVariableFromConfigString("SQL_GAMEOBJECTTEMPLATE_SHIP_ID_START", configValuesByVariableName, SQL_GAMEOBJECTTEMPLATE_SHIP_ID_START);
            SQL_GAMEOBJECTTEMPLATE_SHIP_ID_END = ReadVariableFromConfigString("SQL_GAMEOBJECTTEMPLATE_SHIP_ID_END", configValuesByVariableName, SQL_GAMEOBJECTTEMPLATE_SHIP_ID_END);
            SQL_GAMETELE_ROWID_START = ReadVariableFromConfigString("SQL_GAMETELE_ROWID_START", configValuesByVariableName, SQL_GAMETELE_ROWID_START);
            SQL_GAMETELE_ROWID_END = ReadVariableFromConfigString("SQL_GAMETELE_ROWID_END", configValuesByVariableName, SQL_GAMETELE_ROWID_END);
            SQL_GOSSIPMENU_MENUID_START = ReadVariableFromConfigString("SQL_GOSSIPMENU_MENUID_START", configValuesByVariableName, SQL_GOSSIPMENU_MENUID_START);
            SQL_GOSSIPMENU_MENUID_END = ReadVariableFromConfigString("SQL_GOSSIPMENU_MENUID_END", configValuesByVariableName, SQL_GOSSIPMENU_MENUID_END);
            SQL_ITEM_TEMPLATE_ENTRY_START = ReadVariableFromConfigString("SQL_ITEM_TEMPLATE_ENTRY_START", configValuesByVariableName, SQL_ITEM_TEMPLATE_ENTRY_START);
            SQL_ITEM_TEMPLATE_ENTRY_END = ReadVariableFromConfigString("SQL_ITEM_TEMPLATE_ENTRY_END", configValuesByVariableName, SQL_ITEM_TEMPLATE_ENTRY_END);
            SQL_NPCTEXT_ID_START = ReadVariableFromConfigString("SQL_NPCTEXT_ID_START", configValuesByVariableName, SQL_NPCTEXT_ID_START);
            SQL_NPCTEXT_ID_END = ReadVariableFromConfigString("SQL_NPCTEXT_ID_END", configValuesByVariableName, SQL_NPCTEXT_ID_END);
            SQL_PAGETEXT_ID_START = ReadVariableFromConfigString("SQL_PAGETEXT_ID_START", configValuesByVariableName, SQL_PAGETEXT_ID_START);
            SQL_PAGETEXT_ID_END = ReadVariableFromConfigString("SQL_PAGETEXT_ID_END", configValuesByVariableName, SQL_PAGETEXT_ID_END);
            SQL_PETNAMEGENERATION_ID_START = ReadVariableFromConfigString("SQL_PETNAMEGENERATION_ID_START", configValuesByVariableName, SQL_PETNAMEGENERATION_ID_START);
            SQL_POOL_TEMPLATE_ID_START = ReadVariableFromConfigString("SQL_POOL_TEMPLATE_ID_START", configValuesByVariableName, SQL_POOL_TEMPLATE_ID_START);
            SQL_POOL_TEMPLATE_ID_END = ReadVariableFromConfigString("SQL_POOL_TEMPLATE_ID_END", configValuesByVariableName, SQL_POOL_TEMPLATE_ID_END);
            SQL_QUEST_TEMPLATE_ID_START = ReadVariableFromConfigString("SQL_QUEST_TEMPLATE_ID_START", configValuesByVariableName, SQL_QUEST_TEMPLATE_ID_START);
            SQL_QUEST_TEMPLATE_ID_END = ReadVariableFromConfigString("SQL_QUEST_TEMPLATE_ID_END", configValuesByVariableName, SQL_QUEST_TEMPLATE_ID_END);
            SQL_QUEST_TEMPLATE_ID_REPEATABLE_SHIFT = ReadVariableFromConfigString("SQL_QUEST_TEMPLATE_ID_REPEATABLE_SHIFT", configValuesByVariableName, SQL_QUEST_TEMPLATE_ID_REPEATABLE_SHIFT);
            SQL_REFERENCE_LOOT_TEMPLATE_ID_START = ReadVariableFromConfigString("SQL_REFERENCE_LOOT_TEMPLATE_ID_START", configValuesByVariableName, SQL_REFERENCE_LOOT_TEMPLATE_ID_START);
            SQL_REFERENCE_LOOT_TEMPLATE_ID_END = ReadVariableFromConfigString("SQL_REFERENCE_LOOT_TEMPLATE_ID_END", configValuesByVariableName, SQL_REFERENCE_LOOT_TEMPLATE_ID_END);
            SQL_SPELL_GROUP_ID_START = ReadVariableFromConfigString("SQL_SPELL_GROUP_ID_START", configValuesByVariableName, SQL_SPELL_GROUP_ID_START);
            SQL_SPELL_GROUP_ID_FOR_BARD_AURA_START = ReadVariableFromConfigString("SQL_SPELL_GROUP_ID_FOR_BARD_AURA_START", configValuesByVariableName, SQL_SPELL_GROUP_ID_FOR_BARD_AURA_START);
            SQL_SPELL_GROUP_ID_END = ReadVariableFromConfigString("SQL_SPELL_GROUP_ID_END", configValuesByVariableName, SQL_SPELL_GROUP_ID_END);
            SQL_TRAINER_ID_START = ReadVariableFromConfigString("SQL_TRAINER_ID_START", configValuesByVariableName, SQL_TRAINER_ID_START);
            SQL_TRAINER_ID_END = ReadVariableFromConfigString("SQL_TRAINER_ID_END", configValuesByVariableName, SQL_TRAINER_ID_END);
            SQL_TRANSPORTS_GUID_START = ReadVariableFromConfigString("SQL_TRANSPORTS_GUID_START", configValuesByVariableName, SQL_TRANSPORTS_GUID_START);
            SQL_TRANSPORTS_GUID_END = ReadVariableFromConfigString("SQL_TRANSPORTS_GUID_END", configValuesByVariableName, SQL_TRANSPORTS_GUID_END);
        }
    }
}
