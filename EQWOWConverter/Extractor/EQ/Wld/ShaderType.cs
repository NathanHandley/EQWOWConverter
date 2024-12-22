namespace LanternExtractor.EQ.Wld
{
    /// <summary>
    /// The shaders used by the EQ client to render surfaces
    /// </summary>
    public enum ShaderType
    {
        Diffuse = 0,
        Transparent25 = 1,
        Transparent50 = 2,
        Transparent75 = 3,       
        TransparentAdditive = 4,
        TransparentAdditiveUnlit = 5,
        TransparentMasked = 6,
        DiffuseSkydome = 7,
        TransparentSkydome = 8, 
        TransparentAdditiveUnlitSkydome = 9,
        Invisible = 10,
        Boundary = 11,
    }
}