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
        public QuestReactionType ReactionType;
        public string ReactionValue = string.Empty;
        public bool UsePlayerX = false;
        public bool UsePlayerY = false;
        public bool UsePlayerZ = false;
        public bool UsePlayerHeading = false;
        public float X;
        public float Y;
        public float Z;
        public float EQHeading;
        public float WOWOrientation;
        public float AddedX;
        public float AddedY;
        public bool TargetSelf = false;
        public int TargetCreatureEQID = 0;
        public int TargetCreatureWOWID = 0;
    }
}
