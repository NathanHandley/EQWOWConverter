namespace LanternExtractor.EQ.Sound
{
    public abstract class SoundInstance : AudioInstance
    {
        public string Sound1 { get; }
        public float Volume1 { get; }
        public int Cooldown1 { get; }
        public int CooldownRandom { get; }

        protected SoundInstance(AudioType type, float posX, float posY, float posZ, float radius, float volume1,
            string sound1, int cooldown1, int cooldownRandom) : base(type, posX, posY, posZ, radius)
        {
            Sound1 = sound1;
            Volume1 = volume1;
            Cooldown1 = cooldown1;
            CooldownRandom = cooldownRandom;
        }
    }
}
