//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

using System.Text;
using EQWOWConverter.Common;

namespace EQWOWConverter.WOWFiles
{
    internal class PlayerTotemModelSQL : SQLFile
    {
        private static readonly List<RaceType> RACES_WITH_STOCK_TOTEM_MODELS = new List<RaceType>()
            { RaceType.Orc, RaceType.Dwarf, RaceType.Tauren, RaceType.Troll, RaceType.Draenei };
        private static readonly Dictionary<int, int> ORC_MODEL_IDS_BY_TOTEM_ID = new Dictionary<int, int>()
            { { 1, 30758 }, { 2, 30757 }, { 3, 30759 }, { 4, 30756 } };

        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("DELETE FROM `player_totem_model` WHERE `RaceID` IN (");
            List<RaceType> raceTypes = GetRacesNeedingTotemModels();
            for (int i = 0; i < raceTypes.Count; i++)
            {
                stringBuilder.Append(Convert.ToInt32(raceTypes[i]).ToString());
                if (i < raceTypes.Count - 1)
                    stringBuilder.Append(", ");
            }
            stringBuilder.Append(");");
            return stringBuilder.ToString();
        }

        public void AddRowsForRacesWithoutStockModels()
        {
            foreach (RaceType raceType in GetRacesNeedingTotemModels())
            {
                foreach (var modelIDByTotemID in ORC_MODEL_IDS_BY_TOTEM_ID)
                {
                    SQLRow newRow = new SQLRow();
                    newRow.AddInt("TotemID", modelIDByTotemID.Key);
                    newRow.AddInt("RaceID", Convert.ToInt32(raceType));
                    newRow.AddInt("ModelID", modelIDByTotemID.Value);
                    Rows.Add(newRow);
                }
            }
        }

        private static List<RaceType> GetRacesNeedingTotemModels()
        {
            List<RaceType> raceTypes = new List<RaceType>();
            foreach (RaceType raceType in Enum.GetValues(typeof(RaceType)))
            {
                if (raceType == RaceType.All)
                    continue;
                if (RACES_WITH_STOCK_TOTEM_MODELS.Contains(raceType) == true)
                    continue;
                raceTypes.Add(raceType);
            }
            return raceTypes;
        }
    }
}
