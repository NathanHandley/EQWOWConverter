using System.Collections.Generic;

namespace LanternExtractor.EQ.Sound
{
    public static class ClientSounds
    {
        // Hardcoded client sounds - verified that no other references exist in Trilogy client
        private static Dictionary<int, string> _clientSounds = new Dictionary<int, string>
        {
            { 39, "death_me" },
            { 143, "thunder1" },
            { 144, "thunder2" },
            { 158, "wind_lp1" },
            { 159, "rainloop" },
            { 160, "torch_lp" },
            { 161, "watundlp" },
        };

        public static string GetClientSound(int index)
        {
            return _clientSounds.TryGetValue(index, out var soundName) ? soundName : SoundConstants.Unknown;
        }
    }
}