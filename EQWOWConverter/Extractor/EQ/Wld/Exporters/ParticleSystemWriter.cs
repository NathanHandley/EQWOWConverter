using System.Globalization;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class ParticleSystemWriter : TextAssetWriter
    {
        private string _attachedSkeleton = string.Empty;
        private string _attachedBone = string.Empty;

        public ParticleSystemWriter()
        {
            Export.AppendLine(LanternStrings.ExportHeaderTitle + "Particle System");
            Export.AppendLine(LanternStrings.ExportHeaderFormat + "key,value");
        }

        public void SetBoneAttachment(string skeletonModelBase, string boneName)
        {
            _attachedSkeleton = skeletonModelBase;
            _attachedBone = boneName;
        }

        public override void AddFragmentData(WldFragment data)
        {
            if (!(data is ParticleCloud cloud))
            {
                return;
            }

            CultureInfo ci = CultureInfo.InvariantCulture;

            Export.AppendLine("name," + FragmentNameCleaner.CleanName(cloud));

            if (string.IsNullOrEmpty(_attachedSkeleton) == false)
                Export.AppendLine("attached_skeleton," + _attachedSkeleton);
            if (string.IsNullOrEmpty(_attachedBone) == false)
                Export.AppendLine("attached_bone," + _attachedBone);

            Export.AppendLine("movement," + cloud.Movement);
            Export.AppendLine("flag_high_opacity," + cloud.HighOpacity);
            Export.AppendLine("flag_follows_item," + cloud.FollowsItem);
            Export.AppendLine("flag_1," + cloud.Flag1);
            Export.AppendLine("flag_3," + cloud.Flag3);
            Export.AppendLine("flags_raw,0x" + cloud.FlagsRaw.ToString("X2"));

            Export.AppendLine("simultaneous_particles," + cloud.SimultaneousParticles.ToString(ci));
            Export.AppendLine("spawn_radius," + cloud.SpawnRadius.ToString(ci));
            Export.AppendLine("spawn_angle," + cloud.SpawnAngle.ToString(ci));
            Export.AppendLine("spawn_lifespan_ms," + cloud.SpawnLifespanMs.ToString(ci));
            Export.AppendLine("spawn_velocity," + cloud.SpawnVelocity.ToString(ci));
            Export.AppendLine("spawn_normal_x," + cloud.SpawnNormal.x.ToString(ci));
            Export.AppendLine("spawn_normal_y," + cloud.SpawnNormal.y.ToString(ci));
            Export.AppendLine("spawn_normal_z," + cloud.SpawnNormal.z.ToString(ci));
            Export.AppendLine("spawn_rate_ms," + cloud.SpawnRateMs.ToString(ci));
            Export.AppendLine("spawn_scale," + cloud.SpawnScale.ToString(ci));

            Export.AppendLine("tint_r," + cloud.Tint.R.ToString(ci));
            Export.AppendLine("tint_g," + cloud.Tint.G.ToString(ci));
            Export.AppendLine("tint_b," + cloud.Tint.B.ToString(ci));
            Export.AppendLine("tint_a," + cloud.Tint.A.ToString(ci));

            ParticleSprite sprite = cloud.ParticleSprite;
            if (sprite != null)
            {
                Export.AppendLine("sprite_name," + FragmentNameCleaner.CleanName(sprite));
                Export.AppendLine("render_method,0x" + sprite.RenderMethod.ToString("X8"));
                MaterialType materialType = (MaterialType)(sprite.RenderMethod & ~0x80000000);
                Export.AppendLine("render_method_name," + materialType);

                BitmapInfo? bitmapInfo = sprite.BitmapReference?.BitmapInfo;
                if (bitmapInfo != null)
                {
                    Export.AppendLine("texture_animated," + bitmapInfo.IsAnimated);
                    if (bitmapInfo.IsAnimated)
                    {
                        Export.AppendLine("texture_animation_delay_ms," + bitmapInfo.AnimationDelayMs.ToString(ci));
                    }

                    if (bitmapInfo.BitmapNames != null)
                    {
                        for (int i = 0; i < bitmapInfo.BitmapNames.Count; i++)
                        {
                            BitmapName bitmap = bitmapInfo.BitmapNames[i];
                            Export.AppendLine("texture_frame_" + i + "," + bitmap.GetFilenameWithoutExtension());
                        }
                    }
                }
            }
        }

        public override void ClearExportData()
        {
            base.ClearExportData();
            _attachedSkeleton = string.Empty;
            _attachedBone = string.Empty;
        }
    }
}