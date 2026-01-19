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

using EQWOWConverter.GameObjects;

namespace EQWOWConverter.WOWFiles
{
    internal class GameObjectTemplateSQL : SQLFile
    {
        //private static int CUR_ID = Configuration.SQL_GAMEOBJECTTEMPLATE_ID_START;

        public override string DeleteRowSQL()
        {
            return "DELETE FROM gameobject_template WHERE `entry` >= " + Configuration.SQL_GAMEOBJECTTEMPLATE_ID_START.ToString() + " AND `entry` <= " + Configuration.SQL_GAMEOBJECTTEMPLATE_ID_END + ";";
        }

        public void AddRowForTransportShip(int entryID, int displayID, string name, int taxiPathID, int spawnMap, int moveSpeed)
        {
            AddRow(entryID, 15, displayID, name, taxiPathID, moveSpeed, Configuration.TRANSPORT_ACCELERATION, spawnMap, 0, 1, string.Empty);
        }

        public void AddRowForTransportLift(int entryID, int displayID, string name, int endTimestamp)
        {
            AddRow(entryID, 11, displayID, name, endTimestamp, 0, 0, 0, 0, 1, string.Empty);
        }

        public void AddRowForTransportLiftTrigger(int entryID, int displayID, string name, int resetInMS)
        {
            AddRow(entryID, 1, displayID, name, 0, 0, resetInMS, 0, 0, 1, string.Empty);
        }

        public void AddRowForGameObject(string name, GameObject gameObject)
        {
            string aiName = string.Empty;
            if (gameObject.TriggerGameObjectGUID != 0 || gameObject.ObjectType == GameObjectType.Teleport)
                aiName = "SmartGameObjectAI";

            switch (gameObject.ObjectType)
            {
                case GameObjectType.Mailbox:
                    {
                        AddRow(gameObject.GameObjectTemplateEntryID,
                            19, // Mailbox
                            gameObject.GameObjectDisplayInfoID, 
                            name,
                            0, 0, 0, 0, 0, 
                            gameObject.Scale, aiName);
                    }
                    break;
                case GameObjectType.Bridge:
                    {
                        AddRow(gameObject.GameObjectTemplateEntryID,
                            0, // Door
                            gameObject.GameObjectDisplayInfoID, name,
                            0, // Start open
                            0, // "ID" from Lock.dbc
                            gameObject.CloseTimeInMS, // Autoclose time in MS
                            1, // "Area of Interest" is set to infinite (see from any distance)
                            0, gameObject.Scale, aiName);
                    }
                    break;
                case GameObjectType.Door:
                    {
                        AddRow(gameObject.GameObjectTemplateEntryID, 
                            0, // Door
                            gameObject.GameObjectDisplayInfoID, name, 
                            0, // Start open
                            0, // "ID" from Lock.dbc
                            gameObject.CloseTimeInMS, // Autoclose time in MS
                            1, // "Area of Interest" is set to infinite (see from any distance)
                            0, gameObject.Scale, aiName);
                    } break;
                case GameObjectType.Teleport:
                    {
                        AddRow(gameObject.GameObjectTemplateEntryID,
                            0, // Door <- Overrides the visibility distance making it visible from very far away, but should be "10" (Goober)
                            gameObject.GameObjectDisplayInfoID, name,
                            0, // Start clickable
                            0, // "ID" from Lock.dbc
                            50, // Autoclose time in MS (which is the 'make reusable time' in this case)
                            1, // "Area of Interest" is set to infinite (see from any distance)
                            0, gameObject.Scale, aiName);
                    } break;
                case GameObjectType.TradeskillFocus:
                    {
                        int spellFocusTypeID = 0;
                        switch (gameObject.TradeskillFocusType)
                        {
                            case GameObjectTradeskillFocusType.CookingFire: spellFocusTypeID = 4; break;
                            case GameObjectTradeskillFocusType.Forge: spellFocusTypeID = 3; break;
                            default: break; // Do Nothing
                        }
                        AddRow(gameObject.GameObjectTemplateEntryID,
                            8, // Spell focus (forge, etc)
                            gameObject.GameObjectDisplayInfoID, name,
                            spellFocusTypeID, // Fire vs Forge
                            Configuration.OBJECT_GAMEOBJECT_TRADESKILLFOCUS_EFFECT_AREA_MIN_SIZE,
                            0, 0, 0, gameObject.Scale, aiName);
                        } break;
                default:
                    {
                        AddRow(gameObject.GameObjectTemplateEntryID, 0, gameObject.GameObjectDisplayInfoID, name, 0, 0, 0, 0, 0, gameObject.Scale, aiName);
                    } break;
            }
        }

        public void AddRow(int entryID, int type, int displayID, string name, int data0, int data1, int data2, int data6, int data10, float scale, string aiName)
        {
            SQLRow newRow = new SQLRow();
			newRow.AddInt("entry", entryID);
            newRow.AddInt("type", type); // 10 = ActiveDoodad (button / lever),  11 = Transport (lift), 15 = Mobile Transport (ship)
            newRow.AddInt("displayId", displayID);
			newRow.AddString("name", 100, name);
            newRow.AddString("IconName", 100, string.Empty);
            newRow.AddString("castBarCaption", 100, string.Empty);
            newRow.AddString("unk1", 100, string.Empty);
			newRow.AddFloat("size", scale);
            newRow.AddInt("Data0", data0);
            newRow.AddInt("Data1", data1);
            newRow.AddInt("Data2", data2); 
            newRow.AddInt("Data3", 0);
            newRow.AddInt("Data4", 0);
            newRow.AddInt("Data5", 0); // Transport physics, 0 or 1
            newRow.AddInt("Data6", data6);
            newRow.AddInt("Data7", 0);
            newRow.AddInt("Data8", 0);
            newRow.AddInt("Data9", 0);
            newRow.AddInt("Data10", data10);
            newRow.AddInt("Data11", 0);
            newRow.AddInt("Data12", 0);
            newRow.AddInt("Data13", 0);
            newRow.AddInt("Data14", 0);
            newRow.AddInt("Data15", 0);
            newRow.AddInt("Data16", 0);
            newRow.AddInt("Data17", 0);
            newRow.AddInt("Data18", 0);
            newRow.AddInt("Data19", 0);
            newRow.AddInt("Data20", 0);
            newRow.AddInt("Data21", 0);
            newRow.AddInt("Data22", 0);
            newRow.AddInt("Data23", 0);
            newRow.AddString("AIName", 64, aiName);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddInt("VerifiedBuild", 12340);            
            Rows.Add(newRow);
        }

        //public static int GenerateID()
        //{
        //    int id = CUR_ID;
        //    CUR_ID++;
        //    return id;
        //}
    }
}
