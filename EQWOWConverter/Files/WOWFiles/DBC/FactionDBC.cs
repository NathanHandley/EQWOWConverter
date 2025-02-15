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

using EQWOWConverter.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

// WOW Faction Bands:
// - Hated: -42,000 to -6,000
// - Hostile: -5,999 to -3,000
// - Unfriendly: -2,999 to -1
// - Neutral: 0 to 2,999
// - Friendly: 3,000 to 5,999
// - Honored: 6,000 to 11,999
// - Revered: 12,000 to 20,999
// - Exalted: 21,000+
// EQ Faction Bands:
// - Scowling (KOS - Kill on Sight): -2000 to -751
// - Threateningly: -750 to -501
// - Dubious: -500 to -101
// - Apprehensive: -100 to -1
// - Indifferent: 0 to 99
// - Amiable: 100 to 499
// - Kindly: 500 to 749
// - Warmly: 750 to 1099
// - Ally: 1100+

namespace EQWOWConverter.WOWFiles
{
    internal class FactionDBC : DBCFile
    {
        public void AddRow(CreatureFaction creatureFaction)
        {
            // Calc flags
            int reputationFlags = 0;
            if (creatureFaction.Name == Configuration.CONFIG_CREATURE_FACTION_ROOT_NAME)
                reputationFlags = 12;
            else
                reputationFlags |= 0x02; // FACTION_FLAG_AT_WAR

            DBCRow newRow = new DBCRow();
            newRow.AddInt(creatureFaction.FactionID); // ID
            newRow.AddInt(creatureFaction.ReputationIndex); // ReputationIndex (Must be a unique number, max 127, no gain -1)
            newRow.AddInt(creatureFaction.GetGood1RaceMask()); // Reputation Race Mask 1 (Netherwing has "1791")
            newRow.AddInt(creatureFaction.GetGood2RaceMask()); // Reputation Race Mask 2
            newRow.AddInt(creatureFaction.GetEvil1RaceMask()); // Reputation Race Mask 3
            newRow.AddInt(creatureFaction.GetEvil2RaceMask()); // Reputation Race Mask 4
            newRow.AddInt(creatureFaction.GetGood1ClassMask()); // Reputation Class Mask 1 (Note: Netherwing has "1535")
            newRow.AddInt(creatureFaction.GetGood2ClassMask()); // Reputation Class Mask 2
            newRow.AddInt(creatureFaction.GetEvil1ClassMask()); // Reputation Class Mask 3
            newRow.AddInt(creatureFaction.GetEvil2ClassMask()); // Reputation Class Mask 4
            newRow.AddInt(creatureFaction.GetGood1BaseRep()); // Reputation Base 1 (Used by Race/Class Mask 1)
            newRow.AddInt(creatureFaction.GetGood2BaseRep()); // Reputation Base 2 (Used by Race/Class Mask 2)
            newRow.AddInt(creatureFaction.GetEvil1BaseRep()); // Reputation Base 3 (Used by Race/Class Mask 3)
            newRow.AddInt(creatureFaction.GetEvil2BaseRep()); // Reputation Base 4 (Used by Race/Class Mask 4)
            newRow.AddInt(reputationFlags); // Reputation Flags 1
            newRow.AddInt(reputationFlags); // Reputation Flags 2
            newRow.AddInt(reputationFlags); // Reputation Flags 3
            newRow.AddInt(reputationFlags); // Reputation Flags 4
            newRow.AddInt(creatureFaction.ParentFactionID); // Parent Faction ID (Faction.ID)
            newRow.AddFloat(0); // ParentFactionMod 1
            newRow.AddFloat(0); // ParentFactionMod 2
            newRow.AddInt(5); // Parent Faction Cap 1 // Unsure why this is 5, but most rows are
            newRow.AddInt(5); // Parent Faction Cap 2 // Unsure why this is 5, but most rows are
            newRow.AddStringLang(creatureFaction.Name); // Name
            newRow.AddStringLang(creatureFaction.Description); // Description
            Rows.Add(newRow);
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Convert any raw data rows to actual data rows (which should be all of them)
            foreach(DBCRow row in Rows)
            {
                // This shouldn't be possible, but control for it just in case
                if (row.SourceRawBytes.Count == 0)
                {
                    Logger.WriteError("FactionDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor); // ID
                row.AddIntFromSourceRawBytes(ref byteCursor); // ReputationIndex
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Race Mask 1
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Race Mask 2
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Race Mask 3
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Race Mask 4
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Class Mask 1
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Class Mask 2
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Class Mask 3
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Class Mask 4
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Base 1
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Base 2
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Base 3
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Base 4
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Flags 1
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Flags 2
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Flags 3
                row.AddIntFromSourceRawBytes(ref byteCursor); // Reputation Flags 4
                row.AddIntFromSourceRawBytes(ref byteCursor); // Parent Faction ID
                row.AddFloatFromSourceRawBytes(ref byteCursor); // Parent Faction Mod 1
                row.AddFloatFromSourceRawBytes(ref byteCursor); // Parent Faction Mod 2
                row.AddIntFromSourceRawBytes(ref byteCursor); // Parent Faction Cap 1
                row.AddIntFromSourceRawBytes(ref byteCursor); // Parent Faction Cap 2
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock); // Name
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock); // Description

                // Purge raw data
                row.SourceRawBytes.Clear();
            }

            // Remove reputationindex from specific rows for reuse
            foreach(DBCRow row in Rows)
            {
                // Fields to clear are as follows (reputation index):
                // "Wildhammer Clan" (8)
                // "Caer Darrow" (34)
                // "REUSE" (82)
                // "Test Faction 1" (85)
                // "Test Faction 2" (86)
                // "Test Faction 3" (87)
                // "Tranquillen Conversion" (100)
                // "Wintersaber Conversion" (101)
                // "Silver Covenant Conversion" (102)
                // "Sunreavers Conversion" (103)

                // Clear out specific rep index fields
                DBCRow.DBCFieldInt repIndexField = (DBCRow.DBCFieldInt)row.AddedFields[1];
                if (repIndexField.Value == 8 || repIndexField.Value == 34 || repIndexField.Value == 82 || repIndexField.Value == 85
                    || repIndexField.Value == 86 || repIndexField.Value == 87 || repIndexField.Value == 100
                    || repIndexField.Value == 101 || repIndexField.Value == 102 || repIndexField.Value == 103)
                { 
                    repIndexField.Value = -1;
                }
            }
        }
    }
}
