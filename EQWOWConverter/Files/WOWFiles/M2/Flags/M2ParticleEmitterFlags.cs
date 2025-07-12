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

namespace EQWOWConverter.WOWFiles
{
    // Some notes and comments pulled 2025/07/09 from https://wowdev.wiki/M2#Particle_emitters
    internal enum M2ParticleEmitterFlags : UInt32
    {
        None = 0,
        LitByLight = 0x1,
        Unknown = 0x2,
        AffectedByPlayerOrientation = 0x4,
        TravelAbsoluteUp = 0x8, // 'Up' in world space as opposed to the model
        DoNotTrail = 0x10,
        NoLighting = 0x20, // Confirm this
        UseBurstMultiplier = 0x40, // What is this exactly?
        ParticlesAreInModelSpace = 0x80, // Transfers the animation of the emitter to the particles
        Randomizer = 0x200,
        AlignXYAxisToZ = 0x1000,
        ClampOnGround = 0x2000,
        UseRandomTexture = 0x10000,
        MoveParticlesAwayFromOrigin = 0x20000,
        UnknownOppositeOf0x20000 = 0x40000, // Get specifics on this. May be set whenever 0x20000 isn't
        ScaleVaryXAndYSeparately = 0x80000, // If not set, ScaleVary.Y is ignored and X impacts both
        RandomizeFlipBookStart = 0x200000,
        IgnoreDistance = 0x400000, // Unsure exactly what distance is ignored
        UseCompressedGravityVectors = 0x800000, // If set, use compressed vectors instead of z-axis values
        BoneGenerator = 0x1000000, // Unknown exactly (bone, not joint)
        DontThrottleEmissionRateByDistance = 0x4000000
    }
}
