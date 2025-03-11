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

using EQWOWConverter.Common;
using EQWOWConverter.EQFiles;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelAnimation : IByteSerializable
    {
        public AnimationType AnimationType = AnimationType.Stand; // This correlates to AnimationData.dbc.  0 is standing
        public EQAnimationType EQAnimationType = EQAnimationType.Unknown;
        public UInt16 SubAnimationID = 0; // wowdev also refers to this as variationIndex
        public UInt32 DurationInMS = 10000;
        public float MoveSpeed = 2.5f;
        public ObjectModelAnimationFlags Flags = ObjectModelAnimationFlags.AnimationInM2 | ObjectModelAnimationFlags.Set0x80OnLoad;
        public Int16 PlayFrequency = 32767; // Always make this add up to 32767 for animations of same type
        public UInt16 Padding = 0;
        public UInt32 ReplayMin = 0;
        public UInt32 ReplayMax = 0;
        public UInt32 BlendTime = 150; 
        public BoundingBox BoundingBox = new BoundingBox();
        public float BoundingRadius = 0f;
        public Int16 NextAnimation = -1; // aka, variationNext
        public UInt16 AliasNext = 0; // Id in the list of animations if this is an alias (?)
        public int NumOfFrames = 0;

        static public List<EQAnimationType> GetPrioritizedCompatibleEQAnimationTypes(AnimationType animationType)
        {
            List<EQAnimationType> returnTypes = new List<EQAnimationType>();
            switch (animationType)
            {
                case AnimationType.Stand:
                    {
                        returnTypes.Add(EQAnimationType.o02StandArmsToSide);
                        returnTypes.Add(EQAnimationType.p01StandPassive);
                        returnTypes.Add(EQAnimationType.o01StandIdle);
                        returnTypes.Add(EQAnimationType.l09SwimIdle);
                        returnTypes.Add(EQAnimationType.posStandPose);
                    }
                    break;
                case AnimationType.Death:
                    {
                        returnTypes.Add(EQAnimationType.d05Death);
                    }
                    break;
                case AnimationType.Spell:
                    {
                        returnTypes.Add(EQAnimationType.t06CastPushForward);
                        returnTypes.Add(EQAnimationType.t05CastLoopArms);
                        returnTypes.Add(EQAnimationType.t04CastPullBack);
                    }
                    break;
                case AnimationType.Stop:
                    {
                        // No 'stop' animation
                    }
                    break;
                case AnimationType.Walk:
                    {
                        returnTypes.Add(EQAnimationType.l01Walk);
                        returnTypes.Add(EQAnimationType.l02Run);
                    }
                    break;
                case AnimationType.Run:
                    {
                        returnTypes.Add(EQAnimationType.l02Run);
                        returnTypes.Add(EQAnimationType.l01Walk);
                    }
                    break;
                case AnimationType.Dead: // TODO: Create "dead" using the last frame of "death"
                    {
                        returnTypes.Add(EQAnimationType.d05Death);
                    }
                    break;
                case AnimationType.Rise: 
                    {
                        // TODO: Create "Rise" by reversing "Death"
                    }
                    break;
                case AnimationType.StandWound:
                    {
                        returnTypes.Add(EQAnimationType.d01Damage1);
                        returnTypes.Add(EQAnimationType.d02Damage2);
                    }
                    break;
                case AnimationType.CombatWound:
                    {
                        returnTypes.Add(EQAnimationType.d01Damage1);
                        returnTypes.Add(EQAnimationType.d02Damage2);
                    }
                    break;
                case AnimationType.CombatCritical:
                    {
                        returnTypes.Add(EQAnimationType.d01Damage1);
                        returnTypes.Add(EQAnimationType.d02Damage2);
                    }
                    break;
                case AnimationType.ShuffleLeft:
                    {
                        returnTypes.Add(EQAnimationType.p03rShuffleFeetReverse);
                        returnTypes.Add(EQAnimationType.p03ShuffleFeet);
                    }
                    break;
                case AnimationType.ShuffleRight:
                    {
                        returnTypes.Add(EQAnimationType.p03ShuffleFeet);
                        returnTypes.Add(EQAnimationType.p03rShuffleFeetReverse);
                    }
                    break;
                case AnimationType.Walkbackwards:
                    {
                        returnTypes.Add(EQAnimationType.l01rWalkBackwards);
                        returnTypes.Add(EQAnimationType.l02rRunBackwards);
                        returnTypes.Add(EQAnimationType.l01Walk);
                        returnTypes.Add(EQAnimationType.l02Run);
                    }
                    break;
                case AnimationType.Stun:
                    {
                        // No good ones identified
                    }
                    break;
                case AnimationType.HandsClosed:
                    {
                        // This is a 'casting' animation, which in EQ is just standing
                    }
                    break;
                case AnimationType.AttackUnarmed:
                    {
                        returnTypes.Add(EQAnimationType.c08AttackUnarmed);
                        returnTypes.Add(EQAnimationType.c05AttackThrown);
                        returnTypes.Add(EQAnimationType.c02AttackPierce);
                        returnTypes.Add(EQAnimationType.c03Attack2H);
                        returnTypes.Add(EQAnimationType.c04Attack2HBlunt);
                        returnTypes.Add(EQAnimationType.c10AttackUnderwater);
                        returnTypes.Add(EQAnimationType.c07ShieldBash);
                        returnTypes.Add(EQAnimationType.c01Kick);
                        returnTypes.Add(EQAnimationType.c11RoundhouseKick);                        
                        returnTypes.Add(EQAnimationType.c09AttackBow);
                    }
                    break;
                case AnimationType.Attack1H:
                    {
                        returnTypes.Add(EQAnimationType.c05AttackThrown);
                        returnTypes.Add(EQAnimationType.c08AttackUnarmed);
                        returnTypes.Add(EQAnimationType.c02AttackPierce);
                        returnTypes.Add(EQAnimationType.c03Attack2H);
                        returnTypes.Add(EQAnimationType.c04Attack2HBlunt);
                        returnTypes.Add(EQAnimationType.c10AttackUnderwater);
                        returnTypes.Add(EQAnimationType.c07ShieldBash);
                        returnTypes.Add(EQAnimationType.c01Kick);
                        returnTypes.Add(EQAnimationType.c11RoundhouseKick);
                        returnTypes.Add(EQAnimationType.c09AttackBow);
                    }
                    break;
                case AnimationType.Attack2H:
                    {
                        returnTypes.Add(EQAnimationType.c03Attack2H);
                        returnTypes.Add(EQAnimationType.c04Attack2HBlunt);
                        returnTypes.Add(EQAnimationType.c05AttackThrown);
                        returnTypes.Add(EQAnimationType.c08AttackUnarmed);
                        returnTypes.Add(EQAnimationType.c02AttackPierce);
                        returnTypes.Add(EQAnimationType.c10AttackUnderwater);
                        returnTypes.Add(EQAnimationType.c07ShieldBash);
                        returnTypes.Add(EQAnimationType.c01Kick);
                        returnTypes.Add(EQAnimationType.c11RoundhouseKick);
                        returnTypes.Add(EQAnimationType.c09AttackBow);
                    }
                    break;
                case AnimationType.Attack2HL:
                    {
                        returnTypes.Add(EQAnimationType.c04Attack2HBlunt);
                        returnTypes.Add(EQAnimationType.c03Attack2H);
                        returnTypes.Add(EQAnimationType.c05AttackThrown);
                        returnTypes.Add(EQAnimationType.c08AttackUnarmed);
                        returnTypes.Add(EQAnimationType.c02AttackPierce);
                        returnTypes.Add(EQAnimationType.c10AttackUnderwater);
                        returnTypes.Add(EQAnimationType.c07ShieldBash);
                        returnTypes.Add(EQAnimationType.c01Kick);
                        returnTypes.Add(EQAnimationType.c11RoundhouseKick);
                        returnTypes.Add(EQAnimationType.c09AttackBow);
                    }
                    break;
                case AnimationType.ParryUnarmed:
                    {
                        returnTypes.Add(EQAnimationType.c07ShieldBash);
                    }
                    break;
                case AnimationType.Parry1H:
                    {
                        returnTypes.Add(EQAnimationType.c07ShieldBash);
                    }
                    break;
                case AnimationType.Parry2H:
                    {
                        returnTypes.Add(EQAnimationType.c07ShieldBash);
                    }
                    break;
                case AnimationType.Parry2HL:
                    {
                        returnTypes.Add(EQAnimationType.c07ShieldBash);
                    }
                    break;
                case AnimationType.ShieldBlock:
                    {
                        returnTypes.Add(EQAnimationType.c07ShieldBash);
                    }
                    break;
                case AnimationType.ReadyUnarmed:
                    {
                        returnTypes.Add(EQAnimationType.p01StandPassive);
                    }
                    break;
                case AnimationType.Ready1H:
                    {
                        returnTypes.Add(EQAnimationType.p01StandPassive);
                    }
                    break;
                case AnimationType.Ready2H:
                    {
                        returnTypes.Add(EQAnimationType.p01StandPassive);
                    }
                    break;
                case AnimationType.Ready2HL:
                    {
                        returnTypes.Add(EQAnimationType.p01StandPassive);
                    }
                    break;
                case AnimationType.ReadyBow:
                    {
                        returnTypes.Add(EQAnimationType.p01StandPassive);
                    }
                    break;
                case AnimationType.Dodge:
                    {
                        // There is no dodge animation
                    }
                    break;
                case AnimationType.SpellPrecast:
                    {
                        // Nothing
                    }
                    break;
                case AnimationType.SpellCast:
                    {
                        returnTypes.Add(EQAnimationType.t06CastPushForward);
                        returnTypes.Add(EQAnimationType.t04CastPullBack);
                        returnTypes.Add(EQAnimationType.t05CastLoopArms);
                    }
                    break;
                case AnimationType.SpellCastArea:
                    {
                        returnTypes.Add(EQAnimationType.t05CastLoopArms);
                        returnTypes.Add(EQAnimationType.t04CastPullBack);
                        returnTypes.Add(EQAnimationType.t06CastPushForward);
                    }
                    break;
                case AnimationType.NPCWelcome:
                    {
                        // TODO:
                    }
                    break;
                case AnimationType.NPCGoodbye:
                    {
                        // TODO:
                    }
                    break;
                case AnimationType.Block:
                    {
                        returnTypes.Add(EQAnimationType.c07ShieldBash);
                    }
                    break;
                case AnimationType.JumpStart:
                    {
                        // TODO:
                    }
                    break;
                case AnimationType.Jump:
                    {
                        returnTypes.Add(EQAnimationType.l04JumpStanding);
                        returnTypes.Add(EQAnimationType.l03JumpRunning);
                    }
                    break;
                case AnimationType.JumpEnd:
                    {
                        // TODO:
                    }
                    break;
                case AnimationType.Fall:
                    {
                        returnTypes.Add(EQAnimationType.l05Fall);
                    }
                    break;
                case AnimationType.SwimIdle:
                    {
                        returnTypes.Add(EQAnimationType.l09SwimIdle);
                        returnTypes.Add(EQAnimationType.p06Swim);
                    }
                    break;
                case AnimationType.Swim:
                    {
                        returnTypes.Add(EQAnimationType.p06Swim);
                        returnTypes.Add(EQAnimationType.l09SwimIdle);
                    }
                    break;
                case AnimationType.SwimLeft:
                    {
                        returnTypes.Add(EQAnimationType.l09SwimIdle);
                        returnTypes.Add(EQAnimationType.p06Swim);                        
                    }
                    break;
                case AnimationType.SwimRight:
                    {
                        returnTypes.Add(EQAnimationType.l09SwimIdle);
                        returnTypes.Add(EQAnimationType.p06Swim);                        
                    }
                    break;
                case AnimationType.SwimBackwards:
                    {
                        returnTypes.Add(EQAnimationType.l09SwimIdle);
                        returnTypes.Add(EQAnimationType.p06Swim);                        
                    }
                    break;
                case AnimationType.AttackBow:
                    {
                        returnTypes.Add(EQAnimationType.p01StandPassive);
                    }
                    break;
                case AnimationType.FireBow:
                    {
                        returnTypes.Add(EQAnimationType.c09AttackBow);
                    }
                    break;
                case AnimationType.ReadyRifle:
                    {
                        returnTypes.Add(EQAnimationType.p01StandPassive);
                    }
                    break;
                case AnimationType.AttackRifle:
                    {
                        returnTypes.Add(EQAnimationType.c09AttackBow);
                    }
                    break;
                default: break;
            }

            // Return the types
            return returnTypes;
        }

        public UInt32 GetBytesSize()
        {
            UInt32 size = 0;
            size += 2; // AnimationType
            size += 2; // SubAnimationID
            size += 4; // DurationInMs
            size += 4; // MoveSpeed
            size += 4; // Flags
            size += 2; // PlayFrequency
            size += 2; // Padding
            size += 4; // ReplayMin
            size += 4; // ReplayMax
            size += 4; // BlendTime
            size += 24;// Bounding Box
            size += 4; // BoundingRadius
            size += 2; // NextAnimation;
            size += 2; // AliasNext
            return size;
        }

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(AnimationType)));
            bytes.AddRange(BitConverter.GetBytes(SubAnimationID));
            bytes.AddRange(BitConverter.GetBytes(DurationInMS));
            bytes.AddRange(BitConverter.GetBytes(MoveSpeed));
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Flags)));
            bytes.AddRange(BitConverter.GetBytes(PlayFrequency));
            bytes.AddRange(BitConverter.GetBytes(Padding));
            bytes.AddRange(BitConverter.GetBytes(ReplayMin));
            bytes.AddRange(BitConverter.GetBytes(ReplayMax));
            bytes.AddRange(BitConverter.GetBytes(BlendTime));
            bytes.AddRange(BoundingBox.ToBytesHighRes());
            bytes.AddRange(BitConverter.GetBytes(BoundingRadius));
            bytes.AddRange(BitConverter.GetBytes(NextAnimation));
            bytes.AddRange(BitConverter.GetBytes(AliasNext));
            return bytes;
        }
    }
}
