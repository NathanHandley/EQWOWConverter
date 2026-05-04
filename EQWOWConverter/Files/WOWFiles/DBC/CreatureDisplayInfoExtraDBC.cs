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
    internal class CreatureDisplayInfoExtraDBC : DBCFile
    {
        private static readonly object IDLock = new object();
        private static int CurrentID = Configuration.DBCID_CREATUREDISPLAYINFOEXTRA_ID_START;

        public void AddRow(int id, int displayRaceID, int displaySexID, int skinID, int faceID,
            int hairStyleID, int hairColorID, int facialHairID, int chestDisplayID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(displayRaceID); // DisplayRaceID
            newRow.AddInt32(displaySexID); // DisplaySexID
            newRow.AddInt32(skinID); // SkinID
            newRow.AddInt32(faceID); // FaceID
            newRow.AddInt32(hairStyleID); // HairStyleID
            newRow.AddInt32(hairColorID); // HairColorID
            newRow.AddInt32(facialHairID); // FacialHairID
            newRow.AddInt32(0); // Helm ItemDisplayInfo.ID
            newRow.AddInt32(0); // Shoulder ItemDisplayInfo.ID
            newRow.AddInt32(0); // Shirt ItemDisplayInfo.ID
            newRow.AddInt32(chestDisplayID); // Cuirass ItemDisplayInfo.ID
            newRow.AddInt32(0); // Belt ItemDisplayInfo.ID
            newRow.AddInt32(0); // Legs ItemDisplayInfo.ID
            newRow.AddInt32(0); // Boots ItemDisplayInfo.ID
            newRow.AddInt32(0); // Wrist ItemDisplayInfo.ID
            newRow.AddInt32(0); // Gloves ItemDisplayInfo.ID
            newRow.AddInt32(0); // Tabard ItemDisplayInfo.ID
            newRow.AddInt32(0); // Cape ItemDisplayInfo.ID
            newRow.AddInt32(0); // Flags (CanEquipWeapons (0 = no, 1 = yes) ?)
            newRow.AddString("EQ" + id.ToString()); // BakeName
            Rows.Add(newRow);
        }

        public static int GenerateAndGetGetID()
        {
            lock (IDLock)
            {
                int returnID = CurrentID;
                CurrentID++;
                return returnID;
            }
        }
    }
}
