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
        // Paths
        // ====================================================================
        // Location of eq data exports before conditioning (from LaternExtractor)
        public static readonly string CONFIG_PATH_EQEXPORTSRAW = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQExports-Int";

        // Where the "conditioned" eq data export files go
        public static readonly string CONFIG_PATH_EQEXPORTSCONDITIONED = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQExportsConditioned";

        // Where the generated WOW files go
        public static readonly string CONFIG_PATH_EXPORT_FOLDER = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\WOWExports";

        // The root of the tools directory (included in this source repository)
        public static readonly string CONFIG_PATH_TOOLS_FOLDER = "E:\\Development\\EQWOW\\Tools";

        // ====================================================================
        // Logging
        // ====================================================================
        // Level of logs to write to the console and log file.
        // 1 = Error, 2 = Info, 3 = Detail
        public static readonly int CONFIG_LOGGING_CONSOLE_MIN_LEVEL = 2;
        public static readonly int CONFIG_LOGGING_FILE_MIN_LEVEL = 3;

        // ====================================================================
        // Generator Rules
        // ====================================================================
        // The value EQ vertices multiply by when translated into WOW vertices
        // 0.3 is the default.  A value of 0.25 seems to be 1:1 with EQ. 0.3 allows most races to enter small doors. 0.4 allows taurens through rivervale bank door
        public static readonly float CONFIG_EQTOWOW_WORLD_SCALE = 0.3f;

        // Maximum number of faces that fit into a WMO group before it subdivides (true max is 21,840)
        public static readonly int CONFIG_WOW_MAX_FACES_PER_WMOGROUP = 21000;

        // An extra amount to add to the boundary boxes when generating wow assets from EQ.  Needed to handle rounding.
        public static readonly float CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT = 0.01f;

        // Maximum size of any zone-to-material-object creation along the X and Y axis
        public static readonly float CONFIG_EQTOWOW_ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE = 325.0f;

        // If this is true, it will show the true surface line of water and not just the material from EQ.  This should only be used
        // for debugging as it very visually unpleasant
        public static readonly bool CONFIG_EQTOWOW_ZONE_LIQUID_SHOW_TRUE_SURFACE = false;

        // Any surface liquid that has an x or y dimension larger than this will be cut down in size
        public static readonly int CONFIG_EQTOWOW_LIQUID_SURFACE_MAX_XY_DIMENSION = 1300;

        // How much 'height' to add to liquid surface, helps with rendering the waves
        public static readonly float CONFIG_EQTOTWOW_LIQUID_SURFACE_ADD_Z_HEIGHT = 0.001f;

        // How much to 'walk' the x value when generating an irregular quad of liquid for each plane
        public static readonly float CONFIG_EQTOWOW_LIQUID_QUADGEN_EDGE_WALK_SIZE = 0.2f;

        // How much to overlap the planes when generating an irregular quad of liquid
        public static readonly float CONFIG_EQTOWOW_LIQUID_QUADGEN_PLANE_OVERLAP_SIZE = 0.0001f; 

        // Maxinum number of triangle faces that can be in any zone-to-material-object
        public static readonly int CONFIG_EQTOWOW_ZONE_MATERIAL_TO_OBJECT_SPLIT_MAX_FACE_TRIANGLE_COUNT = 21800;

        // If populated, only the zones listed here will be generated
        public static readonly List<string> CONFIG_EQTOWOW_RESTRICTD_ZONE_SHORTNAMES_FOR_GENERATION = new List<string>() { };

        // If this is set to false, any static graphics (like dirt, etc) are not rendered.  Only set to false for debugging
        public static readonly bool CONFIG_EQTOWOW_ZONE_GENERATE_STATIC_GEOMETRY = true;

        // If true, then objects are generated
        public static readonly bool CONFIG_GENERATE_OBJECTS = true;

        // If true, zones for Kunark are generated
        public static readonly bool CONFIG_GENERATE_KUNARK_ZONES = false;

        // If true, zones for Velious are generated
        public static readonly bool CONFIG_GENERATE_VELIOUS_ZONES = false;

        //=====================================================================
        // Collision
        //=====================================================================
        // If true, allow collision with world model objects
        public static readonly bool CONFIG_WORLD_MODEL_OBJECT_COLLISION_ENABLED = true;
        
        // Maximum number of BTREE faces that fit into a WMO group before it subdivides
        public static readonly int CONFIG_WOW_MAX_BTREE_FACES_PER_WMOGROUP = 2100;

        //=====================================================================
        // Lighting and Coloring
        //=====================================================================
        // If true, light instances are enabled.  They don't work at this time, so leave false
        public static readonly bool CONFIG_LIGHT_INSTANCES_ENABLED = true;

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
        // How much to increase the music sound when converted from EverQuest
        public static readonly decimal CONFIG_AUDIO_MUSIC_CONVERSION_GAIN_AMOUNT = 2;

        //=====================================================================
        // Objects
        //=====================================================================
        // The minimum size of a bounding box for a static doodad.  Bigger means it can be seen further away
        public static readonly float CONFIG_STATIC_OBJECT_MIN_BOUNDING_BOX_SIZE = 25.1f;

        // If set to true, the collision is rendered and not the actual render geometry. Leave false unless debugging.
        // TODO: Add a custom material for this purpose, as some boundary boxes don't show
        public static readonly bool CONFIG_STATIC_OBJECT_RENDER_AS_COLLISION = false;

        // For ladders, this is how far to extend out the steppable area in front and back of it (value is before world scaling)
        public static readonly float CONFIG_STATIC_OBJECT_LADDER_EXTEND_DISTANCE = 1.0f;

        // How much space between each step of a ladder along the Z axis (value is before world scaling)
        public static readonly float CONFIG_STATIC_OBJECT_LADDER_STEP_DISTANCE = 0.5f;

        // ====================================================================
        // WOW DBC/File IDs
        // ====================================================================
        // Identifies WMO Groups. Found in WMOAreaTable.dbc and the .wmo files. (~30000-38000)
        public static readonly UInt32 CONFIG_DBCID_WMOGROUPID_START = 30000;

        // Identifies WMO Roots.  Found in WMOAreaTable.dbc and AreaTable.dbc. (~7000-7200)
        public static readonly UInt32 CONFIG_DBCID_WMOID_START = 7000;

        // Specific rows in WMOAreaTable.dbc. (~52000-60000)
        public static readonly int CONFIG_DBCID_WMOAREATABLEID_START = 52000;

        // Identifies Area rows in AreaTable.dbc. (~6000-6200)
        public static readonly UInt32 CONFIG_DBCID_AREAID_START = 6000;

        // Identifies Maps in Map.dbc and MapDifficulty.dbc. (~750-900)
        public static readonly int CONFIG_DBCID_MAPID_START = 750;

        // Specific rows in MapDifficulty.dbc. (~800-922)
        public static readonly int CONFIG_DBCID_MAPDIFFICULTYID_START = 800;

        // IDs for the loading screen
        public static readonly int CONFIG_DBCID_LOADINGSCREENID_START = 255;

        // IDs for AreaBit used in AreaTable, should be unique
        public static readonly int CONFIG_DBCID_AREATABLE_AREABIT_START = 3800;

        // IDs for AreaTrigger.DBC. These will be generated in ascending order by MapID, and referenced in AzerothCore scripts
        // for teleports as well any other area-based triggers
        public static readonly int CONFIG_DBCID_AREATRIGGERID_START = 6500;

        // Identifies the Light.DBC row, used for environmental properties
        public static readonly int CONFIG_DBCID_LIGHT_START = 3500;

        // Identifies the LightParams.dbc, used for detailed values related to a Light.DBC row
        public static readonly int CONFIG_DBCID_LIGHTPARAMS_START = 1050;

        // ID for music in ZoneMusic.dbc
        public static readonly int CONFIG_DBCID_ZONEMUSIC_START = 700;

        // ID for sounds found in SoundEntries.dbc
        public static readonly int CONFIG_DBCID_SOUNDENTRIES_START = 22000;

        // ====================================================================
        // AzerothCore Database IDs
        // ====================================================================
        // Start row for `game_tele` records. (~2000-2200)
        public static readonly int CONFIG_GAMETELE_ROWID_START = 2000;

        // ====================================================================
        // Lookups
        // ====================================================================
        public static readonly List<string> CONFIG_LOOKUP_VELIOUS_ZONE_SHORTNAMES = new List<string>() { "cobaltscar", "crystal", "eastwastes",
            "frozenshadow", "greatdivide", "growthplane", "iceclad", "kael", "mischiefplane", "necropolis", "sirens", "skyshrine",
            "sleeper", "templeveeshan", "thurgadina", "thurgadinb", "velketor", "westwastes" };

        public static readonly List<string> CONFIG_LOOKUP_KUNARK_ZONE_SHORTNAMES = new List<string>() { "burningwood", "cabeast", "cabwest",
            "charasis", "chardok", "citymist", "dalnir", "dreadlands", "droga", "emeraldjungle", "fieldofbone", "firiona", "frontiermtns",
            "kaesora", "karnor", "kurn", "lakeofillomen", "nurga", "overthere", "sebilis", "skyfire", "swampofnohope", "timorous",
            "trakanon", "veeshan", "wakening", "warslikswood" };
    }
}
