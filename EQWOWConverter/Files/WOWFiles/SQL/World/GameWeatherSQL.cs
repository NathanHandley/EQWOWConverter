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

namespace EQWOWConverter.WOWFiles
{
    internal class GameWeatherSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM `game_weather` WHERE `zone` >= " + Configuration.DBCID_AREATABLE_ID_START.ToString() + " AND `zone` <= " + Configuration.DBCID_AREATABLE_ID_END + ";";
        }

        public void AddRow(int zoneID, int winterRainChance, int winterSnowChance, int springRainChance, int springSnowChance, 
            int summerRainChance, int summerSnowChance, int fallRainChance, int fallSnowChance)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("zone", zoneID);
            newRow.AddInt("spring_rain_chance", springRainChance);
            newRow.AddInt("spring_snow_chance", springSnowChance);
            newRow.AddInt("spring_storm_chance", 0);
            newRow.AddInt("summer_rain_chance", summerRainChance);
            newRow.AddInt("summer_snow_chance", summerSnowChance);
            newRow.AddInt("summer_storm_chance", 0);
            newRow.AddInt("fall_rain_chance", fallRainChance);
            newRow.AddInt("fall_snow_chance", fallSnowChance);
            newRow.AddInt("fall_storm_chance", 0);
            newRow.AddInt("winter_rain_chance", winterRainChance);
            newRow.AddInt("winter_snow_chance", winterSnowChance);
            newRow.AddInt("winter_storm_chance", 0);
            newRow.AddString("ScriptName", 64, string.Empty);
            Rows.Add(newRow);
        }

    }
}
