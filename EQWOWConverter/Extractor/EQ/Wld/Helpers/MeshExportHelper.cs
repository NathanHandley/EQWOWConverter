using GlmSharp;
using LanternExtractor.EQ.Wld.Fragments;
using System.Collections.Generic;

namespace LanternExtractor.EQ.Wld.Helpers
{
    public static class MeshExportHelper
    {
        /// <summary>
        /// Transforms vertices of mesh for the given animation and frame.
        /// </summary>
        /// <param name="mesh">The mesh that will have vertices shifted</param>
        /// <param name="skeleton">The SkeletonHierarchy that contains the bone transformations</param>
        /// <param name="animName">The name of the animation used for the transform</param>
        /// <param name="frame">The frame of the animation</param>
        /// <param name="singularBoneIndex">The bone index for the mesh when there is a 1:1 relationship</param>
        /// <returns>The original vertex positions</returns>
        public static List<vec3> ShiftMeshVertices(Mesh mesh, SkeletonHierarchy skeleton, bool isCharacterAnimation, 
            string animName, int frame, int singularBoneIndex = -1)
        {
            var originalVertices = new List<vec3>();
            if (!skeleton.Animations.ContainsKey(animName) ||
                mesh.Vertices.Count == 0)
            {
                return originalVertices;
            }

            var animation = skeleton.Animations[animName];
            var tracks = isCharacterAnimation ? animation.TracksCleanedStripped : animation.TracksCleaned;

            if (singularBoneIndex > -1)
            {
                var bone = skeleton.Skeleton[singularBoneIndex].CleanedName;
                if (!tracks.ContainsKey(bone)) return originalVertices;
                var modelMatrix = skeleton.GetBoneMatrix(singularBoneIndex, animName, frame);
                originalVertices.AddRange(ShiftMeshVerticesWithIndices(
                    0, mesh.Vertices.Count - 1, mesh, modelMatrix));

                return originalVertices;
            }

            foreach (var mobVertexPiece in mesh.MobPieces)
            {
                var boneIndex = mobVertexPiece.Key;
                var bone = skeleton.Skeleton[boneIndex].CleanedName;

                if (!tracks.ContainsKey(bone)) continue;

                var modelMatrix = skeleton.GetBoneMatrix(boneIndex, animName, frame);

                originalVertices.AddRange(ShiftMeshVerticesWithIndices(
                    mobVertexPiece.Value.Start,
                    mobVertexPiece.Value.Start + mobVertexPiece.Value.Count - 1, 
                    mesh, modelMatrix));
            }

            return originalVertices;
        }

        private static List<vec3> ShiftMeshVerticesWithIndices(int start, int end, Mesh mesh, mat4 boneMatrix)
        {
            var originalVertices = new List<vec3>();
            for (int i = start; i <= end; i++)
            {
                if (i >= mesh.Vertices.Count) break;

                var vertex = mesh.Vertices[i];
                originalVertices.Add(vertex);
                var newVertex = boneMatrix * new vec4(vertex, 1f);
                mesh.Vertices[i] = newVertex.xyz;
            }
            return originalVertices;
        }
    }
}
