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

using EQWOWConverter.WOWFiles;
using Mysqlx.Expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EQWOWConverter.Creatures;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM creature_template WHERE `entry` >= " + Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `entry` <= " + Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_HIGH + ";";
        }

        public void AddRow(CreatureTemplate creatureTemplate, float scale)
        {
            int npcFlags = 0;
            if (creatureTemplate.MerchantID != 0)
                npcFlags |= 128; // 	0x00000080 = Vendor flag.  TODO: Add Vendor Ammo/Food/Poison/Reagent flags

            SQLRow newRow = new SQLRow();
            newRow.AddInt("entry", creatureTemplate.WOWCreatureTemplateID);
            newRow.AddInt("difficulty_entry_1", 0);
            newRow.AddInt("difficulty_entry_2", 0);
            newRow.AddInt("difficulty_entry_3", 0);
            newRow.AddInt("KillCredit1", 0);
            newRow.AddInt("KillCredit2", 0);
            newRow.AddString("name", 100, creatureTemplate.Name);
            newRow.AddString("subname", 100, creatureTemplate.SubName);
            newRow.AddString("IconName", 100, string.Empty);
            newRow.AddInt("gossip_menu_id", 0);
            newRow.AddInt("minlevel", creatureTemplate.Level);
            newRow.AddInt("maxlevel", creatureTemplate.Level);
            newRow.AddInt("exp", 0); // Unsure what this is used for, but commonly 0 and can be 2 sometimes
            newRow.AddInt("faction", creatureTemplate.WOWFactionTemplateID); // References FactionTemplate.dbc
            newRow.AddInt("npcflag", npcFlags);
            newRow.AddFloat("speed_walk", 1); // 1 is very common, but can be other values
            newRow.AddFloat("speed_run", 1.14286f); // 1.14286 seems common
            newRow.AddFloat("speed_swim", 1);
            newRow.AddFloat("speed_flight", 1);
            newRow.AddFloat("detection_range", 20); // 20 is very common, but see a lot of 18 as well
            newRow.AddFloat("scale", scale);
            newRow.AddInt("rank", Convert.ToInt32(creatureTemplate.Rank));
            newRow.AddInt("dmgschool", 0);
            newRow.AddFloat("DamageModifier", creatureTemplate.DamageMod);
            newRow.AddInt("BaseAttackTime", 2000); // 2,000 very common, but can be lower like 1,500
            newRow.AddInt("RangeAttackTime", 2000);
            newRow.AddFloat("BaseVariance", 1);
            newRow.AddFloat("RangeVariance", 1);
            if (creatureTemplate.HasMana == true)
                newRow.AddInt("unit_class", 2);
            else
                newRow.AddInt("unit_class", 1);
            newRow.AddInt("unit_flags", 0);
            newRow.AddInt("unit_flags2", 2048); // Most have 2048 here, TODO Look into it
            newRow.AddInt("dynamicflags", 0);
            newRow.AddInt("family", 0); // I see other values here, like 1 and 3
            newRow.AddInt("trainer_type", 0);
            newRow.AddInt("trainer_spell", 0);
            newRow.AddInt("trainer_class", 0);
            newRow.AddInt("trainer_race", 0);
            newRow.AddInt("type", 0); // 0: None, 1: Beast, 2: Dragonkin, 3: Demon, 4: Elemental, 5: Giant, 6: Undead, 8: Critter, 9: Mechanical, 10: Non-Specified, 11: Totem, 12: Non-Combat Pet, 13: Gas Cloud
            newRow.AddInt("type_flags", 0); // "Is this minable, tameable, etc"
            newRow.AddInt("lootid", creatureTemplate.WOWLootID);
            newRow.AddInt("pickpocketloot", 0);
            newRow.AddInt("skinloot", 0);
            newRow.AddInt("PetSpellDataId", 0);
            newRow.AddInt("VehicleId", 0);
            newRow.AddInt("mingold", creatureTemplate.MoneyMinInCopper); // "mingold" in the DB, but value is actually copper
            newRow.AddInt("maxgold", creatureTemplate.MoneyMaxInCopper); // "maxgold" in the DB, but value is actually copper
            newRow.AddString("AIName", 64, string.Empty);
            newRow.AddInt("MovementType", 0); // 0 = Stay in Place, 1 = Random Move within wander_distance, 2 = Waypoint Movement
            newRow.AddFloat("HoverHeight", 1);
            newRow.AddFloat("HealthModifier", creatureTemplate.HPMod);
            newRow.AddFloat("ManaModifier", 1);
            newRow.AddFloat("ArmorModifier", 1);
            newRow.AddFloat("ExperienceModifier", 1);
            newRow.AddInt("RacialLeader", 0);
			newRow.AddInt("movementId", 0);
            newRow.AddInt("RegenHealth", 1);
            newRow.AddInt("mechanic_immune_mask", 0);
            newRow.AddInt("spell_school_immune_mask", 0);
            newRow.AddInt("flags_extra", 0);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddInt("VerifiedBuild", 12340);
            Rows.Add(newRow);
        }
    }
}
