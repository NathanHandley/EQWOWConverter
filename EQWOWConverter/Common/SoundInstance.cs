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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class SoundInstance
    {
        protected static int CURRENT_GAMEOBJECTID = Configuration.CONFIG_DBCID_GAMEOBJECT_ID_START;
        protected static int CURRENT_GAMEOBJECTDISPLAYINFOID = Configuration.CONFIG_DBCID_GAMEOBJECTDISPLAYINFO_ID_START;

        public Vector3 Position = new Vector3();
        public bool Is2DSound = false;
        public int Radius = 0;
        public string SoundFileNameDayNoExt = string.Empty;
        public string SoundFileNameNightNoExt = string.Empty;
        public float VolumeDay = 0f;
        public float VolumeNight = 0f;
        public int CooldownInMSDay = 0;
        public int CooldownInMSNight = 0;
        public int CooldownInMSRandom = 0;
        // public Multiplier -- Unsure what this is

        public Sound? Sound = null;
        public int GameObjectID = 0;
        public int GameObjectDisplayInfoID = 0;

        public string GenerateDBCName(string zoneName, int instanceID)
        {
            // Instance ID
            string instanceIDPart;
            if (instanceID < 10)
                instanceIDPart = "00" + instanceID.ToString();
            else if (instanceID < 100)
                instanceIDPart = "0" + instanceID.ToString();
            else
                instanceIDPart = instanceID.ToString();

            // FileName
            string fileNamePart = string.Empty;
            if (SoundFileNameDayNoExt == SoundFileNameNightNoExt)
                fileNamePart = SoundFileNameDayNoExt;
            else
            {
                if (SoundFileNameDayNoExt == string.Empty)
                    fileNamePart = SoundFileNameDayNoExt;
                else if (SoundFileNameNightNoExt == string.Empty)
                    fileNamePart = SoundFileNameNightNoExt;
                else
                {
                    Logger.WriteError("Could not generate name for sound instance for zone '" + zoneName + "' with file names '" + SoundFileNameDayNoExt + "' and '" + SoundFileNameNightNoExt + "'");
                    return "Invalid Name";
                }
            }

            // Dimension
            string dimensionPart = "3D";
            if (Is2DSound == true)
                dimensionPart = "2D";

            // Form the name
            string generatedName = "EQ_" + zoneName + "_SoundInstance" + dimensionPart + "_"+ instanceIDPart + "_" + fileNamePart;
            return generatedName;
        }

        public void GenerateGameObjectIDs()
        {
            GameObjectID = CURRENT_GAMEOBJECTID;
            CURRENT_GAMEOBJECTID++;
            if (CURRENT_GAMEOBJECTID > Configuration.CONFIG_DBCID_GAMEOBJECT_ID_END)
                throw new Exception("CURRENT_GAMEOBJECTID has extended the maximum set in Configuration.CONFIG_DBCID_GAMEOBJECT_ID_END");
            GameObjectDisplayInfoID = CURRENT_GAMEOBJECTDISPLAYINFOID;
            CURRENT_GAMEOBJECTDISPLAYINFOID++;
        }
    }
}
