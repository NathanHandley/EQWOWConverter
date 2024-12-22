namespace LanternExtractor.EQ.Sound
{
    public abstract class AudioInstance
    {
        protected AudioInstance(AudioType type, float posX, float posY, float posZ, float radius)
        {
            AudioType = type;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            Radius = radius;
        }

        public AudioType AudioType { get; }
        public float PosX { get; }
        public float PosY { get; }
        public float PosZ { get; }
        public float Radius { get; }
    }
}
