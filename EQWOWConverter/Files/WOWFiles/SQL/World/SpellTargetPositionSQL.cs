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

using EQWOWConverter.Common;

namespace EQWOWConverter.WOWFiles
{
    internal class SpellTargetPositionSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return string.Concat("DELETE FROM `spell_target_position` WHERE `MapID` >= ", Configuration.DBCID_MAP_ID_START, " AND `MapID` <= " + Configuration.DBCID_MAP_ID_END + ";");
        }

        public void AddRow(int spellID, int effectIndex, int mapID, Vector3 position, float orientation)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", spellID);
            newRow.AddInt("EffectIndex", effectIndex);
            newRow.AddInt("MapID", mapID);
            newRow.AddFloat("PositionX", position.X);
            newRow.AddFloat("PositionY", position.Y);
            newRow.AddFloat("PositionZ", position.Z);
            newRow.AddFloat("Orientation", orientation);
            newRow.AddInt("VerifiedBuild", 0);
            Rows.Add(newRow);
        }
    }
}
