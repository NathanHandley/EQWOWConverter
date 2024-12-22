using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlmSharp;
using LanternExtractor.EQ.Archive;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public static class ActorObjExporter
    {
        public static Dictionary<Mesh, List<vec3>> BackupVertices = new Dictionary<Mesh, List<vec3>>();

        public static void ExportActors(WldFile wldFile, Settings settings, ILogger logger)
        {
            // For a zone wld, we ignore actors and just export all meshes
            if (wldFile.WldType == WldType.Zone)
            {
                ExportZone((WldFileZone)wldFile, settings, logger);
                return;
            }

            List<Actor> actors = wldFile.GetFragmentsOfType<Actor>();

            foreach (var actor in actors)
            {
                switch (actor.ActorType)
                {
                    case ActorType.Static:
                        ExportStaticActor(actor, settings, wldFile);
                        break;
                    case ActorType.Skeletal:
                        ExportSkeletalActor(actor, settings, wldFile);
                        break;
                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// Exports all of the meshes of a zone in a single obj
        /// </summary>
        /// <param name="wldFile"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        private static void ExportZone(WldFileZone wldFile, Settings settings, ILogger logger)
        {
            List<Mesh> meshes = wldFile.GetFragmentsOfType<Mesh>();
            List<MaterialList> materialLists = wldFile.GetFragmentsOfType<MaterialList>();
            List<ObjectInstance> objects = new List<ObjectInstance>();

            if (settings.ExportZoneWithObjects)
            {
                var path = wldFile.BasePath;
                var s3dArchive = wldFile.BaseS3DArchive;
                var wldFileToInject = wldFile.WldFileToInject;
                var rootFolder = wldFile.RootFolder;
                var shortName = wldFile.ShortName;

                // Get object instances within this zone file to map up and instantiate later
                var zoneObjectsFileInArchive = s3dArchive.GetFile("objects" + LanternStrings.WldFormatExtension);
                if (zoneObjectsFileInArchive != null)
                {
                    var zoneObjectsWldFile = new WldFileZoneObjects(zoneObjectsFileInArchive, shortName,
                        WldType.ZoneObjects, logger, settings, wldFileToInject);
                    zoneObjectsWldFile.Initialize(rootFolder, false);
                    objects.AddRange(zoneObjectsWldFile.GetFragmentsOfType<ObjectInstance>());
                }

                // Find associated _obj archive e.g. qeytoqrg_obj.s3d, open it and add meshes and materials to our list
                string objPath = EqFileHelper.ObjArchivePath(path);
                string objArchive = Path.GetFileNameWithoutExtension(objPath);
                var s3dObjArchive = ArchiveFactory.GetArchive(objPath, logger);
                if (s3dObjArchive.Initialize())
                {
                    string wldFileName = objArchive + LanternStrings.WldFormatExtension;
                    var objWldFile = new WldFileZone(s3dObjArchive.GetFile(wldFileName), shortName, WldType.Objects, logger, settings);
                    objWldFile.Initialize(rootFolder, false);
                    ArchiveExtractor.WriteWldTextures(s3dObjArchive, objWldFile, rootFolder + shortName + "/Zone/Textures/", logger);
                    meshes.AddRange(objWldFile.GetFragmentsOfType<Mesh>());
                    materialLists.AddRange(objWldFile.GetFragmentsOfType<MaterialList>());
                }
            }


            if (meshes.Count == 0)
            {
                return;
            }

            var meshWriter = new MeshObjWriter(ObjExportType.Textured, settings.ExportHiddenGeometry,
                settings.ExportZoneMeshGroups, wldFile.ZoneShortname);
            var collisionMeshWriter = new MeshObjWriter(ObjExportType.Collision, settings.ExportHiddenGeometry,
                settings.ExportZoneMeshGroups, wldFile.ZoneShortname);
            var materialListWriter = new MeshObjMtlWriter(settings, wldFile.ZoneShortname);

            foreach (var mesh in meshes)
            {
                // Find all associated objects with this mesh and instantiate each one.
                // If settings for ExportZoneWithObjects is false, this will immediately skip because objects will be an empty list
                var associatedObjects = objects.FindAll(o => !o.ObjectName.Contains("door") &&
                    mesh.Name.StartsWith(o.ObjectName, System.StringComparison.InvariantCultureIgnoreCase));
                foreach (var associatedObj in associatedObjects)
                {
                    // Pass in the associated object for offset, scale and rotation within the zone
                    meshWriter.AddFragmentData(mesh, associatedObj);
                }
                if (associatedObjects.Count == 0)
                {
                    meshWriter.AddFragmentData(mesh);
                    collisionMeshWriter.AddFragmentData(mesh);
                }
            }

            meshWriter.WriteAssetToFile(GetMeshPath(wldFile, wldFile.ZoneShortname));
            collisionMeshWriter.WriteAssetToFile(GetCollisionMeshPath(wldFile, wldFile.ZoneShortname));
            foreach (var materialList in materialLists)
            {
                materialListWriter.AddFragmentData(materialList);

            }
            materialListWriter.WriteAssetToFile(GetMaterialListPath(wldFile, FragmentNameCleaner.CleanName(materialLists[0])));

        }

        private static void ExportStaticActor(Actor actor, Settings settings, WldFile wldFile)
        {
            var mesh = actor?.MeshReference?.Mesh;

            if (mesh == null)
            {
                return;
            }

            var meshWriter = new MeshObjWriter(ObjExportType.Textured, settings.ExportHiddenGeometry,
                settings.ExportZoneMeshGroups, wldFile.ZoneShortname);
            var collisionMeshWriter = new MeshObjWriter(ObjExportType.Collision, settings.ExportHiddenGeometry,
                settings.ExportZoneMeshGroups, wldFile.ZoneShortname);
            var materialListWriter = new MeshObjMtlWriter(settings, wldFile.ZoneShortname);

            meshWriter.AddFragmentData(mesh);
            meshWriter.WriteAssetToFile(GetMeshPath(wldFile, FragmentNameCleaner.CleanName(mesh)));

            if (mesh.ExportSeparateCollision)
            {
                collisionMeshWriter.AddFragmentData(mesh);
                collisionMeshWriter.WriteAssetToFile(GetCollisionMeshPath(wldFile, FragmentNameCleaner.CleanName(mesh)));
            }

            var materialList = mesh.MaterialList;
            materialListWriter.AddFragmentData(materialList);
            materialListWriter.WriteAssetToFile(GetMaterialListPath(wldFile, FragmentNameCleaner.CleanName(materialList)));
        }

        private static void ExportSkeletalActor(Actor actor, Settings settings, WldFile wldFile)
        {
            var skeleton = actor?.SkeletonReference?.SkeletonHierarchy;

            if (skeleton == null)
            {
                return;
            }

            if (settings.ExportAllAnimationFrames)
            {
                foreach (var animation in skeleton.Animations)
                {
                    for (int i = 0; i < animation.Value.FrameCount; ++i)
                    {
                        WriteAnimationFrame(skeleton, animation.Key, i, settings, wldFile);
                    }
                }
            }
            else
            {
                WriteAnimationFrame(skeleton, "pos", 0, settings, wldFile);
            }

            List<MaterialList> materialLists = new List<MaterialList>();

            var materialListWriter = new MeshObjMtlWriter(settings, wldFile.ZoneShortname);

            var materialList = skeleton.Meshes?.FirstOrDefault()?.MaterialList;

            if (materialList != null)
            {
                materialLists.Add(materialList);
            }

            foreach (var skeletonBones in skeleton.Skeleton)
            {
                var boneMaterialList = skeletonBones.MeshReference?.Mesh?.MaterialList;

                if (boneMaterialList != null)
                {
                    if (!materialLists.Contains(boneMaterialList))
                    {
                        materialLists.Add(boneMaterialList);
                    }
                }
            }

            foreach (var ml in materialLists)
            {
                materialListWriter.AddFragmentData(ml);

                string savePath;

                if (wldFile.WldType == WldType.Characters)
                {
                    savePath = GetMaterialListPath(wldFile, FragmentNameCleaner.CleanName(ml), 0);
                }
                else
                {
                    savePath = GetMaterialListPath(wldFile, FragmentNameCleaner.CleanName(ml));
                }

                materialListWriter.WriteAssetToFile(savePath);

                for (int i = 0; i < ml.VariantCount; ++i)
                {
                    materialListWriter.ClearExportData();
                    materialListWriter.SetSkinId(i + 1);
                    materialListWriter.AddFragmentData(ml);
                    materialListWriter.WriteAssetToFile(GetMaterialListPath(wldFile, FragmentNameCleaner.CleanName(ml), i + 1));
                }

                materialListWriter.ClearExportData();
            }
        }

        private static void WriteAnimationFrame(SkeletonHierarchy skeleton, string animation, int frameIndex, Settings settings, WldFile wldFile)
        {
            var meshWriter = new MeshObjWriter(ObjExportType.Textured, settings.ExportHiddenGeometry,
                settings.ExportZoneMeshGroups, wldFile.ZoneShortname);

            meshWriter.SetIsCharacterModel(wldFile.WldType == WldType.Characters);

            foreach (var bone in skeleton.Skeleton)
            {
                var mesh = bone?.MeshReference?.Mesh;

                if (mesh != null)
                {
                    meshWriter.AddFragmentData(mesh);
                }
            }

            if (skeleton.Meshes != null)
            {
                foreach (var mesh in skeleton.Meshes)
                {
                    BackupVertices[mesh] = MeshExportHelper.ShiftMeshVertices(mesh, skeleton,
                        wldFile.WldType == WldType.Characters, animation, frameIndex);
                    meshWriter.AddFragmentData(mesh);
                }

                for (var i = 0; i < skeleton.SecondaryMeshes.Count; i++)
                {
                    var m2 = skeleton.SecondaryMeshes[i];
                    var meshWriter2 = new MeshObjWriter(ObjExportType.Textured, settings.ExportHiddenGeometry,
                        settings.ExportZoneMeshGroups, wldFile.ZoneShortname);
                    meshWriter2.SetIsCharacterModel(true);
                    meshWriter2.AddFragmentData(skeleton.Meshes[0]);
                    BackupVertices[m2] = MeshExportHelper.ShiftMeshVertices(m2, skeleton,
                        wldFile.WldType == WldType.Characters, animation, frameIndex);
                    meshWriter2.AddFragmentData(m2);
                    meshWriter2.WriteAssetToFile(GetMeshPath(wldFile, FragmentNameCleaner.CleanName(skeleton), i + 1));
                }
            }

            string fileName;

            if (settings.ExportAllAnimationFrames)
            {
                fileName = FragmentNameCleaner.CleanName(skeleton) + "_" + animation + "_" + frameIndex;
            }
            else
            {
                fileName = FragmentNameCleaner.CleanName(skeleton);
            }

            meshWriter.WriteAssetToFile(GetMeshPath(wldFile, fileName));
            RestoreVertices();
        }

        private static void RestoreVertices()
        {
            foreach (var shiftedMesh in BackupVertices)
            {
                shiftedMesh.Key.Vertices = shiftedMesh.Value;
            }
        }

        private static string GetMeshPath(WldFile wldFile, string meshName)
        {
            return wldFile.GetExportFolderForWldType() +
                   meshName + ".obj";
        }

        private static string GetMeshPath(WldFile wldFile, string meshName, int variant)
        {
            return wldFile.GetExportFolderForWldType() +
                   meshName + "_" + variant + ".obj";
        }

        private static string GetCollisionMeshPath(WldFile wldFile, string meshName)
        {
            return wldFile.GetExportFolderForWldType() +
                   meshName + "_collision.obj";
        }

        private static string GetMaterialListPath(WldFile wldFile, string materialListName)
        {
            return wldFile.GetExportFolderForWldType() + "/" +
                   materialListName + ".mtl";
        }

        private static string GetMaterialListPath(WldFile wldFile, string materialListName, int skinIndex)
        {
            return wldFile.GetExportFolderForWldType() + "/" +
                   materialListName + "_" + skinIndex + ".mtl";
        }
    }
}
