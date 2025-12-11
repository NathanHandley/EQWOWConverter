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

namespace EQWOWConverter.Zones.Properties
{
    internal class TempleOfDrogaZoneProperties : ZoneProperties
    {
        public TempleOfDrogaZoneProperties() : base()
        {
            AddOctagonLiquidShape(ZoneLiquidType.Blood, "t50_redwat01", 546.104858f, 493.395386f, 1986.771484f, 1916.085938f, 1968.937866f, 1933.504761f, 1968.937866f, 1933.504761f,
                529.461609f, 509.902405f, 529.461609f, 509.902405f, -269.906158f, 15f);
        }
    }
}
