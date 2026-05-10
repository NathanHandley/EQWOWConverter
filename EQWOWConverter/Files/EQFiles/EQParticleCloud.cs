//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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
using Org.BouncyCastle.Asn1.X509;

namespace EQWOWConverter.EQFiles
{
    internal class EQParticleCloud
    {
        public string Name = string.Empty;
        public string AttachedSkeletonName = string.Empty;
        public string AttachedBoneName = string.Empty;
        public ParticleCloudMovementType ParticleMovementType = ParticleCloudMovementType.None;
        public bool HighOpacity = false;
        public bool FollowsItem = false;
        public int NumSimultaneousParticles = 1;
        public float SpawnRadius = 0;
        public float SpawnAngle = 0;
        public int SpawnLifespanInMS = 0;
        public float SpawnVelocity = 1;
        public float SpawnNormalX = 0;
        public float SpawnNormalY = 0;
        public float SpawnNormalZ = 0;
        public int SpawnRateInMS = 1;
        public float SpawnScale = 1f;
        public ColorRGBA TintColor = new ColorRGBA(255, 255, 255, 0);
        public string SpriteName = string.Empty;
        public MaterialType MaterialType = MaterialType.Diffuse;
        public bool IsTextureAnimated = false;
        public int TextureAnimationDelayInMS = 100;
        public List<string> TextureFrameNames = new List<string>();

        public EQParticleCloud()
        {

        }

        public EQParticleCloud(EQParticleCloud other)
        {
            Name = other.Name;
            AttachedSkeletonName = other.AttachedSkeletonName;
            AttachedBoneName = other.AttachedBoneName;
            ParticleMovementType = other.ParticleMovementType;
            HighOpacity = other.HighOpacity;
            FollowsItem = other.FollowsItem;
            NumSimultaneousParticles = other.NumSimultaneousParticles;
            SpawnRadius = other.SpawnRadius;
            SpawnAngle = other.SpawnAngle;
            SpawnLifespanInMS = other.SpawnLifespanInMS;
            SpawnVelocity = other.SpawnVelocity;
            SpawnNormalX = other.SpawnNormalX;
            SpawnNormalY = other.SpawnNormalY;
            SpawnNormalZ = other.SpawnNormalZ;
            SpawnRateInMS = other.SpawnRateInMS;
            SpawnScale = other.SpawnScale;
            TintColor = other.TintColor;
            SpriteName = other.SpriteName;
            MaterialType = other.MaterialType;
            IsTextureAnimated = other.IsTextureAnimated;
            TextureAnimationDelayInMS = other.TextureAnimationDelayInMS;
            TextureFrameNames = other.TextureFrameNames;
        }

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDebug(" - Reading EQ Particle Cloud Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find particle cloud file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the data
            string inputData = FileTool.ReadAllDataFromFile(fileFullPath);
            string[] inputRows = inputData.Split(Environment.NewLine);
            foreach (string inputRow in inputRows)
            {
                // Nothing for blank lines
                if (inputRow.Length == 0)
                    continue;

                // # = comment
                else if (inputRow.StartsWith("#"))
                    continue;

                // All of these rows have a comma
                string[] blocks = inputRow.Split(",");
                if (blocks.Length < 2)
                    continue;
                switch(blocks[0].ToLower().Trim())
                {
                    case "name": Name = blocks[1].Trim(); break;
                    case "attached_skeleton": AttachedSkeletonName = blocks[1].Trim(); break;
                    case "attached_bone": AttachedBoneName = blocks[1].Trim(); break;
                    case "movement":
                        {
                            switch(blocks[1].ToLower().Trim())
                            {
                                case "none": ParticleMovementType = ParticleCloudMovementType.None; break;
                                case "sphere": ParticleMovementType = ParticleCloudMovementType.Sphere; break;
                                case "plane": ParticleMovementType = ParticleCloudMovementType.Plane; break;
                                case "stream": ParticleMovementType = ParticleCloudMovementType.Stream; break;
                                default:
                                    {
                                        Logger.WriteError("Unknown movement type '", blocks[1], "' in Particle Cloud file '", fileFullPath, "'");
                                    } break;
                            }
                        } break;
                    case "flag_high_opacity": HighOpacity = bool.Parse(blocks[1].Trim()); break;
                    case "flag_follows_item": FollowsItem = bool.Parse(blocks[1].Trim()); break;
                    case "flag_1": break; // Skip
                    case "flag_3": break; // Skip
                    case "flags_raw": break; // Skip
                    case "simultaneous_particles": NumSimultaneousParticles = int.Parse(blocks[1].Trim()); break;
                    case "spawn_radius": SpawnRadius = float.Parse(blocks[1].Trim()); break;
                    case "spawn_angle": SpawnAngle = float.Parse(blocks[1].Trim()); break;
                    case "spawn_lifespan_ms": SpawnLifespanInMS = int.Parse(blocks[1].Trim()); break;
                    case "spawn_velocity": SpawnVelocity = float.Parse(blocks[1].Trim()); break;
                    case "spawn_normal_x": SpawnNormalX = float.Parse(blocks[1].Trim()); break;
                    case "spawn_normal_y": SpawnNormalZ = float.Parse(blocks[1].Trim()); break; // Swap Y and Z
                    case "spawn_normal_z": SpawnNormalY = float.Parse(blocks[1].Trim()); break; // Swap Y and Z
                    case "spawn_rate_ms": SpawnRateInMS = int.Parse(blocks[1].Trim()); break;
                    case "spawn_scale": SpawnScale = float.Parse(blocks[1].Trim()); break;
                    case "tint_r": TintColor.R = byte.Parse(blocks[1].Trim()); break;
                    case "tint_g": TintColor.G = byte.Parse(blocks[1].Trim()); break;
                    case "tint_b": TintColor.B = byte.Parse(blocks[1].Trim()); break;
                    case "tint_a": TintColor.A = byte.Parse(blocks[1].Trim()); break;
                    case "sprite_name": SpriteName = blocks[1].Trim(); break;
                    case "render_method": break; // Skip
                    case "render_method_name":
                        {
                            switch (blocks[1].ToLower().Trim())
                            {
                                case "transparentadditive": MaterialType = MaterialType.TransparentAdditive; break;
                                case "diffuse5": MaterialType = MaterialType.Diffuse; break;
                                default:
                                    {
                                        Logger.WriteError("Unknown render method type '", blocks[1], "' in Particle Cloud file '", fileFullPath, "'");
                                    }
                                    break;
                            }
                        }
                        break;
                    case "texture_animated": IsTextureAnimated = bool.Parse(blocks[1].Trim()); break;
                    case "texture_animation_delay_ms": TextureAnimationDelayInMS = int.Parse(blocks[1].Trim()); break;
                    case "texture_frames":
                        {
                            // Next value is the count, so start at the 3rd index
                            for (int i = 2; i < blocks.Length; i++)
                                TextureFrameNames.Add(blocks[i].Trim());
                        } break;
                    default: Logger.WriteError("Unknown row with key '", blocks[0], "' in Particle Cloud file '", fileFullPath, "'"); break;
                }
            }

            Logger.WriteDebug(" - Done reading EQ Particle Cloud Data from '" + fileFullPath + "'");
            return true;
        }
    }
}
