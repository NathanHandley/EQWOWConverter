namespace LanternExtractor.EQ.Sound
{
    /// <summary>
    /// Describes the way the sound is heard by the player
    /// </summary>
    public enum AudioType : byte
    {
        /// <summary>
        /// Sounds that play at a constant volume
        /// </summary>
        Sound2d = 0,

        /// <summary>
        /// Music instance. Can specify both day and night trackID.
        /// </summary>
        Music = 1,

        /// <summary>
        /// Sounds that have a falloff - the farther the player is from the center, the quieter it becomes.
        /// </summary>
        Sound3d = 2,
    }
}