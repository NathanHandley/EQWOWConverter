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

namespace EQWOWConverter.Items
{
    internal class ItemDisplayInfo
    {
        public static List<ItemDisplayInfo> ItemDisplayInfos = new List<ItemDisplayInfo>();
        private static int CURRENT_DBCID_ITEMDISPLAYINFO = Configuration.DBCID_ITEMDISPLAYINFO_START;

        public int DBCID = 0;
        public string IconFileNameNoExt = string.Empty;

        public ItemDisplayInfo()
        {
            DBCID = CURRENT_DBCID_ITEMDISPLAYINFO;
            CURRENT_DBCID_ITEMDISPLAYINFO++;
        }

        public static ItemDisplayInfo GetOrCreateItemDisplayInfo(string iconFileNameNoExt)
        {
            foreach(ItemDisplayInfo itemDisplayInfo in ItemDisplayInfos)
            {
                if (itemDisplayInfo.IconFileNameNoExt == iconFileNameNoExt)
                    return itemDisplayInfo;
            }
            ItemDisplayInfo newItemDisplayInfo = new ItemDisplayInfo();
            newItemDisplayInfo.IconFileNameNoExt = iconFileNameNoExt;
            ItemDisplayInfos.Add(newItemDisplayInfo);
            return newItemDisplayInfo;
        }
    }
}
