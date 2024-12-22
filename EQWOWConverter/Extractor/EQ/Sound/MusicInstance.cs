namespace LanternExtractor.EQ.Sound
{
    public class MusicInstance : AudioInstance
    {
        public MusicInstance(AudioType type, float posX, float posY, float posZ, float radius, int trackIndexDay,
            int trackIndexNight, int loopCountDay, int loopCountNight, int fadeOutMs) : base(type, posX, posY, posZ,
            radius)
        {
            TrackIndexDay = trackIndexDay;
            TrackIndexNight = trackIndexNight;
            LoopCountDay = loopCountDay;
            LoopCountNight = loopCountNight;
            FadeOutMs = fadeOutMs;
        }

        public int TrackIndexDay { get; }
        public int TrackIndexNight { get; }
        public int LoopCountDay { get; }
        public int LoopCountNight { get; }
        public int FadeOutMs { get; }
    }
}
