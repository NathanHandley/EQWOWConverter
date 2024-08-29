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
    internal class LightFloatBandDBC : DBCFile
    {
        public class LightRow
        {
            public int Id;
            public int NumOfTimeSlices;
            public int[] TimeValues = new int[16]; // Defaults to zero
            public float[] DataValues = new float[16]; // Defaults to zero

            public LightRow(int id, int numOfTimeSlices)
            {
                Id = id;
                NumOfTimeSlices = numOfTimeSlices;
            }
        }

        public void AddRows(ZoneEnvironmentSettings.ZoneEnvironmentParameters environmentParameters)
        {
            int numOfTimeSlices = environmentParameters.ParametersTimeSlices.Count;
            int startID = (environmentParameters.DBCLightParamsID * 6) - 5;

            // Create rows
            List<LightRow> lightRows = new List<LightRow>();
            lightRows.Add(new LightRow(startID, numOfTimeSlices)); // Fog Distance
            lightRows.Add(new LightRow(startID + 1, numOfTimeSlices)); // Fog Multiplier
            lightRows.Add(new LightRow(startID + 2, numOfTimeSlices)); // Celestial Glow Through
            lightRows.Add(new LightRow(startID + 3, numOfTimeSlices)); // Cloud Density
            lightRows.Add(new LightRow(startID + 4, numOfTimeSlices)); // Unknown 1
            lightRows.Add(new LightRow(startID + 5, numOfTimeSlices)); // Unknown 2

            // Fill data in the rows
            for (int i = 0; i < numOfTimeSlices; i++)
            {
                lightRows[0].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[0].DataValues[i] = environmentParameters.ParametersTimeSlices[i].FogDistance * 36f; // Yards -> Inches
                lightRows[1].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[1].DataValues[i] = environmentParameters.ParametersTimeSlices[i].FogMultiplier;
                lightRows[2].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[2].DataValues[i] = environmentParameters.ParametersTimeSlices[i].CelestialGlowThrough;
                lightRows[3].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[3].DataValues[i] = environmentParameters.ParametersTimeSlices[i].CloudDensity;
                lightRows[4].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[4].DataValues[i] = environmentParameters.ParametersTimeSlices[i].UnknownFloat1;
                lightRows[5].TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                lightRows[5].DataValues[i] = environmentParameters.ParametersTimeSlices[i].UnknownFloat2;
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
                newRow.AddFloat(lightRow.DataValues[0]);
                newRow.AddFloat(lightRow.DataValues[1]);
                newRow.AddFloat(lightRow.DataValues[2]);
                newRow.AddFloat(lightRow.DataValues[3]);
                newRow.AddFloat(lightRow.DataValues[4]);
                newRow.AddFloat(lightRow.DataValues[5]);
                newRow.AddFloat(lightRow.DataValues[6]);
                newRow.AddFloat(lightRow.DataValues[7]);
                newRow.AddFloat(lightRow.DataValues[8]);
                newRow.AddFloat(lightRow.DataValues[9]);
                newRow.AddFloat(lightRow.DataValues[10]);
                newRow.AddFloat(lightRow.DataValues[11]);
                newRow.AddFloat(lightRow.DataValues[12]);
                newRow.AddFloat(lightRow.DataValues[13]);
                newRow.AddFloat(lightRow.DataValues[14]);
                newRow.AddFloat(lightRow.DataValues[15]);
                Rows.Add(newRow);
            }
        }
    }
}
