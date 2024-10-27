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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EQWOWConverter.EQFiles.EQSkeleton;

namespace EQWOWConverter.EQFiles
{
    internal class EQAnimation
    {
        public Animation Animation = new Animation(AnimationType.Stand, 0, 0);

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDetail(" - Reading EQ Animation Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find EQ Animation file that should be at '" + fileFullPath + "'");
                return false;
            }

            string animationFileName = Path.GetFileNameWithoutExtension(fileFullPath);
            string animationName = animationFileName.Split("_")[1];
            Animation = new Animation(DetermineAnimationType(animationName), 0, 0);

            // Load the core data
            string inputData = File.ReadAllText(fileFullPath);
            string[] inputRows = inputData.Split(Environment.NewLine);

            // Make sure there is the minimum number of rows
            if (inputRows.Length < 4)
            {
                Logger.WriteError("- Could not load EQ Animation file because there were less than 4 rows at '" + fileFullPath + "'");
                return false;
            }

            foreach (string inputRow in inputRows)
            {
                // Nothing for blank lines
                if (inputRow.Length == 0)
                    continue;

                // # = comment
                else if (inputRow.StartsWith("#"))
                    continue;

                // Get the blocks for this row
                string[] blocks = inputRow.Split(",");
                if (blocks.Length == 0)
                    continue;

                // Number of frames
                if (blocks[0] == "framecount")
                {
                    Animation.FrameCount = int.Parse(blocks[1]);
                    continue;
                }

                // Total time of animation
                if (blocks[0] == "totalTimeMs")
                {
                    Animation.TotalTimeInMS = int.Parse(blocks[1]);
                    continue;
                }

                // Ensure it's an animation block
                if (blocks.Length != 11)
                {
                    Logger.WriteError("EQ Animation Frame data must be 11 components");
                    continue;
                }

                Animation.BoneAnimationFrame animationFrame = new Animation.BoneAnimationFrame();
                animationFrame.BoneFullNameInPath = blocks[0];
                animationFrame.FrameIndex = int.Parse(blocks[1]);
                animationFrame.XPosition = float.Parse(blocks[2]);
                animationFrame.YPosition = float.Parse(blocks[3]);
                animationFrame.ZPosition = float.Parse(blocks[4]);
                animationFrame.XRotation = float.Parse(blocks[5]);
                animationFrame.YRotation = float.Parse(blocks[6]);
                animationFrame.ZRotation = float.Parse(blocks[7]);
                animationFrame.WRotation = float.Parse(blocks[8]);
                animationFrame.Scale = float.Parse(blocks[9]);
                animationFrame.FramesMS = int.Parse(blocks[10]);
                Animation.AnimationFrames.Add(animationFrame);
            }

            Logger.WriteDetail(" - Done reading EQ Animation Data from '" + fileFullPath + "'");
            return true;
        }

        private AnimationType DetermineAnimationType(string animationName)
        {
            switch (animationName.ToLower())
            {
                case "c01": return AnimationType.Kick;
                case "c02": return AnimationType.Attack1HPierce; // "Piercing" so may be 2h
                case "c03": return AnimationType.Attack2H; // 2H
                case "c04": return AnimationType.Attack2HL; // 2H Blunt
                case "c05": return AnimationType.AttackThrown;
                case "c06": return AnimationType.AttackOff;
                case "c07": return AnimationType.ShieldBash;
                case "c08": return AnimationType.AttackUnarmed;
                case "c09": return AnimationType.AttackBow; // This may need more than this one animation
                case "c10": return AnimationType.AttackUnarmed; // This is underwater attacking
                case "c11": return AnimationType.Kick; // This is a roundhouse kick
                case "d01": return AnimationType.CombatWound; // "Damage 1"
                case "d02": return AnimationType.CombatCritical; // "Damage 2"
                case "d03": return AnimationType.CombatCritical; // Damage from a trap
                case "d04": return AnimationType.CombatWound; // This is drowning and burning
                case "d05": return AnimationType.Death;
                case "drf": return AnimationType.Stand; // "Pose" for tentacles
                case "l01": return AnimationType.Walk;
                case "l01r": return AnimationType.Walkbackwards;
                case "l02": return AnimationType.Run;
                case "l02r": return AnimationType.Walkbackwards; // Running in reverse
                case "l03": return AnimationType.Jump; // Jump Running: WoW has jump start, jump, and jump end.  This will be funny for players
                case "l04": return AnimationType.Jump; // Jump Standing
                case "l05": return AnimationType.Fall;
                case "l06": return AnimationType.StealthWalk; // Crouch Walk
                case "l07": return AnimationType.Walk; // Climbing
                case "l08": return AnimationType.StealthStand; // Crouch
                case "l08r": return AnimationType.StealthWalk; // Crouch walking in reverse
                case "l09": return AnimationType.SwimIdle; // Swim and Treading Water.  May need forward swimming animation for this 
                case "o01": return AnimationType.Stand; // Idle
                case "s01": return AnimationType.EmoteCheer;
                case "s02": return AnimationType.EmoteCry; // Mourn
                case "s03": return AnimationType.EmoteWave;
                case "s04": return AnimationType.EmoteRude;
                case "s05": return AnimationType.EmoteShy; // This is actually a Yawn.  Could be an issue
                case "o02": return AnimationType.Stand; // Idle with arms at the sides
                case "o03": return AnimationType.SitGround; // Idle sitting
                case "p01": return AnimationType.Stand;
                case "p02": return AnimationType.SitGroundUp; // From sitting to standing
                case "p02r": return AnimationType.SitGroundDown; // From standing to sitting
                case "p03": return AnimationType.ShuffleLeft; // This is "shuffle feet"
                case "p03r": return AnimationType.ShuffleRight; // This is "shuffle feet, reversed"
                case "p04": return AnimationType.Stand; // This is actually "float/walk/strafe"
                case "p05": return AnimationType.KneelStart; // Note that there is an 'emote kneel' as well 'kneel start'
                case "p05r": return AnimationType.KneelEnd;
                case "p06": return AnimationType.Swim;
                case "p07": return AnimationType.SitGround;
                case "p08": return AnimationType.Stand; // Standing with arms at the sides
                case "pos": return AnimationType.Stand; // "Pose", unsure what this is exactly
                case "t01": return AnimationType.Stand; // Unknown (unused?)
                case "t02": return AnimationType.EmoteDance; // Stringed Instrument
                case "t03": return AnimationType.EmoteDance; // Wind Instrument
                case "t04": return AnimationType.SpellCastDirected; // Cast Pull Back
                case "t05": return AnimationType.SpellCastOmni; // Raise and Loop Arms
                case "t06": return AnimationType.SpellCastArea; // Cast Push Forward
                case "t07": return AnimationType.Kick; // Flying Kick
                case "t08": return AnimationType.AttackUnarmed; // Rapid Punches
                case "t09": return AnimationType.AttackUnarmedOff; // Large Punch
                case "s06": return AnimationType.Stand; // Nod
                case "s07": return AnimationType.EmoteCheer; // Amazed
                case "s08": return AnimationType.EmoteBeg;
                case "s09": return AnimationType.EmoteApplaud;
                case "s10": return AnimationType.EmoteCry; // Distress
                case "s11": return AnimationType.Stand; // Blush
                case "s12": return AnimationType.EmoteLaugh;
                case "s13": return AnimationType.EmoteShout; // Burp
                case "s14": return AnimationType.StealthStand; // Duck
                case "s15": return AnimationType.Stand; // Look around
                case "s16": return AnimationType.EmoteDance;
                case "s17": return AnimationType.Stand; // Blink
                case "s18": return AnimationType.Stand; // Glare
                case "s19": return AnimationType.Stand; // Drool
                case "s20": return AnimationType.KneelLoop; // Kneel, consider the other kneel options
                case "s21": return AnimationType.EmoteLaugh;
                case "s22": return AnimationType.EmotePoint;
                case "s23": return AnimationType.EmoteTalkQuestion; // Ponder
                case "s24": return AnimationType.EmoteYes; // Ready
                case "s25": return AnimationType.EmoteSalute;
                case "s26": return AnimationType.Stand; // Shiver
                case "s27": return AnimationType.Stand; // Tap Foot
                case "s28": return AnimationType.EmoteBow;
                default:
                    {
                        Logger.WriteError("Could not determine animation type with animation name '" + animationName + "'");
                        return AnimationType.Stand;
                    }
            }

        }
    }
}
