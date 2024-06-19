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
        public static readonly float CONFIG_EQTOWOW_WORLD_SCALE = 0.3f;//1.0f;//0.3f;

        // Maximum number of faces that fit into a WMO group before it subdivides
        // max value can only ever be 21,840, but >2100 not advised due to btree size
        public static readonly int CONFIG_WOW_MAX_FACES_PER_WMOGROUP = 2100;

        // An extra amount to add to the boundary boxes when generating wow assets from EQ.  Needed to handle rounding.
        public static readonly float CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT = 0.01f;

        // Maximum size of any zone-to-material-object creation along the X and Y axis
        public static readonly float CONFIG_EQTOWOW_ZONE_MATERIAL_TO_OBJECT_SPLIT_MIN_XY_CENTER_TO_EDGE_DISTANCE = 325.0f;

        // Maxinum number of triangle faces that can be in any zone-to-material-object
        public static readonly int CONFIG_EQTOWOW_ZONE_MATERIAL_TO_OBJECT_SPLIT_MAX_FACE_TRIANGLE_COUNT = 21800;

        // If populated, only the zones listed here will be generated
        public static readonly List<string> CONFIG_EQTOWOW_RESTRICTD_ZONE_SHORTNAMES_FOR_GENERATION = new List<string>() { };

        // If true, zones for Kunark are generated
        public static readonly bool CONFIG_GENERATE_KUNARK_ZONES = false;

        // If true, zones for Velious are generated
        public static readonly bool CONFIG_GENERATE_VELIOUS_ZONES = false;

        //=====================================================================
        // Objects
        //=====================================================================
        // The minimum size of a bounding box for a static doodad.  Bigger means it can be seen further away
        public static readonly float CONFIG_STATIC_OBJECT_MIN_BOUNDING_BOX_SIZE = 25f;

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
        public static readonly int CONFIG_DBIC_AREATABLE_AREABIT_START = 3800;

        // IDs for AreaTrigger.DBC. These will be generated in ascending order by MapID, and referenced in AzerothCore scripts
        // for teleports as well any other area-based triggers
        public static readonly int CONFIG_DBIC_AREATRIGGERID_START = 6500;

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
