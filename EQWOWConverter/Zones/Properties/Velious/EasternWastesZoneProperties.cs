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
    internal class EasternWastesZoneProperties : ZoneProperties
    {
        public EasternWastesZoneProperties() : base()
        {
            AddDiscardGeometryBox(4209.238281f, -7449.717285f, 104.720337f, -13431.015625f, -9256.558594f, -283.213837f); // East strip, up to the bridge
            AddDiscardGeometryBoxObjectsOnly(2011.942383f, -4501.451172f, 853.347229f, 9.276880f, -7326.402344f, -621.480286f); // Trees in the water
            AddDiscardGeometryBox(4280.932129f, 8445.250000f, 16.208639f, -659.456482f, 5984.957031f, -490.946625f); // Northwest water
            AddDiscardGeometryBox(-8961.625977f, 7940.808105f, 155.420395f, -13581.579102f, 5991.803711f, -314.520233f); // Southwest water
        }
    }
}
