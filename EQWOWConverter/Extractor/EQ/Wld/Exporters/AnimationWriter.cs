using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class AnimationWriter : TextAssetWriter
    {
        private string _targetAnimation;
        private bool _isCharacterAnimation;

        public AnimationWriter(bool isCharacterAnimation)
        {
            Export.AppendLine(LanternStrings.ExportHeaderTitle + "Animation");
            _isCharacterAnimation = isCharacterAnimation;
        }

        public void SetTargetAnimation(string animationName)
        {
            _targetAnimation = animationName;
        }

        public override void AddFragmentData(WldFragment data)
        {
            if (!(data is SkeletonHierarchy skeleton))
            {
                return;
            }

            if (_targetAnimation == string.Empty)
            {
                return;
            }

            if (!skeleton.Animations.ContainsKey(_targetAnimation))
            {
                return;
            }

            Animation anim = skeleton.Animations[_targetAnimation];

            Export.AppendLine("# Animation: " + _targetAnimation);
            Export.AppendLine("framecount," + anim.FrameCount);
            Export.AppendLine("totalTimeMs," + anim.AnimationTimeMs);

            for (int i = 0; i < skeleton.Skeleton.Count; ++i)
            {
                string boneName = Animation.CleanBoneAndStripBase(skeleton.BoneMapping[i], skeleton.ModelBase);
                string fullPath = skeleton.Skeleton[i].CleanedFullPath;

                var trackArray = anim.TracksCleanedStripped;
                var poseArray = skeleton.Animations["pos"].TracksCleanedStripped;

                if (!trackArray.ContainsKey(boneName))
                {
                    if (poseArray == null || !poseArray.ContainsKey(boneName))
                    {
                        return;
                    }

                    var bt = poseArray[boneName].TrackDefFragment.Frames[0];

                    if (bt == null)
                    {
                        return;
                    }

                    CreateTrackString(fullPath, 0, bt, anim.AnimationTimeMs);
                }
                else
                {
                    for (int j = 0; j < anim.FrameCount; ++j)
                    {
                        if (j >= trackArray[boneName].TrackDefFragment.Frames.Count)
                        {
                            break;
                        }

                        BoneTransform boneTransform = trackArray[boneName].TrackDefFragment.Frames[j];
                        int delay = _isCharacterAnimation
                            ? anim.AnimationTimeMs / anim.FrameCount
                            : skeleton.Skeleton[i].Track.FrameMs;
                        CreateTrackString(fullPath, j, boneTransform, delay);
                    }
                }
            }
        }

        private void CreateTrackString(string fullPath, int frame, BoneTransform boneTransform, int delay)
        {
            Export.Append(fullPath);
            Export.Append(",");

            Export.Append(frame);
            Export.Append(",");

            Export.Append(boneTransform.Translation.x);
            Export.Append(",");

            Export.Append(boneTransform.Translation.z);
            Export.Append(",");

            Export.Append(boneTransform.Translation.y);
            Export.Append(",");

            Export.Append(-boneTransform.Rotation.x);
            Export.Append(",");

            Export.Append(-boneTransform.Rotation.z);
            Export.Append(",");

            Export.Append(-boneTransform.Rotation.y);
            Export.Append(",");

            Export.Append(boneTransform.Rotation.w);
            Export.Append(",");

            Export.Append(boneTransform.Scale);
            Export.Append(",");

            Export.Append(delay.ToString());

            Export.AppendLine();
        }


        private string StripModelBase(string boneName, string modelBase)
        {
            if (boneName.StartsWith(modelBase + "/"))
            {
                boneName = boneName.Substring(modelBase.Length);
                boneName = boneName.Insert(0, "root");
            }


            return boneName;
        }
    }
}
