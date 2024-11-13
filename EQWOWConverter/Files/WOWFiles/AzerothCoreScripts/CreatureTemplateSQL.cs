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
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureTemplateSQL
    {
        public class Row
        {
            public int EntryID;
			public int DifficultyEntry1 = 0;
            public int DifficultyEntry2 = 0;
            public int DifficultyEntry3 = 0;
			public int KillCredit1 = 0;
			public int KillCredit2 = 0;
            public string Name = string.Empty; // Max 100 characters
			public string SubName = string.Empty; // Max 100 characters
			public string IconName = string.Empty; // Max 100 characters
			public int GossipMenuID = 0;
			public int MinLevel = 1;
			public int MaxLevel = 1;
			public int Exp = 0; // Unsure what this is used for, but commonly 0 and can be 2 sometimes
			public int Faction = 7; // Faction 7 is 'creature', references Faction.dbc
			public int NPCFlag = 0;
			public float SpeedWalk = 1f; // 1 is very common, but can be other values
			public float SpeedRun = 1.14286f; // 1.14286 seems common
			public float SpeedSwim = 1;
			public float SpeedFlight = 1;
			public float DetectionRange = 20; // 20 is very common, but see a lot of 18 as well
			public float Scale = 1;
			public int Rank = 0;
			public int DamageSchool = 0;
			public float DamageModifier = 1f;
			public int BaseAttackTime = 2000; // 2,000 very common, but can be lower like 1,500
			public int RangeAttackTime = 2000;
			public float BaseVariance = 1;
			public float RangeVariance = 1;
			public int UnitClass = 1;
			public int UnitFlags1 = 0; // TODO: Figure out what these are
			public int UnitFlags2 = 2048; // Most have 2048 here, TODO Look into it
			public int DynamicFlags = 0;
			public int Family = 0; // I see other values here, like 1 and 3
			public int TrainerType = 0;
			public int TrainerSpell = 0;
			public int TrainerClass = 0;
			public int TrainerRace = 0;
			public int Type = 0; // 0: None, 1: Beast, 2: Dragonkin, 3: Demon, 4: Elemental, 5: Giant, 6: Undead, 8: Critter, 9: Mechanical, 10: Non-Specified, 11: Totem, 12: Non-Combat Pet, 13: Gas Cloud
			public int TypeFlags = 0; // "Is this minable, tameable, etc"
			public int LootID = 0;
			public int PickpocketLoot = 0;
			public int SkinLoot = 0;
			public int PetSpellDataID = 0;
			public int VehicleID = 0;
			public int MinGold = 0;
			public int MaxGold = 0;
			public string AIName = string.Empty; // Lots of rows have "SmartAI"
			public int MovementType = 0; // 0 = Stay in Place, 1 = Random Move within wander_distance, 2 = Waypoint Movement
			public float HoverHeight = 1;
			public float HealthModifier = 1;
			public float ManaModifier = 1;
			public float ArmorModifier = 1;
			public float ExperienceModifier = 1;
			public int RacialLeader = 0;
			public int MovementID = 0;
			public int RegenHealth = 1;
			public int MechanicImmuneMask = 0;
			public int SpellSchoolImmuneMask = 0;
			public int FlagsExtra = 0;
			public string ScriptName = string.Empty;
			public int VerifiedBuild = 12340;
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
            stringBuilder.AppendLine("DELETE FROM creature_template WHERE `entry` >= " + Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `entry` <= " + Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_HIGH + " ;");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `creature_template` (`entry`, `difficulty_entry_1`, `difficulty_entry_2`, `difficulty_entry_3`, `KillCredit1`, `KillCredit2`, `name`, `subname`, `IconName`, `gossip_menu_id`, `minlevel`, `maxlevel`, `exp`, `faction`, `npcflag`, `speed_walk`, `speed_run`, `speed_swim`, `speed_flight`, `detection_range`, `scale`, `rank`, `dmgschool`, `DamageModifier`, `BaseAttackTime`, `RangeAttackTime`, `BaseVariance`, `RangeVariance`, `unit_class`, `unit_flags`, `unit_flags2`, `dynamicflags`, `family`, `trainer_type`, `trainer_spell`, `trainer_class`, `trainer_race`, `type`, `type_flags`, `lootid`, `pickpocketloot`, `skinloot`, `PetSpellDataId`, `VehicleId`, `mingold`, `maxgold`, `AIName`, `MovementType`, `HoverHeight`, `HealthModifier`, `ManaModifier`, `ArmorModifier`, `ExperienceModifier`, `RacialLeader`, `movementId`, `RegenHealth`, `mechanic_immune_mask`, `spell_school_immune_mask`, `flags_extra`, `ScriptName`, `VerifiedBuild`) VALUES (");
                stringBuilder.Append(row.EntryID.ToString());
                stringBuilder.Append(", " + row.DifficultyEntry1.ToString());
                stringBuilder.Append(", " + row.DifficultyEntry2.ToString());
                stringBuilder.Append(", " + row.DifficultyEntry3.ToString());
                stringBuilder.Append(", " + row.KillCredit1.ToString());
                stringBuilder.Append(", " + row.KillCredit2.ToString());
                stringBuilder.Append(", \"" + row.Name + "\"");
                stringBuilder.Append(", \"" + row.SubName + "\"");
                stringBuilder.Append(", \"" + row.IconName + "\"");
                stringBuilder.Append(", " + row.GossipMenuID.ToString());
                stringBuilder.Append(", " + row.MinLevel.ToString());
                stringBuilder.Append(", " + row.MaxLevel.ToString());
                stringBuilder.Append(", " + row.Exp.ToString());
                stringBuilder.Append(", " + row.Faction.ToString());
                stringBuilder.Append(", " + row.NPCFlag.ToString());
                stringBuilder.Append(", " + row.SpeedWalk.ToString());
                stringBuilder.Append(", " + row.SpeedRun.ToString());
                stringBuilder.Append(", " + row.SpeedSwim.ToString());
                stringBuilder.Append(", " + row.SpeedFlight.ToString());
                stringBuilder.Append(", " + row.DetectionRange.ToString());
                stringBuilder.Append(", " + row.Scale.ToString());
                stringBuilder.Append(", " + row.Rank.ToString());
                stringBuilder.Append(", " + row.DamageSchool.ToString());
                stringBuilder.Append(", " + row.DamageModifier.ToString());
                stringBuilder.Append(", " + row.BaseAttackTime.ToString());
                stringBuilder.Append(", " + row.RangeAttackTime.ToString());
                stringBuilder.Append(", " + row.BaseVariance.ToString());
                stringBuilder.Append(", " + row.RangeVariance.ToString());
                stringBuilder.Append(", " + row.UnitClass.ToString());
                stringBuilder.Append(", " + row.UnitFlags1.ToString());
                stringBuilder.Append(", " + row.UnitFlags2.ToString());
                stringBuilder.Append(", " + row.DynamicFlags.ToString());
                stringBuilder.Append(", " + row.Family.ToString());
                stringBuilder.Append(", " + row.TrainerType.ToString());
                stringBuilder.Append(", " + row.TrainerSpell.ToString());
                stringBuilder.Append(", " + row.TrainerClass.ToString());
                stringBuilder.Append(", " + row.TrainerRace.ToString());
                stringBuilder.Append(", " + row.Type.ToString());
                stringBuilder.Append(", " + row.TypeFlags.ToString());
                stringBuilder.Append(", " + row.LootID.ToString());
                stringBuilder.Append(", " + row.PickpocketLoot.ToString());
                stringBuilder.Append(", " + row.SkinLoot.ToString());
                stringBuilder.Append(", " + row.PetSpellDataID.ToString());
                stringBuilder.Append(", " + row.VehicleID.ToString());
                stringBuilder.Append(", " + row.MinGold.ToString());
                stringBuilder.Append(", " + row.MaxGold.ToString());
                stringBuilder.Append(", \"" + row.AIName + "\"");
                stringBuilder.Append(", " + row.MovementType.ToString());
                stringBuilder.Append(", " + row.HoverHeight.ToString());
                stringBuilder.Append(", " + row.HealthModifier.ToString());
                stringBuilder.Append(", " + row.ManaModifier.ToString());
                stringBuilder.Append(", " + row.ArmorModifier.ToString());
                stringBuilder.Append(", " + row.ExperienceModifier.ToString());
                stringBuilder.Append(", " + row.RacialLeader.ToString());
                stringBuilder.Append(", " + row.MovementID.ToString());
                stringBuilder.Append(", " + row.RegenHealth.ToString());
                stringBuilder.Append(", " + row.MechanicImmuneMask.ToString());
                stringBuilder.Append(", " + row.SpellSchoolImmuneMask.ToString());
                stringBuilder.Append(", " + row.FlagsExtra.ToString());
                stringBuilder.Append(", \"" + row.ScriptName + "\"");
                stringBuilder.Append(", " + row.VerifiedBuild.ToString());
                stringBuilder.AppendLine(");");
            }         

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
