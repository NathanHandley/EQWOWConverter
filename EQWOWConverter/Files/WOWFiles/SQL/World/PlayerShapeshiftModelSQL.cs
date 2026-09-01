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
using EQWOWConverter.Player;

namespace EQWOWConverter.WOWFiles
{
    internal class PlayerShapeshiftModelSQL : SQLFile
    {
        private static readonly List<RaceType> RACES_WITH_STOCK_SHAPESHIFT_MODELS = new List<RaceType>()
            { RaceType.NightElf, RaceType.Tauren };

        private static readonly int CUSTOMIZATION_ID_DEFAULT = 255; // Anything but a hair/skin color specific model
        private static readonly int GENDER_ID_NONE = 2;             // Applies to both genders

        // Display IDs of the forms that have a different model per side, by ShapeshiftForm
        private static readonly Dictionary<int, (int, int)> ALLIANCE_AND_HORDE_MODEL_IDS_BY_FORM_ID = new Dictionary<int, (int, int)>()
        {
            { 1,  (892, 8571) },     // FORM_CAT
            { 5,  (2281, 2289) },    // FORM_BEAR
            { 8,  (2281, 2289) },    // FORM_DIREBEAR
            { 27, (21243, 21244) },  // FORM_FLIGHT_EPIC
            { 29, (20857, 20872) }   // FORM_FLIGHT
        };

        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("DELETE FROM `player_shapeshift_model` WHERE `RaceID` IN (");
            List<RaceType> raceTypes = GetRacesNeedingShapeshiftModels();
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
            foreach (RaceType raceType in GetRacesNeedingShapeshiftModels())
            {
                bool isAllianceRace = PlayerWOWRaceProperties.IsAllianceRace(raceType);
                foreach (var modelIDsByFormID in ALLIANCE_AND_HORDE_MODEL_IDS_BY_FORM_ID)
                {
                    SQLRow newRow = new SQLRow();
                    newRow.AddInt("ShapeshiftID", modelIDsByFormID.Key);
                    newRow.AddInt("RaceID", Convert.ToInt32(raceType));
                    newRow.AddInt("CustomizationID", CUSTOMIZATION_ID_DEFAULT);
                    newRow.AddInt("GenderID", GENDER_ID_NONE);
                    newRow.AddInt("ModelID", isAllianceRace == true ? modelIDsByFormID.Value.Item1 : modelIDsByFormID.Value.Item2);
                    Rows.Add(newRow);
                }
            }
        }

        private static List<RaceType> GetRacesNeedingShapeshiftModels()
        {
            List<RaceType> raceTypes = new List<RaceType>();
            foreach (RaceType raceType in Enum.GetValues(typeof(RaceType)))
            {
                if (raceType == RaceType.All)
                    continue;
                if (RACES_WITH_STOCK_SHAPESHIFT_MODELS.Contains(raceType) == true)
                    continue;
                raceTypes.Add(raceType);
            }
            return raceTypes;
        }
    }
}
