namespace LanternExtractor.EQ.Sound
{
    public enum EmissionType
    {
        None = 0,
        
        /// <summary>
        /// Emitted sounds - things like bird noises
        /// </summary>
        Emit = 1,

        /// <summary>
        /// Looped sounds - things like the ocean or a lake
        /// </summary>
        Loop = 2,

        /// <summary>
        /// Sounds that are internal to the client
        /// </summary>
        Internal = 3
    }
}