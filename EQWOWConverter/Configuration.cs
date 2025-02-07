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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter
{
    internal class Configuration
    {
        // ====================================================================
        // Console
        // ====================================================================
        // Plays a beep sound when the generate completes if set to true
        public static readonly bool CONFIG_CONSOLE_BEEP_ON_COMPLETE = true;

        // ====================================================================
        // Logging
        // ====================================================================
        // Level of logs to write to the console and log file.
        // 1 = Error, 2 = Info, 3 = Detail
        public static readonly int CONFIG_LOGGING_CONSOLE_MIN_LEVEL = 2;
        public static readonly int CONFIG_LOGGING_FILE_MIN_LEVEL = 3;

        // ====================================================================
        // Paths and Files
        // ====================================================================
        // Location of the installed everquest trilogy
        public static readonly string CONFIG_PATH_EQTRILOGY_FOLDER = "E:\\Development\\EQWOW-Reference\\EverQuestTrilogy";

        // Location of eq data exports before conditioning (from LaternExtractor)
        public static readonly string CONFIG_PATH_EQEXPORTSRAW_FOLDER = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQClientExportRaw";

        // Location of your enUS World of Warcaft client, where your wow.exe is (such as C:\WorldOfWarcraft)
        public static readonly string CONFIG_PATH_WOW_ENUS_CLIENT_FOLDER = "E:\\Development\\azerothcore-wotlk\\Client\\";

        // Where the "conditioned" eq data export files go
        public static readonly string CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQClientExportConditioned";

        // Where the intermediate generated WOW files go
        public static readonly string CONFIG_PATH_EXPORT_FOLDER = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\WOWExports";

        // The root of the tools directory (included in this source repository)
        public static readonly string CONFIG_PATH_TOOLS_FOLDER = "E:\\Development\\EQWOW\\Tools";

        // The root of the assets directory (included in this source repository)
        public static readonly string CONFIG_PATH_ASSETS_FOLDER = "E:\\Development\\EQWOW\\Assets";

        // Name of the newely generated patch file, without the extension. Note: Will be deleted when extracting DBC data, so make sure it's the last and new
        public static readonly string CONFIG_PATH_PATCH_NEW_FILE_NAME_NO_EXT = "patch-enUS-5";

        // Location of where the server DBC files would be deployed to if set to deploy
        public static readonly string CONFIG_PATH_DEPLOY_SERVER_DBC_FILES_FOLDER = "E:\\Development\\azerothcore-wotlk\\Build\\bin\\RelWithDebInfo\\data\\dbc";

        // ====================================================================
        // Deployment Rules
        // ====================================================================
        // If true, deploy the client file (patch mpq) after building it
        public static readonly bool CONFIG_DEPLOY_CLIENT_FILES = false;

        // If true, deploy to the server files/data after building
        public static readonly bool CONFIG_DEPLOY_SERVER_FILES = false;

        // If true, deploy to the SQL to the server
        // Note: May not work on remote servers (not tested)
        public static readonly bool CONFIG_DEPLOY_SERVER_SQL = false;

        // If true, DBC files are extracted every time.
        public static readonly bool CONFIG_EXTRACT_DBC_FILES = true;

        // If true and when deploying client files, clear the cache
        public static readonly bool CONFIG_DEPLOY_CLEAR_CACHE_ON_CLIENT_DEPLOY = true;

        // If deploying to SQL, you need to set this to something real that points to your world database
        public static readonly string CONFIG_DEPLOY_SQL_CONNECTION_STRING_WORLD = "Server=127.0.0.1;Database=acore_world;Uid=root;Pwd=rootpass;";

        // ====================================================================
        // Generator Rules
        // ====================================================================
        // The value EQ vertices multiply by when translated into WOW vertices
        // A value of 0.25 seems to be 1:1 with EQ.  0.28 allows humans and 0.4 allows taurens to enter rivervale bank door
        public static readonly float CONFIG_GENERATE_WORLD_SCALE = 0.29f;
        public static readonly float CONFIG_GENERATE_CREATURE_SCALE = 0.255f;

        // If true, then objects are generated
        public static readonly bool CONFIG_GENERATE_OBJECTS = true;

        // If true, then creatures are generated
        public static readonly bool CONFIG_GENERATE_CREATURES_AND_SPAWNS = true;

        // If this has any zone short names in it, the ouput of the generator will perform an update only for these zones. If there is no previously
        // built patch mpq, it will be forced to do a complete build first
        public static readonly List<string> CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES = new List<string>() { };

        // If this is true and you do an update build, only the zones in the CONFIG_GENERATE_UPDATE_BUILD_INCLUDED_ZONE_SHORTNAMES will be functional
        // Allows for much faster builds when debugging
        public static readonly bool CONFIG_GENERATE_UPDATE_BUILD_ONLY_HAVE_INCLUDED_ZONES_FUNCTIONAL = false;

        // If true, zones for Kunark are generated
        public static readonly bool CONFIG_GENERATE_KUNARK_ZONES = false;

        // Kunark zone shortnames
        public static readonly List<string> CONFIG_GENERATE_KUNARK_ZONE_SHORTNAMES = new List<string>() { "burningwood", "cabeast", "cabwest",
            "charasis", "chardok", "citymist", "dalnir", "dreadlands", "droga", "emeraldjungle", "fieldofbone", "firiona", "frontiermtns",
            "kaesora", "karnor", "kurn", "lakeofillomen", "nurga", "overthere", "sebilis", "skyfire", "swampofnohope", "timorous",
            "trakanon", "veeshan", "wakening", "warslikswood" };

        // If true, zones for Velious are generated
        public static readonly bool CONFIG_GENERATE_VELIOUS_ZONES = false;

        // Velious zone shortnames
        public static readonly List<string> CONFIG_GENERATE_VELIOUS_ZONE_SHORTNAMES = new List<string>() { "cobaltscar", "crystal", "eastwastes",
            "frozenshadow", "greatdivide", "growthplane", "iceclad", "kael", "mischiefplane", "necropolis", "sirens", "skyshrine",
            "sleeper", "templeveeshan", "thurgadina", "thurgadinb", "velketor", "westwastes" };

        // Identifier for what subset of expansion data to work with.  0 = Classic, 1 = Kunark, 2 = Velious
        public static readonly int CONFIG_GENERATE_EQ_EXPANSION_ID = 0;

        // An extra amount to add to the boundary boxes when generating wow assets from EQ.  Needed to handle rounding.
        public static readonly float CONFIG_GENERATE_ADDED_BOUNDARY_AMOUNT = 0.01f;

        // How many insert rows to restrict in a SQL output file
        public static readonly int CONFIG_GENERATE_SQL_FILE_BATCH_SIZE = 50000;
        public static readonly int CONFIG_GENERATE_SQL_FILE_INLINE_INSERT_ROWCOUNT_SIZE = 5000;

        // How many file names to batch up when converting 
        public static readonly int CONFIG_GENERATE_BLPCONVERTBATCHSIZE = 50;

        // What edge buffer to add when doing floating point month
        public static readonly float CONFIG_GENERATE_FLOAT_EPSILON = 0.001f;

        //=====================================================================
        // Zone General
        //=====================================================================
        // If this is set to false, any static graphics (like dirt, etc) are not rendered.  Only set to false for debugging
        public static readonly bool CONFIG_ZONE_SHOW_STATIC_GEOMETRY = true;

        // Maximum number of faces that fit into a render WMO group before it subdivides (max is due to various variable limits)
        public static readonly int CONFIG_ZONE_MAX_FACES_PER_WMOGROUP = 21840;

        // Maximum size of any zone-to-material-object creation along the X and Y axis
        public static readonly float CONFIG_ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE = 325.0f;

        // How much bigger to make the box which causes the shadow in a shadowbox
        public static readonly float CONFIG_ZONE_SHADOW_BOX_ADDED_SIZE = 50f;

        // If true, allow collision with world model objects. This will also impact music and ambient sounds, since they align to areas which require collision detection
        public static readonly bool CONFIG_ZONE_COLLISION_ENABLED = true;

        // When zone geometry gets broken into cuboids, this is the max side length of the area
        public static readonly int CONFIG_ZONE_SPLIT_COLLISION_CUBOID_MAX_EDGE_LEGNTH = 1200;

        // If set to 'true', show a box where the music zones are. This is for debugging only.  Only works when collision is enabled
        public static readonly bool CONFIG_ZONE_DRAW_COLLIDABLE_SUB_AREAS_AS_BOXES = false;

        // Maxinum number of triangle faces that can be in any zone-to-material-object
        public static readonly int CONFIG_ZONE_MAX_FACES_PER_ZONE_MATERIAL_OBJECT = 2000;

        // BSP tree nodes will stop subdividing when this many (or less) triangles are found
        public static readonly UInt16 CONFIG_ZONE_BTREE_MIN_SPLIT_SIZE = 10;
        
        // BSP tree nodes won't operate on bounding boxes smaller than this (X + Y + Z lengths)
        public static readonly float CONFIG_ZONE_BTREE_MIN_BOX_SIZE_TOTAL = 5f;

        // BSP tree nodes won't generate deeper than this many iterations
        public static readonly int CONFIG_ZONE_BTREE_MAX_NODE_GEN_DEPTH = 50;

        //=====================================================================
        // Liquid
        //=====================================================================
        // If this is true, it will show the true surface line of water and not just the material from EQ.  This should only be used
        // for debugging as it very visually unpleasant
        public static readonly bool CONFIG_LIQUID_SHOW_TRUE_SURFACE = false;

        // How much 'height' to add to liquid surface, helps with rendering the waves
        public static readonly float CONFIG_LIQUID_SURFACE_ADD_Z_HEIGHT = 0.001f;

        // How much to 'walk' the x value when generating an irregular quad of liquid for each plane
        public static readonly float CONFIG_LIQUID_QUADGEN_EDGE_WALK_SIZE = 0.2f;

        // How much to overlap the planes when generating an irregular quad of liquid
        public static readonly float CONFIG_LIQUID_QUADGEN_PLANE_OVERLAP_SIZE = 0.0001f;

        //=====================================================================
        // Lighting and Coloring
        //=====================================================================
        // If true, light instances are enabled.  They don't work at this time, so leave false
        public static readonly bool CONFIG_LIGHT_INSTANCES_ENABLED = false;

        // If true, light instances are rendered as torches.  Use for debugging only, and typically leave false
        public static readonly bool CONFIG_LIGHT_INSTANCES_DRAWN_AS_TORCHES = false;

        // Sets the modifier to add to the attenuation to define the start, calculated by multiplying this value to it
        public static readonly float CONFIG_LIGHT_INSTANCE_ATTENUATION_START_PROPORTION = 0.25f;

        // How much of the EQ original vertex color to apply to surfaces
        public static readonly double CONFIG_LIGHT_DEFAULT_VERTEX_COLOR_INTENSITY = 0.2;

        // Default ambience to apply to indoor areas (sets to r g and b). To have colors pop more like in EQ, set it lower like 96 or so.
        public static readonly byte CONFIG_LIGHT_DEFAULT_INDOOR_AMBIENCE = 165;

        // Amonut of glow to add to outdoor areas (ranges are 0-1)
        public static readonly float CONFIG_LIGHT_OUTSIDE_GLOW_CLEAR_WEATHER = 0.4f;
        public static readonly float CONFIG_LIGHT_OUTSIDE_GLOW_STORMY_WEATHER = 0.2f;
        public static readonly float CONFIG_LIGHT_OUTSIDE_GLOW_UNDERWATER = 0.6f;

        // Brightness of outdoor areas based on time
        public static readonly byte CONFIG_LIGHT_OUTSIDE_AMBIENT_TIME_0 = 58; // Midnight
        public static readonly byte CONFIG_LIGHT_OUTSIDE_AMBIENT_TIME_3 = 58; // 3 AM
        public static readonly byte CONFIG_LIGHT_OUTSIDE_AMBIENT_TIME_6 = 164; // 6 AM
        public static readonly byte CONFIG_LIGHT_OUTSIDE_AMBIENT_TIME_12 = 239; // Noon - Must always be the most bright
        public static readonly byte CONFIG_LIGHT_OUTSIDE_AMBIENT_TIME_21 = 192; // 9 PM
        public static readonly byte CONFIG_LIGHT_OUTSIDE_AMBIENT_TIME_22 = 58; // 10 PM

        //=====================================================================
        // Audio
        //=====================================================================       
        // Set which soundfont to use in the Tools/soundfont folder.  Alternate option is synthusr_samplefix.sf2
        public static readonly string CONFIG_AUDIO_SOUNDFONT_FILE_NAME = "AweROMGM.sf2";

        // If set to true, some audio tracks are swapped vs the original tracks.  Make it false if you want a more classic-like experience
        public static readonly bool CONFIG_AUDIO_USE_ALTERNATE_TRACKS = false;

        // How much to increase the music sound when converted from EverQuest
        public static readonly decimal CONFIG_AUDIO_MUSIC_CONVERSION_GAIN_AMOUNT = 1;

        // Mod / multiplier to volumes (multiplies the volume by this value)
        // NOTE: Sound Instance volumes can be found in Sound.GetVolume
        public static readonly float CONFIG_AUDIO_AMBIENT_SOUND_VOLUME_MOD = 1f;
        public static readonly float CONFIG_AUDIO_SOUNDINSTANCE_VOLUME_MOD = 1f;
        public static readonly float CONFIG_AUDIO_MUSIC_VOLUME_MOD = 1f;

        // If this is 'true', draw any sound instances in a zone as a little box
        public static readonly bool CONFIG_AUDIO_SOUNDINSTANCE_DRAW_AS_BOX = false;

        // The radius of a sound instance is multiplied by this to get the min distance, which is the range which the sound is 100% volume
        public static readonly float CONFIG_AUDIO_SOUNDINSTANCE_3D_MIN_DISTANCE_MOD = 0.4f;
        public static readonly float CONFIG_AUDIO_SOUNDINSTANCE_2D_MIN_DISTANCE_MOD = 0.8f;

        // Size of the box when rendering a sound instance (Note: It's 1/2 the in-game size)
        public static readonly float CONFIG_AUDIO_SOUNDINSTANCE_RENDEROBJECT_BOX_SIZE = 1f;

        // Name of the object material that is used when rendering the soundinstance object
        public static readonly string CONFIG_AUDIO_SOUNDINSTANCE_RENDEROBJECT_MATERIAL_NAME = "akafloorgrate";

        // Volume of creature sound effects like attacks and being hit
        public static readonly float CONFIG_AUDIO_CREATURE_SOUND_VOLUME = 0.3f;

        //=====================================================================
        // Objects
        //=====================================================================
        // The minimum size of a bounding box for a static doodad.  Bigger means it can be seen further away
        public static readonly float CONFIG_OBJECT_STATIC_MIN_BOUNDING_BOX_SIZE = 25.1f;

        // If set to true, the collision is rendered and not the actual render geometry. Leave false unless debugging.
        // TODO: Add a custom material for this purpose, as some boundary boxes don't show
        public static readonly bool CONFIG_OBJECT_STATIC_RENDER_AS_COLLISION = false;

        // For ladders, this is how far to extend out the steppable area in front and back of it (value is before world scaling)
        public static readonly float CONFIG_OBJECT_STATIC_LADDER_EXTEND_DISTANCE = 1.0f;

        // How much space between each step of a ladder along the Z axis (value is before world scaling)
        public static readonly float CONFIG_OBJECT_STATIC_LADDER_STEP_DISTANCE = 0.5f;

        //=====================================================================
        // Creatures
        //=====================================================================
        // This is the percent of the idle time that a 'fidget' occurs (1-100)
        // Note: There are two fidget animations, 1/2 this number applies to each fidget
        public static readonly int CONFIG_CREATURE_FIDGET_TIME_PERCENT = 20;

        // If true, the entity name is put in the creature name for easy debugging. Default to false.
        public static readonly bool CONFIG_CREATURE_ADD_ENTITY_ID_TO_NAME = false;

        // Stat modifiers for creatures
        // - "MIN" and "MAX" are applied after all other calculations
        public static readonly float CONFIG_CREATURE_STAT_MOD_HP_ADD = 0.5f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_HP_MIN = 0.8f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_HP_MAX_NORMAL = 2f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_HP_MAX_RARE = 2f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_HP_SET_ELITE = 20f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_HP_SET_ELITERARE = 80f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_HP_SET_BOSS = 200f;

        public static readonly float CONFIG_CREATURE_STAT_MOD_AVGDMG_ADD = 0.2f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_AVGDMG_MIN = 0.8f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL = 1.5f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_AVGDMG_MAX_RARE = 1.5f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_AVGDMG_SET_ELITE = 8f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE = 10f;
        public static readonly float CONFIG_CREATURE_STAT_MOD_AVGDMG_SET_BOSS = 18f;

        // The value to name the everquest parent reputation item as
        public static readonly string CONFIG_CREATURE_FACTION_ROOT_NAME = "Everquest";

        //=====================================================================
        // Items
        //=====================================================================
        // This is how much is reduced from the weapon delay of EQ weapons, value is 0 - 1;
        public static readonly float CONFIG_ITEMS_WEAPON_DELAY_REDUCTION_AMT = 0.2f;

        // If this is true, then weapons and armor that allow all normally-aligned classes to be
        //  classified as 'all'.  For example: Bronze Breastplate allows all plate classes, so
        //  it will have the classes allowed list set to 'any'
        public static readonly bool CONFIG_ITEMS_ALLOW_ALL_CLASSES_ON_GENERIC_EQUIP = true;

        // This is how much 'weight' the lower stat has when converting EQ to WoW stats, with 
        //  values closer to 1 leaning towards the lower stat, and further from 1 leaning towards
        //  the higher stat.  Don't make it less than 1.  
        public static readonly float CONFIG_ITEMS_STATS_LOW_BIAS_WEIGHT = 2.5f;

        // ====================================================================
        // WOW DBC/File IDs
        // ====================================================================
        // IDs for AreaBit used in AreaTable, should be unique (max of 4095)
        public static readonly int CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_1_START = 3092;
        public static readonly int CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_1_END = 3172;
        public static readonly int CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_2_START = 3462;
        public static readonly int CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_2_END = 3616;
        public static readonly int CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_3_START = 3800;
        public static readonly int CONFIG_DBCID_AREATABLE_AREABIT_BLOCK_3_END = 4095;

        // Identifies Area rows in AreaTable.dbc
        public static readonly UInt32 CONFIG_DBCID_AREATABLE_ID_START = 5100;

        // IDs for AreaTrigger.DBC. These will be generated in ascending order by MapID, and referenced in SQL scripts
        // for teleports as well any other area-based triggers
        public static readonly int CONFIG_DBCID_AREATRIGGER_ID_START = 6500;

        // IDs for CreatureDisplayInfo.dbc
        public static readonly int CONFIG_DBCID_CREATUREDISPLAYINFO_ID_START = 34000;
        public static readonly int CONFIG_DBCID_CREATUREDISPLAYINFO_ID_END = 40000;

        // IDs for CreatureModelData.dbc
        public static readonly int CONFIG_DBCID_CREATUREMODELDATA_ID_START = 3500;
        public static readonly int CONFIG_DBCID_CREATUREMODELDATA_ID_END = 5000;

        // IDs for CreatureSoundData.dbc
        public static readonly int CONFIG_DBCID_CREATURESOUNDDATA_ID_START = 3300;

        // IDs for FactionTemplate.dbc
        public static readonly int CONFIG_DBCID_FACTIONTEMPLATE_ID_START = 2300;

        // IDs for FootstepTerrainLookup.dbc
        public static readonly int CONFIG_DBCID_FOOTSTEPTERRAINLOOKUP_ID_START = 600;
        public static readonly int CONFIG_DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START = 250;

        // IDs for GameObjects found in GameObject.dbc (Reserving 10k)
        public static readonly int CONFIG_DBCID_GAMEOBJECT_ID_START = 270000;
        public static readonly int CONFIG_DBCID_GAMEOBJECT_ID_END = 279999;

        // IDs for rows inside GameObjectDisplayInfo.dbc
        public static readonly int CONFIG_DBCID_GAMEOBJECTDISPLAYINFO_ID_START = 11000;

        // Start ID for item display info
        public static readonly int CONFIG_DBCID_ITEMDISPLAYINFO_START = 86000;

        // Identifies the Light.DBC row, used for environmental properties
        public static readonly int CONFIG_DBCID_LIGHT_ID_START = 3500;

        // Identifies the LightParams.dbc, used for detailed values related to a Light.DBC row
        public static readonly int CONFIG_DBCID_LIGHTPARAMS_ID_START = 1050;

        // IDs for the loading screen
        public static readonly int CONFIG_DBCID_LOADINGSCREEN_ID_START = 255;

        // Identifies Maps in Map.dbc and MapDifficulty.dbc
        public static readonly int CONFIG_DBCID_MAP_ID_START = 750;

        // Specific rows in MapDifficulty.dbc. (~800-922)
        public static readonly int CONFIG_DBCID_MAPDIFFICULTY_ID_START = 800;

        // ID for sounds found in SoundEntries.dbc
        public static readonly int CONFIG_DBCID_SOUNDENTRIES_ID_START = 22000;

        // ID for sounds found in SoundAmbience.dbc
        public static readonly int CONFIG_DBCID_SOUNDAMBIENCE_ID_START = 600;

        // Specific rows in WMOAreaTable.dbc
        public static readonly int CONFIG_DBCID_WMOAREATABLE_ID_START = 52000;

        // Identifies WMO Roots.  Found in WMOAreaTable.dbc and AreaTable.dbc
        public static readonly UInt32 CONFIG_DBCID_WMOAREATABLE_WMOID_START = 7000;

        // Identifies WMO Groups. Found in WMOAreaTable.dbc and the .wmo files
        public static readonly UInt32 CONFIG_DBCID_WMOAREATABLE_WMOGROUPID_START = 30000;

        // ID for music in ZoneMusic.dbc, and how many IDs to reserve on a per-zone basis
        public static readonly int CONFIG_DBCID_ZONEMUSIC_START = 700;

        // ====================================================================
        // SQL IDs
        // ====================================================================
        // Record identifier for the creature sql table, need at least 31k
        public static readonly int CONFIG_SQL_CREATURE_GUID_LOW = 310000;
        public static readonly int CONFIG_SQL_CREATURE_GUID_HIGH = 399999;

        // Record identifier for the creature template SQL table, need at least 25k
        public static readonly int CONFIG_SQL_CREATURETEMPLATE_ENTRY_LOW = 210000;
        public static readonly int CONFIG_SQL_CREATURETEMPLATE_ENTRY_HIGH = 249999;

        // Start GUIDs for gameobjects
        public static readonly int CONFIG_SQL_GAMEOBJECT_GUID_START = 310000;

        // Start row for `game_tele` records. (~2000-2400)
        public static readonly int CONFIG_SQL_GAMETELE_ROWID_START = 2000;
        public static readonly int CONFIG_SQL_GAMETELE_ROWID_END = 2400;

        // Start and end IDs for template entries
        public static readonly int CONFIG_SQL_ITEM_TEMPLATE_ENTRY_START = 85000;
        public static readonly int CONFIG_SQL_ITEM_TEMPLATE_ENTRY_END = 120000;

        // Start ID for pool_template data rows (reserve 40k records)
        public static readonly int CONFIG_SQL_POOL_TEMPLATE_ID_START = 110000;
        public static readonly int CONFIG_SQL_POOL_TEMPLATE_ID_END = 150000;
    }
}
