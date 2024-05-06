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
        // Generator Rules
        // ====================================================================
        // The value EQ verticies multiply by when translated into WOW verticies
        public static readonly float CONFIG_EQTOWOW_WORLD_SCALE = 0.40f;

        // Maximum number of faces that fit into a WMO group before it subdivides
        // - Note: Any more than this (20000) seems to not load
        public static readonly int CONFIG_WOW_MAX_FACES_PER_WMOGROUP = 20000;

        // ====================================================================
        // WOW DBC/File IDs
        // ====================================================================
        // Identifies WMO Groups. Found in WMOAreaTable.dbc and the .wmo files. (~30000-38000)
        public static readonly UInt32 CONFIG_DBCID_WMOGROUPID_START = 30000;

        // Identifies WMO Roots.  Found in WMOAreaTable.dbc and AreaTable.dbc. (~7000-7200)
        public static readonly UInt32 CONFIG_DBCID_WMOID_START = 7000;

        // Identifies Area rows in AreaTable.dbc. (~6000-6200)
        public static readonly UInt32 CONFIG_DBCID_AREAID_START = 6000;

        // Identifies Maps in Map.dbc and MapDifficulty.dbc. (~750-900)
        public static readonly int CONFIG_DBCID_MAPID_START = 750;

        // Specific rows in MapDifficulty.dbc. (~800-922)
        public static readonly int CONFIG_DBCID_MAPDIFFICULTYID_START = 800;

        // ====================================================================
        // AzerothCore Database IDs
        // ====================================================================
        // Start row for `game_tele` records. (~2000-2200)
        public static readonly int CONFIG_GAMETELE_ROWID_START = 2000;


        // ====================================================================
        // BSP Tree Generation
        // ====================================================================

        // BSP tree nodes will stop subdividing when this many (or less) triangles are found
        public static readonly UInt16 CONFIG_BSPTREE_MIN_SPLIT_SIZE = 50;

        // BSP tree nodes won't operate on bounding boxes smaller than this
        public static readonly float CONFIG_BSPTREE_MIN_BOX_SIZE_TOTAL = 1.0f;
    }
}
