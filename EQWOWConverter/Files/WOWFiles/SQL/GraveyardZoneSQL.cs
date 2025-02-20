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

namespace EQWOWConverter.WOWFiles
{
    internal class GraveyardZoneSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM graveyard_zone WHERE `ID` >= " + Configuration.CONFIG_DBCID_WORLDSAFELOCS_ID_START + " AND `ID` <= " + Configuration.CONFIG_DBCID_WORLDSAFELOCS_ID_END + ";";
        }

        public void AddRow(ZonePropertiesGraveyard graveyard, int zoneAreaID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", graveyard.WorldSafeLocsDBCID);
            newRow.AddInt("GhostZone", zoneAreaID);
            newRow.AddFloat("Faction", 0); // 0 = Any, 67 = Horde, 469 = Alliance
            newRow.AddString("Comment", graveyard.Description);
            Rows.Add(newRow);
        }
    }
}
