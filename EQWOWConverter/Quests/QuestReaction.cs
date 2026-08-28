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

namespace EQWOWConverter.Quests
{
    internal class QuestReaction
    {
        public static bool TryParseWalkGridValue(string value, out int gridID, out int startNode, out int endNode)
        {
            // Examples of this pattern: "13" walks a whole grid, while "13:0-18" walks nodes 0 through 18
            gridID = -1;
            startNode = -1;
            endNode = -1;
            string[] gridAndRange = value.Split(':');
            if (int.TryParse(gridAndRange[0].Trim(), out gridID) == false)
                return false;
            if (gridAndRange.Length == 1)
                return true;
            string[] range = gridAndRange[1].Split('-');
            if (range.Length != 2)
                return false;
            if (int.TryParse(range[0].Trim(), out startNode) == false)
                return false;
            if (int.TryParse(range[1].Trim(), out endNode) == false)
                return false;
            return true;
        }

        public QuestReactionType ReactionType;
        public string ReactionValue = string.Empty;
        public bool UsePlayerX = false;
        public bool UsePlayerY = false;
        public bool UsePlayerZ = false;
        public bool UsePlayerHeading = false;
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float EQHeading;
        public float WOWOrientation;
        public float AddedX;
        public float AddedY;
        public bool CreatureIsSelf = false;
        public int CreatureEQID = 0;
        public int CreatureWOWID = 0;
        public int SpellEQID = 0;
        public int PathGridID = -1;
        public int PathGridStartNode = -1;
        public int PathGridEndNode = -1;
        public int PathListID = 0;
        public int GameObjectID = -1;
        public int GameObjectEntryID = 0;
        public int GameObjectLifetimeSec = 0;
        public int DelayInMS = 0;
        public bool UseNpcX = false;
        public bool UseNpcY = false;
        public bool UseNpcZ = false;
        public bool UseNpcHeading = false;
        public bool MovementIsRun = false;
        public bool FiresOnArrival = false;
    }
}
