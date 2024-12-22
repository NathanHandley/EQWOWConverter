namespace LanternExtractor.EQ.Sound
{
    public class SoundInstance2d : SoundInstance
    {
        public string Sound2 { get; }
        public float Volume2 { get; }
        public int Cooldown2 { get; }

        public SoundInstance2d(AudioType type, float posX, float posY, float posZ, float radius, float volume1,
            string sound1, int cooldown1, string sound2, int cooldown2,  int cooldownRandom, float volume2) : base(type, posX, posY, posZ, radius, volume1,
            sound1, cooldown1, cooldownRandom)
        {
            Sound2 = sound2;
            Cooldown2 = cooldown2;
            Volume2 = volume2;
        }
    }
}
