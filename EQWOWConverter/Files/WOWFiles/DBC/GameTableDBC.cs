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

namespace EQWOWConverter.WOWFiles
{
    internal class GameTableDBC : DBCFile
    {
        protected override void OnPostLoadDataFromDisk()
        {
            // Rows are positional, so save load order for the sort in SaveToDisk
            for (int i = 0; i < Rows.Count; i++)
                Rows[i].SortValue1 = i;
        }

        public float GetSingleFloatValue(int rowIndex)
        {
            // Reads the single float value of a row (all gt tables are one float per record)
            if (rowIndex < 0 || rowIndex >= Rows.Count)
            {
                Logger.WriteError("GameTableDBC GetSingleFloatValue failure for '" + FileName + "', as row index '" + rowIndex + "' falls outside the " + Rows.Count + " loaded rows");
                return 0f;
            }
            return BitConverter.ToSingle(Rows[rowIndex].SourceRawBytes.ToArray(), 0);
        }

        public void CopyClassRows(int sourceClassID, int targetClassID, int rowsPerClass)
        {
            // Copies all values for one class onto another
            int sourceStartRow = (sourceClassID - 1) * rowsPerClass;
            int targetStartRow = (targetClassID - 1) * rowsPerClass;
            if (sourceStartRow + rowsPerClass > Rows.Count || targetStartRow + rowsPerClass > Rows.Count)
            {
                Logger.WriteError("GameTableDBC CopyClassRows failure for '" + FileName + "', as class rows for source class '" + sourceClassID + "' or target class '" + targetClassID + "' fall outside the " + Rows.Count + " loaded rows");
                return;
            }
            for (int i = 0; i < rowsPerClass; i++)
            {
                Rows[targetStartRow + i].SourceRawBytes.Clear();
                Rows[targetStartRow + i].SourceRawBytes.AddRange(Rows[sourceStartRow + i].SourceRawBytes);
            }
        }
    }
}
