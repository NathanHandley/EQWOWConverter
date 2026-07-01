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

namespace EQWOWConverter.Creatures
{
    // Correlates to AzerothCore's "UNIT_STAND_STATE"
    internal enum CreatureStandStateType : Int32
    {
        Stand = 0,
        Sit = 1,
        SitChair = 2,
        Sleep = 3,
        SitLowChair = 4,
        SitMediumChair = 5,
        SitHighChair = 6,
        Dead = 7,
        Kneel = 8,
        Submerged = 9
    }
}
