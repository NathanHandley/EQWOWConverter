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

namespace EQWOWConverter.EQFiles
{
    internal enum EQAnimationType
    {
        Unknown,
        c01Kick,
        c02AttackPierce, // "Piercing" so may be 2h
        c03Attack2H,
        c04Attack2HBlunt,
        c05AttackThrown,
        c06AttackOff,
        c07ShieldBash,
        c08AttackUnarmed,
        c09AttackBow, // This may need more than this one animation
        c10AttackUnderwater,
        c11RoundhouseKick,
        d01Damage1,
        d02Damage2,
        d03TrapDamage,
        d04DrownAndBurn,
        d05Death,
        drfStandPose, // "Pose" for tentacles
        l01Walk,
        l01rWalkBackwards,
        l02Run,
        l02rRunBackwards,
        l03JumpRunning, // Jump Running: WoW has jump start, jump, and jump end.  This will be funny for players
        l04JumpStanding,
        l05Fall,
        l06CrouchWalk,
        l07Climbing,
        l08Crouch,
        l08rCrouchWalkReverse,
        l09SwimIdle, // Swim and Treading Water.  May need forward swimming animation for this 
        o01StandIdle,
        s01EmoteCheer,
        s02EmoteMourn,
        s03EmoteWave,
        s04EmoteRude,
        s05EmoteYawn,
        o02StandArmsToSide,
        o03SitIdle,
        p01StandPassive,
        p02StandToSit,
        p02rSitToStand,
        p03ShuffleFeet,
        p03rShuffleFeetReverse,
        p04FloatWalkStrafe, // This is actually "float/walk/strafe"
        p05KneelStart,
        p05rKneelEnd,
        p06Swim,
        p07SitGround,
        p08StandArmsToSide,
        posStandPose,
        t01Unknown,
        t02StringedInstrument,
        t03WindInstrument,
        t04CastPullBack,
        t05CastLoopArms,
        t06CastPushForward,
        t07FlyingKick,
        t08RapidPunches,
        t09LargePunch,
        s06EmoteNod,
        s07EmoteAmazed,
        s08EmoteBeg,
        s09EmoteApplaud,
        s10EmoteDistress,
        s11EmoteBlush,
        s12EmoteLaugh,
        s13EmoteBurp,
        s14EmoteDuck,
        s15LookAround,
        s16EmoteDance,
        s17EmoteBlink,
        s18EmoteGlare,
        s19EmoteDrool,
        s20EmoteKneel,
        s21EmoteLaugh,
        s22EmotePoint,
        s23EmotePonder,
        s24EmoteReady,
        s25EmoteSalute,
        s26EmoteShiver,
        s27EmoteTapFoot,
        s28EmoteBow
    }
}
