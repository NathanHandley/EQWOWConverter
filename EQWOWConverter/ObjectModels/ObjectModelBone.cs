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

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelBone
    {
        public string BoneNameEQ = string.Empty;
        public string ParentBoneNameEQ = string.Empty;
        public Int32 KeyBoneID = Convert.ToInt32(KeyBoneType.None);
        public ObjectModelBoneFlags Flags = ObjectModelBoneFlags.Transformed;
        public Int16 ParentBone = -1; // Why is this Int16 instead of Int32?
        public UInt16 SubMeshID = 0;
        public UInt32 BoneNameCRC = 0;
        public ObjectModelTrackSequences<Vector3> TranslationTrack = new ObjectModelTrackSequences<Vector3>();
        public ObjectModelTrackSequences<QuaternionShort> RotationTrack = new ObjectModelTrackSequences<QuaternionShort>();
        public ObjectModelTrackSequences<Vector3> ScaleTrack = new ObjectModelTrackSequences<Vector3>();
        public Vector3 PivotPoint = new Vector3();
    }
}
