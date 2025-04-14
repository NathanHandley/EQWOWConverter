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
    internal class TransportAnimationDBC : DBCFile
    {
        private static int CUR_ID = Configuration.DBCID_TRANSPORTANIMATION_ID_START;

        public void AddRow(int gameObjectTemplateID, int timestampInMS, float posX, float posY, float posZ, int animationSequenceID)
        {
            int id = CUR_ID;
            CUR_ID++;

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(gameObjectTemplateID); // TransportID (game object template ID)
            newRow.AddInt32(timestampInMS); // TimeIndex
            newRow.AddFloat(posX); // PosX (delta move amount?  Is it absolute relative to 0,0 of object?)
            newRow.AddFloat(posY); // PosY (delta move amount?  Is it absolute relative to 0,0 of object?)
            newRow.AddFloat(posZ); // PosZ (delta move amount?  Is it absolute relative to 0,0 of object?)
            newRow.AddInt32(animationSequenceID); // SequenceID (Animation type. 0 = standing)

            // Sorting
            newRow.SortValue1 = gameObjectTemplateID;
            newRow.SortValue2 = timestampInMS;
            newRow.SortValue3 = id;
            Rows.Add(newRow);
        }
    }
}
