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
    internal class QuestCompletionFactionReward : IEquatable<QuestCompletionFactionReward>, IComparable<QuestCompletionFactionReward>
    {
        public int EQFactionID = 0;
        public int WOWFactionID = 0;
        public int SortOrder = 0;
        public int CompletionRewardValue = 0;

        public int CompareTo(QuestCompletionFactionReward? other)
        {
            if (other == null)
                return 1;
            if (EQFactionID != other.EQFactionID)
                return EQFactionID.CompareTo(other.EQFactionID);
            else if (WOWFactionID != other.WOWFactionID)
                return WOWFactionID.CompareTo(other.WOWFactionID);
            else if (CompletionRewardValue != other.CompletionRewardValue)
                return CompletionRewardValue.CompareTo(other.CompletionRewardValue);
            return SortOrder.CompareTo(other.SortOrder);
        }

        public bool Equals(QuestCompletionFactionReward? other)
        {
            if (other == null) return false;
            if (EQFactionID != other.EQFactionID) return false;
            if (WOWFactionID != other.WOWFactionID) return false;
            if (SortOrder != other.SortOrder) return false;
            if (CompletionRewardValue != other.CompletionRewardValue) return false;
            return true;
        }
    }
}
