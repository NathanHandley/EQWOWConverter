﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelBone
    {
        private string _BoneNameEQ = string.Empty;
        public string BoneNameEQ
        {
            get { return _BoneNameEQ; }
            set
            {
                _BoneNameCRC = GetCRCValue(value);
                _BoneNameEQ = value; 
            }
        }

        public UInt32 _BoneNameCRC = 0;
        public UInt32 BoneNameCRC
        {
            get { return _BoneNameCRC; }
        }

        public string ParentBoneNameEQ = string.Empty;
        public Int32 KeyBoneID = Convert.ToInt32(KeyBoneType.None);
        public UInt16 Flags = Convert.ToUInt16(ObjectModelBoneFlags.Transformed);
        public Int16 ParentBone = -1; // Why is this Int16 instead of Int32?
        public UInt16 SubMeshID = 0;
        public ObjectModelTrackSequences<Vector3> TranslationTrack = new ObjectModelTrackSequences<Vector3>();
        public ObjectModelTrackSequences<QuaternionShort> RotationTrack = new ObjectModelTrackSequences<QuaternionShort>();
        public ObjectModelTrackSequences<Vector3> ScaleTrack = new ObjectModelTrackSequences<Vector3>();
        public Vector3 PivotPoint = new Vector3();

        public ObjectModelBone() { }
        public ObjectModelBone(string boneNameEQ, Int16 parentBoneID)
        {
            BoneNameEQ = boneNameEQ;
            ParentBone = parentBoneID;
        }


        private UInt32 GetCRCValue(string name)
        {
            switch (name.ToLower())
            {
                case "breath":  return 3299126614;
                case "cah":     return 3987563274; // Used for $CAH event (HandleCombatAnimEvent)
                case "cpp":     return 2904086604; // Used for $CPP event (PlayCombatActionAnimKit)
                case "css":     return 524081717;  // Used for $CSS event (PlayWeaponSwooshSound)                
                case "dth":     return 3747058587; // Used for $DTH event (DeathThud_LootEffect)
                case "fd1":     return 3217595452; // Used for $FD1 event (PlayFidgetSound 1)
                case "fd2":     return 650235270;  // Used for $FD2 event (PlayFidgetSound 2)
                case "fsd":     return 2586090777; // Used for $FSD event (HandleFootfallAnimEvent)
                case "hit":     return 1025530540; // Used for $HIT event (PlayWoundAnimKit)
                case "main":    return 521822810;
                case "name":    return 2738135331;
                case "root":    return 3066451557;
                default:        return 0;
            }
        }
    }
}
