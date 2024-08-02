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

namespace EQWOWConverter.Files.WOWFiles
{
    internal class LightFloatBandDBC
    {
        public class Row
        {
            public int Id;
            public int NumOfTimeSlices;
            public int[] TimeValues = new int[16]; // Defaults to zero
            public float[] DataValues = new float[16]; // Defaults to zero

            public Row(int id, int numOfTimeSlices)
            {
                Id = id;
                NumOfTimeSlices = numOfTimeSlices;
            }
        }

        private List<Row> rows = new List<Row>();

        public void AddRows(ZoneEnvironmentSettings.ZoneEnvironmentParameters environmentParameters)
        {
            int numOfTimeSlices = environmentParameters.ParametersTimeSlices.Count;
            int startID = (environmentParameters.DBCLightParamsID * 6) - 5;

            // Create rows
            Row fogDistanceRow = new Row(startID, numOfTimeSlices);
            Row fogMultiplierRow = new Row(startID + 1, numOfTimeSlices);
            Row celestialGlowThroughRow = new Row(startID + 2, numOfTimeSlices);
            Row cloudDensityRow = new Row(startID + 3, numOfTimeSlices);
            Row unknown1Row = new Row(startID + 4, numOfTimeSlices);
            Row unknown2Row = new Row(startID + 5, numOfTimeSlices);

            // Fill data in the rows
            for(int i = 0; i < numOfTimeSlices; i++)
            {
                fogDistanceRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                fogDistanceRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].FogDistance * 36f; // Yards -> Inches
                fogMultiplierRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                fogMultiplierRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].FogMultiplier;
                celestialGlowThroughRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                celestialGlowThroughRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].CelestialGlowThrough;
                cloudDensityRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                cloudDensityRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].CloudDensity;
                unknown1Row.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                unknown1Row.DataValues[i] = 0.95f;
                unknown2Row.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                unknown2Row.DataValues[i] = 1f;
            }

            // Save rows
            rows.Add(fogDistanceRow);
            rows.Add(fogMultiplierRow);
            rows.Add(celestialGlowThroughRow);
            rows.Add(cloudDensityRow);
            rows.Add(unknown1Row);
            rows.Add(unknown2Row);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "LightFloatBandDBC.csv");

            // Add each row of data
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Row row in rows)
            {
                stringBuilder.Append("\"" + row.Id.ToString() + "\"");
                stringBuilder.Append(",\"" + row.NumOfTimeSlices.ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[0].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[1].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[2].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[3].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[4].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[5].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[6].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[7].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[8].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[9].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[10].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[11].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[12].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[13].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[14].ToString() + "\"");
                stringBuilder.Append(",\"" + row.TimeValues[15].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[0].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[1].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[2].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[3].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[4].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[5].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[6].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[7].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[8].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[9].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[10].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[11].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[12].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[13].ToString() + "\"");
                stringBuilder.Append(",\"" + row.DataValues[14].ToString() + "\"");
                stringBuilder.AppendLine(",\"" + row.DataValues[15].ToString() + "\"");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
