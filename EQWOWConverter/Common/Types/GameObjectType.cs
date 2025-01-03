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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal enum GameObjectType : int
    {
        DOOR = 0,
        BUTTON = 1,
        QUESTGIVER = 2,
        CHEST = 3,
        BINDER = 4,
        GENERIC = 5,
        TRAP = 6,
        CHAIR = 7,
        SPELL_FOCUS = 8,
        TEXT = 9,
        GOOBER = 10,
        TRANSPORT = 11,
        AREADAMAGE = 12,
        CAMERA = 13,
        MAP_OBJECT = 14,
        MO_TRANSPORT = 15,
        DUEL_ARBITER = 16,
        FISHINGNODE = 17,
        RITUAL = 18,
        MAILBOX = 19,
        AUCTIONHOUSE = 20,
        GUARDPOST = 21,
        SPELLCASTER = 22,
        MEETINGSTONE = 23,
        FLAGSTAND = 24,
        FISHINGHOLE = 25,
        FLAGDROP = 26,
        MINI_GAME = 27,
        LOTTERY_KIOSK = 28,
        CAPTURE_POINT = 29,
        AURA_GENERATOR = 30,
        DUNGEON_DIFFICULTY = 31,
        BARBER_CHAIR = 32,
        DESTRUCTIBLE_BUILDING = 33,
        GUILD_BANK = 34,
        TRAPDOOR = 35
    }
}
