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

using EQWOWConverter.Items;

namespace EQWOWConverter.WOWFiles
{
    internal class ItemDisplayInfoDBC : DBCFile
    {
        public void AddRow(ItemDisplayInfo itemDisplayInfo)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(itemDisplayInfo.ItemDisplayInfoDBCID);
            newRow.AddString(itemDisplayInfo.ModelName); // ModelName 1
            newRow.AddString(string.Empty); // ModelName 2
            newRow.AddString(itemDisplayInfo.ModelTexture1); // ModelTexture 1
            newRow.AddString(itemDisplayInfo.ModelTexture2); // ModelTexture 2
            newRow.AddString(itemDisplayInfo.IconFileNameNoExt); // InventoryIcon 1
            newRow.AddString(string.Empty); // InventoryIcon 2
            newRow.AddInt32(itemDisplayInfo.GeosetGroup1); // GeosetGroup 1
            newRow.AddInt32(itemDisplayInfo.GeosetGroup2); // GeosetGroup 2
            newRow.AddInt32(itemDisplayInfo.GeosetGroup3); // GeosetGroup 3
            newRow.AddInt32(0); // Flags
            newRow.AddInt32(0); // SpellVisualID
            newRow.AddInt32(7); // GroupSoundIndex (comes from ItemGroupSounds.dbc)
            newRow.AddInt32(0); // HelmetGeosetVis 1
            newRow.AddInt32(0); // HelmetGeosetVis 2
            newRow.AddString(itemDisplayInfo.ArmorTexture1); // Texture 1
            newRow.AddString(itemDisplayInfo.ArmorTexture2); // Texture 2
            newRow.AddString(itemDisplayInfo.ArmorTexture3); // Texture 3
            newRow.AddString(itemDisplayInfo.ArmorTexture4); // Texture 4
            newRow.AddString(itemDisplayInfo.ArmorTexture5); // Texture 5
            newRow.AddString(itemDisplayInfo.ArmorTexture6); // Texture 6
            newRow.AddString(itemDisplayInfo.ArmorTexture7); // Texture 7
            newRow.AddString(itemDisplayInfo.ArmorTexture8); // Texture 8
            newRow.AddInt32(0); // ItemVisual
            newRow.AddInt32(0); // ParticleColorID
            Rows.Add(newRow);
        }
    }
}
