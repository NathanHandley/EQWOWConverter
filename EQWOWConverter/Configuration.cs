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
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // \/ Make sure you set the configuration values below this line \/ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // ====================================================================
        // Paths and Files
        // ====================================================================
        // Location of the installed everquest trilogy client (this must have the eqgame.exe file in it)
        public static readonly string PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER = "E:\\Development\\EQWOW-Reference\\EverQuestTrilogy";

        // Location of the installed enUS version of World of Warcaft client (this must have the wow.exe in it)
        public static readonly string PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER = "E:\\Development\\azerothcore-wotlk\\Client\\";

        // The root of the tools directory (comes with this source code in a folder)
        public static readonly string PATH_TOOLS_FOLDER = "E:\\Development\\EQWOW\\Tools";

        // The root of the assets directory (comes with this source code in a folder)
        public static readonly string PATH_ASSETS_FOLDER = "E:\\Development\\EQWOW\\Assets";

        // The root folder where temporary folders and file will be generated (ensure at least 10GB of space is available in this location)
        public static readonly string PATH_WORKING_FOLDER = "E:\\Development\\EQWOW-Reference\\Working\\Assets";

        // ID to append to the end of the /Data/ patch file (such as the "4" in "patch-4.mpq). Make it uniquely new.
        public static readonly string PATCH_CLIENT_DATA_ID = "4";

        // ID to append to the localized patch file in /Data/<locale> (such as the "5" in patch-enUS-5.mpq). Make it uniquely new.
        public static readonly string PATCH_CLIENT_DATA_LOC_ID = "5";

        // What language to generate things as
        public static readonly string PATCH_LOCALIZATION_STRING = "enUS";

        // ====================================================================
        // Deployment Rules
        // ====================================================================
        // If true, deploy the client file (patch mpq) after building it
        public static readonly bool DEPLOY_CLIENT_FILES = true;

        // If true and when deploying client files, clear the cache (only relevant if you set DEPLOY_CLIENT_FILES to true, otherwise ignored)
        public static readonly bool DEPLOY_CLEAR_CACHE_ON_CLIENT_DEPLOY = true;

        // If true, deploy to the server files/data after building
        public static readonly bool DEPLOY_SERVER_FILES = true;

        // Location of where the server DBC files would be deployed to (only relevant if you set DEPLOY_SERVER_FILES to true, otherwise ignored)
        public static readonly string DEPLOY_SERVER_DBC_FOLDER_LOCATION = "E:\\Development\\azerothcore-wotlk\\Build\\bin\\RelWithDebInfo\\data\\dbc";

        // If true, deploy to the SQL to the server
        // Note: May not work on remote servers (not tested)
        public static readonly bool DEPLOY_SERVER_SQL = true;

        // If deploying to SQL, you need to set these to something real that points to your databases (only relevant if you set DEPLOY_SERVER_SQL to true, otherwise ignored)
        public static readonly string DEPLOY_SQL_CONNECTION_STRING_CHARACTERS = "Server=127.0.0.1;Database=acore_characters;Uid=root;Pwd=rootpass;";
        public static readonly string DEPLOY_SQL_CONNECTION_STRING_WORLD = "Server=127.0.0.1;Database=acore_world;Uid=root;Pwd=rootpass;";
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // /\ Make sure you set the configuration values above this line /\ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // ====================================================================
        // Core
        // ====================================================================
        // This is the version that the mod-everquest AzerothCore module needs to be compatible with
        public static readonly int CORE_MOD_VERSION = 2;
        
        // Plays a beep sound when the generate completes if set to true
        public static readonly bool CORE_CONSOLE_BEEP_ON_COMPLETE = true;

        // If true, the conditioner & generator will run in multithreading mode
        public static readonly bool CORE_ENABLE_MULTITHREADING = true;
        public static readonly int CORE_ZONEGEN_THREAD_COUNT = 4;
        public static readonly int CORE_PNGTOBLPCONVERSION_THREAD_COUNT = 20;

        // ====================================================================
        // Logging
        // ====================================================================
        // Level of logs to write to the console and log file.
        // 1 = Error, 2 = Info, 3 = Debug
        public static readonly int LOGGING_CONSOLE_MIN_LEVEL = 2;
        public static readonly int LOGGING_FILE_MIN_LEVEL = 3;

        // ====================================================================
        // Generator Rules
        // ====================================================================
        // The value EQ vertices multiply by when translated into WOW vertices
        // A WORLD_SCALE value of 0.25 seems to be 1:1 with EQ.  0.28 allows humans and 0.4 allows taurens to enter rivervale bank door
        public static readonly float GENERATE_WORLD_SCALE = 0.29f;
        public static readonly float GENERATE_CREATURE_SCALE = 0.255f;
        public static readonly float GENERATE_EQUIPMENT_PLAYER_SCALE = 0.35f; // The size of equipment on players
        public static readonly float GENERATE_EQUIPMENT_CREATURE_SCALE = 0.255f; // The size of equipment on creatures/npcs

        // Identifier for what subset of expansion data to work with.  0 = Classic, 1 = Kunark, 2 = Velious
        public static readonly int GENERATE_EQ_EXPANSION_ID_GENERAL = 2; // Not advisable to set this lower than 2
        public static readonly int GENERATE_EQ_EXPANSION_ID_ZONES = 1; // Zone lines and water volumes will not work in most Kunark and Velious zones (currently)
        public static readonly int GENERATE_EQ_EXPANSION_ID_TRANSPORTS = 1;
        public static readonly int GENERATE_EQ_EXPANSION_ID_TRADESKILLS = 1;
        public static readonly int GENERATE_EQ_EXPANSION_ID_EQUIPMENT_GRAPHICS = 1;

        // If true, DBC files are extracted every time.
        public static readonly bool GENERATE_EXTRACT_DBC_FILES = true;

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

        // If true, generate and copy maps / minimaps
        public static readonly bool GENERATE_WORLDMAPS = true;

        // If this has any zone short names in it, the ouput of the generator will perform an update only for these zones. If there is no previously
        // built patch mpq, it will be forced to do a complete build first.  Note that if any zones are entered in here, ONLY those zones
        // will load and work properly
        public static readonly List<string> GENERATE_ONLY_LISTED_ZONE_SHORTNAMES = new List<string>() {  };

        // An extra amount to add to the boundary boxes when generating wow assets from EQ.  Needed to handle rounding.
        public static readonly float GENERATE_ADDED_BOUNDARY_AMOUNT = 0.01f;

        // How many insert rows to restrict in a SQL output file
        public static readonly int GENERATE_SQL_FILE_BATCH_SIZE = 50000;
        public static readonly int GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE = 5000;

        // How many file names to batch up when converting (must be >= 1)
        public static readonly int GENERATE_BLPCONVERTBATCHSIZE = 50;

        // What edge buffer to add when doing floating point month
        public static readonly float GENERATE_FLOAT_EPSILON = 0.001f;

        // If true, SQL files will be generated in a way where they will have a unique ID to force an update if ran by azerothcore, regardless of changes
        public static readonly bool GENERATE_FORCE_SQL_UPDATES = true;

        // The minimum size that boundary boxes should be for any object models when output
        public static readonly float GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE = 25.1f;

        //=====================================================================
        // Player
        //=====================================================================
        // If true, new players created will use the everquest start locations defined in PlayerClassRaceProperties
        // WARNING: This will DELETE the existing start locations in WoW zones, so be certain you want this enabled before deploying
        public static readonly bool PLAYER_USE_EQ_START_LOCATION = true;

        // If true, players will start with an EQ item loadout instead of a WOW item loadout
        public static readonly bool PLAYER_USE_EQ_START_ITEMS = true;

        // If true, this will also add a hearthstone if using EQ items
        public static readonly bool PLAYER_ADD_HEARTHSTONE_IF_USE_EQ_START_ITEMS = false;

        // If true, players start with a bind and gate spell regardless of class (with no costs)
        public static readonly bool PLAYER_ADD_CUSTOM_BIND_AND_GATE_ON_START = true;

        // These properties are for replacing the collision for many race models that otherwise wouldn't fit in most doorways (bigger than human male)
        // NOTE: This WILL change the camera-center value for any reduced models.
        public static readonly bool PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED = true;
        // Default value here is max that allows all but Halfling doors to be entered by all, which seems to be just above Night Elf Female
        public static readonly float PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX = 2.275f;

        //=====================================================================
        // Zone General
        //=====================================================================
        // If this is set to false, any static graphics (like dirt, etc) are not rendered.  Only set to false for debugging
        public static readonly bool ZONE_SHOW_STATIC_GEOMETRY = true;

        // Maximum number of faces that fit into a render WMO group before it subdivides (max is due to various variable limits)
        public static readonly int ZONE_MAX_FACES_PER_WMOGROUP = 21840;

        // Maximum size of any zone-to-material-object creation along the X and Y axis
        public static readonly float ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE = 325.0f;

        // If true, some zones (like gfay and halas) will hide sunlight with a shadowbox
        public static readonly bool ZONE_ALLOW_SUN_HIDING_WITH_SHADOWBOX_ENABLED = true;

        // How much bigger to make the box which causes the shadow in a shadowbox
        public static readonly float ZONE_SHADOW_BOX_ADDED_SIZE = 50f;

        // If true, allow collision with world model objects. This will also impact music and ambient sounds, since they align to areas which require collision detection
        public static readonly bool ZONE_COLLISION_ENABLED = true;

        // If true, this makes all visable geometry collidable. Should only be used for development/debugging purposes (like water coordinates).
        public static readonly bool ZONE_ENABLE_COLLISION_ON_ALL_ZONE_RENDER_MATERIALS = false;

        // When zone geometry gets broken into cuboids, this is the max side length of the area
        public static readonly int ZONE_SPLIT_COLLISION_CUBOID_MAX_EDGE_LEGNTH = 1200;

        // If set to 'true', show a box where the music zones are. This is for debugging only. Only works when collision is enabled
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

        // If true, enable weather in zones.  Note that it currently doesn't work properly.
        public static readonly bool ZONE_WEATHER_ENABLED = true;

        // If true, characters can fly in the zones if they have a mount
        public static readonly bool ZONE_FLYING_ALLOWED = true;

        //=====================================================================
        // World Maps (and Minimaps)
        //=====================================================================
        // When true, many various proprties are changed to support generation of minimaps, such as 'baking' in animated textures
        // Leave this false unless generating maps in order to make minimaps, otherwise you'll have terrible visuals and performance
        public static readonly bool WORLDMAP_DEBUG_GENERATION_MODE_ENABLED = false;

        // Borders on any maps that were generated, which is blank space and important to mark since coordinates in map space are calculated at generation
        public static readonly int WORLDMAP_LEFT_BORDER_PIXEL_SIZE = 2;
        public static readonly int WORLDMAP_RIGHT_BORDER_PIXEL_SIZE = 2;
        public static readonly int WORLDMAP_TOP_BORDER_PIXEL_SIZE = 2;
        public static readonly int WORLDMAP_BOTTOM_BORDER_PIXEL_SIZE = 2;

        // Controls showing suggested levels on the linked maps
        public static readonly bool WORLDMAP_SHOW_SUGGESTED_LEVELS_ON_LINKED_MAPS = true;

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

        // Storm brightness
        public static readonly float LIGHT_STORM_COLOR_MOD = 0.8f;

        //=====================================================================
        // Audio
        //=====================================================================       
        // Set which soundfont to use in the Tools/soundfont folder.  Alternate option is synthusr_samplefix.sf2
        public static readonly string AUDIO_SOUNDFONT_FILE_NAME = "AweROMGM.sf2";

        // If set to true, some audio tracks are swapped vs the original tracks.  Make it false if you want a more classic-like experience
        public static readonly bool AUDIO_USE_ALTERNATE_TRACKS = true;

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

        // Volume of spells and other effects
        public static readonly float AUDIO_SPELL_SOUND_VOLUME = 0.3f;

        //=====================================================================
        // Objects
        //=====================================================================
        // For ladders, this is how far to extend out the steppable area in front and back of it (percentage of thickness)
        public static readonly float OBJECT_STATIC_LADDER_EXTEND_DISTANCE = 0.29f;

        // How much space between each step of a ladder along the Z axis (true units)
        public static readonly float OBJECT_STATIC_LADDER_STEP_DISTANCE = 0.145f;

        // How much the lower edge of a ladder step-down plane should be in proportion to its thickness.
        public static readonly float OBJECT_STATIC_LADDER_STEP_DROP_DISTANCE_MOD = 1.07f;

        // How long (in ms) the open/close animation will be for game objects
        public static readonly int OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS = 1000;

        // Minimum distance/size that clickable items (doors, mailboxes, bridges, etc) should be interactable
        public static readonly int OBJECT_GAMEOBJECT_DOORBRIDGE_INTERACT_BOUNDARY_MIN_SIZE = 5;
        public static readonly int OBJECT_GAMEOBJECT_MAILBOX_INTERACT_BOUNDARY_MIN_SIZE = 3;

        // How big of an area that a tradeskill focus item (forge, cooking fire) covers in effect
        public static readonly int OBJECT_GAMEOBJECT_TRADESKILLFOCUS_EFFECT_AREA_MIN_SIZE = 5;

        // If true, custom mailboxes are put into the game as 'postmen'
        public static readonly bool OBJECT_GAMEOBJECT_ENABLE_MAILBOXES = true;

        // The starting ID for any material index that should be ignored from rendering
        public static readonly int OBJECT_IGNORE_RENDER_MATERIAL_ID_START = 10000;

        //=====================================================================
        // Creatures
        //=====================================================================
        // This is the percent of the idle time that a 'fidget' occurs (1-100)
        // Note: There are two fidget animations, 1/2 this number applies to each fidget
        public static readonly int CREATURE_FIDGET_TIME_PERCENT = 30;

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

        // If true, riding trainers will be generated
        public static readonly bool CREATURE_RIDING_TRAINERS_ENABLED = true;

        // If true, riding trainers will include flying mounts as well
        public static readonly bool CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY = true;

        // Minimum amount of duration a creature buff buff needs to be in order to be cast out of combat
        public static readonly int CREATURE_SPELL_OOC_BUFF_MIN_DURATION_IN_MS = 60000;

        // How long to wait initially before casting a buff, to stagger casting a bit
        public static readonly int CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MIN_IN_MS = 500;
        public static readonly int CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MAX_IN_MS = 5000;
        public static readonly int CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_RANDOM_RANGE_ADD_IN_MS = 2000;

        // How much time to add the the max recast delay for combat spells so that there's a bit of variation
        public static readonly float CREATURE_SPELL_COMBAT_RECAST_DELAY_MAX_ADD_MOD = 0.25f;

        // At what level of life a creature should cast a heal spell, if they have one
        public static readonly int CREATURE_SPELL_COMBAT_HEAL_MIN_LIFE_PERCENT = 30;

        // If true, all creatures and their waypoints will spawn as a default non-mobile object. This should only be
        // done for debugging reasons, as the game will not look or feel anything like it should
        public static readonly bool CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE = false;

        //=====================================================================
        // Items
        //=====================================================================
        // If true, this uses alternate stats for items that have been tweaked for balance reasons
        public static readonly bool ITEMS_USE_ALTERNATE_STATS = true;
        
        // This is how much is reduced from the weapon delay of EQ weapons, value is 0 - 1;
        public static readonly float ITEMS_WEAPON_DELAY_REDUCTION_AMT = 0.2f;

        // This is the base PPM (Procs Per Minute) used for weapon proc weapons
        public static readonly float ITEMS_WEAPON_EFFECT_PPM_BASE_RATE = 2f;

        // If true, gear that has a worn effect will show as a buff on the character
        public static readonly bool ITEMS_SHOW_WORN_EFFECT_AURA_ICON = true;

        // If true, any item that is clickable item that also has a spell will be replaced with a 
        // container item that contains both the equippable item as well as a non-equipable version
        // that can be clicked from inventory.  WOW doesn't let you click equipable spell items
        // from inventory
        public static readonly bool ITEMS_CREATE_ESSENCE_ITEM_FOR_EQUIPEABLE_CLICK_SPELL_ITEMS = true;

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

        // If true, weight reduction on bags will translate to additional slots
        // Note that the add percent rounds up to the next multiple of 2
        public static readonly bool ITEMS_BAG_WEIGHT_REDUCTION_INCREASES_SLOTS_ENABLED = true;
        public static readonly float ITEM_BAG_WEIGHT_REDUCTION_INCREASE_SLOTS_ADD_PER_PERCENT = 0.08f;

        // This is the icon ID that is used for multi-item containers that contain more than one item
        // The ID here is the icon ID as defined by X in "INV_EQ_X.blp"
        public static readonly int ITEMS_MULTI_ITEMS_CONTAINER_ICON_ID = 57;

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
        // If this is true, use the level as defined in everquest for summoned pets as well
        // as the control behavior.  Otherwise it will behave like a controllable pet and
        // is level aligned with the player
        // NOTE: Not currently working right, so leave false.  Lots of work left before this is good.
        public static readonly bool SPELL_EFFECT_SUMMON_PETS_USE_EQ_LEVEL_AND_BEHAVIOR = false;

        // If true, spells will balance around level 60 being the cap (EQ-like),
        // otherwise it will be 80 like WOTLK
        public static readonly bool SPELL_EFFECT_BALANCE_LEVEL_USE_60_VERSION = true;

        // This is how high (WOW side) stats will be be scaled to.  This should almost always be
        // set to the server max level configuration.  This is different than the property
        // SPELL_EFFECT_BALANCE_LEVEL_USE_60_VERSION which balances values to level 60
        // WOW content, in that this just lets a trickle-up of stats
        public static readonly int SPELL_EFFECT_CALC_STATS_FOR_MAX_LEVEL = 80;

        // If true, the player can return to their gate point by clicking off the buff (within 30 minutes)
        public static readonly bool SPELLS_GATE_TETHER_ENABLED = true;

        // IDs for special spells that need an exact match of ID between this and mod-everquest
        public static readonly int SPELLS_GATECUSTOM_SPELLDBC_ID = 86900;
        public static readonly int SPELLS_BINDCUSTOM_SPELLDBC_ID = 86901;
        public static readonly int SPELLS_DAYPHASE_SPELLDBC_ID = 86903;
        public static readonly int SPELLS_NIGHTPHASE_SPELLDBC_ID = 86904;

        // How much to multiply the EQ range value for WoW
        public static readonly float SPELLS_RANGE_MULTIPLIER = 0.3333f;

        // The most that a movement speed reduction can slow a target.  Should be above -100
        public static readonly int SPELLS_SLOWEST_MOVE_SPEED_EFFECT_VALUE = -90;

        // Everquest has a 'tick' every 6 seconds, so buffs and debuffs should use this as a multiplier
        // Increase or decrease this to modify how long spells work for and, in effect, the damage they do
        // World of Warcraft typically uses a reduced amount and dummy targets start out of combat events at >5 seconds
        // It's highly advisable to use 3 or 2 since they are divisors of 6, but note that generally smaller has
        // rounding issues on the bottom end from a balance perspective
        public static readonly int SPELL_PERIODIC_SECONDS_PER_TICK_EQ = 6;
        public static readonly int SPELL_PERIODIC_SECONDS_PER_TICK_WOW = 3;

        // This is 'added time' in the periodic tick that comes from bard casters.  WOW 3.3.5 does not have
        // 'rolling dots', so without any kind of buffer there won't be a damage/heal 'tick' that occurs
        // on targets near the bard since the spell would get overridden right when a tick would occur
        public static readonly int SPELL_PERIODIC_BARD_TICK_BUFFER_IN_MS = 50;

        // Bards can have this many songs playing at the same time.
        // - Set as 0 to disable this entirely, allowing all songs to play at once
        // - Set as 1 to have a more EQ like experience
        public static readonly int SPELL_MAX_CONCURRENT_BARD_SONGS = 3;

        // This is the minimum allowable recovery time any spell can have, which any smaller will become zero
        // and only subjected to the global cooldown of 1.5 seconds.  This is only enforced on the raw spell
        // records and not the SpellTemplate, to ensure cast repeats are correct for creatures
        public static readonly int SPELL_RECOVERY_TIME_MINIMUM_IN_MS = 3501;

        // If true, you can learn spells from items
        public static readonly bool SPELLS_LEARNABLE_FROM_ITEMS_ENABLED = true;

        // All spell properties
        public static readonly int SPELLS_EFFECT_EMITTER_LONGEST_SPELL_TIME_IN_MS = 16000;

        // How often weapon procs occur
        public static readonly int SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_PROC_CHANCE = 25;
        public static readonly int SPELLS_ENCHANT_SPELL_IMBUE_PROC_CHANGE = 25;

        // How long rogue poisons stay on the weapons
        public static readonly int SPELL_ENCHANT_ROGUE_POISON_ENCHANT_DURATION_ON_WEAPON_TIME_IN_SECONDS = 3600;

        // How long it takes to apply rogue poison
        public static readonly int SPELL_ENCHANT_ROGUE_POISON_APPLY_TIME_IN_MS = 4000;

        // What to show when a rogue has a poison, 0 will disable it (and be more EQ like)
        public static readonly int SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_APPLYING_VISUAL_ID = 1168;
        public static readonly int SPELLS_ENCHANT_ROGUE_POISON_ENCHANT_EFFECT_VISUAL_ID = 26;

        // Spell emitter particles
        public static readonly float SPELLS_EFFECT_EMITTER_SIZE_SCALE_MIN = 0.05f;
        public static readonly float SPELLS_EFFECT_EMITTER_SIZE_SCALE_MAX = 0.8f;
        public static readonly int SPELLS_EFFECT_EMITTER_TARGET_DURATION_IN_MS = 5000;
        public static readonly float SPELLS_EFFECT_EMITTER_DISTANCE_SCALE_MOD = 0.3048f;
        public static readonly float SPELLS_EFFECT_EMITTER_LIFESPAN_TIME_MOD = 0.75f;
        public static readonly int SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MINIMUM = 25;
        public static readonly int SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_DEFAULT = 25;
        public static readonly int SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_DEFAULT = 25;
        public static readonly float SPELL_EFFECT_EMITTER_SPAWN_RATE_SPHERE_MOD = 4f;
        public static readonly float SPELL_EFFECT_EMITTER_SPAWN_RATE_DISC_MOD = 1f;
        public static readonly float SPELL_EFFECT_EMITTER_SPAWN_RATE_OTHER_MOD = 1.25f;

        // Sprite List particles
        public static readonly float SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MIN = 0.1f;
        public static readonly float SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX = 10f;
        public static readonly float SPELLS_EFFECT_SPRITE_LIST_SIZE_SCALE_MAX_EQ_VALUE = 25f; // Anything greater than 15 is treated as 15
        public static readonly float SPELL_EFFECT_SPRITE_LIST_RADIUS_MOD = 0.3048f;
        public static readonly float SPELL_EFFECT_SPRITE_LIST_ANIMATION_SCALE_MOD = 0.5f;
        public static readonly int SPELL_EFFECT_SPRITE_LIST_ANIMATION_FRAME_DELAY_IN_MS = 60;
        public static readonly int SPELL_EFFECT_SPRITE_LIST_MAX_NON_PREJECTILE_ANIM_TIME_IN_MS = 2500;
        public static readonly float SPELL_EFFECT_SPRITE_LIST_PULSE_RANGE = 1.5f;
        public static readonly float SPELL_EFFECT_SPRITE_LIST_CIRCULAR_SHIFT_MOD = 0.5f; // Various reports show this as 0.066 for EQ like (divide by 15) but that seems wrong
        public static readonly float SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_HIGH = 3f;
        public static readonly float SPELL_EFFECT_SPRITE_LIST_VERTICAL_FORCE_LOW = -1f;

        // BardTick visuals have their durations multiplied by this ammount.  It uses the
        // SPELL_PERIODIC_SECONDS_PER_TICK_WOW variable to calculate the amount.  So at 0.333,
        // a 6 second tick would be 2 seconds (EQ like) and 3 second tick would be 1 second
        public static readonly float SPELL_EFFECT_BARD_TICK_VISUAL_DURATION_MOD_FROM_TICK = 0.333f;

        // If this is true, then when a bard song is cast then a tick is applied immediately on targets
        public static readonly bool SPELL_EFFECT_BARD_ADDITIONAL_TICK_ON_CAST = true;

        // This is how much 'weight' the lower effect value has when converting EQ to WoW spell effects,
        //  with values closer to 1 leaning towards the lower effect, and further from 1 leaning towards
        //  the higher effect.  Don't make it less than 1.  
        public static readonly float SPELL_EFFECT_VALUE_LOW_BIAS_WEIGHT = 2.5f;

        // If true, the damage formula will honor spell level based values, otherwise it'll use maximum
        public static readonly bool SPELL_EFFECT_USE_DYNAMIC_EFFECT_VALUES = true;
        public static readonly bool SPELL_EFFECT_USE_DYNAMIC_AURA_DURATIONS = true;

        // Revive will give HP/MP instead of EXP on revive, so this is the multiplier to use for that
        public static readonly int SPELL_EFFECT_REVIVE_EXPPCT_TO_HPMP_MULTIPLIER = 22;

        // Default time that a shrink/grow spell will last for
        public static readonly int SPELL_MODEL_SIZE_CHANGE_EFFECT_DEFAULT_TIME_IN_MS = 1800000;

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

        // Tradeskill items that need a totem in TotemCategory.dbc will align under this
        public static readonly int TRADESKILL_TOTEM_CATEGORY_START = 30;

        // ID for TotemCategory.dbc for specific tradeskills
        public static readonly int TRADESKILL_TOTEM_CATEGORY_DBCID_TAILORING = 210;
        public static readonly int TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_TOOLBOX = 211;
        public static readonly int TRADESKILL_TOTEM_CATEGORY_DBCID_JEWELCRAFTING = 212;
        public static readonly int TRADESKILL_TOTEM_CATEGORY_DBCID_ALCHEMY = 213;
        public static readonly int TRADESKILL_TOTEM_CATEGORY_DBCID_ENGINEERING_FLETCHING = 214;

        //=====================================================================
        // Transports
        //=====================================================================
        public static readonly float TRANSPORT_PAUSE_MULTIPLIER = 0.5f; // Pause as in 'stop at a port'. 1 will be EQ-like
        public static readonly int TRANSPORT_MOVE_SPEED = 30; // Most boats are 30 in WoW, but a value of around 9 is EQ-like
        public static readonly int TRANSPORT_ACCELERATION = 1;
        public static readonly bool TRANSPORT_ALLOW_FIXED_SPEEDS = true; // If true, allows "fixed_speed" to override TRANSPORT_MOVE_SPEED

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
        public static readonly UInt32 DBCID_AREATABLE_ID_END = 6500;

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
        // Note: This value is hard coded in /WorldData/ZoneProperties.csv, TransportShips.csv, and ZoneDisplayMapContinents.csv, so you cannot change only this value
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
        // - Converted spells IDs start at 92000 and base spells range to 95826, with IDs after 96200 used for 'generated spell IDs
        // - SpellIDs 96000 - 96049 reserved for 'worn' effects (effects that always take effect when worn)
        // - SpellIDs 96100 - 96199 reserved for 'coat' effects that come from rogue poisons, triggering another spell
        public static readonly int DBCID_SPELL_ID_START = 86900;
        public static readonly int DBCID_SPELL_ID_GENERATED_START = 96200;
        public static readonly int DBCID_SPELL_ID_END = 99999;

        // ID for spellcasttimes.dbc
        public static readonly int DBCID_SPELLCASTTIME_ID_START = 215;

        // ID for spellduration.dbc
        public static readonly int DBCID_SPELLDURATION_AURA_ID = 610;

        // Stand ID for spell item ennchantments
        public static readonly int DBCID_SPELLITEMENCHANTMENT_ID_START = 4000;

        // ID for spellicon.dbc
        public static readonly int DBCID_SPELLICON_ID_START = 4400;

        // ID for spellradius.dbc
        public static readonly int DBCID_SPELLRADIUS_ID_START = 70;

        // ID for SpellRange.dbc
        public static readonly int DBCID_SPELLRANGE_ID_START = 190;

        // IDs for the SpellVisual line of DBC files
        public static readonly int DBCID_SPELLVISUAL_ID_START = 17000;
        public static readonly int DBCID_SPELLVISUALKIT_ID_START = 16000;
        public static readonly int DBCID_SPELLVISUALEFFECTNAME_ID_START = 7200;

        // IDs for SummonProperties.dbc
        public static readonly int DBCID_SUMMONPROPERTIES_ID_START = 3100;

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
        public static readonly int SQL_CREATURE_GUID_DEBUG_LOW = 600000; // Used for CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE
        public static readonly int SQL_CREATURE_GUID_DEBUG_HIGH = 799999; // Used for CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE

        // Record identifier for the creature template SQL table
        public static readonly int SQL_CREATURETEMPLATE_ENTRY_LOW = 45000;
        public static readonly int SQL_CREATURETEMPLATE_ENTRY_HIGH = 60000;
        public static readonly int SQL_CREATURETEMPLATE_GENERATED_START_ID = 56000;
        public static readonly int SQL_CREATURETEMPLATE_DEBUG_ENTRY_LOW = 300000; // Used for CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE
        public static readonly int SQL_CREATURETEMPLATE_DEBUG_ENTRY_HIGH = 499999; // Used for CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE

        // Start GUIDs for gameobjects
        public static readonly int SQL_GAMEOBJECT_GUID_ID_START = 310000;
        public static readonly int SQL_GAMEOBJECT_GUID_ID_END = 319999;

        // IDs for game_event records
        public static readonly int SQL_GAMEEVENT_ID_DAY = 125;
        public static readonly int SQL_GAMEEVENT_ID_NIGHT = 126;

        // Start and end IDs for gameobject_template rows
        // - GameObjects.csv owns rows 270000 - 274999
        // - TarnsportLifts.csv and TransportLiftTriggers.csv own rows 279900 - 279999
        public static readonly int SQL_GAMEOBJECTTEMPLATE_ID_START = 270000;
        public static readonly int SQL_GAMEOBJECTTEMPLATE_ID_END = 279999;

        // Start row for `game_tele` records. (~2000-2400)
        public static readonly int SQL_GAMETELE_ROWID_START = 2000;
        public static readonly int SQL_GAMETELE_ROWID_END = 2400;

        // Start and end IDs for custom gossip menu records
        public static readonly int SQL_GOSSIPMENU_MENUID_START = 62000;
        public static readonly int SQL_GOSSIPMENU_MENUID_END = 69999;

        // Start and end IDs for template entries
        // - Class-Specific scroll IDs range 110500 - 112887
        // - Equipped Click Bag IDs range 113000 - 113932
        // - Equipped Click Essence IDs range 114000 - 114932
        // - Quest Template multi-item reward containers IDs range 116000 - 116200
        // - Tradeskill multi-item creation containers IDs range 117000 - 117217
        // - NPC-worn version of items range 120000-133783 (TODO Review: May be much less)
        public static readonly int SQL_ITEM_TEMPLATE_ENTRY_START = 85000;
        public static readonly int SQL_ITEM_TEMPLATE_ENTRY_END = 134000;
        public static readonly int SQL_ITEM_TEMPLATE_ENTRY_GENERATED_CREATURE_START = 120000;

        // Stand and end IDs for npc_text sql records
        public static readonly int SQL_NPCTEXT_ID_START = 80000;
        public static readonly int SQL_NPCTEXT_ID_END = 99999;

        // Start ID for npc_trainer entries
        public static readonly int SQL_NPCTRAINER_ID_START = 210000;
        public static readonly int SQL_NPCTRAINER_ID_END = 211000;

        // Start ID for pet_name_generation entries
        public static readonly int SQL_PETNAMEGENERATION_ID_START = 400;

        // Start and end ID for pool_template data rows (reserve 40k records)
        public static readonly int SQL_POOL_TEMPLATE_ID_START = 110000;
        public static readonly int SQL_POOL_TEMPLATE_ID_END = 150000;

        // Start and end IDs for quest template data rows
        // The 'shift' value is the value to add for the repeatable versions
        public static readonly int SQL_QUEST_TEMPLATE_ID_START = 30000;
        public static readonly int SQL_QUEST_TEMPLATE_ID_END = 40000;
        public static readonly int SQL_QUEST_TEMPLATE_ID_REPEATABLE_SHIFT = 5000;

        // Start and end IDs for spell groups
        public static readonly int SQL_SPELL_GROUP_ID_START = 1500;
        public static readonly int SQL_SPELL_GROUP_ID_FOR_BARD_AURA_START = 1750;
        public static readonly int SQL_SPELL_GROUP_ID_END = 1999;

        // Start and end IDs for transports
        public static readonly int SQL_TRANSPORTS_GUID_START = 21;
        public static readonly int SQL_TRANSPORTS_GUID_END = 41;

        // DO NOT CHANGE THESE VALUES UNLESS YOU KNOW WHAT YOU ARE DOING
        // They are temporary values used for configuration purposes, resolved at runtime
        public static string PATH_EQEXPORTSRAW_FOLDER { get => Path.Combine(PATH_WORKING_FOLDER, "EQClientExportRaw"); }
        public static string PATH_EQEXPORTSCONDITIONED_FOLDER { get => Path.Combine(PATH_WORKING_FOLDER, "EQClientExportConditioned"); }
        public static string PATH_EXPORT_FOLDER { get => Path.Combine(PATH_WORKING_FOLDER, "WOWExports"); }
    }
}
