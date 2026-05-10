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
        SortParticlesOnDepth = 0x2,
        BillboardAlignWithVelocityVector = 0x4,
        Unshaded = 0x8, // According to wowdev.wiki - "Clears lit flag on CParticleMat"
        InWorldSpace = 0x10, // Skips bone matrix transform
        InheritBoneScale = 0x20,
        InheritVelocity = 0x40,
        ImplosionFilter = 0x80, // wowdev.wiki - "Particles going away from the center are killed"
        ZUpVelocityDirInSphereEmitters = 0x100,
        HalfRandomOppositeSpin = 0x200,
        InheritParentPosition = 0x800, // Also "random emission spacing?"
        AlignXYAxisToZ = 0x1000,
        ClampOnGround = 0x2000, // Particles won't go through the ground if set
        FollowPosition = 0x4000,
        SquirtBehavior = 0x8000, // Burts of particles emit whenever emissionRate anim key value is > 0
        UseRandomTexture = 0x10000,
        ParticlesAreHeadParticles = 0x20000,
        ParticlesAreTailParticles = 0x40000,
        UnknownOppositeOf0x20000 = 0x40000, // Get specifics on this. May be set whenever 0x20000 isn't
        ScaleVaryXAndYSeparately = 0x80000, // If not set, ScaleVary.Y is ignored and X impacts both
        RandomizeFlipBookStart = 0x200000,
        IgnoreDistance = 0x400000, // Unsure exactly what distance is ignored
        UseCompressedGravityVectors = 0x800000, // If set, use compressed vectors instead of z-axis values
        BoneGenerator = 0x1000000, // Unknown exactly (bone, not joint)
        DontThrottleEmissionRateByDistance = 0x4000000
    }
}
