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
    internal class LightIntBandDBC
    {
        public class Row
        {
            public int Id;
            public int NumOfTimeSlices;
            public int[] TimeValues = new int[16]; // Defaults to zero
            public int[] DataValues = new int[16]; // Defaults to zero

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
            int startID = (environmentParameters.DBCLightParamsID * 18) - 17;

            // Create rows
            Row skyCastDiffuseLightRow = new Row(startID, numOfTimeSlices);
            Row ambientLightRow = new Row(startID + 1, numOfTimeSlices);
            Row skyTopColorRow = new Row(startID + 2, numOfTimeSlices);
            Row skyMiddleColorRow = new Row(startID + 3, numOfTimeSlices);
            Row skyMiddleToHorizonColorRow = new Row(startID + 4, numOfTimeSlices);
            Row skyAboveHorizonColorRow = new Row(startID + 5, numOfTimeSlices);
            Row skyHorizonColorRow = new Row(startID + 6, numOfTimeSlices);
            Row fogColorRow = new Row(startID + 7, numOfTimeSlices);
            Row unknown1Row = new Row(startID + 8, numOfTimeSlices);
            Row sunColorRow = new Row(startID + 9, numOfTimeSlices);
            Row sunLargeHaloColorRow = new Row(startID + 10, numOfTimeSlices);
            Row cloudEdgeColorRow = new Row(startID + 11, numOfTimeSlices);
            Row cloudColorRow = new Row(startID + 12, numOfTimeSlices);
            Row unknown2Row = new Row(startID + 13, numOfTimeSlices);
            Row oceanShallowColorRow = new Row(startID + 14, numOfTimeSlices);
            Row oceanDeepColorRow = new Row(startID + 15, numOfTimeSlices);
            Row riverShallowColorRow = new Row(startID + 16, numOfTimeSlices);
            Row riverDeepColorRow = new Row(startID + 17, numOfTimeSlices);

            // Fill data in the rows
            for (int i = 0; i < numOfTimeSlices; i++)
            {
                skyCastDiffuseLightRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                skyCastDiffuseLightRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyCastDiffuseLightColor.ToDecimalNoAlpha();
                ambientLightRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                ambientLightRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].AmbientLightColor.ToDecimalNoAlpha();
                skyTopColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                skyTopColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyTopColor.ToDecimalNoAlpha();
                skyMiddleColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                skyMiddleColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyMiddleColor.ToDecimalNoAlpha();
                skyMiddleToHorizonColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                skyMiddleToHorizonColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyMiddleToHorizonColor.ToDecimalNoAlpha();
                skyAboveHorizonColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                skyAboveHorizonColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyAboveHorizonColor.ToDecimalNoAlpha();
                skyHorizonColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                skyHorizonColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].SkyHorizonColor.ToDecimalNoAlpha();
                fogColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                fogColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].FogColor.ToDecimalNoAlpha();
                unknown1Row.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                unknown1Row.DataValues[i] = 0;
                sunColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                sunColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].SunColor.ToDecimalNoAlpha();
                sunLargeHaloColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                sunLargeHaloColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].SunLargeHaloColor.ToDecimalNoAlpha();
                cloudEdgeColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                cloudEdgeColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].CloudEdgeColor.ToDecimalNoAlpha();
                cloudColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                cloudColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].CloudColor.ToDecimalNoAlpha();
                unknown2Row.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                unknown2Row.DataValues[i] = 0;
                oceanShallowColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                oceanShallowColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].OceanShallowColor.ToDecimalNoAlpha();
                oceanDeepColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                oceanDeepColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].OceanDeepColor.ToDecimalNoAlpha();
                riverShallowColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                riverShallowColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].RiverShallowColor.ToDecimalNoAlpha();
                riverDeepColorRow.TimeValues[i] = environmentParameters.ParametersTimeSlices[i].HourTimestamp * 120; // Hours -> Half Minutes
                riverDeepColorRow.DataValues[i] = environmentParameters.ParametersTimeSlices[i].RiverDeepColor.ToDecimalNoAlpha();
            }

            // Save rows
            rows.Add(skyCastDiffuseLightRow);
            rows.Add(ambientLightRow);
            rows.Add(skyTopColorRow);
            rows.Add(skyMiddleColorRow);
            rows.Add(skyMiddleToHorizonColorRow);
            rows.Add(skyAboveHorizonColorRow);
            rows.Add(skyHorizonColorRow);
            rows.Add(fogColorRow);
            rows.Add(unknown1Row);
            rows.Add(sunColorRow);
            rows.Add(sunLargeHaloColorRow);
            rows.Add(cloudEdgeColorRow);
            rows.Add(cloudColorRow);
            rows.Add(unknown2Row);
            rows.Add(oceanShallowColorRow);
            rows.Add(oceanDeepColorRow);
            rows.Add(riverShallowColorRow);
            rows.Add(riverDeepColorRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "LightIntBandDBC.csv");

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
