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

namespace EQWOWConverter.Zones
{
    // Potential options
    // - Group on Z height (bottom up?)
    // - Group on normal direction ('floors' vs 'walls'?)
    // - Group based on range (start in a corner and group nearest
    // - Group connected triangles (stop if there are no connected)
    internal enum WorldModelObjectGenerationType
    {
        BY_MATERIAL,        // Group faces by material
        BY_MAP_CHUNK,       // Group by EQ MapChunks
        BY_XY_REGION        // Generate by breaking up the map into x/y regions (ignoring Z)
    }
}
