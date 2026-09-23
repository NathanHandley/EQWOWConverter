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

// Nathan: This is a huge beast.  While it was started by me, Claude Opus took it over as we debugged the issues one-by-one.  Leaving the Opus notes as-is for future debugging.

using EQWOWConverter.Common;
using EQWOWConverter.Creatures;
using EQWOWConverter.EQFiles;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;

namespace EQWOWConverter.ObjectModels
{
    // The client dresses a player-character-format model by compositing item texture components into fixed regions of one body
    // texture.  EQ character models keep their own geometry, UVs and piece textures: every EQ material's UV window is placed (scale
    // and offset only) inside the composite block that its item slot writes, the base skin bakes the naked textures into those same
    // rects, and per-race worn armor components bake the other EQ texture sets into them (GenerateNativeArmorComponentTextures), so
    // the art lies on the mesh exactly as it does on the pre-baked creature versions.  Layout runs in ApplyUVRemap over the final
    // model vertices, and the bakes sample the source art through the same windows and rects
    internal class ObjectModelCharacterComposite
    {
        // Composite layout space (the client's region table is in these units; textures are generated at a multiple of it)
        public const int BODY_COMPOSITE_WIDTH = 256;
        public const int BODY_COMPOSITE_HEIGHT = 256;

        // The client's FaceLower and FaceUpper regions, which it fills from the CharSections face textures
        public const int FACE_LOWER_X0 = 0;
        public const int FACE_LOWER_Y0 = 192;
        public const int FACE_LOWER_X1 = 128;
        public const int FACE_LOWER_Y1 = 256;
        public const int FACE_UPPER_X0 = 0;
        public const int FACE_UPPER_Y0 = 160;
        public const int FACE_UPPER_X1 = 128;
        public const int FACE_UPPER_Y1 = 192;
        public const UInt32 ROBE_BASE_GEOSET_ID = 1301;  // Geoset group 13 default: the body pieces a robe replaces (hidden while a robe is worn)
        public const UInt32 ROBE_SKIRT_GEOSET_ID = 1302; // Geoset the client enables for robe chest pieces (geoset group 13, value 1): the robe mesh
        public const string BLANK_TEXTURE_NAME = "EQBlank";
        public const byte MESH_CONTEXT_BODY = 0;
        public const byte MESH_CONTEXT_HEAD = 1;
        public const byte MESH_CONTEXT_ROBE = 2;
        public const byte MESH_CONTEXT_HELM_HEAD_BASE = 10; // + helm index 1-3: the helmed head meshes ('{skeleton}he01'..'he03'), shown as hair style geosets
        // Erudite hood: a head-mesh piece textured with one fixed robe-cloth texture whose colour comes from the worn robe.  It gets a
        // reserved corner of a region only chest items and robes ever paint: the base skin and every chest component bake the default
        // hood there, the robe components bake it tinted
        // The erudite hood's reserved corner, in the upper arm region: the erudite upper arm art fills only the left 65 columns of that 128x64
        // region, so the hood (a 64x45 texel window) fits at ~0.94 there, where a 32x32 torso corner held it at 0.45.  Chest and robe items
        // both rewrite the upper arm region, so the worn robe still paints the hood (see GenerateNativeArmorComponentTextures)
        public const int HOOD_RESERVED_X0 = 66;
        public const int HOOD_RESERVED_Y0 = 0;
        public const int HOOD_RESERVED_X1 = 128;
        public const int HOOD_RESERVED_Y1 = 48;
        public const UInt32 HEAD_GEOSET_ID = 1; // Hair style 0 = the bare head; the helmed heads take 1 + helm index (see CharHairGeosets rows)
        // Masked helm art whose transparent texels stay opaque anyway (Nathan 2026-09-23: they read as holes straight into the head in game):
        // the gnome male chain cap's slit (helm18) and the back of the human female plate helm (helm15).  Keyed 'skeleton:texture'
        private static readonly HashSet<string> MASKED_ALPHA_IGNORED_TEXTURES_BY_SKELETON = new HashSet<string>() { "gnm:helm18", "huf:helm15" };
        public const int HELM_TEXTURE_SIZE = 128; // Layout units of a helmed head's own texture (its private pieces), generated at the texture scale
        public const int HELM_TINT_MAX = 9; // Hair color indexes 1-9 carry tinted copies of the helm textures for NPC helm colors
        // Every packed piece keeps a margin of its own wrapped art on all sides (the source texture's continuation, which is what the EQ
        // mesh wraps to at that edge).  The client samples the composite bilinear with mipmaps: at a piece edge, mip level L blends the
        // texel inside with the 2^L composite texels outside, so the margin has to hold 2^L texels of the piece's own art or the blend
        // takes in the neighbour (the gnome collar's corner texel took the chest plate's grey, and the mirrored chest's centre line, which
        // runs along the piece's edge, took the collar).  Four texels keep mips 0-2 clean.  The 32-row hand and foot blocks keep two
        // (mips 0-1): their pieces are small on screen, their neighbours are the same material, and four would cost them a quarter of
        // their rows.  The face block keeps one (mip 0): it is what players look at up close, its main piece fills the column
        // texel-for-texel with only a sliver left for the small pieces, and wider gutters pushed the whole block down to a shared 73%
        // (the gnome face went blurry).  Between two pieces the gutter holds both margins; at an area's edge the margin sits in the
        // border shrunk off the area (the texture border wraps, block boundaries meet the client's other sources)
        private const int EDGE_MARGIN_TEXELS = 4;
        private const int EDGE_MARGIN_TEXELS_SHORT_BLOCK = 2;
        private const int EDGE_MARGIN_TEXELS_FACE = 1;

        private static int GetEdgeMarginTexels(CharacterCompositeBlock block)
        {
            if (block == CharacterCompositeBlock.Hand || block == CharacterCompositeBlock.Foot)
                return EDGE_MARGIN_TEXELS_SHORT_BLOCK;
            if (block == CharacterCompositeBlock.Face)
                return EDGE_MARGIN_TEXELS_FACE;
            return EDGE_MARGIN_TEXELS;
        }

        // Where a vertex's bones sit on the body (used when extracting the robe skirt geometry)
        public enum CharacterBoneGroup
        {
            Pelvis,
            Thigh,
            Shin,
            Foot,
            Torso,
            NeckHead,
            Bicep,
            Forearm,
            Hand,
            Tail,
            Other
        }

        // The composite blocks materials are placed in.  Each is the union of the regions one item slot writes, so a worn piece
        // replaces exactly the EQ materials that piece's texture set covers
        public enum CharacterCompositeBlock
        {
            Torso,     // TorsoUpper + TorsoLower (chest item)
            Legs,      // LegUpper + LegLower (legs item; boots leave LegLower alone)
            Foot,      // Foot (feet item)
            ArmUpper,  // ArmUpper (chest item's arm component)
            ArmLower,  // ArmLower (wrist item; gloves leave it alone)
            Hand,      // Hand (hands item)
            Face,      // FaceUpper + FaceLower (head mesh and misc pieces, baked per face into the CharSections face textures)
            Robe,      // The robe mesh pieces (robe geoset): laid over the Legs, Torso and ArmUpper blocks, which the robe item rewrites
            Helm       // A helmed head's private pieces: its own texture, delivered through the CharSections hair rows of its hair style
        }

        public static bool IsHeadMeshContext(byte meshContext)
        {
            return meshContext == MESH_CONTEXT_HEAD || (meshContext >= MESH_CONTEXT_HELM_HEAD_BASE + 1 && meshContext <= MESH_CONTEXT_HELM_HEAD_BASE + 3);
        }

        // 0 for the bare head mesh, 1-3 for the helmed head meshes, -1 for anything else
        public static int GetHelmIndexForMeshContext(byte meshContext)
        {
            if (meshContext == MESH_CONTEXT_HEAD)
                return 0;
            if (meshContext >= MESH_CONTEXT_HELM_HEAD_BASE + 1 && meshContext <= MESH_CONTEXT_HELM_HEAD_BASE + 3)
                return meshContext - MESH_CONTEXT_HELM_HEAD_BASE;
            return -1;
        }

        public class CharacterCompositeSlice
        {
            public string MaterialUniqueName = string.Empty;
            public string BakeSourceTextureName = string.Empty; // Blank = no bake (the robe skirt samples the leg regions)
            public bool IsFaceRegionSlice = false; // Lives in the face block, so it bakes into the FaceLower/FaceUpper textures instead of the body
            public bool IsHeadSlice = false; // Head piece, so the bake swaps in the face variant source textures per face index
            public bool IsRobeLayer = false; // Robe mesh piece: shown only with a robe, so it is absent from the base skin and armor components and may overlap body rects
            public int HelmIndex = -1; // Head pieces: 0 = bare head mesh, 1-3 = helmed head meshes (hair style geosets); -1 for body pieces
            public bool IsHelmArt = false; // Helm piece of a helmed head (takes the creature tint's helm color; the face piece of that head does not)
            public bool IsHoodSlice = false; // Robe cloth on the head (erudite hood): lives in the reserved torso corner so the worn robe colours it
            public bool IsSharedHeadPiece = false; // Face block piece shown with every head variant (the face itself, tails); a helmed head's private pieces go to its helm texture instead
            public bool IsMaskedMaterial = false; // Masked material (EQ 'tm_' art: helm eye holes, ear cutouts); a helmed head keeps such pieces private (its helm texture can carry alpha)
            public bool IsAlphaKeyed = false; // ... and the bake keeps its alpha there, unless the texture is listed in MASKED_ALPHA_IGNORED_TEXTURES_BY_SKELETON
            public CharacterCompositeBlock Block = CharacterCompositeBlock.Face;
            public int SourceWidth = 16;
            public int SourceHeight = 16;
            public int VertexCount = 0;
            public float RectX0 = 0; // Composite (256) space
            public float RectY0 = 0;
            public float RectX1 = 0;
            public float RectY1 = 0;
            public float UVMinU = 0; // Folded EQ UV window the rect holds (the max edges are snapped up to whole source texels)
            public float UVMinV = 0;
            public float UVMaxU = 1;
            public float UVMaxV = 1;
            public bool IsLaidOut = false;
            public float ClampX0 = 0; // Area the piece was packed into (composite units); the bake bleed never leaves it
            public float ClampY0 = 0;
            public float ClampX1 = 0;
            public float ClampY1 = 0;
            public int EdgeMarginTexels = 1; // Own wrapped art baked around the rect on every side (composite texels), see EDGE_MARGIN_TEXELS
            public bool IsRotated = false;   // Placed turned a quarter turn: the rect's X runs along the source's V axis and its Y along U
            public bool PullInX0 = false;    // The rect sits on its area's edge with no margin there (see PackBlock): the tile-edge vertices on
            public bool PullInX1 = false;    // that side move inward instead, FULL_BLOCK_EDGE_PULL_IN_TEXELS (composite axes and sides)
            public bool PullInY0 = false;
            public bool PullInY1 = false;
            public float MaxFitFactor = 1f;  // Largest scale the packer gives the piece (a seam piece at reduced resolution, see ObjectModelEQData.SplitSeamPieces)
            public bool IsBodyTexturedHeadPiece = false; // Bare-head piece textured from a body part (the high elf female's hair bow, a scrap of the leg texture): its own
                                                        // rect, kept out of the robe layout and left clear in the robe components so the worn leg armor shows through
            public float MeshArea = 0f;      // Surface the piece covers on the model (model units squared), the packer's weight for its resolution
            public float TileSeamPhaseU = 0; // Where the mesh's texture tile boundary sits on each axis (tiles from the texture's edge, see
            public float TileSeamPhaseV = 0; // ObjectModelEQData.CalculateTileSeamPhases): tile-edge vertices lie at the phase plus a whole number
            public bool WrapsU = false;      // The mesh continues across the window's U edges (a vertex sits on both), so the margin holds
            public bool WrapsV = false;      // the wrapped art; otherwise the margin replicates the edge (a mirrored top of the head)
        }

        // A worn component texture: its name suffix and the region rect (256 space) it is cut from its block's canvas
        private class ComponentRegion
        {
            public string Name;
            public CharacterCompositeBlock Block;
            public int X0;
            public int Y0;
            public int X1;
            public int Y1;

            public ComponentRegion(string name, CharacterCompositeBlock block, int x0, int y0, int x1, int y1)
            {
                Name = name;
                Block = block;
                X0 = x0;
                Y0 = y0;
                X1 = x1;
                Y1 = y1;
            }
        }

        private static readonly ComponentRegion[] ComponentRegions = new ComponentRegion[]
        {
            new ComponentRegion("Chest_TU", CharacterCompositeBlock.Torso, 128, 0, 256, 64),
            new ComponentRegion("Chest_TL", CharacterCompositeBlock.Torso, 128, 64, 256, 96),
            new ComponentRegion("Legs_LU", CharacterCompositeBlock.Legs, 128, 96, 256, 160),
            new ComponentRegion("Legs_LL", CharacterCompositeBlock.Legs, 128, 160, 256, 224),
            new ComponentRegion("Feet_FO", CharacterCompositeBlock.Foot, 128, 224, 256, 256),
            new ComponentRegion("Arms_AU", CharacterCompositeBlock.ArmUpper, 0, 0, 128, 64),
            new ComponentRegion("Wrist_AL", CharacterCompositeBlock.ArmLower, 0, 64, 128, 128),
            new ComponentRegion("Hand_HA", CharacterCompositeBlock.Hand, 0, 128, 128, 160)
        };


        private static readonly object CharacterTextureBakeLock = new object();
        private static bool BlankTextureGenerated = false;

        public string SkeletonName = string.Empty;
        public List<CharacterCompositeSlice> Slices = new List<CharacterCompositeSlice>();
        public List<int> ValidFaceIndexes = new List<int>();
        public List<int> HelmVariantIndexes = new List<int>(); // Helmed heads (1-3) that have a helm texture
        private List<ColorRGBA> HelmTintPalette = new List<ColorRGBA>(); // Hair color 1.. = these tints of the helm textures

        // The NPC helm colors this race's looks use, so tinted helm textures exist for them (hair color indexes 1..)
        public void SetHelmTintPalette(List<ColorRGBA> helmColors)
        {
            HelmTintPalette = new List<ColorRGBA>();
            foreach (ColorRGBA helmColor in helmColors)
            {
                if (HelmTintPalette.Count >= HELM_TINT_MAX)
                    break;
                bool exists = false;
                foreach (ColorRGBA existing in HelmTintPalette)
                    if (existing == helmColor)
                        exists = true;
                if (exists == false)
                    HelmTintPalette.Add(helmColor);
            }
        }

        public int GetHelmTintCount()
        {
            return HelmTintPalette.Count;
        }

        // Hair color index for an NPC helm color (0 = untinted / not in the palette)
        public int GetHelmTintIndex(ColorRGBA? helmColor)
        {
            if (helmColor == null)
                return 0;
            for (int i = 0; i < HelmTintPalette.Count; i++)
                if (HelmTintPalette[i] == (ColorRGBA)helmColor)
                    return i + 1;
            return 0;
        }

        // A helmed head's race-prefixed body-part textures follow the helm's armor set in EQ (leather 01, chain 02, plate 03), as the old
        // per-set models did: the human female plate helm is her leg texture ('huflg0001' -> 'huflg0301'), the erudite plate hood the hand
        // texture ('ermhn0002' -> 'ermhn0302').  Head textures ('he', the face variants) and unprefixed helm art ('helm15') stay as they are
        private string GetHelmSetTextureName(string textureName, int helmIndex, string sourceTexturesFolder)
        {
            string skeletonLower = SkeletonName.ToLower();
            string textureLower = textureName.ToLower();
            if (helmIndex <= 0 || textureLower.Length < skeletonLower.Length + 6 || textureLower.StartsWith(skeletonLower) == false)
                return textureName;
            if (textureLower.Substring(skeletonLower.Length, 2) == "he")
                return textureName;
            string setTextureName = string.Concat(textureName.Substring(0, skeletonLower.Length + 2), helmIndex.ToString("00"), textureName.Substring(skeletonLower.Length + 4));
            if (File.Exists(Path.Combine(sourceTexturesFolder, setTextureName + ".png")) == false)
                return textureName;
            return setTextureName;
        }

        public string GetHelmTextureName(int helmIndex, int tintIndex)
        {
            return string.Concat("EQ", SkeletonName.ToUpper(), "Helm", helmIndex.ToString("00"), "C", tintIndex.ToString());
        }

        // True once the material's laid-out slice sits in a helmed head's own texture (Helm block), which ApplyUVRemap decides
        public bool IsHelmTextureMaterial(string materialUniqueName)
        {
            foreach (CharacterCompositeSlice slice in Slices)
                if (slice.MaterialUniqueName == materialUniqueName && slice.IsLaidOut == true && slice.Block == CharacterCompositeBlock.Helm)
                    return true;
            return false;
        }
        private Dictionary<string, UInt32> GeosetIDByMaterialUniqueName = new Dictionary<string, UInt32>();

        // Geoset (skin section) a material's render groups take: the robe mesh pieces show only with a robe, the body pieces a robe
        // replaces (chest, legs, arms) hide while one is worn, and everything else (hands, feet, head) always shows
        // The erudite hood texture of a robe set: the base head carries 'clk{race}06', the sets carry 'clk{set}06' like their other cloth
        private static string GetRobeSetHoodTextureName(string hoodTextureName, string robeSetDigits, string sourceTexturesFolder)
        {
            if (hoodTextureName.Length < 2)
                return hoodTextureName;
            string setHoodTextureName = string.Concat("clk", robeSetDigits, hoodTextureName.Substring(hoodTextureName.Length - 2));
            if (File.Exists(Path.Combine(sourceTexturesFolder, setHoodTextureName + ".png")) == true)
                return setHoodTextureName;
            return hoodTextureName;
        }

        // The packed rects (256 space, grown by the margin) of the body-textured head pieces, see CharacterCompositeSlice.IsBodyTexturedHeadPiece
        private List<int[]> GetBodyTexturedHeadPieceRects(int marginTexels)
        {
            List<int[]> rects = new List<int[]>();
            foreach (CharacterCompositeSlice slice in Slices)
            {
                if (slice.IsBodyTexturedHeadPiece == false || slice.IsLaidOut == false || slice.RectX1 <= slice.RectX0)
                    continue;
                rects.Add(new int[4] { Convert.ToInt32(Math.Floor(slice.RectX0)) - marginTexels, Convert.ToInt32(Math.Floor(slice.RectY0)) - marginTexels,
                    Convert.ToInt32(Math.Ceiling(slice.RectX1)) + marginTexels, Convert.ToInt32(Math.Ceiling(slice.RectY1)) + marginTexels });
            }
            return rects;
        }

        // Cuts a rect out of every area it overlaps, leaving the parts above and below at full width and the strips beside it
        private static List<int[]> SubtractRectFromAreas(List<int[]> areas, int[] cut)
        {
            List<int[]> result = new List<int[]>();
            foreach (int[] area in areas)
            {
                if (cut[2] <= area[0] || cut[0] >= area[2] || cut[3] <= area[1] || cut[1] >= area[3])
                {
                    result.Add(area);
                    continue;
                }
                int cutY0 = Math.Max(cut[1], area[1]);
                int cutY1 = Math.Min(cut[3], area[3]);
                int cutX0 = Math.Max(cut[0], area[0]);
                int cutX1 = Math.Min(cut[2], area[2]);
                if (cutY0 > area[1])
                    result.Add(new int[4] { area[0], area[1], area[2], cutY0 });
                if (cutY1 < area[3])
                    result.Add(new int[4] { area[0], cutY1, area[2], area[3] });
                if (cutX0 > area[0])
                    result.Add(new int[4] { area[0], cutY0, cutX0, cutY1 });
                if (cutX1 < area[2])
                    result.Add(new int[4] { cutX1, cutY0, area[2], cutY1 });
            }
            return result;
        }

        public bool HasHoodSlice()
        {
            foreach (CharacterCompositeSlice slice in Slices)
                if (slice.IsHoodSlice == true && slice.IsLaidOut == true)
                    return true;
            return false;
        }

        public UInt32 GetGeosetIDForMaterial(string materialUniqueName)
        {
            if (GeosetIDByMaterialUniqueName.ContainsKey(materialUniqueName) == true)
                return GeosetIDByMaterialUniqueName[materialUniqueName];
            return 0;
        }

        public string GetBodyTextureName()
        {
            return string.Concat("EQ", SkeletonName.ToUpper(), "Body");
        }

        public string GetFaceLowerTextureName(int faceIndex)
        {
            return string.Concat("EQ", SkeletonName.ToUpper(), "FaceL", faceIndex.ToString("00"));
        }

        public string GetFaceUpperTextureName(int faceIndex)
        {
            return string.Concat("EQ", SkeletonName.ToUpper(), "FaceU", faceIndex.ToString("00"));
        }

        // Worn component texture name (no gender suffix; the item display infos reference these with the color appended)
        public static string GetNativeComponentTextureName(string skeletonName, string componentName, int armorID)
        {
            return string.Concat("EQ_Native_", skeletonName.ToUpper(), "_", componentName, "_", armorID.ToString("00"));
        }

        public static string GetNativeComponentFolder()
        {
            return Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedEquipmentTextures", "EQNativeComponents");
        }

        public static CharacterBoneGroup GetBoneGroupForBoneName(string boneName)
        {
            string name = boneName.ToLower();
            if (name == "pe")
                return CharacterBoneGroup.Pelvis;
            if (name.StartsWith("th"))
                return CharacterBoneGroup.Thigh;
            if (name.StartsWith("ca"))
                return CharacterBoneGroup.Shin;
            if (name.StartsWith("bo") || name.StartsWith("to"))
                return CharacterBoneGroup.Foot;
            if (name == "ch")
                return CharacterBoneGroup.Torso;
            if (name == "ne" || name == "he" || name == "ja")
                return CharacterBoneGroup.NeckHead;
            if (name.StartsWith("bi"))
                return CharacterBoneGroup.Bicep;
            if (name.StartsWith("fo"))
                return CharacterBoneGroup.Forearm;
            if (name.StartsWith("fi"))
                return CharacterBoneGroup.Hand;
            if (name.StartsWith("ta"))
                return CharacterBoneGroup.Tail;
            return CharacterBoneGroup.Other;
        }

        // Used when extracting the robe skirt geometry (the flowing part of the robe mesh sits on the pelvis/leg bones)
        public static bool IsBoneGroupCountsLegsDominant(Dictionary<CharacterBoneGroup, int> boneGroupCounts)
        {
            int legCount = 0;
            int otherCount = 0;
            foreach (var boneGroupCount in boneGroupCounts)
            {
                if (boneGroupCount.Key == CharacterBoneGroup.Pelvis || boneGroupCount.Key == CharacterBoneGroup.Thigh || boneGroupCount.Key == CharacterBoneGroup.Shin)
                    legCount += boneGroupCount.Value;
                else
                    otherCount += boneGroupCount.Value;
            }
            return legCount > otherCount;
        }

        // Face piece textures are named '{skeleton}he00{faceIndex}{headPieceDigit}', so the face swaps in at the second-to-last character
        public static string GetFaceVariantTextureName(string faceZeroTextureName, int faceIndex)
        {
            if (faceZeroTextureName.Length < 2)
                return faceZeroTextureName;
            return string.Concat(faceZeroTextureName.Substring(0, faceZeroTextureName.Length - 2), faceIndex.ToString(),
                faceZeroTextureName[faceZeroTextureName.Length - 1]);
        }

        // Block rect in composite (256) space
        public static void GetBlockRect(CharacterCompositeBlock block, out int x0, out int y0, out int x1, out int y1)
        {
            switch (block)
            {
                case CharacterCompositeBlock.Torso: x0 = 128; y0 = 0; x1 = 256; y1 = 96; break;
                case CharacterCompositeBlock.Legs: x0 = 128; y0 = 96; x1 = 256; y1 = 224; break;
                case CharacterCompositeBlock.Foot: x0 = 128; y0 = 224; x1 = 256; y1 = 256; break;
                case CharacterCompositeBlock.ArmUpper: x0 = 0; y0 = 0; x1 = 128; y1 = 64; break;
                case CharacterCompositeBlock.ArmLower: x0 = 0; y0 = 64; x1 = 128; y1 = 128; break;
                case CharacterCompositeBlock.Hand: x0 = 0; y0 = 128; x1 = 128; y1 = 160; break;
                case CharacterCompositeBlock.Robe: x0 = 128; y0 = 0; x1 = 256; y1 = 224; break; // Torso + Legs (ArmUpper is its other area)
                case CharacterCompositeBlock.Helm: x0 = 0; y0 = 0; x1 = HELM_TEXTURE_SIZE; y1 = HELM_TEXTURE_SIZE; break; // Its own texture's space
                case CharacterCompositeBlock.Face:
                default: x0 = FACE_UPPER_X0; y0 = FACE_UPPER_Y0; x1 = FACE_LOWER_X1; y1 = FACE_LOWER_Y1; break;
            }
        }

        private static int GetTextureScale()
        {
            return Math.Max(1, Configuration.GENERATE_ILLUSION_CHARACTER_TEXTURE_SCALE);
        }

        // Classifies every material of a player character mesh into its composite block.  Windows and rects come later in ApplyUVRemap,
        // over the final model vertices
        public static ObjectModelCharacterComposite BuildForCharacterModel(string skeletonName, MeshData meshData, List<Material> materials,
            EQSkeleton skeletonData, List<byte> meshContextByVertexIndex)
        {
            ObjectModelCharacterComposite composite = new ObjectModelCharacterComposite();
            composite.SkeletonName = skeletonName;
            string skeletonNameLower = skeletonName.ToLower();
            string sourceTexturesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "characters", "Textures");

            foreach (Material material in materials)
            {
                if (material.TextureNames.Count == 0)
                    continue;

                // Triangle contexts and vertex count for this material
                int bodyCount = 0;
                int headCount = 0;
                int robeCount = 0;
                Dictionary<int, int> countsByHelmIndex = new Dictionary<int, int>();
                HashSet<int> vertexIndexes = new HashSet<int>();
                foreach (TriangleFace triangleFace in meshData.TriangleFaces)
                {
                    if (Convert.ToUInt32(triangleFace.MaterialIndex) != material.Index)
                        continue;
                    vertexIndexes.Add(triangleFace.V1);
                    vertexIndexes.Add(triangleFace.V2);
                    vertexIndexes.Add(triangleFace.V3);
                    byte context = MESH_CONTEXT_BODY;
                    if (triangleFace.V1 < meshContextByVertexIndex.Count)
                        context = meshContextByVertexIndex[triangleFace.V1];
                    if (IsHeadMeshContext(context) == true)
                    {
                        headCount++;
                        int helmIndex = GetHelmIndexForMeshContext(context);
                        if (countsByHelmIndex.ContainsKey(helmIndex) == false)
                            countsByHelmIndex.Add(helmIndex, 0);
                        countsByHelmIndex[helmIndex] = countsByHelmIndex[helmIndex] + 1;
                    }
                    else if (context == MESH_CONTEXT_ROBE)
                        robeCount++;
                    else
                        bodyCount++;
                }
                int dominantHelmIndex = 0;
                int dominantHelmCount = -1;
                foreach (var countByHelmIndex in countsByHelmIndex)
                {
                    if (countByHelmIndex.Value > dominantHelmCount)
                    {
                        dominantHelmCount = countByHelmIndex.Value;
                        dominantHelmIndex = countByHelmIndex.Key;
                    }
                }
                if (vertexIndexes.Count == 0)
                    continue;

                CharacterCompositeSlice slice = new CharacterCompositeSlice();
                slice.MaterialUniqueName = material.UniqueName;
                slice.BakeSourceTextureName = material.TextureNames[0];
                slice.VertexCount = vertexIndexes.Count;
                slice.TileSeamPhaseU = material.TileSeamPhaseU;
                slice.TileSeamPhaseV = material.TileSeamPhaseV;
                slice.MaxFitFactor = GetSeamPieceMaxFitFactor(material.UniqueName);
                slice.IsMaskedMaterial = material.MaterialType == MaterialType.TransparentMasked || material.MaterialType == MaterialType.Transparent25Percent
                    || material.MaterialType == MaterialType.Transparent50Percent || material.MaterialType == MaterialType.Transparent75Percent;
                slice.IsAlphaKeyed = slice.IsMaskedMaterial == true
                    && MASKED_ALPHA_IGNORED_TEXTURES_BY_SKELETON.Contains(string.Concat(composite.SkeletonName.ToLower(), ":", material.TextureNames[0].ToLower())) == false;
                int sourceWidth;
                int sourceHeight;
                if (TryReadPNGDimensions(Path.Combine(sourceTexturesFolder, material.TextureNames[0] + ".png"), out sourceWidth, out sourceHeight) == true)
                {
                    slice.SourceWidth = sourceWidth;
                    slice.SourceHeight = sourceHeight;
                }

                // Part code from the texture name ('ch', 'ua', 'fa', 'hn', 'lg', 'ft', 'he', or 'clk')
                string textureNameLower = material.TextureNames[0].ToLower();
                string partCode = string.Empty;
                if (textureNameLower.StartsWith("clk"))
                    partCode = "clk";
                else if (textureNameLower.StartsWith(skeletonNameLower) && textureNameLower.Length >= skeletonNameLower.Length + 2)
                    partCode = textureNameLower.Substring(skeletonNameLower.Length, 2);

                if (robeCount > 0 && (partCode == "ft" || partCode == "hn"))
                {
                    // The robe mesh's own feet and hands: ordinary foot and hand pieces (the feet and hands items dress them) that show
                    // only with a robe, in place of the base body's
                    slice.Block = (partCode == "ft") ? CharacterCompositeBlock.Foot : CharacterCompositeBlock.Hand;
                    composite.GeosetIDByMaterialUniqueName[material.UniqueName] = ROBE_SKIRT_GEOSET_ID;
                }
                else if (robeCount > 0)
                {
                    // Robe mesh pieces: the robe cloth (and on iksar the legs and forearms that show under it), shown only with a robe
                    slice.Block = CharacterCompositeBlock.Robe;
                    slice.IsRobeLayer = true;
                    composite.GeosetIDByMaterialUniqueName[material.UniqueName] = ROBE_SKIRT_GEOSET_ID;
                }
                else if ((headCount >= bodyCount) && partCode == "clk")
                {
                    // The erudite hood (see HOOD_RESERVED_*): shown with its head's geoset, textured from the upper arm region's reserved corner
                    slice.Block = CharacterCompositeBlock.ArmUpper;
                    slice.IsHoodSlice = true;
                    slice.HelmIndex = dominantHelmIndex;
                    composite.GeosetIDByMaterialUniqueName[material.UniqueName] = HEAD_GEOSET_ID + Convert.ToUInt32(dominantHelmIndex);
                }
                else if ((headCount >= bodyCount) && dominantHelmIndex == 0 && (partCode == "ch" || partCode == "lg" || partCode == "ft" || partCode == "ua" || partCode == "fa" || partCode == "hn"))
                {
                    // Bare head pieces textured with a body part's texture (the high elf female's hair bow is a scrap of the leg texture) follow that
                    // part's block, so the worn armor's art colors them the way EQ does; they still show with the bare head geoset.  They keep a
                    // rect of their own (a robe rewrites the whole block, and in EQ the bow follows the leggings, not the robe), at half
                    // resolution: the bow is a hand's width on the model
                    slice.IsBodyTexturedHeadPiece = true;
                    slice.MaxFitFactor = 0.5f;
                    switch (partCode)
                    {
                        case "ch": slice.Block = CharacterCompositeBlock.Torso; break;
                        case "lg": slice.Block = CharacterCompositeBlock.Legs; break;
                        case "ft": slice.Block = CharacterCompositeBlock.Foot; break;
                        case "ua": slice.Block = CharacterCompositeBlock.ArmUpper; break;
                        case "fa": slice.Block = CharacterCompositeBlock.ArmLower; break;
                        default: slice.Block = CharacterCompositeBlock.Hand; break;
                    }
                    composite.GeosetIDByMaterialUniqueName[material.UniqueName] = HEAD_GEOSET_ID;
                }
                else if (headCount >= bodyCount || partCode == "he")
                {
                    // Head mesh pieces (and the neck slivers of the head texture split onto the body mesh) live in the face block and swap
                    // face variant textures per face index.  Helm textures on the helmed head meshes are face-independent and bake
                    // identically into every face's textures.  The bare head is hair style geoset 1, the helmed heads 2-4
                    slice.Block = CharacterCompositeBlock.Face;
                    slice.IsFaceRegionSlice = true;
                    slice.IsHeadSlice = true;
                    slice.HelmIndex = dominantHelmIndex;
                    slice.IsHelmArt = (partCode != "he" && partCode != "clk" && dominantHelmIndex > 0); // Only the helmed heads' private pieces; body-textured pieces of the bare head (e.g. the high elf female's hair bow) keep their base art untinted
                    composite.GeosetIDByMaterialUniqueName[material.UniqueName] = HEAD_GEOSET_ID + Convert.ToUInt32(dominantHelmIndex);
                }
                else
                {
                    // The pieces a robe replaces hide while one is worn (the robe mesh carries its own chest, arms, legs and feet)
                    // The robe mesh replaces the whole base body (it carries its own cloth, feet and hands), so every base part hides with it
                    UInt32 baseGeosetID = ROBE_BASE_GEOSET_ID;
                    switch (partCode)
                    {
                        case "ch": slice.Block = CharacterCompositeBlock.Torso; composite.GeosetIDByMaterialUniqueName[material.UniqueName] = baseGeosetID; break;
                        case "lg": slice.Block = CharacterCompositeBlock.Legs; composite.GeosetIDByMaterialUniqueName[material.UniqueName] = baseGeosetID; break;
                        case "ft": slice.Block = CharacterCompositeBlock.Foot; composite.GeosetIDByMaterialUniqueName[material.UniqueName] = baseGeosetID; break;
                        case "ua": slice.Block = CharacterCompositeBlock.ArmUpper; composite.GeosetIDByMaterialUniqueName[material.UniqueName] = baseGeosetID; break;
                        case "fa": slice.Block = CharacterCompositeBlock.ArmLower; composite.GeosetIDByMaterialUniqueName[material.UniqueName] = baseGeosetID; break;
                        case "hn": slice.Block = CharacterCompositeBlock.Hand; composite.GeosetIDByMaterialUniqueName[material.UniqueName] = baseGeosetID; break;
                        default:
                            {
                                slice.Block = CharacterCompositeBlock.Face;
                                slice.IsFaceRegionSlice = true;
                                Logger.WriteDebug(string.Concat("Character composite for '", skeletonName, "' material '", material.UniqueName,
                                    "' has no item slot (part code '", partCode, "'), so it is packed into the face block as a misc piece"));
                            }
                            break;
                    }
                }
                composite.Slices.Add(slice);
            }

            // Determine which face indexes exist by checking for face variant textures of the head pieces
            composite.ValidFaceIndexes.Add(0);
            for (int faceIndex = 1; faceIndex <= 9; faceIndex++)
            {
                bool faceTextureExists = false;
                foreach (CharacterCompositeSlice slice in composite.Slices)
                {
                    if (slice.IsHeadSlice == false)
                        continue;
                    string faceTextureName = GetFaceVariantTextureName(slice.BakeSourceTextureName, faceIndex);
                    if (faceTextureName != slice.BakeSourceTextureName && File.Exists(Path.Combine(sourceTexturesFolder, faceTextureName + ".png")) == true)
                    {
                        faceTextureExists = true;
                        break;
                    }
                }
                if (faceTextureExists == true)
                    composite.ValidFaceIndexes.Add(faceIndex);
            }

            return composite;
        }

        // Lays out every material inside its block and rewrites the model's UVs into the composite.  Windows come from the tile-folded
        // EQ UVs of the final model vertices, so the mesh and the bakes always share one window per material
        public void ApplyUVRemap(List<ObjectModelVertex> modelVertices, List<TriangleFace> modelTriangles, List<ObjectModelMaterial> modelMaterials)
        {
            Dictionary<string, CharacterCompositeSlice> slicesByMaterialUniqueName = new Dictionary<string, CharacterCompositeSlice>();
            foreach (CharacterCompositeSlice slice in Slices)
                slicesByMaterialUniqueName[slice.MaterialUniqueName] = slice;

            // Triangles per material
            Dictionary<string, List<int[]>> trianglesByMaterialUniqueName = new Dictionary<string, List<int[]>>();
            HashSet<string> unmappedMaterialNames = new HashSet<string>();
            foreach (TriangleFace triangleFace in modelTriangles)
            {
                if (triangleFace.MaterialIndex >= modelMaterials.Count)
                    continue;
                string materialUniqueName = modelMaterials[triangleFace.MaterialIndex].Material.UniqueName;
                if (slicesByMaterialUniqueName.ContainsKey(materialUniqueName) == false)
                {
                    if (unmappedMaterialNames.Contains(materialUniqueName) == false)
                    {
                        unmappedMaterialNames.Add(materialUniqueName);
                        Logger.WriteError(string.Concat("Character composite for '", SkeletonName, "' has no slice for material '", materialUniqueName, "', so those UVs are unmapped"));
                    }
                    continue;
                }
                if (trianglesByMaterialUniqueName.ContainsKey(materialUniqueName) == false)
                    trianglesByMaterialUniqueName.Add(materialUniqueName, new List<int[]>());
                trianglesByMaterialUniqueName[materialUniqueName].Add(new int[3] { triangleFace.V1, triangleFace.V2, triangleFace.V3 });
            }

            // Fold each material's UVs onto a shared tile (see CalculateTileFoldedUVs) and take the window.  The window's far edges snap
            // up to whole source texels at the generated texture scale, so the bake lands texel-for-texel on the rect
            int textureScale = GetTextureScale();
            Dictionary<string, Dictionary<int, float[]>> foldedUVsByMaterialUniqueName = new Dictionary<string, Dictionary<int, float[]>>();
            foreach (var trianglesByMaterialName in trianglesByMaterialUniqueName)
            {
                CharacterCompositeSlice slice = slicesByMaterialUniqueName[trianglesByMaterialName.Key];
                Dictionary<int, float[]> rawUVsByVertexIndex = new Dictionary<int, float[]>();
                foreach (int[] triangleCornerVertexIndexes in trianglesByMaterialName.Value)
                    foreach (int cornerVertexIndex in triangleCornerVertexIndexes)
                        if (rawUVsByVertexIndex.ContainsKey(cornerVertexIndex) == false)
                            rawUVsByVertexIndex.Add(cornerVertexIndex, new float[2] { modelVertices[cornerVertexIndex].Texture1TextureCoordinates.X, modelVertices[cornerVertexIndex].Texture1TextureCoordinates.Y });
                Dictionary<int, float[]> foldedUVs = CalculateTileFoldedUVs(trianglesByMaterialName.Value, rawUVsByVertexIndex);
                foldedUVsByMaterialUniqueName.Add(trianglesByMaterialName.Key, foldedUVs);
                if (foldedUVs.Count == 0)
                    continue;
                slice.MeshArea = CalculateMeshArea(trianglesByMaterialName.Value, modelVertices);
                float minU = float.MaxValue;
                float maxU = float.MinValue;
                float minV = float.MaxValue;
                float maxV = float.MinValue;
                foreach (var foldedUVByVertexIndex in foldedUVs)
                {
                    minU = Math.Min(minU, foldedUVByVertexIndex.Value[0]);
                    maxU = Math.Max(maxU, foldedUVByVertexIndex.Value[0]);
                    minV = Math.Min(minV, foldedUVByVertexIndex.Value[1]);
                    maxV = Math.Max(maxV, foldedUVByVertexIndex.Value[1]);
                }
                // Anchor the window's center inside the first tile (a whole-tile shift, so the texture samples identically).  The fold is
                // per material, so the same texture on two materials (the face on the bare head and on a helmed head) could land on
                // different tiles, and the same-texture merge below would then union two non-overlapping tiles into a window twice as wide
                // as the art, which the packer has to shrink
                float tileShiftU = Convert.ToSingle(Math.Floor((minU + maxU) * 0.5f));
                float tileShiftV = Convert.ToSingle(Math.Floor((minV + maxV) * 0.5f));
                if (tileShiftU != 0 || tileShiftV != 0)
                {
                    foreach (var foldedUVByVertexIndex in foldedUVs)
                    {
                        foldedUVByVertexIndex.Value[0] -= tileShiftU;
                        foldedUVByVertexIndex.Value[1] -= tileShiftV;
                    }
                    minU -= tileShiftU; maxU -= tileShiftU;
                    minV -= tileShiftV; maxV -= tileShiftV;
                }
                float texelsPerU = slice.SourceWidth * textureScale;
                float texelsPerV = slice.SourceHeight * textureScale;
                // The window's near edges snap down to whole source texels like the far edges snap up, so the rect edge sits on a texel
                // boundary of the art.  The ogre chest's back centre has one vertex at u = 1/256 (a quarter texel in) beside vertices at
                // u = 0: with the window starting at that quarter texel, the u = 0 vertices sat in the margin and the whole back centre
                // blended with the wrapped far column, a line up the back
                minU = Convert.ToSingle(Math.Floor((minU * texelsPerU) + 0.001f)) / texelsPerU;
                minV = Convert.ToSingle(Math.Floor((minV * texelsPerV) + 0.001f)) / texelsPerV;
                float spanTexelsU = Math.Max(1, Convert.ToSingle(Math.Ceiling(((maxU - minU) * texelsPerU) - 0.001f)));
                float spanTexelsV = Math.Max(1, Convert.ToSingle(Math.Ceiling(((maxV - minV) * texelsPerV) - 0.001f)));
                slice.UVMinU = minU;
                slice.UVMinV = minV;
                slice.UVMaxU = minU + (spanTexelsU / texelsPerU);
                slice.UVMaxV = minV + (spanTexelsV / texelsPerV);
                slice.VertexCount = foldedUVs.Count;
                slice.IsLaidOut = true;
                DetectWrapSeams(slice, foldedUVs, modelVertices);
            }

            // Slices of one block that sample the same texture (the face on the bare head and on each helmed head) share one window and,
            // in the packer, one rect, so the art is only stored once
            foreach (CharacterCompositeSlice slice in Slices)
            {
                if (slice.IsLaidOut == false)
                    continue;
                foreach (CharacterCompositeSlice otherSlice in Slices)
                {
                    if (otherSlice == slice || otherSlice.IsLaidOut == false || otherSlice.Block != slice.Block || otherSlice.BakeSourceTextureName != slice.BakeSourceTextureName)
                        continue;
                    // A seam piece (ObjectModelEQData.SplitSeamPieces) exists to keep its own small window, a body-textured head piece its own rect
                    if (IsSeamPiece(slice) == true || IsSeamPiece(otherSlice) == true || slice.IsBodyTexturedHeadPiece == true || otherSlice.IsBodyTexturedHeadPiece == true)
                        continue;
                    float minU = Math.Min(slice.UVMinU, otherSlice.UVMinU);
                    float minV = Math.Min(slice.UVMinV, otherSlice.UVMinV);
                    float maxU = Math.Max(slice.UVMaxU, otherSlice.UVMaxU);
                    float maxV = Math.Max(slice.UVMaxV, otherSlice.UVMaxV);
                    slice.UVMinU = minU; slice.UVMinV = minV; slice.UVMaxU = maxU; slice.UVMaxV = maxV;
                    otherSlice.UVMinU = minU; otherSlice.UVMinV = minV; otherSlice.UVMaxU = maxU; otherSlice.UVMaxV = maxV;
                }
            }

            // Face block pieces shown with every head variant (a texture on more than one head, and non-head pieces) are shared; the rest
            // are private to their head variant and only one variant ever shows, so those overlap each other in the layout
            foreach (CharacterCompositeSlice slice in Slices)
            {
                if (slice.IsLaidOut == false || slice.Block != CharacterCompositeBlock.Face)
                    continue;
                if (slice.HelmIndex < 0)
                {
                    slice.IsSharedHeadPiece = true;
                    continue;
                }
                // Masked art (a plate helm's eye holes) needs its alpha, which only a helmed head's own texture carries: the face regions of
                // the composite are opaque.  So a helmed head keeps such pieces private even when the bare head shares the texture
                if (slice.IsMaskedMaterial == true && slice.HelmIndex > 0)
                    continue;
                foreach (CharacterCompositeSlice otherSlice in Slices)
                {
                    if (otherSlice == slice || otherSlice.IsLaidOut == false || otherSlice.Block != CharacterCompositeBlock.Face)
                        continue;
                    if (otherSlice.BakeSourceTextureName == slice.BakeSourceTextureName && otherSlice.HelmIndex != slice.HelmIndex)
                        slice.IsSharedHeadPiece = true;
                }
            }

            // A helmed head's private pieces (its helm art) live in that head's own texture, which the client applies through the hair
            // texture of the head's hair style: they leave the face block, so the face keeps its resolution and the pieces need no room there
            HelmVariantIndexes.Clear();
            foreach (CharacterCompositeSlice slice in Slices)
            {
                if (slice.IsLaidOut == false || slice.Block != CharacterCompositeBlock.Face || slice.HelmIndex <= 0 || slice.IsSharedHeadPiece == true)
                    continue;
                slice.Block = CharacterCompositeBlock.Helm;
                slice.IsFaceRegionSlice = false;
                // A seam piece's reduced resolution (ObjectModelEQData.SplitSeamPieces) pays for room in the crowded body blocks; a helm texture
                // holds one head's few pieces, so every piece may go texel-for-texel there (the human female's leather helm sides, the
                // erudite plate helm's rim)
                slice.MaxFitFactor = 1f;
                if (HelmVariantIndexes.Contains(slice.HelmIndex) == false)
                    HelmVariantIndexes.Add(slice.HelmIndex);
            }
            HelmVariantIndexes.Sort();

            // Pack the laid-out slices into their blocks (each helmed head's texture packs on its own)
            foreach (CharacterCompositeBlock block in Enum.GetValues(typeof(CharacterCompositeBlock)))
            {
                if (block == CharacterCompositeBlock.Helm)
                {
                    foreach (int helmIndex in HelmVariantIndexes)
                    {
                        List<CharacterCompositeSlice> helmSlices = new List<CharacterCompositeSlice>();
                        foreach (CharacterCompositeSlice slice in Slices)
                            if (slice.Block == block && slice.IsLaidOut == true && slice.HelmIndex == helmIndex)
                                helmSlices.Add(slice);
                        if (helmSlices.Count > 0)
                            PackBlock(block, helmSlices, textureScale);
                    }
                    continue;
                }
                List<CharacterCompositeSlice> blockSlices = new List<CharacterCompositeSlice>();
                foreach (CharacterCompositeSlice slice in Slices)
                    if (slice.Block == block && slice.IsLaidOut == true && slice.IsHoodSlice == false)
                        blockSlices.Add(slice);
                if (blockSlices.Count > 0)
                    PackBlock(block, blockSlices, textureScale);
            }
            // Hood pieces sit in the reserved corner (HOOD_RESERVED_*), as large as it allows (at most texel-for-texel), sharing the one rect
            foreach (CharacterCompositeSlice slice in Slices)
            {
                if (slice.IsHoodSlice == false || slice.IsLaidOut == false)
                    continue;
                float hoodTexelsU = Math.Max(1f, (slice.UVMaxU - slice.UVMinU) * slice.SourceWidth);
                float hoodTexelsV = Math.Max(1f, (slice.UVMaxV - slice.UVMinV) * slice.SourceHeight);
                // One texel clear on every side of the reserved corner, like every packed area (see PackBlock)
                float hoodFactor = Math.Min(1f, Math.Min((HOOD_RESERVED_X1 - HOOD_RESERVED_X0 - 2) / hoodTexelsU, (HOOD_RESERVED_Y1 - HOOD_RESERVED_Y0 - 2) / hoodTexelsV));
                slice.RectX0 = HOOD_RESERVED_X0 + 1;
                slice.RectY0 = HOOD_RESERVED_Y0 + 1;
                slice.RectX1 = HOOD_RESERVED_X0 + (hoodTexelsU * hoodFactor);
                slice.RectY1 = HOOD_RESERVED_Y0 + (hoodTexelsV * hoodFactor);
                slice.ClampX0 = HOOD_RESERVED_X0;
                slice.ClampY0 = HOOD_RESERVED_Y0;
                slice.ClampX1 = HOOD_RESERVED_X1;
                slice.ClampY1 = HOOD_RESERVED_Y1;
            }

            // Rewrite the UVs
            HashSet<int> remappedVertexIndexes = new HashSet<int>();
            foreach (var trianglesByMaterialName in trianglesByMaterialUniqueName)
            {
                string materialUniqueName = trianglesByMaterialName.Key;
                CharacterCompositeSlice slice = slicesByMaterialUniqueName[materialUniqueName];
                if (slice.IsLaidOut == false)
                    continue;
                foreach (int[] triangleCornerVertexIndexes in trianglesByMaterialName.Value)
                {
                    foreach (int cornerVertexIndex in triangleCornerVertexIndexes)
                    {
                        if (remappedVertexIndexes.Contains(cornerVertexIndex) == true)
                            continue;
                        remappedVertexIndexes.Add(cornerVertexIndex);
                        float foldedU = modelVertices[cornerVertexIndex].Texture1TextureCoordinates.X;
                        float foldedV = modelVertices[cornerVertexIndex].Texture1TextureCoordinates.Y;
                        if (foldedUVsByMaterialUniqueName[materialUniqueName].ContainsKey(cornerVertexIndex) == true)
                        {
                            foldedU = foldedUVsByMaterialUniqueName[materialUniqueName][cornerVertexIndex][0];
                            foldedV = foldedUVsByMaterialUniqueName[materialUniqueName][cornerVertexIndex][1];
                        }
                        float normalizedU = (foldedU - slice.UVMinU) / (slice.UVMaxU - slice.UVMinU);
                        float normalizedV = (foldedV - slice.UVMinV) / (slice.UVMaxV - slice.UVMinV);
                        float targetWidth = (slice.Block == CharacterCompositeBlock.Helm) ? HELM_TEXTURE_SIZE : BODY_COMPOSITE_WIDTH;
                        float targetHeight = (slice.Block == CharacterCompositeBlock.Helm) ? HELM_TEXTURE_SIZE : BODY_COMPOSITE_HEIGHT;
                        float alongX = slice.IsRotated ? normalizedV : normalizedU;
                        float alongY = slice.IsRotated ? normalizedU : normalizedV;
                        float compositeX = slice.RectX0 + (alongX * (slice.RectX1 - slice.RectX0));
                        float compositeY = slice.RectY0 + (alongY * (slice.RectY1 - slice.RectY0));

                        // A vertex sitting exactly on a tile edge (wrap seams, tile cuts, mirrored centre lines) normally stays put:
                        // the margin beyond the edge holds the wrapped continuation of the art (or the edge itself where the mesh
                        // mirrors), so the client's bilinear sample blends across the seam the way the GPU wrap did in EQ (a pull-in
                        // there hardened the half elf's bicep seam into a light/dark step).  Only where the piece's edge sits on its
                        // area's boundary with no margin (see PackBlock) is the vertex pulled two composite texels inward: the texel
                        // beyond belongs to another component, and two texels keep the mip 1 and mip 2 samples inside the piece.  The
                        // seam then shows as a hard cut, which is why the mesh's seam was placed where the art is most continuous
                        // (ObjectModelEQData.CalculateTileSeamPhases)
                        int edgeU = GetTileEdgeSide(foldedU, slice.UVMinU, slice.UVMaxU, slice.SourceWidth);
                        int edgeV = GetTileEdgeSide(foldedV, slice.UVMinV, slice.UVMaxV, slice.SourceHeight);
                        float pullAlongU = 0f;
                        if (edgeU < 0 && (slice.IsRotated ? slice.PullInY0 : slice.PullInX0) == true)
                            pullAlongU = FULL_BLOCK_EDGE_PULL_IN_TEXELS;
                        if (edgeU > 0 && (slice.IsRotated ? slice.PullInY1 : slice.PullInX1) == true)
                            pullAlongU = -FULL_BLOCK_EDGE_PULL_IN_TEXELS;
                        float pullAlongV = 0f;
                        if (edgeV < 0 && (slice.IsRotated ? slice.PullInX0 : slice.PullInY0) == true)
                            pullAlongV = FULL_BLOCK_EDGE_PULL_IN_TEXELS;
                        if (edgeV > 0 && (slice.IsRotated ? slice.PullInX1 : slice.PullInY1) == true)
                            pullAlongV = -FULL_BLOCK_EDGE_PULL_IN_TEXELS;
                        if (slice.IsRotated == true)
                        {
                            compositeY += pullAlongU;
                            compositeX += pullAlongV;
                        }
                        else
                        {
                            compositeX += pullAlongU;
                            compositeY += pullAlongV;
                        }
                        modelVertices[cornerVertexIndex].Texture1TextureCoordinates.X = compositeX / targetWidth;
                        modelVertices[cornerVertexIndex].Texture1TextureCoordinates.Y = compositeY / targetHeight;
                    }
                }
            }
            WriteLayoutReport();
        }

        // Whether the mesh wraps across the piece's tile edges on each axis: the fold and the tile cut leave a vertex at u = 0 and a
        // duplicate at u = 1 on the same position where the art continues around (the back seam of a chest, every tile cut).  A
        // mirrored edge (the top of a head, where both halves meet at v = 0) has vertices on one edge only.  The bake fills the
        // margin beyond a wrapping edge with the wrapped art and beyond any other edge with the edge texels themselves: the dark elf's
        // scalp sat on the face block's top row, and the wrapped row above it was the under-chin art, which the client blended in
        // at every mip past the first (black planes on the top of the head at a distance or from above)
        // Summed triangle area on the model
        private static float CalculateMeshArea(List<int[]> triangles, List<ObjectModelVertex> modelVertices)
        {
            double area = 0;
            foreach (int[] triangle in triangles)
            {
                if (triangle[0] >= modelVertices.Count || triangle[1] >= modelVertices.Count || triangle[2] >= modelVertices.Count)
                    continue;
                Vector3 a = modelVertices[triangle[0]].Position;
                Vector3 b = modelVertices[triangle[1]].Position;
                Vector3 c = modelVertices[triangle[2]].Position;
                double abx = b.X - a.X, aby = b.Y - a.Y, abz = b.Z - a.Z;
                double acx = c.X - a.X, acy = c.Y - a.Y, acz = c.Z - a.Z;
                double cx = (aby * acz) - (abz * acy);
                double cy = (abz * acx) - (abx * acz);
                double cz = (abx * acy) - (aby * acx);
                area += 0.5 * Math.Sqrt((cx * cx) + (cy * cy) + (cz * cz));
            }
            return Convert.ToSingle(area);
        }

        private static void DetectWrapSeams(CharacterCompositeSlice slice, Dictionary<int, float[]> foldedUVs, List<ObjectModelVertex> modelVertices)
        {
            slice.WrapsU = HasWrapSeam(foldedUVs, modelVertices, 0, slice.TileSeamPhaseU, slice.SourceWidth);
            slice.WrapsV = HasWrapSeam(foldedUVs, modelVertices, 1, slice.TileSeamPhaseV, slice.SourceHeight);
            // A seam piece is a window onto the middle of a continuous surface: the art continues beyond every edge
            if (IsSeamPiece(slice) == true)
            {
                slice.WrapsU = true;
                slice.WrapsV = true;
            }
        }

        private static bool IsSeamPiece(CharacterCompositeSlice slice)
        {
            return slice.MaterialUniqueName.Contains(ObjectModelEQData.SEAM_PIECE_MATERIAL_SUFFIX);
        }

        // A seam piece's name ends its suffix with the resolution it was costed at, in percent ('_seamu50')
        private static float GetSeamPieceMaxFitFactor(string materialUniqueName)
        {
            int suffixIndex = materialUniqueName.IndexOf(ObjectModelEQData.SEAM_PIECE_MATERIAL_SUFFIX);
            if (suffixIndex < 0)
                return 1f;
            int digitsStart = suffixIndex + ObjectModelEQData.SEAM_PIECE_MATERIAL_SUFFIX.Length + 1;
            int digitsEnd = digitsStart;
            while (digitsEnd < materialUniqueName.Length && char.IsDigit(materialUniqueName[digitsEnd]) == true)
                digitsEnd++;
            int percent;
            if (digitsEnd > digitsStart && int.TryParse(materialUniqueName.Substring(digitsStart, digitsEnd - digitsStart), out percent) == true && percent > 0 && percent <= 100)
                return percent / 100f;
            return 1f;
        }

        private static bool HasWrapSeam(Dictionary<int, float[]> foldedUVs, List<ObjectModelVertex> modelVertices, int axis, float seamPhase, int sourceTexels)
        {
            // A vertex on the tile edge, or the mesh's own edge vertex half a texel in from it (ObjectModelEQData.InsetWholeTextureCoordinates,
            // and EQ's data itself puts the far side of the human female's back seam at u = 63.5/64)
            float edgeTolerance = WRAP_EDGE_TOLERANCE_TEXELS / Math.Max(1, sourceTexels);
            Dictionary<int, List<Vector3>> edgePositionsByTile = new Dictionary<int, List<Vector3>>();
            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;
            foreach (var foldedUV in foldedUVs)
            {
                if (foldedUV.Key >= modelVertices.Count)
                    continue;
                Vector3 position = modelVertices[foldedUV.Key].Position;
                minX = Math.Min(minX, position.X); minY = Math.Min(minY, position.Y); minZ = Math.Min(minZ, position.Z);
                maxX = Math.Max(maxX, position.X); maxY = Math.Max(maxY, position.Y); maxZ = Math.Max(maxZ, position.Z);
                float value = foldedUV.Value[axis] - seamPhase;
                float nearest = Convert.ToSingle(Math.Round(value));
                if (Math.Abs(value - nearest) > edgeTolerance)
                    continue;
                int tile = Convert.ToInt32(nearest);
                List<Vector3> positions;
                if (edgePositionsByTile.TryGetValue(tile, out positions) == false)
                {
                    positions = new List<Vector3>();
                    edgePositionsByTile.Add(tile, positions);
                }
                positions.Add(position);
            }
            if (edgePositionsByTile.Count < 2)
                return false;
            // The two edges meet where a vertex on one lies within a small share of the piece's extent of a vertex on the other: the
            // human female's back seam is a hair open (x = 0 on one side, 0.007 on the other), and an exact position match missed it
            double diagonal = Math.Sqrt(((maxX - minX) * (maxX - minX)) + ((maxY - minY) * (maxY - minY)) + ((maxZ - minZ) * (maxZ - minZ)));
            double positionTolerance = diagonal * WRAP_SEAM_POSITION_TOLERANCE_SHARE;
            List<int> tiles = new List<int>(edgePositionsByTile.Keys);
            for (int i = 0; i < tiles.Count; i++)
            {
                for (int j = i + 1; j < tiles.Count; j++)
                {
                    foreach (Vector3 positionA in edgePositionsByTile[tiles[i]])
                        foreach (Vector3 positionB in edgePositionsByTile[tiles[j]])
                            if (positionA.GetDistance(positionB) <= positionTolerance)
                                return true;
                }
            }
            return false;
        }

        // Which edge of the piece window a folded UV component sits on: -1 the near edge, +1 the far edge, 0 neither (see ApplyUVRemap).
        // The window's near edge snaps down to a whole source texel below the lowest vertex and its far edge up above the highest, so
        // a vertex within a texel of an edge is on it (wrap seams, tile cuts, mirrored centre lines, or just the piece's outline)
        private static int GetTileEdgeSide(float foldedValue, float windowMin, float windowMax, int sourceTexels)
        {
            float texel = 1.001f / Math.Max(1, sourceTexels);
            if (foldedValue < (windowMin + windowMax) * 0.5f)
                return (foldedValue < windowMin + texel) ? -1 : 0;
            return (foldedValue > windowMax - texel) ? 1 : 0;
        }

        // One CSV per race listing every piece's source texture, window (in tiles), placed rect and the resulting texel scale (1 = the
        // art bakes texel-for-texel), for reviewing where resolution is lost
        private void WriteLayoutReport()
        {
            try
            {
                string reportFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "GeneratedCharacterTextures", SkeletonName.ToUpper());
                if (Directory.Exists(reportFolder) == false)
                    Directory.CreateDirectory(reportFolder);
                StringBuilder report = new StringBuilder();
                report.AppendLine("Material,Texture,SourceW,SourceH,WindowTilesU,WindowTilesV,Block,Geoset,HelmIndex,RectW,RectH,TexelScaleU,TexelScaleV,VertexCount,Rotated,MarginTexels,PullIn,SeamU,SeamV,Wraps");
                foreach (CharacterCompositeSlice slice in Slices)
                {
                    if (slice.IsLaidOut == false)
                        continue;
                    float windowU = slice.UVMaxU - slice.UVMinU;
                    float windowV = slice.UVMaxV - slice.UVMinV;
                    float rectW = slice.RectX1 - slice.RectX0;
                    float rectH = slice.RectY1 - slice.RectY0;
                    float rectAlongU = slice.IsRotated ? rectH : rectW;
                    float rectAlongV = slice.IsRotated ? rectW : rectH;
                    float scaleU = (windowU * slice.SourceWidth) > 0 ? rectAlongU / (windowU * slice.SourceWidth) : 0;
                    float scaleV = (windowV * slice.SourceHeight) > 0 ? rectAlongV / (windowV * slice.SourceHeight) : 0;
                    report.AppendLine(string.Join(",", slice.MaterialUniqueName, slice.BakeSourceTextureName, slice.SourceWidth.ToString(), slice.SourceHeight.ToString(),
                        windowU.ToString("0.###", CultureInfo.InvariantCulture), windowV.ToString("0.###", CultureInfo.InvariantCulture), slice.Block.ToString(),
                        GetGeosetIDForMaterial(slice.MaterialUniqueName).ToString(), slice.HelmIndex.ToString(), rectW.ToString("0.#", CultureInfo.InvariantCulture),
                        rectH.ToString("0.#", CultureInfo.InvariantCulture), scaleU.ToString("0.###", CultureInfo.InvariantCulture), scaleV.ToString("0.###", CultureInfo.InvariantCulture),
                        slice.VertexCount.ToString(), slice.IsRotated ? "1" : "0", slice.EdgeMarginTexels.ToString(),
                        string.Concat(slice.PullInX0 ? "L" : "", slice.PullInX1 ? "R" : "", slice.PullInY0 ? "T" : "", slice.PullInY1 ? "B" : ""),
                        Convert.ToInt32(Math.Round(slice.TileSeamPhaseU * slice.SourceWidth)).ToString(), Convert.ToInt32(Math.Round(slice.TileSeamPhaseV * slice.SourceHeight)).ToString(),
                        string.Concat(slice.WrapsU ? "U" : "", slice.WrapsV ? "V" : "")));
                }
                File.WriteAllText(Path.Combine(reportFolder, "layout_report.csv"), report.ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteError(string.Concat("Character composite layout report for '", SkeletonName, "' could not be written: ", ex.Message));
            }
        }

        // Shelf-packs a block's slices.  Pieces are sized by their UV window in source texels at the generated texture scale (so a piece
        // renders texel-for-texel at fit factor 1).  The face block's two areas (FaceUpper and FaceLower) are packed separately since the
        // client composes them from two textures and a piece crossing the seam renders with a gap line through it; its shared pieces (the
        // face) pack first, then each head variant's private pieces pack into what is left, every variant starting from the same state so
        // the variants overlap each other (only one head shows at a time).  The robe pieces spread over the Legs, Torso and ArmUpper
        // blocks (the regions a robe item rewrites and no other item does), overlapping the body pieces there since the two never show
        // together
        private void PackBlock(CharacterCompositeBlock block, List<CharacterCompositeSlice> blockSlices, int textureScale)
        {
            int blockX0, blockY0, blockX1, blockY1;
            GetBlockRect(block, out blockX0, out blockY0, out blockX1, out blockY1);
            List<int[]> areas = new List<int[]>(); // x0, y0, x1, y1 in generated pixels
            if (block == CharacterCompositeBlock.Face)
            {
                // FaceUpper and FaceLower stack into one 128x96 column; the client blits the two face textures into adjacent regions of the
                // one composite (the stock face art itself runs across that boundary), so pieces may span it
                areas.Add(new int[4] { Math.Min(FACE_UPPER_X0, FACE_LOWER_X0) * textureScale, Math.Min(FACE_UPPER_Y0, FACE_LOWER_Y0) * textureScale,
                    Math.Max(FACE_UPPER_X1, FACE_LOWER_X1) * textureScale, Math.Max(FACE_UPPER_Y1, FACE_LOWER_Y1) * textureScale });
            }
            else if (block == CharacterCompositeBlock.Robe)
            {
                // The torso and leg regions stack into one contiguous column of the composite (a robe's art flows across them on the stock
                // models too), so they pack as a single area; the upper arm region is separate
                int torsoX0, torsoY0, torsoX1, torsoY1, legsX0, legsY0, legsX1, legsY1, armX0, armY0, armX1, armY1;
                GetBlockRect(CharacterCompositeBlock.Torso, out torsoX0, out torsoY0, out torsoX1, out torsoY1);
                GetBlockRect(CharacterCompositeBlock.Legs, out legsX0, out legsY0, out legsX1, out legsY1);
                GetBlockRect(CharacterCompositeBlock.ArmUpper, out armX0, out armY0, out armX1, out armY1);
                int columnX0 = Math.Min(torsoX0, legsX0);
                int columnY0 = Math.Min(torsoY0, legsY0);
                int columnX1 = Math.Max(torsoX1, legsX1);
                int columnY1 = Math.Max(torsoY1, legsY1);
                areas.Add(new int[4] { columnX0 * textureScale, columnY0 * textureScale, columnX1 * textureScale, columnY1 * textureScale });
                if (HasHoodSlice() == true)
                {
                    // The hood's reserved corner is cut out of the upper arm region: the part beside it at full height, and the strip below it
                    areas.Add(new int[4] { armX0 * textureScale, armY0 * textureScale, HOOD_RESERVED_X0 * textureScale, armY1 * textureScale });
                    areas.Add(new int[4] { HOOD_RESERVED_X0 * textureScale, HOOD_RESERVED_Y1 * textureScale, armX1 * textureScale, armY1 * textureScale });
                }
                else
                    areas.Add(new int[4] { armX0 * textureScale, armY0 * textureScale, armX1 * textureScale, armY1 * textureScale });
                // The body-textured head pieces keep their rects (the robe components leave them clear, see the robe bake)
                foreach (int[] headPieceRect in GetBodyTexturedHeadPieceRects(GetEdgeMarginTexels(block)))
                    areas = SubtractRectFromAreas(areas, new int[4] { headPieceRect[0] * textureScale, headPieceRect[1] * textureScale, headPieceRect[2] * textureScale, headPieceRect[3] * textureScale });
            }
            else if (block == CharacterCompositeBlock.ArmUpper && HasHoodSlice() == true)
            {
                // The hood's reserved corner is cut out: the part beside it at full height, and the strip below it
                areas.Add(new int[4] { blockX0 * textureScale, blockY0 * textureScale, HOOD_RESERVED_X0 * textureScale, blockY1 * textureScale });
                areas.Add(new int[4] { HOOD_RESERVED_X0 * textureScale, HOOD_RESERVED_Y1 * textureScale, blockX1 * textureScale, blockY1 * textureScale });
            }
            else
                areas.Add(new int[4] { blockX0 * textureScale, blockY0 * textureScale, blockX1 * textureScale, blockY1 * textureScale });

            // Every area keeps the edge margin clear on all sides for the pieces' own bleed.  At the texture's outer border the client
            // samples with wrap (row 0 blends with row 255), and at block boundaries the client's own compositing puts another source
            // next to ours (a face texture, another item's component), so a piece touching the edge blends with foreign texels and its
            // bleed could not be laid down beyond it.  Region boundaries inside one area (TorsoUpper/TorsoLower, the face and robe
            // columns) stay continuous, our own art is on both sides
            int edgeMarginPixels = GetEdgeMarginTexels(block) * textureScale;

            // Slices sharing a texture and window are one piece (aliases get the placed slice's rect at the end)
            List<CharacterCompositeSlice> pieceSlices = new List<CharacterCompositeSlice>();
            Dictionary<string, CharacterCompositeSlice> placedSliceByPieceKey = new Dictionary<string, CharacterCompositeSlice>();
            Dictionary<CharacterCompositeSlice, CharacterCompositeSlice> placedSliceByAlias = new Dictionary<CharacterCompositeSlice, CharacterCompositeSlice>();
            List<CharacterCompositeSlice> vertexSortedSlices = new List<CharacterCompositeSlice>(blockSlices);
            vertexSortedSlices.Sort(CompareSlicesForPacking);
            foreach (CharacterCompositeSlice slice in vertexSortedSlices)
            {
                string pieceKey = string.Concat(slice.BakeSourceTextureName, "|", slice.UVMinU.ToString("0.####"), "|", slice.UVMinV.ToString("0.####"), "|",
                    slice.UVMaxU.ToString("0.####"), "|", slice.UVMaxV.ToString("0.####"), slice.IsBodyTexturedHeadPiece ? "|head" : "");
                if (placedSliceByPieceKey.ContainsKey(pieceKey) == true)
                {
                    placedSliceByAlias[slice] = placedSliceByPieceKey[pieceKey];
                    continue;
                }
                placedSliceByPieceKey.Add(pieceKey, slice);
                pieceSlices.Add(slice);
            }

            // Margins cost room: a 128 wide legs texture has none to spare in the 128 wide legs block, and three pieces stacked with two
            // 8 texel gutters between them lose 16 of the block's 128 rows.  So the block is packed several ways, in order of preference:
            // with the full margin on both axes; without it on one axis or both (a piece edge then sits on the block boundary, where the
            // bleed stops and the edge pull-in keeps the low mip samples inside the piece); then with half the margin, which still keeps
            // the mip 0 and mip 1 samples inside the piece's own art.  A later way is taken only when it serves the model clearly better
            // (see GetPlacedUtility)
            List<int[]> marginVariants = new List<int[]>(); // marginX, marginY, gutter (the margin two pieces keep between them)
            marginVariants.Add(new int[3] { edgeMarginPixels, edgeMarginPixels, edgeMarginPixels * 2 });
            marginVariants.Add(new int[3] { 0, edgeMarginPixels, edgeMarginPixels * 2 });
            marginVariants.Add(new int[3] { edgeMarginPixels, 0, edgeMarginPixels * 2 });
            marginVariants.Add(new int[3] { 0, 0, edgeMarginPixels * 2 });
            int halfMarginPixels = edgeMarginPixels / 2;
            if (halfMarginPixels >= textureScale)
            {
                marginVariants.Add(new int[3] { halfMarginPixels, halfMarginPixels, halfMarginPixels * 2 });
                marginVariants.Add(new int[3] { 0, halfMarginPixels, halfMarginPixels * 2 });
                marginVariants.Add(new int[3] { halfMarginPixels, 0, halfMarginPixels * 2 });
                marginVariants.Add(new int[3] { 0, 0, halfMarginPixels * 2 });
            }
            Dictionary<CharacterCompositeSlice, int[]> placedRects = new Dictionary<CharacterCompositeSlice, int[]>();
            List<int[]> shrunkAreas = new List<int[]>();
            int marginX = edgeMarginPixels;
            int marginY = edgeMarginPixels;
            int gutter = edgeMarginPixels * 2;
            double bestUtility = -1;
            float bestSmallestScale = 0f;
            int bestPulledEdges = int.MaxValue;
            foreach (int[] marginVariant in marginVariants)
            {
                List<int[]> variantAreas;
                Dictionary<CharacterCompositeSlice, int[]> variantRects = PackBlockWithMargins(block, areas, pieceSlices, textureScale, marginVariant[0], marginVariant[1],
                    marginVariant[2], blockX0, blockY0, blockX1, blockY1, out variantAreas);
                double variantUtility = GetLayoutScore(variantRects, textureScale);
                float variantSmallestScale = GetSmallestPlacedScale(variantRects, textureScale);
                int variantPulledEdges = GetPulledEdgeCount(variantRects);
                if (bestUtility >= 0)
                {
                    // Clearly better served (a higher smallest-scale bucket, or the per-surface utility up by the margin), or the
                    // block's worst piece clearly better at no loss (the human robe's neck reaches texel-for-texel with the half margin
                    // for under 1% more in the block)
                    bool clearlyBetter = variantUtility > bestUtility + (MARGIN_VARIANT_MIN_GAIN_PERCENT / 100.0);
                    bool betterWorstPiece = variantSmallestScale >= bestSmallestScale + MARGIN_VARIANT_MIN_SMALLEST_SCALE_GAIN && variantUtility >= bestUtility;
                    // ... or every piece now texel-for-texel where one was not, at no loss elsewhere (the human female's torso: the
                    // half margin fits the chest with its margins and the 32x17 skin patch whole, the full margin cost the patch a row)
                    bool nowAllExact = variantSmallestScale >= 0.999f && bestSmallestScale < 0.999f && variantUtility >= bestUtility;
                    // ... or as much art with fewer pulled-in edges (see GetPulledEdgeCount): the human female's torso block at the half
                    // margin holds the chest with margins above and below its mirror line and every piece texel-for-texel
                    bool fewerPulledEdges = variantUtility >= bestUtility - 0.000001 && variantPulledEdges < bestPulledEdges;
                    if (clearlyBetter == false && betterWorstPiece == false && nowAllExact == false && fewerPulledEdges == false)
                        continue;
                }
                placedRects = variantRects;
                shrunkAreas = variantAreas;
                marginX = marginVariant[0];
                marginY = marginVariant[1];
                gutter = marginVariant[2];
                bestUtility = variantUtility;
                bestSmallestScale = variantSmallestScale;
                bestPulledEdges = variantPulledEdges;
            }
            if (marginX != edgeMarginPixels || marginY != edgeMarginPixels)
                Logger.WriteDebug(string.Concat("Character composite for '", SkeletonName, "' packs its ", block.ToString(), " block with edge margins ",
                    (marginX / textureScale).ToString(), "x", (marginY / textureScale).ToString(), " texels (gutter ", (gutter / textureScale).ToString(), ") to hold more of the art"));
            areas = shrunkAreas;

            foreach (var placedRect in placedRects)
            {
                placedRect.Key.RectX0 = Convert.ToSingle(placedRect.Value[0]) / textureScale;
                placedRect.Key.RectY0 = Convert.ToSingle(placedRect.Value[1]) / textureScale;
                placedRect.Key.RectX1 = Convert.ToSingle(placedRect.Value[2]) / textureScale;
                placedRect.Key.RectY1 = Convert.ToSingle(placedRect.Value[3]) / textureScale;
                placedRect.Key.IsRotated = placedRect.Value.Length > 4 && placedRect.Value[4] == 1;
                int[] clampArea = areas[0];
                foreach (int[] area in areas)
                    if (placedRect.Value[0] >= area[0] && placedRect.Value[1] >= area[1] && placedRect.Value[0] < area[2] && placedRect.Value[1] < area[3])
                        clampArea = area;
                // The bleed may use the margin kept clear around every area (that margin exists for it: the wrapped continuation of
                // the art has to sit there, or the client blends the piece edge with whatever lies beyond) and the gutters inside it
                placedRect.Key.ClampX0 = Convert.ToSingle(clampArea[0] - marginX) / textureScale;
                placedRect.Key.ClampY0 = Convert.ToSingle(clampArea[1] - marginY) / textureScale;
                placedRect.Key.ClampX1 = Convert.ToSingle(clampArea[2] + marginX) / textureScale;
                placedRect.Key.ClampY1 = Convert.ToSingle(clampArea[3] + marginY) / textureScale;
                placedRect.Key.EdgeMarginTexels = Math.Max(1, (gutter / 2) / textureScale);
                placedRect.Key.PullInX0 = (marginX == 0 && placedRect.Value[0] <= clampArea[0]);
                placedRect.Key.PullInX1 = (marginX == 0 && placedRect.Value[2] >= clampArea[2]);
                placedRect.Key.PullInY0 = (marginY == 0 && placedRect.Value[1] <= clampArea[1]);
                placedRect.Key.PullInY1 = (marginY == 0 && placedRect.Value[3] >= clampArea[3]);
            }
            foreach (var placedSliceForAlias in placedSliceByAlias)
            {
                placedSliceForAlias.Key.RectX0 = placedSliceForAlias.Value.RectX0;
                placedSliceForAlias.Key.RectY0 = placedSliceForAlias.Value.RectY0;
                placedSliceForAlias.Key.RectX1 = placedSliceForAlias.Value.RectX1;
                placedSliceForAlias.Key.RectY1 = placedSliceForAlias.Value.RectY1;
                placedSliceForAlias.Key.ClampX0 = placedSliceForAlias.Value.ClampX0;
                placedSliceForAlias.Key.ClampY0 = placedSliceForAlias.Value.ClampY0;
                placedSliceForAlias.Key.ClampX1 = placedSliceForAlias.Value.ClampX1;
                placedSliceForAlias.Key.ClampY1 = placedSliceForAlias.Value.ClampY1;
                placedSliceForAlias.Key.EdgeMarginTexels = placedSliceForAlias.Value.EdgeMarginTexels;
                placedSliceForAlias.Key.IsRotated = placedSliceForAlias.Value.IsRotated;
                placedSliceForAlias.Key.PullInX0 = placedSliceForAlias.Value.PullInX0;
                placedSliceForAlias.Key.PullInX1 = placedSliceForAlias.Value.PullInX1;
                placedSliceForAlias.Key.PullInY0 = placedSliceForAlias.Value.PullInY0;
                placedSliceForAlias.Key.PullInY1 = placedSliceForAlias.Value.PullInY1;
            }
        }

        private const int MARGIN_VARIANT_MIN_GAIN_PERCENT = 3; // A less preferred margin variant must pack this much more art to be taken (see PackBlock)
        private const double UTILITY_TEXEL_EXACT_BONUS = 0.15;  // See GetPlacedUtility
        private const double UTILITY_BELOW_HALF_PENALTY = 4.0;
        private const float LAYOUT_SIGNIFICANT_SURFACE_SHARE = 0.1f; // See GetLayoutScore

        // Layouts compare first by the smallest scale among the pieces that matter (a tenth of the block's surface or more), in eighths,
        // then by GetPlacedUtility per unit of surface.  A weighted sum alone traded the high elf's 35% piece down to 0.56 and the
        // barbarian's kilt to 0.59 to make the smaller pieces texel-exact; the human's 5% belt band, on the other hand, may give way
        // so its 80% piece lands exact
        private static double GetLayoutScore(Dictionary<CharacterCompositeSlice, int[]> placedRects, int textureScale)
        {
            double totalSurface = 0;
            foreach (var placedRect in placedRects)
                totalSurface += placedRect.Key.MeshArea;
            // The weaker axis counts (the barbarian's kilt at full width and 56% height drops every other row of its rivets, no better
            // than 56% both ways); a piece too small to count still must keep half its resolution (the iksar's tail)
            double smallestSignificantScale = 1.0;
            foreach (var placedRect in placedRects)
            {
                int naturalWidth, naturalHeight;
                GetPieceSize(placedRect.Key, textureScale, 1f, 1f, false, out naturalWidth, out naturalHeight);
                bool rotated = placedRect.Value.Length > 4 && placedRect.Value[4] == 1;
                int placedAlongU = rotated ? (placedRect.Value[3] - placedRect.Value[1]) : (placedRect.Value[2] - placedRect.Value[0]);
                int placedAlongV = rotated ? (placedRect.Value[2] - placedRect.Value[0]) : (placedRect.Value[3] - placedRect.Value[1]);
                double scaleU = Math.Min(1.0, Convert.ToDouble(placedAlongU) / Math.Max(1, naturalWidth) / placedRect.Key.MaxFitFactor);
                double scaleV = Math.Min(1.0, Convert.ToDouble(placedAlongV) / Math.Max(1, naturalHeight) / placedRect.Key.MaxFitFactor);
                double weakerScale = Math.Min(scaleU, scaleV);
                bool significant = totalSurface <= 0 || placedRect.Key.MeshArea >= totalSurface * LAYOUT_SIGNIFICANT_SURFACE_SHARE;
                if (significant == true || weakerScale < 0.5 - 0.001)
                    smallestSignificantScale = Math.Min(smallestSignificantScale, weakerScale);
            }
            double bucket = Math.Floor((smallestSignificantScale * 8.0) + 0.001) / 8.0;
            double utilityPerSurface = totalSurface > 0 ? GetPlacedUtility(placedRects, textureScale) / totalSurface : 0;
            return (bucket * 10.0) + utilityPerSurface;
        }
        private const float MARGIN_VARIANT_MIN_SMALLEST_SCALE_GAIN = 0.1f; // ... or raise the block's smallest piece scale by this much

        // The smallest scale (placed over natural size, per axis, against the piece's own cap) over every placed piece
        private static float GetSmallestPlacedScale(Dictionary<CharacterCompositeSlice, int[]> placedRects, int textureScale)
        {
            float smallest = 1f;
            foreach (var placedRect in placedRects)
            {
                int naturalWidth, naturalHeight;
                GetPieceSize(placedRect.Key, textureScale, placedRect.Key.MaxFitFactor, placedRect.Key.MaxFitFactor, false, out naturalWidth, out naturalHeight);
                bool rotated = placedRect.Value.Length > 4 && placedRect.Value[4] == 1;
                int placedAlongU = rotated ? (placedRect.Value[3] - placedRect.Value[1]) : (placedRect.Value[2] - placedRect.Value[0]);
                int placedAlongV = rotated ? (placedRect.Value[2] - placedRect.Value[0]) : (placedRect.Value[3] - placedRect.Value[1]);
                smallest = Math.Min(smallest, Math.Min(Convert.ToSingle(placedAlongU) / Math.Max(1, naturalWidth), Convert.ToSingle(placedAlongV) / Math.Max(1, naturalHeight)));
            }
            return smallest;
        }
        private const float WRAP_EDGE_TOLERANCE_TEXELS = 0.6f;    // A vertex this close to a tile edge (source texels) counts as on it, see HasWrapSeam
        private const float WRAP_SEAM_POSITION_TOLERANCE_SHARE = 0.02f; // ... and two edge vertices this share of the piece's extent apart meet
        private const float FULL_BLOCK_EDGE_PULL_IN_TEXELS = 2f; // Tile-edge vertices on a zero-margin area edge move this far inward

        // How well a layout serves the model: every piece's surface on the model times the resolution it landed at (the geometric mean
        // of its two axes' scales, at most texel-for-texel).  Placed pixels alone let a big texture crowd small ones out: the troll's
        // 128x128 legs texture took the whole block texel-for-texel and left the knee and boot textures four rows each, though they
        // cover as much of the model
        private static double GetPlacedUtility(Dictionary<CharacterCompositeSlice, int[]> placedRects, int textureScale)
        {
            double utility = 0;
            foreach (var placedRect in placedRects)
            {
                int naturalWidth, naturalHeight;
                GetPieceSize(placedRect.Key, textureScale, 1f, 1f, false, out naturalWidth, out naturalHeight);
                bool rotated = placedRect.Value.Length > 4 && placedRect.Value[4] == 1;
                int placedAlongU = rotated ? (placedRect.Value[3] - placedRect.Value[1]) : (placedRect.Value[2] - placedRect.Value[0]);
                int placedAlongV = rotated ? (placedRect.Value[2] - placedRect.Value[0]) : (placedRect.Value[3] - placedRect.Value[1]);
                // Scales against the piece's own cap (a seam piece at reduced resolution is whole at its cap)
                double scaleU = Math.Min(1.0, Convert.ToDouble(placedAlongU) / Math.Max(1, naturalWidth) / placedRect.Key.MaxFitFactor);
                double scaleV = Math.Min(1.0, Convert.ToDouble(placedAlongV) / Math.Max(1, naturalHeight) / placedRect.Key.MaxFitFactor);
                double weight = placedRect.Key.MeshArea > 0 ? placedRect.Key.MeshArea : 0.0001;
                // Weighted toward the weaker axis: full width at half height drops every other row, no better than most of a uniform half
                double scale = Math.Pow(Math.Min(scaleU, scaleV), 0.75) * Math.Pow(Math.Max(scaleU, scaleV), 0.25);
                // Texel-for-texel is worth more than its scale says: the pixel art survives intact, where at 94% every resampling
                // either drops a column (a step in the art) or blurs it.  Below half scale the art is gone, and the piece counts against
                // the layout (the troll's knee and boot textures at four rows each)
                double value = scale;
                if (scaleU >= 0.999 && scaleV >= 0.999)
                    value += UTILITY_TEXEL_EXACT_BONUS;
                if (scale < 0.5)
                    value -= (0.5 - scale) * UTILITY_BELOW_HALF_PENALTY;
                utility += weight * value;
            }
            return utility;
        }

        // How many piece edges a layout pulls in (see TryPlaceInShelfArea).  Pulled edges shift the art where two parts of the mesh meet
        // (the human female's back), so at equal utility the layout with fewer wins; they never buy a margin at the cost of resolution
        // (a penalty in the utility traded the barbarian's face from 128 to 104 texels wide for a one texel margin)
        private static int GetPulledEdgeCount(Dictionary<CharacterCompositeSlice, int[]> placedRects)
        {
            int pulled = 0;
            foreach (var placedRect in placedRects)
                if (placedRect.Value.Length > 5)
                    pulled += placedRect.Value[5];
            return pulled;
        }

        // Shrinks copies of the areas by the margins and packs the pieces into them
        private Dictionary<CharacterCompositeSlice, int[]> PackBlockWithMargins(CharacterCompositeBlock block, List<int[]> unshrunkAreas, List<CharacterCompositeSlice> pieceSlices,
            int textureScale, int marginX, int marginY, int gutter, int blockX0, int blockY0, int blockX1, int blockY1, out List<int[]> shrunkAreas)
        {
            shrunkAreas = new List<int[]>();
            foreach (int[] area in unshrunkAreas)
                shrunkAreas.Add(new int[4] { area[0] + marginX, area[1] + marginY, area[2] - marginX, area[3] - marginY });
            Dictionary<CharacterCompositeSlice, int[]> placedRects = new Dictionary<CharacterCompositeSlice, int[]>();
            List<List<int[]>> shelvesByArea = new List<List<int[]>>();
            foreach (int[] area in shrunkAreas)
                shelvesByArea.Add(new List<int[]>());
            // On an axis packed without the edge margin, a piece whose mesh wraps across its edges there still keeps one (see GetPieceSize)
            int reserveX = (marginX == 0) ? gutter / 2 : 0;
            int reserveY = (marginY == 0) ? gutter / 2 : 0;
            PackPieces(block, pieceSlices, shrunkAreas, shelvesByArea, textureScale, gutter, reserveX, reserveY, placedRects, blockX0, blockY0, blockX1, blockY1);
            return placedRects;
        }

        // Packs one group of pieces (largest first) into the areas' shelves.  Candidate layouts: for each cap from texel-for-texel down,
        // every piece at the largest factor up to the cap that fits, grown afterwards along one axis where there is room (a 64x64 piece
        // over a 64x48 hole keeps its full width and loses only rows, rather than shrinking both ways to 48x48); and the same ungrown.
        // The caps stop at the first where every piece lands ungrown at the cap itself, since nothing smaller can hold more.  The layout
        // serving the model best (see GetPlacedUtility) wins, the earlier one on a tie.  A piece nothing fits takes the block corner
        private void PackPieces(CharacterCompositeBlock block, List<CharacterCompositeSlice> pieces, List<int[]> areas, List<List<int[]>> shelvesByArea, int textureScale,
            int gutter, int reserveX, int reserveY, Dictionary<CharacterCompositeSlice, int[]> placedRects, int blockX0, int blockY0, int blockX1, int blockY1)
        {
            if (pieces.Count == 0)
                return;
            // Fit factors from texel-for-texel down to an eighth in 1/64 steps, so a piece loses no more than it must to fit
            List<float> fitFactorList = new List<float>();
            for (int step = 64; step >= 8; step--)
                fitFactorList.Add(step / 64f);
            float[] fitFactors = fitFactorList.ToArray();
            List<CharacterCompositeSlice> sortedPieces = new List<CharacterCompositeSlice>(pieces);
            sortedPieces.Sort(CompareSlicesForPacking);

            // Pieces too large for an empty area even at the smallest factor can never be placed: they take the block corner
            List<CharacterCompositeSlice> unplaceablePieces = new List<CharacterCompositeSlice>();
            foreach (CharacterCompositeSlice slice in sortedPieces)
            {
                float smallestFactor = fitFactors[fitFactors.Length - 1];
                int pieceWidth, pieceHeight;
                GetPieceSize(slice, textureScale, smallestFactor, smallestFactor, false, out pieceWidth, out pieceHeight);
                bool fitsSomewhere = false;
                foreach (int[] area in areas)
                    if ((pieceWidth <= area[2] - area[0] && pieceHeight <= area[3] - area[1]) || (pieceHeight <= area[2] - area[0] && pieceWidth <= area[3] - area[1]))
                        fitsSomewhere = true;
                if (fitsSomewhere == false)
                    unplaceablePieces.Add(slice);
            }
            foreach (CharacterCompositeSlice slice in unplaceablePieces)
            {
                Logger.WriteError(string.Concat("Character composite for '", SkeletonName, "' piece '", slice.MaterialUniqueName, "' (", slice.BakeSourceTextureName, " ", slice.SourceWidth.ToString(), "x", slice.SourceHeight.ToString(),
                    ", window ", (slice.UVMaxU - slice.UVMinU).ToString("0.##"), " x ", (slice.UVMaxV - slice.UVMinV).ToString("0.##"), " tiles) cannot fit the ", block.ToString(), " block at any size, so it overlaps a corner"));
                placedRects[slice] = new int[4] { blockX0 * textureScale, blockY0 * textureScale, Math.Min(blockX1, blockX0 + 8) * textureScale, Math.Min(blockY1, blockY0 + 8) * textureScale };
                sortedPieces.Remove(slice);
            }

            // Two placement orders: by texels (big rects first pack tightest) and by the surface covered on the model (the human's
            // 64x64 leg texture covers 80% of the legs, its 128x64 one 5%: placed second, the 64x64 only ever got the leftover rows)
            List<List<CharacterCompositeSlice>> orderings = new List<List<CharacterCompositeSlice>>();
            orderings.Add(sortedPieces);
            List<CharacterCompositeSlice> surfaceSortedPieces = new List<CharacterCompositeSlice>(sortedPieces);
            surfaceSortedPieces.Sort(CompareSlicesForSurface);
            for (int i = 0; i < sortedPieces.Count; i++)
            {
                if (surfaceSortedPieces[i] != sortedPieces[i])
                {
                    orderings.Add(surfaceSortedPieces);
                    break;
                }
            }
            Dictionary<CharacterCompositeSlice, int[]>? bestRects = null;
            List<List<int[]>>? bestShelvesByArea = null;
            double bestUtility = -1;
            int bestPulledEdges = int.MaxValue;
            // With the pieces' own margins on the zero-margin axes (see GetPieceSize) and without them: the first piece placed keeps its
            // margin whenever the empty area has room, which can leave the rest short (the dark elf's 120x64 legs texture took the
            // rows its neighbours needed), so both ways compete on the layout score, fewer pulled-in edges winning a tie
            for (int insetPass = 0; insetPass < 2; insetPass++)
            {
                bool keepMargins = (insetPass == 0);
                if (insetPass == 1 && reserveX == 0 && reserveY == 0)
                    break;
                foreach (List<CharacterCompositeSlice> ordering in orderings)
                {
                    foreach (float capFactor in fitFactors)
                    {
                        bool allAtCapUngrown = false;
                        for (int growPass = 1; growPass >= 0; growPass--)
                        {
                            List<List<int[]>> trialShelvesByArea = CopyShelves(shelvesByArea);
                            Dictionary<CharacterCompositeSlice, int[]> trialRects = new Dictionary<CharacterCompositeSlice, int[]>();
                            bool allAtCap;
                            if (TryPackPerPiece(ordering, areas, trialShelvesByArea, textureScale, gutter, reserveX, reserveY, keepMargins, fitFactors, capFactor, growPass == 1, trialRects, out allAtCap) == false)
                                continue;
                            if (growPass == 0)
                                allAtCapUngrown = allAtCap;
                            double utility = GetLayoutScore(trialRects, textureScale);
                            int pulledEdges = GetPulledEdgeCount(trialRects);
                            if (utility > bestUtility + 0.000001 || (Math.Abs(utility - bestUtility) <= 0.000001 && pulledEdges < bestPulledEdges))
                            {
                                bestUtility = utility;
                                bestPulledEdges = pulledEdges;
                                bestRects = trialRects;
                                bestShelvesByArea = trialShelvesByArea;
                            }
                        }
                        if (allAtCapUngrown == true)
                            break;
                    }
                }
            }
            if (bestRects != null && bestShelvesByArea != null)
            {
                for (int areaIndex = 0; areaIndex < areas.Count; areaIndex++)
                    shelvesByArea[areaIndex] = bestShelvesByArea[areaIndex];
                foreach (var bestRect in bestRects)
                    placedRects[bestRect.Key] = bestRect.Value;
                return;
            }

            // Last resort: per piece, whatever lands
            foreach (CharacterCompositeSlice slice in sortedPieces)
            {
                bool placed = false;
                foreach (float fitFactor in fitFactors)
                {
                    placed = TryPlaceEitherWay(shelvesByArea, areas, gutter, slice, textureScale, fitFactor, fitFactor, reserveX, reserveY, true, placedRects);
                    if (placed == true)
                        break;
                }
                if (placed == false)
                {
                    Logger.WriteError(string.Concat("Character composite for '", SkeletonName, "' could not pack '", slice.MaterialUniqueName, "' (", slice.BakeSourceTextureName, " ", slice.SourceWidth.ToString(), "x", slice.SourceHeight.ToString(),
                        ", window ", (slice.UVMaxU - slice.UVMinU).ToString("0.##"), " x ", (slice.UVMaxV - slice.UVMinV).ToString("0.##"), " tiles), so it overlaps a corner"));
                    placedRects[slice] = new int[4] { areas[0][0], areas[0][1], Math.Min(areas[0][2], areas[0][0] + 16), Math.Min(areas[0][3], areas[0][1] + 16) };
                }
            }
        }

        // Places each piece (largest first) at the largest uniform factor up to the cap that fits, then, when allowed, grown along the
        // one axis (the larger gain, U on a tie) that still fits.  Returns false as soon as a piece fits nowhere; allAtCap says whether
        // every piece landed at the cap itself before any growth
        private bool TryPackPerPiece(List<CharacterCompositeSlice> sortedPieces, List<int[]> areas, List<List<int[]>> shelvesByArea, int textureScale, int gutter, int reserveX,
            int reserveY, bool keepMargins, float[] fitFactors, float capFactor, bool growAxis, Dictionary<CharacterCompositeSlice, int[]> placedRects, out bool allAtCap)
        {
            allAtCap = true;
            foreach (CharacterCompositeSlice slice in sortedPieces)
            {
                float pieceCap = Math.Min(capFactor, slice.MaxFitFactor);
                float baseFactor = 0f;
                foreach (float fitFactor in fitFactors)
                {
                    if (fitFactor > pieceCap + 0.0001f)
                        continue;
                    if (CanPlace(shelvesByArea, areas, gutter, slice, textureScale, fitFactor, fitFactor, reserveX, reserveY, keepMargins) == true)
                    {
                        baseFactor = fitFactor;
                        break;
                    }
                }
                if (baseFactor <= 0f)
                    return false;
                if (baseFactor < pieceCap - 0.0001f)
                    allAtCap = false;
                float factorU = baseFactor;
                float factorV = baseFactor;
                if (growAxis == true && baseFactor < slice.MaxFitFactor)
                {
                    float grownU = baseFactor;
                    float grownV = baseFactor;
                    foreach (float fitFactor in fitFactors)
                    {
                        if (fitFactor > slice.MaxFitFactor + 0.0001f)
                            continue;
                        if (fitFactor <= baseFactor + 0.0001f)
                            break;
                        if (CanPlace(shelvesByArea, areas, gutter, slice, textureScale, fitFactor, baseFactor, reserveX, reserveY, keepMargins) == true)
                        {
                            grownU = fitFactor;
                            break;
                        }
                    }
                    foreach (float fitFactor in fitFactors)
                    {
                        if (fitFactor > slice.MaxFitFactor + 0.0001f)
                            continue;
                        if (fitFactor <= baseFactor + 0.0001f)
                            break;
                        if (CanPlace(shelvesByArea, areas, gutter, slice, textureScale, baseFactor, fitFactor, reserveX, reserveY, keepMargins) == true)
                        {
                            grownV = fitFactor;
                            break;
                        }
                    }
                    if (grownU >= grownV)
                        factorU = grownU;
                    else
                        factorV = grownV;
                }
                if (TryPlaceEitherWay(shelvesByArea, areas, gutter, slice, textureScale, factorU, factorV, reserveX, reserveY, keepMargins, placedRects) == false)
                    return false;
            }
            return true;
        }

        // Whether the piece would land at these factors, without changing the shelves
        private static bool CanPlace(List<List<int[]>> shelvesByArea, List<int[]> areas, int gutter, CharacterCompositeSlice slice, int textureScale, float factorU, float factorV,
            int reserveX, int reserveY, bool keepMargins)
        {
            List<List<int[]>> trialShelvesByArea = CopyShelves(shelvesByArea);
            Dictionary<CharacterCompositeSlice, int[]> trialRects = new Dictionary<CharacterCompositeSlice, int[]>();
            return TryPlaceEitherWay(trialShelvesByArea, areas, gutter, slice, textureScale, factorU, factorV, reserveX, reserveY, keepMargins, trialRects);
        }

        private static List<List<int[]>> CopyShelves(List<List<int[]>> shelvesByArea)
        {
            List<List<int[]>> copy = new List<List<int[]>>();
            foreach (List<int[]> shelves in shelvesByArea)
            {
                List<int[]> shelvesCopy = new List<int[]>();
                foreach (int[] shelf in shelves)
                    shelvesCopy.Add(new int[3] { shelf[0], shelf[1], shelf[2] });
                copy.Add(shelvesCopy);
            }
            return copy;
        }

        // Most surface on the model first (see PackPieces)
        private static int CompareSlicesForSurface(CharacterCompositeSlice slice1, CharacterCompositeSlice slice2)
        {
            if (Math.Abs(slice1.MeshArea - slice2.MeshArea) > 0.000001f)
                return slice2.MeshArea.CompareTo(slice1.MeshArea);
            return CompareSlicesForPacking(slice1, slice2);
        }

        private static int CompareSlicesForPacking(CharacterCompositeSlice slice1, CharacterCompositeSlice slice2)
        {
            // Largest pieces (in source texels) first, so the big pieces take the open space at full size and only the small ones ever
            // have to shrink into what is left
            float area1 = (slice1.UVMaxU - slice1.UVMinU) * slice1.SourceWidth * (slice1.UVMaxV - slice1.UVMinV) * slice1.SourceHeight * slice1.MaxFitFactor * slice1.MaxFitFactor;
            float area2 = (slice2.UVMaxU - slice2.UVMinU) * slice2.SourceWidth * (slice2.UVMaxV - slice2.UVMinV) * slice2.SourceHeight * slice2.MaxFitFactor * slice2.MaxFitFactor;
            if (Math.Abs(area1 - area2) > 0.5f)
                return area2.CompareTo(area1);
            if (slice1.VertexCount != slice2.VertexCount)
                return slice2.VertexCount.CompareTo(slice1.VertexCount);
            return string.CompareOrdinal(slice1.MaterialUniqueName, slice2.MaterialUniqueName);
        }

        // Places a piece into a shelf-packed area if it fits (with the gutter kept between pieces and inside the area edges), recording
        // its rect in generated pixels.  Shelves are int arrays of startY, height, usedWidth
        // Piece size in generated pixels at a fit factor, turned a quarter turn when rotated.  Most EQ chest textures are 64 wide by 128
        // tall while the torso block is 128 wide by 96 tall: upright they pack at 69% (the sternum groove then smears into the seam
        // texels, showing as a line down the chest), turned they pack at 94%
        private static void GetPieceSize(CharacterCompositeSlice slice, int textureScale, float fitFactorU, float fitFactorV, bool rotated, out int pieceWidth, out int pieceHeight)
        {
            int insetX, insetY;
            GetPieceSize(slice, textureScale, fitFactorU, fitFactorV, rotated, 0, 0, out pieceWidth, out pieceHeight, out insetX, out insetY);
        }

        // With the room the piece takes in the shelves: on a composite axis packed without the edge margin (reserveX/reserveY, see
        // PackBlockWithMargins) the piece is placed with that margin of its own on both sides (insetX/insetY, generated pixels), so the
        // art beside its edges (the wrapped continuation, or the edge itself at a mirror line) lies there and the edge vertices need
        // no pull-in.  The pull-in (see ApplyUVRemap) moves the edge vertices two texels along the art, and where two parts of the
        // mesh meet on that edge they no longer match: the human female's back is a mirror line at u = 0, and with the chest's 64
        // texels around the torso filling the torso block's height, no margin above, the corset's laces stepped at the centre line
        private static void GetPieceSize(CharacterCompositeSlice slice, int textureScale, float fitFactorU, float fitFactorV, bool rotated, int reserveX, int reserveY,
            out int pieceWidth, out int pieceHeight, out int insetX, out int insetY)
        {
            int alongU = Math.Max(1, Convert.ToInt32(Math.Ceiling((slice.UVMaxU - slice.UVMinU) * slice.SourceWidth * textureScale * fitFactorU - 0.001f)));
            int alongV = Math.Max(1, Convert.ToInt32(Math.Ceiling((slice.UVMaxV - slice.UVMinV) * slice.SourceHeight * textureScale * fitFactorV - 0.001f)));
            insetX = reserveX;
            insetY = reserveY;
            pieceWidth = (rotated ? alongV : alongU) + (insetX * 2);
            pieceHeight = (rotated ? alongU : alongV) + (insetY * 2);
        }

        // Tries the piece upright, then turned, at the given fit factors (per source axis): first keeping the margin its edges need on a
        // zero-margin axis (see GetPieceSize), then, when that room is not there at this size, without it (the edge pull-in then
        // applies, as a piece filling its block's width texel-for-texel loses less that way than by shrinking: the faces)
        private static bool TryPlaceEitherWay(List<List<int[]>> shelvesByArea, List<int[]> areas, int gutter, CharacterCompositeSlice slice, int textureScale, float fitFactorU,
            float fitFactorV, int reserveX, int reserveY, bool keepMargins, Dictionary<CharacterCompositeSlice, int[]> placedRects)
        {
            for (int pass = (keepMargins ? 0 : 1); pass < 2; pass++)
            {
                int passReserveX = (pass == 0) ? reserveX : 0;
                int passReserveY = (pass == 0) ? reserveY : 0;
                bool anyInset = false;
                for (int rotation = 0; rotation < 2; rotation++)
                {
                    bool rotated = (rotation == 1);
                    int pieceWidth, pieceHeight, insetX, insetY;
                    GetPieceSize(slice, textureScale, fitFactorU, fitFactorV, rotated, passReserveX, passReserveY, out pieceWidth, out pieceHeight, out insetX, out insetY);
                    if (insetX > 0 || insetY > 0)
                        anyInset = true;
                    for (int areaIndex = 0; areaIndex < areas.Count; areaIndex++)
                        if (TryPlaceInShelfArea(shelvesByArea[areaIndex], areas[areaIndex], gutter, pieceWidth, pieceHeight, insetX, insetY, reserveX, reserveY, rotated, slice, placedRects) == true)
                            return true;
                }
                // Nothing was reserved, so the second pass would only repeat the first
                if (anyInset == false)
                    break;
            }
            return false;
        }

        private static bool TryPlaceInShelfArea(List<int[]> shelves, int[] area, int gutter, int pieceWidth, int pieceHeight, int insetX, int insetY, int reserveX, int reserveY,
            bool rotated, CharacterCompositeSlice slice, Dictionary<CharacterCompositeSlice, int[]> placedRects)
        {
            // pieceWidth and pieceHeight include the margins the piece keeps (insetX/insetY on each side, see GetPieceSize): the shelves
            // take the whole, the recorded rect is the piece inside it, and the bake's bleed fills the rest.  The rect's sixth value
            // counts the piece edges that will be pulled in (on the area's boundary, on an axis without the margin and without an
            // inset), which the layout score counts against it (see GetPlacedUtility)
            // Gutters (two edge margins, in generated pixels) sit between pieces only; pieces may touch the area edges (the area was shrunk
            // by one margin, and the bake bleed is clamped to that).  A shelf stores startY, its piece height plus one gutter, and the used
            // width including one gutter after the last piece
            int areaWidth = area[2] - area[0];
            int areaHeight = area[3] - area[1];
            if (pieceWidth > areaWidth || pieceHeight > areaHeight)
                return false;

            // Fit onto an existing shelf if the piece is no taller than it and there is width left
            foreach (int[] shelf in shelves)
            {
                if (pieceHeight <= shelf[1] - gutter && shelf[2] + pieceWidth <= areaWidth)
                {
                    int x0 = area[0] + shelf[2];
                    int y0 = area[1] + shelf[0];
                    placedRects[slice] = new int[6] { x0 + insetX, y0 + insetY, x0 + pieceWidth - insetX, y0 + pieceHeight - insetY, rotated ? 1 : 0,
                        CountPulledEdges(area, x0, y0, x0 + pieceWidth, y0 + pieceHeight, insetX, insetY, reserveX, reserveY) };
                    shelf[2] += pieceWidth + gutter;
                    return true;
                }
            }

            // Otherwise open a new shelf below the last one
            int nextShelfY = 0;
            foreach (int[] shelf in shelves)
                nextShelfY = Math.Max(nextShelfY, shelf[0] + shelf[1]);
            if (nextShelfY + pieceHeight > areaHeight)
                return false;
            shelves.Add(new int[3] { nextShelfY, pieceHeight + gutter, pieceWidth + gutter });
            int newX0 = area[0];
            int newY0 = area[1] + nextShelfY;
            placedRects[slice] = new int[6] { newX0 + insetX, newY0 + insetY, newX0 + pieceWidth - insetX, newY0 + pieceHeight - insetY, rotated ? 1 : 0,
                CountPulledEdges(area, newX0, newY0, newX0 + pieceWidth, newY0 + pieceHeight, insetX, insetY, reserveX, reserveY) };
            return true;
        }

        // The edges of a placed piece (outer rect, insets included) that sit on the area boundary without a margin: see TryPlaceInShelfArea
        private static int CountPulledEdges(int[] area, int x0, int y0, int x1, int y1, int insetX, int insetY, int reserveX, int reserveY)
        {
            int pulled = 0;
            if (reserveX > 0 && insetX == 0)
            {
                if (x0 <= area[0]) pulled++;
                if (x1 >= area[2]) pulled++;
            }
            if (reserveY > 0 && insetY == 0)
            {
                if (y0 <= area[1]) pulled++;
                if (y1 >= area[3]) pulled++;
            }
            return pulled;
        }

        // The model's UVs as the mesh loader folded them (ObjectModelEQData.FoldMeshUVsPerTriangle: every triangle shifted by whole tiles
        // onto its texture's window tile, seam vertices duplicated).  Nothing is re-folded here: an earlier per-island union-minimizing
        // shift undid the loader's choice of seam (the iksar legs came back at 1.44 tiles for a seam at texel 29)
        private static Dictionary<int, float[]> CalculateTileFoldedUVs(List<int[]> triangles, Dictionary<int, float[]> rawUVsByVertexIndex)
        {
            Dictionary<int, float[]> foldedUVsByVertexIndex = new Dictionary<int, float[]>();
            foreach (int[] triangle in triangles)
                foreach (int vertexIndex in triangle)
                    if (foldedUVsByVertexIndex.ContainsKey(vertexIndex) == false)
                        foldedUVsByVertexIndex.Add(vertexIndex, new float[2] { rawUVsByVertexIndex[vertexIndex][0], rawUVsByVertexIndex[vertexIndex][1] });
            return foldedUVsByVertexIndex;
        }

        // Reads a PNG's dimensions from its IHDR chunk (big endian width and height right after the 16 byte header lead-in)
        private static bool TryReadPNGDimensions(string filePath, out int width, out int height)
        {
            width = 0;
            height = 0;
            try
            {
                if (File.Exists(filePath) == false)
                    return false;
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] headerBytes = new byte[24];
                    if (stream.Read(headerBytes, 0, 24) < 24)
                        return false;
                    width = (headerBytes[16] << 24) | (headerBytes[17] << 16) | (headerBytes[18] << 8) | headerBytes[19];
                    height = (headerBytes[20] << 24) | (headerBytes[21] << 16) | (headerBytes[22] << 8) | headerBytes[23];
                    return width > 0 && height > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // A bake target: an ARGB pixel buffer covering a composite-space rect at the generated texture scale, with a coverage mask
        private class BakeCanvas
        {
            public int OriginX; // Composite (256) space
            public int OriginY;
            public int Width;   // Pixels
            public int Height;
            public int Scale;
            public int[] Pixels;
            public bool[] Covered;

            public BakeCanvas(int originX, int originY, int widthInCompositeUnits, int heightInCompositeUnits, int scale)
            {
                OriginX = originX;
                OriginY = originY;
                Scale = scale;
                Width = widthInCompositeUnits * scale;
                Height = heightInCompositeUnits * scale;
                Pixels = new int[Width * Height];
                Covered = new bool[Width * Height];
            }

            public void Fill(int colorArgb)
            {
                for (int i = 0; i < Pixels.Length; i++)
                    Pixels[i] = colorArgb;
            }

            // Writes the canvas out as a PNG; uncovered pixels are transparent when transparentWhereUncovered is set
            // Extends every piece's edge colours into the uncovered canvas (nearest covered neighbour, pass by pass, until nothing is
            // left unfilled).  The client mipmaps and bilinear-filters the composite, and a small piece next to a flat fill colour
            // (the gnome's 33x16 neck strip) blends with that fill at any distance, showing as a band of the fill colour on the model.
            // Leaves the coverage flags alone, so region cuts keep their transparency
            // markCovered: the padded pixels count as covered afterwards (an item component then ships them opaque, like the stock
            // components that fill their whole region; the client otherwise shows the base skin between the pieces and its mipmaps
            // blend that into every piece edge).  The except rect (composite units) is never padded: the erudite hood slot in a
            // non-robe chest component stays clear so the base skin's default hood shows through
            public void PadUncoveredFromNeighbors(bool markCovered = false, int exceptX0 = 0, int exceptY0 = 0, int exceptX1 = 0, int exceptY1 = 0)
            {
                List<int[]> exceptRects = new List<int[]>();
                if (exceptX1 > exceptX0 && exceptY1 > exceptY0)
                    exceptRects.Add(new int[4] { exceptX0, exceptY0, exceptX1, exceptY1 });
                PadUncoveredFromNeighbors(markCovered, exceptRects);
            }

            // exceptRects are composite units, never padded (they stay uncovered)
            public void PadUncoveredFromNeighbors(bool markCovered, List<int[]> exceptRects)
            {
                bool[] covered = markCovered ? Covered : (bool[])Covered.Clone();
                bool[] except = new bool[Width * Height];
                bool hasExcept = false;
                foreach (int[] rect in exceptRects)
                {
                    int px0 = Math.Max(0, (rect[0] - OriginX) * Scale);
                    int py0 = Math.Max(0, (rect[1] - OriginY) * Scale);
                    int px1 = Math.Min(Width, (rect[2] - OriginX) * Scale);
                    int py1 = Math.Min(Height, (rect[3] - OriginY) * Scale);
                    for (int y = py0; y < py1; y++)
                        for (int x = px0; x < px1; x++)
                        {
                            except[(y * Width) + x] = true;
                            hasExcept = true;
                        }
                }
                for (int pass = 0; pass < Math.Max(Width, Height); pass++)
                {
                    List<int> fillIndexes = new List<int>();
                    List<int> fillColors = new List<int>();
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            int index = (y * Width) + x;
                            if (covered[index] == true)
                                continue;
                            if (hasExcept == true && except[index] == true)
                                continue;
                            int neighbourIndex = -1;
                            if (x > 0 && covered[index - 1] == true)
                                neighbourIndex = index - 1;
                            else if (x < Width - 1 && covered[index + 1] == true)
                                neighbourIndex = index + 1;
                            else if (y > 0 && covered[index - Width] == true)
                                neighbourIndex = index - Width;
                            else if (y < Height - 1 && covered[index + Width] == true)
                                neighbourIndex = index + Width;
                            if (neighbourIndex < 0)
                                continue;
                            fillIndexes.Add(index);
                            fillColors.Add(Pixels[neighbourIndex]);
                        }
                    }
                    if (fillIndexes.Count == 0)
                        break;
                    for (int i = 0; i < fillIndexes.Count; i++)
                    {
                        Pixels[fillIndexes[i]] = fillColors[i];
                        covered[fillIndexes[i]] = true;
                    }
                }
            }

            // keepPixelAlpha: covered pixels keep the alpha the bake gave them (masked helm art) instead of being written opaque
            public void SavePNG(string filePath, bool transparentWhereUncovered, bool keepPixelAlpha = false)
            {
                if (transparentWhereUncovered == false)
                    PadUncoveredFromNeighbors();
                using (Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb))
                {
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            int index = (y * Width) + x;
                            if (transparentWhereUncovered == true && Covered[index] == false)
                                bitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                            else if (keepPixelAlpha == true)
                                bitmap.SetPixel(x, y, Color.FromArgb(Pixels[index]));
                            else
                                bitmap.SetPixel(x, y, Color.FromArgb(unchecked((int)0xFF000000) | (Pixels[index] & 0x00FFFFFF)));
                        }
                    }
                    bitmap.Save(filePath, ImageFormat.Png);
                }
            }

            // Cuts a composite-space rect out into its own PNG (used for the worn component textures)
            public void SaveRegionPNG(string filePath, int regionX0, int regionY0, int regionX1, int regionY1, out bool anyCovered)
            {
                anyCovered = false;
                int startX = (regionX0 - OriginX) * Scale;
                int startY = (regionY0 - OriginY) * Scale;
                int regionWidth = (regionX1 - regionX0) * Scale;
                int regionHeight = (regionY1 - regionY0) * Scale;
                using (Bitmap bitmap = new Bitmap(regionWidth, regionHeight, PixelFormat.Format32bppArgb))
                {
                    for (int y = 0; y < regionHeight; y++)
                    {
                        for (int x = 0; x < regionWidth; x++)
                        {
                            int sourceX = startX + x;
                            int sourceY = startY + y;
                            if (sourceX < 0 || sourceY < 0 || sourceX >= Width || sourceY >= Height || Covered[(sourceY * Width) + sourceX] == false)
                                bitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                            else
                            {
                                bitmap.SetPixel(x, y, Color.FromArgb(unchecked((int)0xFF000000) | (Pixels[(sourceY * Width) + sourceX] & 0x00FFFFFF)));
                                anyCovered = true;
                            }
                        }
                    }
                    if (anyCovered == true)
                        bitmap.Save(filePath, ImageFormat.Png);
                }
            }
        }

        // Bakes the base skin (the naked texture set at the generated scale) and the per-face lower/upper face textures, sampling the
        // source EQ piece textures through the same windows and rects the mesh UVs were laid out with.  Also generates the shared
        // blank transparent texture on the first call
        public void GenerateBakedTextures(string workingRootFolder, string outputFolder, string blankTextureOutputFolder)
        {
            lock (CharacterTextureBakeLock)
            {
                string sourceTexturesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "characters", "Textures");
                string workingFolder = Path.Combine(workingRootFolder, SkeletonName.ToUpper());
                if (Directory.Exists(workingFolder) == false)
                    Directory.CreateDirectory(workingFolder);
                if (Directory.Exists(outputFolder) == false)
                    Directory.CreateDirectory(outputFolder);
                Dictionary<string, int[]> sourcePixelsByTextureName = new Dictionary<string, int[]>();
                Dictionary<string, int> sourceWidthByTextureName = new Dictionary<string, int>();
                Dictionary<string, int> sourceHeightByTextureName = new Dictionary<string, int>();
                List<string> pngFilePathsToConvert = new List<string>();
                int textureScale = GetTextureScale();

                // Body: neutral fill from the average of the largest body piece so unbaked areas blend in, then every body slice
                BakeCanvas bodyCanvas = new BakeCanvas(0, 0, BODY_COMPOSITE_WIDTH, BODY_COMPOSITE_HEIGHT, textureScale);
                bodyCanvas.Fill(CalculateFillColor(false, unchecked((int)0xFF7A5A44), sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName));
                foreach (CharacterCompositeSlice slice in Slices)
                {
                    if (slice.IsLaidOut == false || slice.IsFaceRegionSlice == true || slice.IsRobeLayer == true || slice.Block == CharacterCompositeBlock.Helm || slice.BakeSourceTextureName.Length == 0)
                        continue;
                    BakeSliceIntoCanvas(bodyCanvas, slice, slice.BakeSourceTextureName, sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName);
                }
                string bodyPngPath = Path.Combine(workingFolder, GetBodyTextureName() + ".png");
                bodyCanvas.SavePNG(bodyPngPath, false);
                pngFilePathsToConvert.Add(bodyPngPath);

                // Face lower and upper textures per face index, which the client blits into the composite's face regions from the
                // CharSections Face rows.  Face-independent content bakes identically into every face's textures.  The fill uses the
                // head's own skin tone so any sliver that shows through blends with the face
                int faceFillColorArgb = CalculateFillColor(true, unchecked((int)0xFF7A5A44), sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName);
                foreach (int faceIndex in ValidFaceIndexes)
                {
                    BakeFaceRegionTexture(faceIndex, FACE_LOWER_X0, FACE_LOWER_Y0, FACE_LOWER_X1, FACE_LOWER_Y1, GetFaceLowerTextureName(faceIndex),
                        faceFillColorArgb, workingFolder, sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName, pngFilePathsToConvert);
                    BakeFaceRegionTexture(faceIndex, FACE_UPPER_X0, FACE_UPPER_Y0, FACE_UPPER_X1, FACE_UPPER_Y1, GetFaceUpperTextureName(faceIndex),
                        faceFillColorArgb, workingFolder, sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName, pngFilePathsToConvert);
                }

                // Helm textures: one per helmed head (hair style 1-3), untinted (hair color 0) plus each palette tint (hair colors 1..).
                // The client applies the hair style's hair texture to the head's geoset, which is where these pieces' UVs point
                foreach (int helmIndex in HelmVariantIndexes)
                {
                    for (int tintIndex = 0; tintIndex <= HelmTintPalette.Count; tintIndex++)
                    {
                        ColorRGBA? tintColor = (tintIndex == 0) ? null : (ColorRGBA?)HelmTintPalette[tintIndex - 1];
                        BakeCanvas helmCanvas = new BakeCanvas(0, 0, HELM_TEXTURE_SIZE, HELM_TEXTURE_SIZE, textureScale);
                        helmCanvas.Fill(faceFillColorArgb);
                        foreach (CharacterCompositeSlice slice in Slices)
                        {
                            if (slice.IsLaidOut == false || slice.Block != CharacterCompositeBlock.Helm || slice.HelmIndex != helmIndex || slice.BakeSourceTextureName.Length == 0)
                                continue;
                            BakeSliceIntoCanvas(helmCanvas, slice, GetHelmSetTextureName(slice.BakeSourceTextureName, helmIndex, sourceTexturesFolder), sourceTexturesFolder,
                                sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName, slice.IsHelmArt ? tintColor : null);
                        }
                        string helmPngPath = Path.Combine(workingFolder, GetHelmTextureName(helmIndex, tintIndex) + ".png");
                        helmCanvas.SavePNG(helmPngPath, false, true); // Masked helm art keeps its alpha (eye holes, ear cutouts)
                        pngFilePathsToConvert.Add(helmPngPath);
                    }
                }

                // Blank transparent texture, shared by every race (only generated once)
                if (BlankTextureGenerated == false)
                {
                    string blankPngPath = Path.Combine(workingRootFolder, BLANK_TEXTURE_NAME + ".png");
                    using (Bitmap blankBitmap = new Bitmap(16, 16, PixelFormat.Format32bppArgb))
                    {
                        using (Graphics graphics = Graphics.FromImage(blankBitmap))
                            graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                        blankBitmap.Save(blankPngPath, ImageFormat.Png);
                    }
                    ImageTool.ConvertPNGTexturesToBLP(new List<string>() { blankPngPath }, ImageTool.ImageAssociationType.Clothing);
                    string blankBlpPath = Path.Combine(workingRootFolder, BLANK_TEXTURE_NAME + ".blp");
                    if (File.Exists(blankBlpPath) == true)
                        FileTool.CopyFile(blankBlpPath, Path.Combine(blankTextureOutputFolder, BLANK_TEXTURE_NAME + ".blp"));
                    else
                        Logger.WriteError("Character composite blank texture BLP was not generated at '" + blankBlpPath + "'");
                    BlankTextureGenerated = true;
                }

                // Convert this race's textures and copy them into the output (MPQ-staged) folder
                List<string> pngFilePathsForBLPs = new List<string>(pngFilePathsToConvert);
                ImageTool.ConvertPNGTexturesToBLP(pngFilePathsForBLPs, ImageTool.ImageAssociationType.Clothing);
                foreach (string pngFilePath in pngFilePathsToConvert)
                {
                    string blpFilePath = Path.ChangeExtension(pngFilePath, ".blp");
                    if (File.Exists(blpFilePath) == false)
                    {
                        Logger.WriteError("Character composite texture BLP was not generated at '" + blpFilePath + "'");
                        continue;
                    }
                    FileTool.CopyFile(blpFilePath, Path.Combine(outputFolder, Path.GetFileName(blpFilePath)));
                }
            }
        }

        // Bakes the worn armor components for this race: for every EQ armor texture set, the body blocks are baked with that set's
        // textures substituted for the naked ones, and each component region is cut out as its own PNG (transparent where no piece
        // sits, so the base skin shows through).  Boots and gloves get no LegLower/ArmLower components, since those regions hold the
        // legs and wrist pieces.  The item display infos reference these per race (see ItemDisplayInfo.CreateNativeVariants)
        public int GenerateNativeArmorComponentTextures(string outputFolder)
        {
            lock (CharacterTextureBakeLock)
            {
                if (Directory.Exists(outputFolder) == false)
                    Directory.CreateDirectory(outputFolder);
                string sourceTexturesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "characters", "Textures");
                Dictionary<string, int[]> sourcePixelsByTextureName = new Dictionary<string, int[]>();
                Dictionary<string, int> sourceWidthByTextureName = new Dictionary<string, int>();
                Dictionary<string, int> sourceHeightByTextureName = new Dictionary<string, int>();
                int setDigitsIndex = SkeletonName.Length + 2;
                int textureScale = GetTextureScale();
                int generatedCount = 0;

                HashSet<int> generatedArmorIDs = new HashSet<int>();
                int[] materialTypeIDs = new int[] { 0, 1, 2, 3, 4, 7, 17, 18, 19, 20, 21, 22, 23 };
                foreach (int materialTypeID in materialTypeIDs)
                {
                    int armorID = Items.ItemDisplayInfo.GetArmorIDForMaterialType(materialTypeID);
                    if (armorID <= 0 || generatedArmorIDs.Contains(armorID) == true)
                        continue;
                    string setDigits = materialTypeID.ToString("00");
                    generatedArmorIDs.Add(armorID);

                    foreach (CharacterCompositeBlock block in Enum.GetValues(typeof(CharacterCompositeBlock)))
                    {
                        if (block == CharacterCompositeBlock.Face || block == CharacterCompositeBlock.Robe)
                            continue;
                        int blockX0, blockY0, blockX1, blockY1;
                        GetBlockRect(block, out blockX0, out blockY0, out blockX1, out blockY1);
                        BakeCanvas blockCanvas = new BakeCanvas(blockX0, blockY0, blockX1 - blockX0, blockY1 - blockY0, textureScale);
                        bool anySliceBaked = false;
                        List<int[]> unbakedRects = new List<int[]>();
                        foreach (CharacterCompositeSlice slice in Slices)
                        {
                            if (slice.Block != block || slice.IsLaidOut == false || slice.BakeSourceTextureName.Length == 0)
                                continue;

                            // Texture set substitution: 'humch0001' -> 'humch0301'.  A piece without art in this set is left uncovered
                            // (transparent in the component, so the base skin shows: the iksar tail is a legs-coded texture with no
                            // armor variants, and padded over it wore the leg armor)
                            string sourceTextureName = slice.BakeSourceTextureName;
                            string setTextureName = string.Empty;
                            if (sourceTextureName.Length >= setDigitsIndex + 2 && sourceTextureName.ToLower().StartsWith(SkeletonName.ToLower()) == true)
                                setTextureName = string.Concat(sourceTextureName.Substring(0, setDigitsIndex), setDigits, sourceTextureName.Substring(setDigitsIndex + 2));
                            if (setTextureName.Length == 0 || File.Exists(Path.Combine(sourceTexturesFolder, setTextureName + ".png")) == false)
                            {
                                unbakedRects.Add(new int[4] { Convert.ToInt32(Math.Floor(slice.RectX0)) - slice.EdgeMarginTexels, Convert.ToInt32(Math.Floor(slice.RectY0)) - slice.EdgeMarginTexels,
                                    Convert.ToInt32(Math.Ceiling(slice.RectX1)) + slice.EdgeMarginTexels, Convert.ToInt32(Math.Ceiling(slice.RectY1)) + slice.EdgeMarginTexels });
                                continue;
                            }
                            BakeSliceIntoCanvas(blockCanvas, slice, setTextureName, sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName);
                            anySliceBaked = true;
                        }
                        if (anySliceBaked == false)
                            continue;
                        // Opaque over the whole block like a stock component, except over the pieces this set has no art for (the
                        // hood slot on non-robe chest sets among them)
                        if (block == CharacterCompositeBlock.ArmUpper && HasHoodSlice() == true)
                            unbakedRects.Add(new int[4] { HOOD_RESERVED_X0, HOOD_RESERVED_Y0, HOOD_RESERVED_X1, HOOD_RESERVED_Y1 });
                        blockCanvas.PadUncoveredFromNeighbors(true, unbakedRects);
                        foreach (ComponentRegion componentRegion in ComponentRegions)
                        {
                            if (componentRegion.Block != block)
                                continue;
                            string fileName = GetNativeComponentTextureName(SkeletonName, componentRegion.Name, armorID) + ".png";
                            bool anyCovered;
                            blockCanvas.SaveRegionPNG(Path.Combine(outputFolder, fileName), componentRegion.X0, componentRegion.Y0, componentRegion.X1, componentRegion.Y1, out anyCovered);
                            if (anyCovered == true)
                                generatedCount++;
                        }
                    }
                }
                // Robes: the robe mesh pieces baked with each robe texture set ('clk0405' -> 'clk0705'; pieces that are not robe cloth,
                // such as the iksar legs under the robe, bake with their own textures), cut into the regions a robe item rewrites
                bool hasRobeLayer = false;
                foreach (CharacterCompositeSlice slice in Slices)
                    if (slice.IsRobeLayer == true && slice.IsLaidOut == true)
                        hasRobeLayer = true;
                if (hasRobeLayer == true)
                {
                    for (int robeMaterialTypeID = 10; robeMaterialTypeID <= 16; robeMaterialTypeID++)
                    {
                        string robeSetDigits = (robeMaterialTypeID - 6).ToString("00");
                        int robeID = robeMaterialTypeID - 9;
                        BakeCanvas robeCanvas = new BakeCanvas(0, 0, BODY_COMPOSITE_WIDTH, BODY_COMPOSITE_HEIGHT, textureScale);
                        bool anyRobeSliceBaked = false;
                        foreach (CharacterCompositeSlice slice in Slices)
                        {
                            if ((slice.IsRobeLayer == false && slice.IsHoodSlice == false) || slice.IsLaidOut == false || slice.BakeSourceTextureName.Length == 0)
                                continue;
                            string setTextureName = slice.BakeSourceTextureName;
                            if (slice.IsHoodSlice == true)
                                setTextureName = GetRobeSetHoodTextureName(setTextureName, robeSetDigits, sourceTexturesFolder);
                            else if (setTextureName.ToLower().StartsWith("clk") == true && setTextureName.Length >= 5)
                                setTextureName = string.Concat(setTextureName.Substring(0, 3), robeSetDigits, setTextureName.Substring(5));
                            if (File.Exists(Path.Combine(sourceTexturesFolder, setTextureName + ".png")) == false)
                                continue;
                            BakeSliceIntoCanvas(robeCanvas, slice, setTextureName, sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName);
                            anyRobeSliceBaked = true;
                        }
                        if (anyRobeSliceBaked == false)
                            continue;
                        // Opaque over the robe's regions like a stock component (the robe carries its own hood art)
                        // Clear over the body-textured head pieces (the high elf female's hair bow): the worn leggings' component shows there
                        robeCanvas.PadUncoveredFromNeighbors(true, GetBodyTexturedHeadPieceRects(0));
                        foreach (ComponentRegion componentRegion in ComponentRegions)
                        {
                            if (componentRegion.Block != CharacterCompositeBlock.Legs && componentRegion.Block != CharacterCompositeBlock.Torso && componentRegion.Block != CharacterCompositeBlock.ArmUpper)
                                continue;
                            string fileName = GetNativeComponentTextureName(SkeletonName, string.Concat("Robe_", componentRegion.Name), robeID) + ".png";
                            bool anyCovered;
                            robeCanvas.SaveRegionPNG(Path.Combine(outputFolder, fileName), componentRegion.X0, componentRegion.Y0, componentRegion.X1, componentRegion.Y1, out anyCovered);
                            if (anyCovered == true)
                                generatedCount++;
                        }
                    }
                }
                Logger.WriteDebug(string.Concat("Character composite for '", SkeletonName, "' generated ", generatedCount.ToString(), " worn armor component textures"));
                return generatedCount;
            }
        }

        // Bakes a complete character skin for an NPC of this race: the texture set (0-4 armor, 10-16 robes) on every piece, the face, the
        // helmed head's pieces, and the creature template's per-slot tint colors, as one texture the client uses whole through a
        // CreatureDisplayInfoExtra row (BakeName) on the shared character model.  Returns false when nothing could be baked
        public bool BakeCreatureSkin(int textureIndex, int faceIndex, int helmIndex, CreatureTemplateColorTint? colorTint, string outputPngPath)
        {
            string sourceTexturesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "characters", "Textures");
            Dictionary<string, int[]> sourcePixelsByTextureName = new Dictionary<string, int[]>();
            Dictionary<string, int> sourceWidthByTextureName = new Dictionary<string, int>();
            Dictionary<string, int> sourceHeightByTextureName = new Dictionary<string, int>();
            int textureScale = GetTextureScale();
            int setDigitsIndex = SkeletonName.Length + 2;
            bool isRobeSet = (textureIndex >= 10 && textureIndex <= 16);
            string bodySetDigits = (isRobeSet ? 0 : textureIndex).ToString("00");
            string robeSetDigits = (textureIndex - 6).ToString("00");
            if (ValidFaceIndexes.Contains(faceIndex) == false)
                faceIndex = 0;

            BakeCanvas canvas = new BakeCanvas(0, 0, BODY_COMPOSITE_WIDTH, BODY_COMPOSITE_HEIGHT, textureScale);
            canvas.Fill(CalculateFillColor(false, unchecked((int)0xFF7A5A44), sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName));
            // Body pieces first, then the robe pieces over them (their rects overlap the body's, and the robe is what shows)
            List<CharacterCompositeSlice> bakeOrderedSlices = new List<CharacterCompositeSlice>();
            foreach (CharacterCompositeSlice slice in Slices)
                if (slice.IsRobeLayer == false)
                    bakeOrderedSlices.Add(slice);
            foreach (CharacterCompositeSlice slice in Slices)
                if (slice.IsRobeLayer == true)
                    bakeOrderedSlices.Add(slice);
            bool anyBaked = false;
            foreach (CharacterCompositeSlice slice in bakeOrderedSlices)
            {
                if (slice.IsLaidOut == false || slice.BakeSourceTextureName.Length == 0)
                    continue;
                if (slice.IsRobeLayer == true && isRobeSet == false)
                    continue;
                if (slice.Block == CharacterCompositeBlock.Helm)
                    continue;
                if (slice.HelmIndex >= 0 && slice.HelmIndex != helmIndex && slice.IsSharedHeadPiece == false)
                    continue;

                // Source texture: the texture set on the body and robe pieces, the face variant on the head pieces
                string sourceTextureName = slice.BakeSourceTextureName;
                string sourceTextureNameLower = sourceTextureName.ToLower();
                ColorRGBA? tintColor = null;
                if (slice.IsRobeLayer == true)
                {
                    if (sourceTextureNameLower.StartsWith("clk") == true && sourceTextureName.Length >= 5)
                        sourceTextureName = string.Concat(sourceTextureName.Substring(0, 3), robeSetDigits, sourceTextureName.Substring(5));
                    if (colorTint != null)
                        tintColor = colorTint.ChestColor;
                }
                else if (slice.IsHeadSlice == true)
                {
                    if (faceIndex > 0)
                    {
                        string faceVariantName = GetFaceVariantTextureName(slice.BakeSourceTextureName, faceIndex);
                        if (File.Exists(Path.Combine(sourceTexturesFolder, faceVariantName + ".png")) == true)
                            sourceTextureName = faceVariantName;
                    }
                    if (slice.IsHelmArt == true && colorTint != null)
                        tintColor = colorTint.HelmColor;
                }
                else if (slice.IsHoodSlice == true)
                {
                    // The hood follows the robe set (grey cloth like the other robe pieces, so the chest tint colours it); without a
                    // robe it keeps its default untinted texture
                    if (isRobeSet == true)
                    {
                        sourceTextureName = GetRobeSetHoodTextureName(sourceTextureName, robeSetDigits, sourceTexturesFolder);
                        if (colorTint != null)
                            tintColor = colorTint.ChestColor;
                    }
                }
                else if (slice.IsFaceRegionSlice == false)
                {
                    if (sourceTextureName.Length >= setDigitsIndex + 2 && sourceTextureNameLower.StartsWith(SkeletonName.ToLower()) == true)
                        sourceTextureName = string.Concat(sourceTextureName.Substring(0, setDigitsIndex), bodySetDigits, sourceTextureName.Substring(setDigitsIndex + 2));
                    if (colorTint != null)
                    {
                        switch (slice.Block)
                        {
                            case CharacterCompositeBlock.Torso: tintColor = colorTint.ChestColor; break;
                            case CharacterCompositeBlock.ArmUpper: tintColor = colorTint.ArmsColor; break;
                            case CharacterCompositeBlock.ArmLower: tintColor = colorTint.BracerColor; break;
                            case CharacterCompositeBlock.Hand: tintColor = colorTint.HandsColor; break;
                            case CharacterCompositeBlock.Legs: tintColor = colorTint.LegsColor; break;
                            case CharacterCompositeBlock.Foot: tintColor = colorTint.FeetColor; break;
                            default: break;
                        }
                    }
                }
                if (File.Exists(Path.Combine(sourceTexturesFolder, sourceTextureName + ".png")) == false)
                    sourceTextureName = slice.BakeSourceTextureName;
                BakeSliceIntoCanvas(canvas, slice, sourceTextureName, sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName, tintColor);
                anyBaked = true;
            }
            if (anyBaked == false)
                return false;
            canvas.SavePNG(outputPngPath, false);
            return true;
        }

        // Bakes one of the two face region textures for a face index
        private void BakeFaceRegionTexture(int faceIndex, int regionX0, int regionY0, int regionX1, int regionY1, string textureName,
            int fillColorArgb, string workingFolder, string sourceTexturesFolder, Dictionary<string, int[]> sourcePixelsByTextureName,
            Dictionary<string, int> sourceWidthByTextureName, Dictionary<string, int> sourceHeightByTextureName, List<string> pngFilePathsToConvert)
        {
            BakeCanvas faceCanvas = new BakeCanvas(regionX0, regionY0, regionX1 - regionX0, regionY1 - regionY0, GetTextureScale());
            faceCanvas.Fill(fillColorArgb);
            foreach (CharacterCompositeSlice slice in Slices)
            {
                if (slice.IsLaidOut == false || slice.IsFaceRegionSlice == false || slice.BakeSourceTextureName.Length == 0)
                    continue;
                if (slice.RectX0 >= regionX1 || slice.RectX1 <= regionX0 || slice.RectY0 >= regionY1 || slice.RectY1 <= regionY0)
                    continue;
                // Players show the bare head, so the helmed heads' private pieces (which overlap the bare head's) stay out
                if (slice.HelmIndex > 0 && slice.IsSharedHeadPiece == false)
                    continue;
                string sourceTextureName = slice.BakeSourceTextureName;
                if (slice.IsHeadSlice == true && faceIndex > 0)
                {
                    string faceVariantName = GetFaceVariantTextureName(slice.BakeSourceTextureName, faceIndex);
                    if (File.Exists(Path.Combine(sourceTexturesFolder, faceVariantName + ".png")) == true)
                        sourceTextureName = faceVariantName;
                }
                BakeSliceIntoCanvas(faceCanvas, slice, sourceTextureName, sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName);
            }
            string facePngPath = Path.Combine(workingFolder, textureName + ".png");
            faceCanvas.SavePNG(facePngPath, false);
            pngFilePathsToConvert.Add(facePngPath);
        }

        // Average color of the largest body (or head) piece's source texture, so fill areas blend with the skin
        private int CalculateFillColor(bool forHead, int fallbackFillColorArgb, string sourceTexturesFolder, Dictionary<string, int[]> sourcePixelsByTextureName,
            Dictionary<string, int> sourceWidthByTextureName, Dictionary<string, int> sourceHeightByTextureName)
        {
            CharacterCompositeSlice? largestSlice = null;
            foreach (CharacterCompositeSlice slice in Slices)
            {
                if (slice.BakeSourceTextureName.Length == 0 || slice.IsHeadSlice != forHead || slice.IsRobeLayer == true)
                    continue;
                if (largestSlice == null || slice.VertexCount > largestSlice.VertexCount)
                    largestSlice = slice;
            }
            if (largestSlice == null)
                return fallbackFillColorArgb;
            if (LoadSourceTexture(largestSlice.BakeSourceTextureName, sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName) == false)
                return fallbackFillColorArgb;
            int[] pixels = sourcePixelsByTextureName[largestSlice.BakeSourceTextureName];
            long totalR = 0;
            long totalG = 0;
            long totalB = 0;
            foreach (int pixel in pixels)
            {
                totalR += (pixel >> 16) & 0xFF;
                totalG += (pixel >> 8) & 0xFF;
                totalB += pixel & 0xFF;
            }
            int averageR = Convert.ToInt32(totalR / pixels.Length);
            int averageG = Convert.ToInt32(totalG / pixels.Length);
            int averageB = Convert.ToInt32(totalB / pixels.Length);
            return unchecked((int)0xFF000000) | (averageR << 16) | (averageG << 8) | averageB;
        }

        // How much the art changes across a seam placed at each texel column and row of a body texture, summed over the texture's
        // armor set variants (ObjectModelEQData.CalculateTileSeamPhases picks the seam by it).  A piece packed without a margin on an
        // axis shows the seam as a hard cut with its edge vertices pulled FULL_BLOCK_EDGE_PULL_IN_TEXELS inward (see ApplyUVRemap), so
        // the texels compared are the ones that then meet on the mesh: the pair the near edge's sample blends, against the pair the far
        // edge's sample blends
        private static readonly object SourceTextureNamesLock = new object();
        private static HashSet<string>? SourceTextureFileNamesLower = null;
        public static void CalculateSeamDiscontinuity(string textureName, string skeletonName, int texelsU, int texelsV, out float[] byColumn, out float[] byRow)
        {
            byColumn = new float[Math.Max(1, texelsU)];
            byRow = new float[Math.Max(1, texelsV)];
            string sourceTexturesFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "characters", "Textures");
            int pull = Math.Max(1, Convert.ToInt32(Math.Round(FULL_BLOCK_EDGE_PULL_IN_TEXELS)));
            foreach (string variantName in GetTextureSetVariantNames(textureName, skeletonName, sourceTexturesFolder))
            {
                int[] pixels;
                int width;
                int height;
                if (TryLoadPixels(Path.Combine(sourceTexturesFolder, variantName + ".png"), out pixels, out width, out height) == false)
                    continue;
                if (width != texelsU || height != texelsV)
                    continue;
                for (int column = 0; column < width; column++)
                {
                    float sum = 0;
                    for (int y = 0; y < height; y++)
                    {
                        int near = AverageColor(pixels[(y * width) + WrapIndex(column + pull - 1, width)], pixels[(y * width) + WrapIndex(column + pull, width)]);
                        int far = AverageColor(pixels[(y * width) + WrapIndex(column - pull - 1, width)], pixels[(y * width) + WrapIndex(column - pull, width)]);
                        sum += ColorDistance(near, far);
                    }
                    byColumn[column] += sum / height;
                }
                for (int row = 0; row < height; row++)
                {
                    float sum = 0;
                    for (int x = 0; x < width; x++)
                    {
                        int near = AverageColor(pixels[(WrapIndex(row + pull - 1, height) * width) + x], pixels[(WrapIndex(row + pull, height) * width) + x]);
                        int far = AverageColor(pixels[(WrapIndex(row - pull - 1, height) * width) + x], pixels[(WrapIndex(row - pull, height) * width) + x]);
                        sum += ColorDistance(near, far);
                    }
                    byRow[row] += sum / width;
                }
            }
        }

        private static int WrapIndex(int index, int count)
        {
            return ((index % count) + count) % count;
        }

        private static int AverageColor(int argb1, int argb2)
        {
            int a = (((argb1 >> 24) & 0xFF) + ((argb2 >> 24) & 0xFF)) / 2;
            int r = (((argb1 >> 16) & 0xFF) + ((argb2 >> 16) & 0xFF)) / 2;
            int g = (((argb1 >> 8) & 0xFF) + ((argb2 >> 8) & 0xFF)) / 2;
            int b = ((argb1 & 0xFF) + (argb2 & 0xFF)) / 2;
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        // Summed channel difference, weighted down where either texel is transparent (nothing of it shows)
        private static float ColorDistance(int argb1, int argb2)
        {
            float alpha = Math.Min((argb1 >> 24) & 0xFF, (argb2 >> 24) & 0xFF) / 255f;
            int distance = Math.Abs(((argb1 >> 16) & 0xFF) - ((argb2 >> 16) & 0xFF)) + Math.Abs(((argb1 >> 8) & 0xFF) - ((argb2 >> 8) & 0xFF)) + Math.Abs((argb1 & 0xFF) - (argb2 & 0xFF));
            return distance * alpha;
        }

        // The texture and every armor set variant of it that exists ('humch0001' -> 'humch0101', ...; 'clk0401' -> 'clk0101', ...)
        private static List<string> GetTextureSetVariantNames(string textureName, string skeletonName, string sourceTexturesFolder)
        {
            List<string> names = new List<string>();
            string nameLower = textureName.ToLower();
            names.Add(nameLower);
            int setDigitsIndex = -1;
            if (nameLower.StartsWith("clk") == true)
                setDigitsIndex = 3;
            else if (skeletonName.Length > 0 && nameLower.StartsWith(skeletonName.ToLower()) == true)
                setDigitsIndex = skeletonName.Length + 2;
            if (setDigitsIndex < 0 || nameLower.Length < setDigitsIndex + 2 || char.IsDigit(nameLower[setDigitsIndex]) == false || char.IsDigit(nameLower[setDigitsIndex + 1]) == false)
                return names;
            HashSet<string> fileNamesLower;
            lock (SourceTextureNamesLock)
            {
                if (SourceTextureFileNamesLower == null)
                {
                    SourceTextureFileNamesLower = new HashSet<string>();
                    if (Directory.Exists(sourceTexturesFolder) == true)
                        foreach (string filePath in Directory.GetFiles(sourceTexturesFolder, "*.png"))
                            SourceTextureFileNamesLower.Add(Path.GetFileNameWithoutExtension(filePath).ToLower());
                }
                fileNamesLower = SourceTextureFileNamesLower;
            }
            for (int set = 0; set < 100; set++)
            {
                string candidate = string.Concat(nameLower.Substring(0, setDigitsIndex), set.ToString("00"), nameLower.Substring(setDigitsIndex + 2));
                if (candidate != nameLower && fileNamesLower.Contains(candidate) == true)
                    names.Add(candidate);
            }
            return names;
        }

        // Reads a PNG's pixels as ARGB ints through a locked bitmap (GetPixel is far too slow for whole textures)
        private static bool TryLoadPixels(string filePath, out int[] pixels, out int width, out int height)
        {
            pixels = new int[0];
            width = 0;
            height = 0;
            if (File.Exists(filePath) == false)
                return false;
            try
            {
                using (Bitmap bitmap = new Bitmap(filePath))
                {
                    width = bitmap.Width;
                    height = bitmap.Height;
                    pixels = new int[width * height];
                    BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    try
                    {
                        for (int y = 0; y < height; y++)
                            System.Runtime.InteropServices.Marshal.Copy(IntPtr.Add(bitmapData.Scan0, y * bitmapData.Stride), pixels, y * width, width);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteError(string.Concat("Character composite could not read texture '", filePath, "': ", ex.Message));
                return false;
            }
        }

        private bool LoadSourceTexture(string textureName, string sourceTexturesFolder, Dictionary<string, int[]> sourcePixelsByTextureName,
            Dictionary<string, int> sourceWidthByTextureName, Dictionary<string, int> sourceHeightByTextureName)
        {
            if (sourcePixelsByTextureName.ContainsKey(textureName) == true)
                return true;
            string sourceFilePath = Path.Combine(sourceTexturesFolder, textureName + ".png");
            if (File.Exists(sourceFilePath) == false)
            {
                Logger.WriteError(string.Concat("Character composite for '", SkeletonName, "' could not find bake source texture '", sourceFilePath, "'"));
                return false;
            }
            using (Bitmap sourceBitmap = new Bitmap(sourceFilePath))
            {
                int width = sourceBitmap.Width;
                int height = sourceBitmap.Height;
                int[] pixels = new int[width * height];
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        pixels[(y * width) + x] = sourceBitmap.GetPixel(x, y).ToArgb();
                sourcePixelsByTextureName.Add(textureName, pixels);
                sourceWidthByTextureName.Add(textureName, width);
                sourceHeightByTextureName.Add(textureName, height);
            }
            return true;
        }

        // Samples a slice's source texture through its UV window across its rect (plus the bake margin around it, which just continues
        // the wrapped tiling, so filtering at the rect edge sees the same art the mesh does), writing into the canvas
        private void BakeSliceIntoCanvas(BakeCanvas canvas, CharacterCompositeSlice slice, string sourceTextureName, string sourceTexturesFolder,
            Dictionary<string, int[]> sourcePixelsByTextureName, Dictionary<string, int> sourceWidthByTextureName, Dictionary<string, int> sourceHeightByTextureName,
            ColorRGBA? tintColor = null)
        {
            if (LoadSourceTexture(sourceTextureName, sourceTexturesFolder, sourcePixelsByTextureName, sourceWidthByTextureName, sourceHeightByTextureName) == false)
                return;
            int[] sourcePixels = sourcePixelsByTextureName[sourceTextureName];
            int sourceWidth = sourceWidthByTextureName[sourceTextureName];
            int sourceHeight = sourceHeightByTextureName[sourceTextureName];

            // Pixel loop runs in the canvas's space
            int bakeMarginPixels = slice.EdgeMarginTexels * canvas.Scale;
            int startX = Convert.ToInt32(Math.Floor((slice.RectX0 - canvas.OriginX) * canvas.Scale)) - bakeMarginPixels;
            int endX = Convert.ToInt32(Math.Ceiling((slice.RectX1 - canvas.OriginX) * canvas.Scale)) + bakeMarginPixels;
            int startY = Convert.ToInt32(Math.Floor((slice.RectY0 - canvas.OriginY) * canvas.Scale)) - bakeMarginPixels;
            int endY = Convert.ToInt32(Math.Ceiling((slice.RectY1 - canvas.OriginY) * canvas.Scale)) + bakeMarginPixels;
            if (slice.ClampX1 > slice.ClampX0 && slice.ClampY1 > slice.ClampY0)
            {
                // The bleed stays inside the piece's own area, so it never writes into a neighbouring region's art
                startX = Math.Max(startX, Convert.ToInt32(Math.Floor((slice.ClampX0 - canvas.OriginX) * canvas.Scale)));
                startY = Math.Max(startY, Convert.ToInt32(Math.Floor((slice.ClampY0 - canvas.OriginY) * canvas.Scale)));
                endX = Math.Min(endX, Convert.ToInt32(Math.Ceiling((slice.ClampX1 - canvas.OriginX) * canvas.Scale)));
                endY = Math.Min(endY, Convert.ToInt32(Math.Ceiling((slice.ClampY1 - canvas.OriginY) * canvas.Scale)));
            }
            startX = Math.Max(startX, 0);
            startY = Math.Max(startY, 0);
            endX = Math.Min(endX, canvas.Width);
            endY = Math.Min(endY, canvas.Height);
            float spanU = Math.Max(slice.UVMaxU - slice.UVMinU, 1f / 4096f);
            float spanV = Math.Max(slice.UVMaxV - slice.UVMinV, 1f / 4096f);
            float rectWidth = Math.Max(slice.RectX1 - slice.RectX0, 0.0001f);
            float rectHeight = Math.Max(slice.RectY1 - slice.RectY0, 0.0001f);

            // A piece packed below texel-for-texel (the human chest: 64x128 art into the 88-row torso block) is box filtered, each
            // generated pixel averaging the source texels it covers.  Nearest sampling there drops rows and columns, which alternately
            // keeps or skips one-texel art features: the human's mesh seam runs down the dark shadow of the sternum groove, and with
            // the shadow's texels kept whole and their bright neighbours dropped, the seam showed as a line the live models (art at
            // 1:1) never showed.  Pieces at 1:1 keep nearest sampling, so their pixel art stays exact
            float rectAlongU = slice.IsRotated ? rectHeight : rectWidth; // Composite extent the source's U axis runs along (a turned piece's U runs down its rect)
            float rectAlongV = slice.IsRotated ? rectWidth : rectHeight;
            float sourceTexelsPerPixelU = (spanU * sourceWidth) / (rectAlongU * canvas.Scale);
            float sourceTexelsPerPixelV = (spanV * sourceHeight) / (rectAlongV * canvas.Scale);
            // Only a real shrink is box filtered.  Near texel-for-texel (the 94% rotated chests) a box footprint straddles two source
            // texels at a drifting phase and washes out one-texel detail (the dark elf's collar rivets vanished), while nearest
            // sampling there drops one column in sixteen and keeps the art crisp; the seam-edge pull-in keeps mirrored centre lines
            // clean either way
            const float BOX_FILTER_MIN_SOURCE_TEXELS_PER_PIXEL = 1.12f;
            bool boxFilter = sourceTexelsPerPixelU > BOX_FILTER_MIN_SOURCE_TEXELS_PER_PIXEL || sourceTexelsPerPixelV > BOX_FILTER_MIN_SOURCE_TEXELS_PER_PIXEL;

            for (int y = startY; y < endY; y++)
            {
                float compositeY = ((Convert.ToSingle(y) + 0.5f) / canvas.Scale) + canvas.OriginY;
                for (int x = startX; x < endX; x++)
                {
                    float compositeX = ((Convert.ToSingle(x) + 0.5f) / canvas.Scale) + canvas.OriginX;
                    float offsetAlongU = slice.IsRotated ? (compositeY - slice.RectY0) : (compositeX - slice.RectX0);
                    float offsetAlongV = slice.IsRotated ? (compositeX - slice.RectX0) : (compositeY - slice.RectY0);
                    float sourceU = slice.UVMinU + ((offsetAlongU / rectAlongU) * spanU);
                    float sourceV = slice.UVMinV + ((offsetAlongV / rectAlongV) * spanV);
                    int index = (y * canvas.Width) + x;
                    int sampledColor;
                    if (boxFilter == true)
                    {
                        float pixelHalfU = (0.5f / canvas.Scale) / rectAlongU * spanU;
                        float pixelHalfV = (0.5f / canvas.Scale) / rectAlongV * spanV;
                        float boxU0 = ClampToWindowUnlessWrapping(sourceU - pixelHalfU, slice.UVMinU, slice.UVMaxU, slice.WrapsU, sourceWidth);
                        float boxU1 = ClampToWindowUnlessWrapping(sourceU + pixelHalfU, slice.UVMinU, slice.UVMaxU, slice.WrapsU, sourceWidth);
                        float boxV0 = ClampToWindowUnlessWrapping(sourceV - pixelHalfV, slice.UVMinV, slice.UVMaxV, slice.WrapsV, sourceHeight);
                        float boxV1 = ClampToWindowUnlessWrapping(sourceV + pixelHalfV, slice.UVMinV, slice.UVMaxV, slice.WrapsV, sourceHeight);
                        sampledColor = SampleSourceBoxWrapped(sourcePixels, sourceWidth, sourceHeight, boxU0, boxV0, Math.Max(boxU1, boxU0 + 0.0001f), Math.Max(boxV1, boxV0 + 0.0001f));
                    }
                    else
                    {
                        sourceU = ClampToWindowUnlessWrapping(sourceU, slice.UVMinU, slice.UVMaxU, slice.WrapsU, sourceWidth);
                        sourceV = ClampToWindowUnlessWrapping(sourceV, slice.UVMinV, slice.UVMaxV, slice.WrapsV, sourceHeight);
                        sampledColor = SampleSourceNearestWrapped(sourcePixels, sourceWidth, sourceHeight, sourceU, sourceV); // Nearest keeps the EQ pixel art crisp; the client's own filtering is the only blur left
                    }
                    if (tintColor != null)
                    {
                        // Same multiply the creature textures were tinted with (ImageTool.GenerateColoredTintedTexture)
                        ColorRGBA tint = (ColorRGBA)tintColor;
                        int red = (((sampledColor >> 16) & 0xFF) * tint.R) / 255;
                        int green = (((sampledColor >> 8) & 0xFF) * tint.G) / 255;
                        int blue = ((sampledColor & 0xFF) * tint.B) / 255;
                        sampledColor = (sampledColor & unchecked((int)0xFF000000)) | (red << 16) | (green << 8) | blue;
                    }
                    // Only masked materials carry meaningful alpha; everything else bakes opaque whatever its source PNG says
                    if (slice.IsAlphaKeyed == false)
                        sampledColor |= unchecked((int)0xFF000000);
                    canvas.Pixels[index] = sampledColor;
                    canvas.Covered[index] = true;
                }
            }
        }

        // The margin beyond a piece edge the mesh does not wrap across replicates the edge texel (see DetectWrapSeams)
        private static float ClampToWindowUnlessWrapping(float value, float windowMin, float windowMax, bool wraps, int sourceTexels)
        {
            if (wraps == true)
                return value;
            float halfTexel = 0.5f / Math.Max(1, sourceTexels);
            return Math.Max(windowMin + halfTexel, Math.Min(windowMax - halfTexel, value));
        }

        private static int SampleSourceNearestWrapped(int[] sourcePixels, int sourceWidth, int sourceHeight, float u, float v)
        {
            // Wrap into [0, 1) the same way the renderer tiles the texture, then take the texel the sample point falls in
            float wrappedU = u - Convert.ToSingle(Math.Floor(u));
            float wrappedV = v - Convert.ToSingle(Math.Floor(v));
            int x = Convert.ToInt32(Math.Floor(wrappedU * sourceWidth));
            int y = Convert.ToInt32(Math.Floor(wrappedV * sourceHeight));
            x = ((x % sourceWidth) + sourceWidth) % sourceWidth;
            y = ((y % sourceHeight) + sourceHeight) % sourceHeight;
            return sourcePixels[(y * sourceWidth) + x];
        }

        // Area average of the source texels under the UV box [u0, u1] x [v0, v1] (wrapped like the renderer tiles the texture), each
        // texel weighted by how much of it the box covers
        private static int SampleSourceBoxWrapped(int[] sourcePixels, int sourceWidth, int sourceHeight, float u0, float v0, float u1, float v1)
        {
            float sourceX0 = u0 * sourceWidth;
            float sourceX1 = u1 * sourceWidth;
            float sourceY0 = v0 * sourceHeight;
            float sourceY1 = v1 * sourceHeight;
            int texelX0 = Convert.ToInt32(Math.Floor(sourceX0));
            int texelX1 = Convert.ToInt32(Math.Ceiling(sourceX1)) - 1;
            int texelY0 = Convert.ToInt32(Math.Floor(sourceY0));
            int texelY1 = Convert.ToInt32(Math.Ceiling(sourceY1)) - 1;
            float sumA = 0, sumR = 0, sumG = 0, sumB = 0, sumWeight = 0;
            for (int texelY = texelY0; texelY <= texelY1; texelY++)
            {
                float coverY = Math.Min(sourceY1, texelY + 1) - Math.Max(sourceY0, texelY);
                if (coverY <= 0)
                    continue;
                int wrappedY = ((texelY % sourceHeight) + sourceHeight) % sourceHeight;
                for (int texelX = texelX0; texelX <= texelX1; texelX++)
                {
                    float coverX = Math.Min(sourceX1, texelX + 1) - Math.Max(sourceX0, texelX);
                    if (coverX <= 0)
                        continue;
                    int wrappedX = ((texelX % sourceWidth) + sourceWidth) % sourceWidth;
                    int color = sourcePixels[(wrappedY * sourceWidth) + wrappedX];
                    float weight = coverX * coverY;
                    sumA += ((color >> 24) & 0xFF) * weight;
                    sumR += ((color >> 16) & 0xFF) * weight;
                    sumG += ((color >> 8) & 0xFF) * weight;
                    sumB += (color & 0xFF) * weight;
                    sumWeight += weight;
                }
            }
            if (sumWeight <= 0)
                return SampleSourceNearestWrapped(sourcePixels, sourceWidth, sourceHeight, (u0 + u1) * 0.5f, (v0 + v1) * 0.5f);
            int a = Math.Min(255, Convert.ToInt32(Math.Round(sumA / sumWeight)));
            int r = Math.Min(255, Convert.ToInt32(Math.Round(sumR / sumWeight)));
            int g = Math.Min(255, Convert.ToInt32(Math.Round(sumG / sumWeight)));
            int b = Math.Min(255, Convert.ToInt32(Math.Round(sumB / sumWeight)));
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        private static int SampleSourceBilinearWrapped(int[] sourcePixels, int sourceWidth, int sourceHeight, float u, float v)
        {
            // Wrap into [0, 1) the same way the renderer tiles the texture
            float wrappedU = u - Convert.ToSingle(Math.Floor(u));
            float wrappedV = v - Convert.ToSingle(Math.Floor(v));
            float sourceX = (wrappedU * sourceWidth) - 0.5f;
            float sourceY = (wrappedV * sourceHeight) - 0.5f;
            int x0 = Convert.ToInt32(Math.Floor(sourceX));
            int y0 = Convert.ToInt32(Math.Floor(sourceY));
            float fractionX = sourceX - x0;
            float fractionY = sourceY - y0;
            int x1 = x0 + 1;
            int y1 = y0 + 1;
            x0 = ((x0 % sourceWidth) + sourceWidth) % sourceWidth;
            x1 = ((x1 % sourceWidth) + sourceWidth) % sourceWidth;
            y0 = ((y0 % sourceHeight) + sourceHeight) % sourceHeight;
            y1 = ((y1 % sourceHeight) + sourceHeight) % sourceHeight;

            int color00 = sourcePixels[(y0 * sourceWidth) + x0];
            int color10 = sourcePixels[(y0 * sourceWidth) + x1];
            int color01 = sourcePixels[(y1 * sourceWidth) + x0];
            int color11 = sourcePixels[(y1 * sourceWidth) + x1];

            int blendedColor = 0;
            for (int channelShift = 0; channelShift <= 24; channelShift += 8)
            {
                float channel00 = (color00 >> channelShift) & 0xFF;
                float channel10 = (color10 >> channelShift) & 0xFF;
                float channel01 = (color01 >> channelShift) & 0xFF;
                float channel11 = (color11 >> channelShift) & 0xFF;
                float blendedTop = channel00 + ((channel10 - channel00) * fractionX);
                float blendedBottom = channel01 + ((channel11 - channel01) * fractionX);
                float blended = blendedTop + ((blendedBottom - blendedTop) * fractionY);
                int blendedByte = Convert.ToInt32(Math.Round(blended));
                if (blendedByte < 0)
                    blendedByte = 0;
                if (blendedByte > 255)
                    blendedByte = 255;
                blendedColor = blendedColor | (blendedByte << channelShift);
            }
            return blendedColor;
        }
    }
}
