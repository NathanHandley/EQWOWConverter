using System.Collections.Generic;
using System.Linq;
using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Helpers
{
    /// <summary>
    /// Fixes numerous issues in EverQuest's character model files.
    /// A number of character models have incorrect shader assignments.
    /// As character models are specific to each zone, there are also conflicts with:
    /// 1. Texture names being used for different characters
    /// 2. Zones using different models/skeleton for the same race id
    /// This code is only run when exporting all characters to a single folder for batch importing.
    /// </summary>
    public class CharacterFixer
    {
        private WldFileCharacters _wld;

        public void Fix(WldFileCharacters wld)
        {
            _wld = wld;
            FixShipNames();
            FixGolemElemental();
            FixSnowDervish();
            FixAkanonKing();
            FixKaladimKing();
            FixFayDrake();
            FixTurtleTextures();
            FixBlackAndWhiteDragon();
            FixGhoulTextures();
            FixHalasFemale();
            FixBetaBeetle();
            FixHighpassMale();
        }

        /// <summary>
        /// Fix Ghoul face being applied to the back of the leg
        /// Thanks to modestlaw for requesting this
        /// </summary>
        private void FixGhoulTextures()
        {
            var meshes = _wld.GetFragmentsOfType<Mesh>();

            if (meshes.Count == 0)
            {
                return;
            }

            foreach (var mesh in meshes)
            {
                if (mesh.Name.StartsWith("GHUHE00"))
                {
                    var materialGroups = mesh.MaterialGroups;
                    materialGroups[1].MaterialIndex = 7;
                }
                else if (mesh.Name.StartsWith("GHU_"))
                {
                    var materialGroups = mesh.MaterialGroups;
                    materialGroups[0].MaterialIndex = 0;
                }
            }
        }

        /// <summary>
        /// Fixes the turtle textures being named incorrectly
        /// They use the seahorse prefix
        /// </summary>
        private void FixTurtleTextures()
        {
            var actors = _wld.GetFragmentsOfType<Actor>();

            foreach (var actor in actors)
            {
                if (!actor.Name.StartsWith("STU"))
                {
                    continue;
                }

                var materialList = actor.SkeletonReference.SkeletonHierarchy.Meshes.First().MaterialList;

                materialList.Name = materialList.Name.Replace("SEA", "STU");

                foreach (var material in materialList.Materials)
                {
                    material.Name = material.Name.Replace("SEA", "STU");
                    var bitmapNames = material.GetAllBitmapNames();

                    for (int i = 0; i < bitmapNames.Count; ++i)
                    {
                        string originalName = bitmapNames[i];
                        string newName = originalName.Replace("sea", "stu");
                        material.SetBitmapName(i, newName);
                        _wld.FilenameChanges[newName] = originalName;
                    }
                }
            }
        }

        /// <summary>
        /// There are two different versions of the Fay Drake (colorful and brown).
        /// The easiest way to differentiate is that the colorful version has two meshes and the brown has one.
        /// This changes the colorful variant to have a unique model name.
        /// </summary>
        private void FixFayDrake()
        {
            var actors = _wld.GetFragmentsOfType<Actor>();

            foreach (var actor in actors)
            {
                if (!actor.Name.StartsWith("FDR"))
                {
                    continue;
                }

                if (actor.SkeletonReference.SkeletonHierarchy.Meshes.Count != 2)
                {
                    continue;
                }

                // Rename actor
                actor.Name = actor.Name.Replace("FDR", "FDF");

                // Rename skeleton reference
                var skeletonRef = actor.SkeletonReference;
                skeletonRef.Name = skeletonRef.Name.Replace("FDR", "FDF");

                // Rename skeleton
                var skeleton = actor.SkeletonReference.SkeletonHierarchy;
                skeleton.Name = skeleton.Name.Replace("FDR", "FDF");

                // Rename skeleton bones
                //foreach(var bone in skeleton.Skeleton)
                //skeleton.Skeleton

                //skeleton.BuildSkeletonData(true);
                skeleton.RenameNodeBase("fdf");

                // Rename all main meshes
                foreach (var mesh in actor.SkeletonReference.SkeletonHierarchy.Meshes)
                {
                    mesh.Name = mesh.Name.Replace("FDR", "FDF");
                }

                // Rename all secondary meshes
                foreach (var mesh in actor.SkeletonReference.SkeletonHierarchy.SecondaryMeshes)
                {
                    mesh.Name = mesh.Name.Replace("FDR", "FDF");
                }

                // Rename all materials
                var materialList = actor.SkeletonReference.SkeletonHierarchy.Meshes.First().MaterialList;

                materialList.Name = materialList.Name.Replace("FDR", "FDF");

                foreach (var material in materialList.Materials)
                {
                    material.Name = material.Name.Replace("FDR", "FDF");

                    var bitmapNames = material.GetAllBitmapNames();

                    for (int i = 0; i < bitmapNames.Count; ++i)
                    {
                        string originalName = bitmapNames[i];
                        string newName = originalName.Replace("fdr", "fdf");
                        material.SetBitmapName(i, newName);
                        _wld.FilenameChanges[newName] = originalName;
                    }
                }
            }
        }

        /// <summary>
        /// Fixes the unused Kaladim king model crown shader assignment
        /// </summary>
        private void FixKaladimKing()
        {
            var crownMaterial = _wld.GetFragmentByName<Material>("KAHE0001_MDF");

            if (crownMaterial != null)
            {
                crownMaterial.ShaderType = ShaderType.TransparentMasked;
            }
        }

        /// <summary>
        /// Fixes the unused Ak'Anon king model crown shader assignment
        /// </summary>
        private void FixAkanonKing()
        {
            var crownMaterial = _wld.GetFragmentByName<Material>("CLHE0004_MDF");

            if (crownMaterial != null)
            {
                crownMaterial.ShaderType = ShaderType.TransparentMasked;
            }
        }

        /// <summary>
        /// Fixes incorrect shader assignment and texture naming conflicts with normal Dervish
        /// </summary>
        private void FixSnowDervish()
        {
            var actors = _wld.GetFragmentsOfType<Actor>();

            foreach (var actor in actors)
            {
                if (actor.SkeletonReference == null)
                {
                    continue;
                }

                if (!actor.Name.StartsWith("SDE"))
                {
                    continue;
                }

                foreach (var mesh in actor.SkeletonReference.SkeletonHierarchy.Meshes)
                {
                    foreach (var material in mesh.MaterialList.Materials)
                    {
                        // This texture needs to be masked
                        if (material.Name == "SDEUA0006_MDF")
                        {
                            material.ShaderType = ShaderType.TransparentMasked;
                        }

                        var bitmapNames = material.GetAllBitmapNames();

                        for (var i = 0; i < bitmapNames.Count; i++)
                        {
                            if (!bitmapNames[i].StartsWith("dml"))
                            {
                                continue;
                            }

                            string originalName = bitmapNames[i];
                            string newName = originalName.Replace("dml", "sde");
                            material.SetBitmapName(i, newName);
                            _wld.FilenameChanges[newName] = originalName;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Golem Elemental incorrectly uses (and overwrites) normal Golem material names
        /// </summary>
        private void FixGolemElemental()
        {
            var actor = _wld.GetFragmentByName<Actor>("GOM_ACTORDEF");

            if (actor == null)
            {
                return;
            }

            foreach (var mesh in actor.SkeletonReference.SkeletonHierarchy.Meshes)
            {
                foreach (var material in mesh.MaterialList.Materials)
                {
                    if (!material.Name.StartsWith("GOL"))
                    {
                        continue;
                    }

                    material.Name = material.Name.Replace("GOL", "GOM");

                    var bitmapNames = material.GetAllBitmapNames();

                    for (var i = 0; i < bitmapNames.Count; i++)
                    {
                        string originalName = bitmapNames[i];
                        string newName = originalName.Replace("gol", "gom");
                        material.SetBitmapName(i, newName);
                        _wld.FilenameChanges[newName] = originalName;
                    }
                }
            }
        }

        private void FixShipNames()
        {
            var actors = _wld.GetFragmentsOfType<Actor>();

            foreach (var actor in actors)
            {
                if (actor.Name.StartsWith("GSP") && (actor?.MeshReference?.Mesh?.Name ?? null) != null)
                {
                    actor.MeshReference.Mesh.Name =
                        actor.MeshReference?.Mesh?.Name?.Replace("GHOSTSHIP", "GSP") ?? "GSP";
                    actor.MeshReference.Mesh.MaterialList.Name =
                        actor?.MeshReference?.Mesh?.MaterialList?.Name?.Replace("GHOSTSHIP", "GSP") ?? "GSP";
                }

                if (actor.Name.StartsWith("LAUNCH") && (actor?.MeshReference?.Mesh?.Name ?? null) != null)
                {
                    actor.Name = actor.MeshReference.Mesh.Name.Replace("DMSPRITEDEF", "ACTORDEF");
                }

                if (actor.Name.StartsWith("PRE"))
                {
                    if (actor.SkeletonReference == null)
                    {
                        continue;
                    }

                    switch (actor.SkeletonReference.SkeletonHierarchy.Name)
                    {
                        // Bloated Belly in Iceclad
                        case "OGS_HS_DEF":
                        {
                            actor.Name = actor.Name.Replace("PRE", "OGS");
                            break;
                        }
                        // Sea King, Golden Maiden, StormBreaker, SirensBane
                        case "PRE_HS_DEF":
                        {
                            break;
                        }
                    }
                }


                if (actor.Name.StartsWith("SHIP"))
                {
                    if (actor.SkeletonReference == null)
                    {
                        continue;
                    }

                    switch (actor.SkeletonReference.SkeletonHierarchy.Name)
                    {
                        // Icebreaker in Iceclad
                        case "GNS_HS_DEF":
                        {
                            actor.Name = actor.Name.Replace("SHIP", "GNS");
                            break;
                        }
                        // Maidens Voyage in Firiona Vie
                        case "ELS_HS_DEF":
                        {
                            actor.Name = actor.Name.Replace("SHIP", "ELS");
                            break;
                        }
                    }
                }
            }
        }

        private void FixBlackAndWhiteDragon()
        {
            var actors = _wld.GetFragmentsOfType<Actor>();

            foreach (var actor in actors)
            {
                if (!actor.Name.StartsWith("BWD"))
                {
                    continue;
                }

                var frag = _wld.GetFragmentByName<Material>("BWDCH0101_MDF");

                if (frag != null)
                {
                    frag.ShaderType = ShaderType.Diffuse;
                }

                var materialFragment = _wld.GetFragmentByName<MaterialList>("BWD_MP");
                if (materialFragment != null)
                {
                    var slot = materialFragment.Slots["bwd_ch01"];
                    // TODO: Fix this
                    //slot[1] = "d_bwdch0101";
                }
            }
        }

        private void FixHalasFemale()
        {
            var skeleton = _wld.GetFragmentByName<SkeletonHierarchy>("HLF_HS_DEF");

            if (skeleton == null)
            {
                return;
            }

            RenameBone(skeleton, 7, "bi_l");
            RenameBone(skeleton, 10, "l_point");
            RenameBone(skeleton, 15, "head_point");
        }

        /// <summary>
        /// Early beta beetles have SPI named bones
        /// </summary>
        private void FixBetaBeetle()
        {
            var skeleton = _wld.GetFragmentByName<SkeletonHierarchy>("BET_HS_DEF");

            if (skeleton == null)
            {
                return;
            }

            for (var i = 0; i < skeleton.Skeleton.Count; i++)
            {
                var bone = skeleton.Skeleton[i];
                var boneName = bone.CleanedName;
                if (boneName.StartsWith("spi"))
                {
                    RenameBone(skeleton, i, boneName.Substring(3));
                }
            }
        }

        /// <summary>
        /// Highpass Citizen (Guard) has two head attach points
        /// This can cause issues with particle emitting from incorrect skeleton point
        /// </summary>
        private void FixHighpassMale()
        {
            var skeleton = _wld.GetFragmentByName<SkeletonHierarchy>("HHM_HS_DEF");

            if (skeleton == null)
            {
                return;
            }

            if (skeleton.Skeleton.Count < 25)
            {
                return;
            }

            skeleton.Skeleton[0].Children = new List<int>
            {
                1
            };

            skeleton.Skeleton.RemoveAt(24);
        }

        private void RenameBone(SkeletonHierarchy skeleton, int index, string newBoneName)
        {
            var oldBoneName = skeleton.BoneMappingClean[index];

            skeleton.BoneMappingClean[index] = newBoneName;
            skeleton.Skeleton[index].CleanedName = newBoneName;

            for (var i = 0; i < skeleton.Skeleton.Count; i++)
            {
                var fullPath = skeleton.Skeleton[i].CleanedFullPath;
                if (!fullPath.Contains(oldBoneName))
                {
                    continue;
                }

                skeleton.Skeleton[i].CleanedFullPath = fullPath.Replace(oldBoneName, newBoneName);
            }
        }
    }
}
