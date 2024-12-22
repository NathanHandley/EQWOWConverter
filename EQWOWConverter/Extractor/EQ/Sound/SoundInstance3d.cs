namespace LanternExtractor.EQ.Sound
{
    public class SoundInstance3d : SoundInstance
    {
        public int Multiplier;

        public SoundInstance3d(AudioType type, float posX, float posY, float posZ, float radius, float volume,
            string sound1, int cooldown1, int cooldownRandom, int multiplier) : base(type, posX, posY, posZ, radius, volume, sound1,
            cooldown1, cooldownRandom)
        {
            Multiplier = multiplier;
        }
    }
}
