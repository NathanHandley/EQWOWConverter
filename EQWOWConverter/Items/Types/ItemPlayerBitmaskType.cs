//  Author: Nathan Handley(nathanhandley@protonmail.com)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Items
{
    internal enum ItemPlayerBitmaskType : Int32
    {
		Warrior = 1,
		Cleric = 2,
		Paladin = 4,
		Ranger = 8,
		ShadowKnight = 16,
		Druid = 32,
		Monk = 64,
		Bard = 128,
		Rogue = 256,
		Shaman = 512,
		Necromancer = 1024,
		Wizard = 2048,
		Magician = 4096,
		Enchanter = 8192,
		Beastlord = 16384
    }
}
