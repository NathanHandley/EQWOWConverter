namespace LanternExtractor.EQ.Wld
{
    public enum WldType
    {
        /// <summary>
        /// Contains main zone geometry, BSP tree
        /// Example: arena.s3d, arena.wld
        /// </summary>
        Zone,

        /// <summary>
        /// Contains the zone object instance data
        /// Example: arena.s3d, objects.wld
        /// </summary>
        ZoneObjects,

        /// <summary>
        /// Contains light instances
        /// Example: arena.s3d, lights.wld
        /// </summary>
        Lights,

        /// <summary>
        /// Contains zone object model geometry
        /// Example: arena_obj.s3d, arena_obj.wld
        /// </summary>
        Objects,

        /// <summary>
        /// Contains sky data, models and animations
        /// Example: sky.s3d, sky.wld
        /// </summary>
        Sky,

        /// <summary>
        /// Contains zone character models, animations
        /// Example: arena_chr.s3d, arena_chr.wld
        /// </summary>
        Characters,
        
        /// <summary>
        /// Contains general models - only a few of these exist
        /// Example: gequip.s3d
        /// </summary>
        Equipment
    }
}