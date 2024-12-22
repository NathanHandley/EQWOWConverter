namespace LanternExtractor.EQ.Wld.DataTypes
{
    public enum MaterialType
    {
        // Used for boundaries that are not rendered. TextInfoReference can be null or have reference.
        Boundary = 0x0,
        // Standard diffuse shader
        Diffuse = 0x01,
        // Diffuse variant
        Diffuse2 = 0x02,
        // Transparent with 0.5 blend strength
        Transparent50 = 0x05,
        // Transparent with 0.25 blend strength
        Transparent25 = 0x09,
        // Transparent with 0.75 blend strength
        Transparent75 = 0x0A,
        // Non solid surfaces that shouldn't really be masked
        TransparentMaskedPassable = 0x07,
        TransparentAdditiveUnlit = 0x0B,
        TransparentMasked = 0x13,
        Diffuse3 = 0x14,
        Diffuse4 = 0x15,
        TransparentAdditive = 0x17,
        Diffuse5 = 0x19,
        InvisibleUnknown = 0x53,
        Diffuse6 = 0x553,
        CompleteUnknown = 0x1A, // TODO: Analyze this
        Diffuse7 = 0x12,
        Diffuse8 = 0x31,
        InvisibleUnknown2 = 0x4B,
        DiffuseSkydome = 0x0D, // Need to confirm
        TransparentSkydome = 0x0F, // Need to confirm
        TransparentAdditiveUnlitSkydome = 0x10,
        InvisibleUnknown3 = 0x03,
    }
}