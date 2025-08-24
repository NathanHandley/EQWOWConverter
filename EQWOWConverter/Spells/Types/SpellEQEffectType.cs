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

namespace EQWOWConverter.Spells
{
    internal enum SpellEQEffectType : int
    {
        None = 254,

        CurrentHitPoints = 0, // value of Negative = Damage and Positive = Heal. 
        ArmorClass = 1,
        Attack = 2,
        MovementSpeed = 3,
        Strength = 4,
        Dexterity = 5,
        Agility = 6,
        Stamina = 7,
        Intelligence = 8,
        Wisdom = 9,
        Charisma = 10,
        AttackSpeed = 11,
        InvisibilityUnstable = 12,
        SeeInvisibility = 13,
        WaterBreathing = 14,
        CurrentMana = 15,
        //NPCFrenzy = 16, // What is this?
        //NPCAwareness = 17, // What is this?
        //Pacify = 18, -- This isn't implemented in eq emulators so always skip it
        //ModFaction = 19,
        //Blind = 20,
        Stun = 21,
        //Charm = 22,
        Fear = 23,
        //Stamina = 24,
        BindAffinity = 25,
        Gate = 26,
        CancelMagic = 27,
        //....
        // Mez = 31
        SummonItems = 32,
        // SummonPet = 33
        // Confuse = 34
        //....
        DiseaseCounter = 35,
        PoisonCounter = 36,
        //....
        // DivineAura = 40 - Note: Can't cast or melee
        //....
        ResistFire = 46,
        ResistCold = 47,
        ResistPoison = 48,
        ResistDisease = 49,
        ResistMagic = 50,
        //....
        // DetectTraps = 51
        // SenseDead = 52
        // SenseSummoned = 53
        // SenseAnimals = 54
        // Rune = 55 // Absorb all melee damage until a maximum amount of damage is taken and it fades
        // TrueNorth = 56
        Levitate = 57,
        // Illusion = 58
        DamageShield = 59, // Attacker takes damage if they hit someone with this
        //....
        // WipeHateList = 63,
        //....
        // InfraVision = 65,
        // UltraVision = 66,
        // EyeOfZomm = 67,
        // ReclainPet = 68 // Destroys pet and gives some of the mana back
        TotalHP = 69,
        //....
        // NecPet = 71 // Undead / Necromancer pet
        //....
        // BindSight = 73
        // FeignDeath = 74
        //....
        // LocateCorpse = 77
        //....
        CurrentHitPointsOnce = 79,
        //....
        Revive = 81,
        // SummonPC = 82 // Summons the player to the casters location
        Teleport = 83,
        // TossUp = 84, // Launches the target in the air
        // WeaponProc = 85, // Add a weapon proc to the melee target
        // Harmony = 86, // Reduce agro reaction radius
        // MagnifyVision = 87,
        // Succor = 88, // Teleport self or the group to a safe spot in a zone
        // ModelSize = 89, // Grow/Shrink
        //....
        // SummonCorpse = 91 // Summon the corpse of the targeted player
        // InstantHate = 92 // Add or remove a set amount of hate
        // StopRain = 93 // Stops any active rain
        //....
        Silence = 96,
        TotalMana = 97,
        AttackSpeed2 = 98, // Looks like this stacks with the other attack speed
        Root = 99,
        HealOverTime = 100,
        //....
        // Fearless = 102,
        // CallPet = 103,
        // Translocate = 104, // Move the target to a place
        // ResistAll = 111,
        // ReverseDamageShield = 121, -- Not used before Planes of Power
        //DispelDetrimental = 154, -- Not used (Purify Soul, AA ability)
        //DispelBeneficial = 209, -- Not used
        Invisibility = 314,
        CurrentManaOnce = 358,
        // TODO: Goes up to 526
    }
}
