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

using EQWOWConverter.WOWFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureTemplateSQL
    {
        public class Row
        {
            public int EntryID;
            public string Name = string.Empty;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int entryID, string name)
        {
            Row newRow = new Row();
            newRow.EntryID = entryID;
            newRow.Name = name;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "CreatureTemplate.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM creature_template WHERE `CreatureID` >= " + Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `CreatureID` <= " + Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_HIGH + " ;");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `creature_template` (`entry`, `difficulty_entry_1`, `difficulty_entry_2`, `difficulty_entry_3`, `KillCredit1`, `KillCredit2`, `name`, `subname`, `IconName`, `gossip_menu_id`, `minlevel`, `maxlevel`, `exp`, `faction`, `npcflag`, `speed_walk`, `speed_run`, `speed_swim`, `speed_flight`, `detection_range`, `scale`, `rank`, `dmgschool`, `DamageModifier`, `BaseAttackTime`, `RangeAttackTime`, `BaseVariance`, `RangeVariance`, `unit_class`, `unit_flags`, `unit_flags2`, `dynamicflags`, `family`, `trainer_type`, `trainer_spell`, `trainer_class`, `trainer_race`, `type`, `type_flags`, `lootid`, `pickpocketloot`, `skinloot`, `PetSpellDataId`, `VehicleId`, `mingold`, `maxgold`, `AIName`, `MovementType`, `HoverHeight`, `HealthModifier`, `ManaModifier`, `ArmorModifier`, `ExperienceModifier`, `RacialLeader`, `movementId`, `RegenHealth`, `mechanic_immune_mask`, `spell_school_immune_mask`, `flags_extra`, `ScriptName`, `VerifiedBuild`) VALUES (");
                stringBuilder.Append(row.EntryID.ToString());
                stringBuilder.Append("1, 0, 0, 0, 0, 0, '");
                stringBuilder.Append(row.Name.ToString());
                stringBuilder.AppendLine("', 'Visual', NULL, 0, 1, 80, 0, 35, 0, 0.91, 1.14286, 1, 1, 18, 1, 0, 0, 1, 2000, 2000, 1, 1, 1, 0, 2048, 0, 0, 0, 0, 0, 0, 8, 0, 1, 0, 0, 0, 0, 0, 0, '', 0, 1, 0.0125, 1, 1, 1, 0, 0, 1, 0, 0, 130, '', 1);");
            }            

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
