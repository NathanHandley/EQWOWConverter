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

// Defined in https://docs.eqemu.dev/server/npc/spawns/wander-types/
// Comments and enum names were copy/pasted from the link above 4/6/2026

namespace EQWOWConverter.Creatures
{
    internal enum CreaturePathGridWanderType : Int32
    {
        None = -1,
        GridCircular = 0, // Circle the points in order (IE 1 through 10); start back at first.
        GridRandom10 = 1, // Pick 10 random points from the grid set and cycle through.
        GridRandom = 2, // Random; every point is randomly selected from the entire grid set.
        GridPatrol = 3, // Patrol; walk way points in order (i.e. 1 through 10) and then reverse order (i.e. 10 through 1).
        GridOneWayRepop = 4, // Go to the end and depop with spawn timer; walk waypoints in order (i.e. 1 through 10), depop at final point, and repop at initial point after spawn timer.
        GridRand5LoS = 5, // Pick random closest 5 and pick one that's in line of sight.
        GridOneWayDepop = 6, // Go the end and depop without spawn timer; walk waypoints in order (i.e. 1 through 10) and then depop.
        GridCenterPoint = 7, // Initial point as the center, fan out to each grid point creating a "star burst" like pattern (1 - 2 - 1 - 4 - 1 - 3 - 1 - 5, etc.).
        GridRandomCenterPoint = 8, // Initial point as the center, fan out to a random grid point. Causes an NPC to alternate between a random waypoint in grid_entries and a random waypoint marked with the center point column set to true. If no waypoints are marked as a center point, this wander type will not work. There is no numbering requirement or limit for center points--you can have as many as you need.
        GridRandomPath = 9, // Randomly select a waypoint but follow path to it instead of walk directly to it ignoring walls.
    }
}
