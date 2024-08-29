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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class LightIntBandDBC : DBCFile
    {
        public class LightRow
        {
            public int Id;
            public int NumOfTimeSlices;
            public int[] TimeValues = new int[16]; // Defaults to zero
            public int[] DataValues = new int[16]; // Defaults to zero

            public LightRow(int id, int numOfTimeSlices)
            {
                Id = id;
                NumOfTimeSlices = numOfTimeSlices;
            }
        }

        private List<LightRow> rows = new List<LightRow>();

        public void AddRows(ZoneEnvironmentSettings.ZoneEnvironmentParameters environmentParameters)
        {
            int numOfTimeSlices = environmentParameters.ParametersTimeSlices.Count;
            int startID = (environmentParameters.DBCLightParamsID * 18) - 17;

            // Create rows
            List<LightRow> lightRows = new List<LightRow>();
            lightRows.Add(new LightRow(startID, numOfTimeSlices)); // Sky Cast Diffuse Light
            lightRows.Add(new LightRow(startID + 1, numOfTimeSlices)); // Ambient Light
            lightRows.Add(new LightRow(startID + 2, numOfTimeSlices)); // Sky Top Color
            lightRows.Add(new LightRow(startID + 3, numOfTimeSlices)); // Sky Middle Color
            lightRows.Add(new LightRow(startID + 4, numOfTimeSlices)); // Sky Middle-to-Horizon Color
            lightRows.Add(new LightRow(startID + 5, numOfTimeSlices)); // Sky Above Horizon Color
            lightRows.Add(new LightRow(startID + 6, numOfTimeSlices)); // Sky Horizon Color
            lightRows.Add(new LightRow(startID + 7, numOfTimeSlices)); // Fog Color
            lightRows.Add(new LightRow(startID + 8, numOfTimeSlices)); // Unknown 1
            lightRows.Add(new LightRow(startID + 9, numOfTimeSlices)); // Sun Color
            lightRows.Add(new LightRow(startID + 10, numOfTimeSlices)); // Sun Large Halo Color
            lightRows.Add(new LightRow(startID + 11, numOfTimeSlices)); // Cloud Edge Color
            lightRows.Add(new LightRow(startID + 12, numOfTimeSlices)); // Cloud Color
            lightRows.Add(new LightRow(startID + 13, numOfTimeSlices)); // Unknown 2
            lightRows.Add(new LightRow(startID + 14, numOfTimeSlices)); // Ocean Shallow Color
            lightRows.Add(new LightRow(startID + 15, numOfTimeSlices)); // Ocean Deep Color
            lightRows.Add(new LightRow(startID + 16, numOfTimeSlices)); // River Shallow Color
            lightRows.Add(new LightRow(startID + 17, numOfTimeSlices)); // River Deep Color

            // Fill data in the rows
            for (int i = 0; i < numOfTimeSlices; i++)
            {
                lightRows[0].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[0].DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyCastDiffuseLightColor.ToDecimalNoAlpha();
                lightRows[1].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[1].DataValues[i] = environmentParameters.ParametersTimeSlices[i].AmbientLightColor.ToDecimalNoAlpha();
                lightRows[2].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[2].DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyTopColor.ToDecimalNoAlpha();
                lightRows[3].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[3].DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyMiddleColor.ToDecimalNoAlpha();
                lightRows[4].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[4].DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyMiddleToHorizonColor.ToDecimalNoAlpha();
                lightRows[5].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[5].DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyAboveHorizonColor.ToDecimalNoAlpha();
                lightRows[6].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[6].DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyHorizonColor.ToDecimalNoAlpha();
                lightRows[7].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[7].DataValues[i] = environmentParameters.ParametersTimeSlices[i].FogColor.ToDecimalNoAlpha();
                lightRows[8].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[8].DataValues[i] = environmentParameters.ParametersTimeSlices[i].Unknown1Color.ToDecimalNoAlpha();
                lightRows[9].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[9].DataValues[i] = environmentParameters.ParametersTimeSlices[i].SunColor.ToDecimalNoAlpha();
                lightRows[10].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[10].DataValues[i] = environmentParameters.ParametersTimeSlices[i].SunLargeHaloColor.ToDecimalNoAlpha();
                lightRows[11].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[11].DataValues[i] = environmentParameters.ParametersTimeSlices[i].CloudEdgeColor.ToDecimalNoAlpha();
                lightRows[12].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[12].DataValues[i] = environmentParameters.ParametersTimeSlices[i].CloudColor.ToDecimalNoAlpha();
                lightRows[13].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[13].DataValues[i] = environmentParameters.ParametersTimeSlices[i].Unknown2Color.ToDecimalNoAlpha();
                lightRows[14].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[14].DataValues[i] = environmentParameters.ParametersTimeSlices[i].OceanShallowColor.ToDecimalNoAlpha();
                lightRows[15].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[15].DataValues[i] = environmentParameters.ParametersTimeSlices[i].OceanDeepColor.ToDecimalNoAlpha();
                lightRows[16].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[16].DataValues[i] = environmentParameters.ParametersTimeSlices[i].RiverShallowColor.ToDecimalNoAlpha();
                lightRows[17].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[17].DataValues[i] = environmentParameters.ParametersTimeSlices[i].RiverDeepColor.ToDecimalNoAlpha();
            }

            foreach (LightRow lightRow in lightRows)
            {
                DBCRow newRow = new DBCRow();
                newRow.AddInt(lightRow.Id);
                newRow.AddInt(lightRow.NumOfTimeSlices);
                newRow.AddInt(lightRow.TimeValues[0]);
                newRow.AddInt(lightRow.TimeValues[1]);
                newRow.AddInt(lightRow.TimeValues[2]);
                newRow.AddInt(lightRow.TimeValues[3]);
                newRow.AddInt(lightRow.TimeValues[4]);
                newRow.AddInt(lightRow.TimeValues[5]);
                newRow.AddInt(lightRow.TimeValues[6]);
                newRow.AddInt(lightRow.TimeValues[7]);
                newRow.AddInt(lightRow.TimeValues[8]);
                newRow.AddInt(lightRow.TimeValues[9]);
                newRow.AddInt(lightRow.TimeValues[10]);
                newRow.AddInt(lightRow.TimeValues[11]);
                newRow.AddInt(lightRow.TimeValues[12]);
                newRow.AddInt(lightRow.TimeValues[13]);
                newRow.AddInt(lightRow.TimeValues[14]);
                newRow.AddInt(lightRow.TimeValues[15]);
                newRow.AddInt(lightRow.DataValues[0]);
                newRow.AddInt(lightRow.DataValues[1]);
                newRow.AddInt(lightRow.DataValues[2]);
                newRow.AddInt(lightRow.DataValues[3]);
                newRow.AddInt(lightRow.DataValues[4]);
                newRow.AddInt(lightRow.DataValues[5]);
                newRow.AddInt(lightRow.DataValues[6]);
                newRow.AddInt(lightRow.DataValues[7]);
                newRow.AddInt(lightRow.DataValues[8]);
                newRow.AddInt(lightRow.DataValues[9]);
                newRow.AddInt(lightRow.DataValues[10]);
                newRow.AddInt(lightRow.DataValues[11]);
                newRow.AddInt(lightRow.DataValues[12]);
                newRow.AddInt(lightRow.DataValues[13]);
                newRow.AddInt(lightRow.DataValues[14]);
                newRow.AddInt(lightRow.DataValues[15]);
                Rows.Add(newRow);
            }
        }
    }
}
