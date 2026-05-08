using GlmSharp;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.Infrastructure;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Fragments
{
    /// <summary>
    /// ParticleCloud (0x34)
    /// Internal name: None
    /// Defines a particle system. Can be referenced from a skeleton bone.
    /// Field names follow Zaela's WLD Editor Suite / libeq conventions.
    /// </summary>
    public class ParticleCloud : WldFragment
    {
        public ParticleSprite ParticleSprite { get; private set; }

        // Two leading constants in Trilogy data (always 4 and 3). Kept for round-tripping.
        public int Setting1 { get; private set; }
        public int Setting2 { get; private set; }

        // Dispersal mode: Sphere, Plane, Stream, or None.
        public ParticleMovementType Movement { get; private set; }

        // Behaviour bitfield (offset 16, 4 bytes). Bits documented by Zaela.
        public bool Flag1 { get; private set; }
        public bool HighOpacity { get; private set; }
        public bool Flag3 { get; private set; }
        public bool FollowsItem { get; private set; }
        public byte FlagsRaw { get; private set; }

        // Maximum number of live particles at once
        public int SimultaneousParticles { get; private set; }

        // Sphere radius (used when Movement is "Sphere")
        public float SpawnRadius { get; private set; }

        // Cone angle for stream/plane spread (degrees)
        public float SpawnAngle { get; private set; }

        // Particle lifetime in milliseconds
        public uint SpawnLifespanMs { get; private set; }

        // Initial dispersal speed magnitude
        public float SpawnVelocity { get; private set; }

        //Dispersal direction unit vector., stored on disk in (Z, X, Y) order
        public vec3 SpawnNormal { get; private set; }

        // Milliseconds between spawns. Lower = denser emission
        public uint SpawnRateMs { get; private set; }

        // Per-particle scale multiplier
        public float SpawnScale { get; private set; }

        // Tint color applied over the sprite texture
        public Color Tint { get; private set; }

        public override void Initialize(int index, int size, byte[] data, List<WldFragment> fragments,
            Dictionary<int, string> stringHash,
            bool isNewWldFormat, ILogger logger)
        {
            base.Initialize(index, size, data, fragments, stringHash, isNewWldFormat, logger);
            Name = stringHash[-Reader.ReadInt32()];

            Setting1 = Reader.ReadInt32();
            Setting2 = Reader.ReadInt32();
            Movement = (ParticleMovementType)Reader.ReadInt32();

            // Behaviour flags occupy 4 bytes with the meaningful bits are in the first byte.
            byte flagsByte = Reader.ReadByte();
            byte flagsPad1 = Reader.ReadByte();
            byte flagsPad2 = Reader.ReadByte();
            byte flagsPad3 = Reader.ReadByte();
            FlagsRaw = flagsByte;
            var flagBits = new BitAnalyzer(flagsByte);
            Flag1 = flagBits.IsBitSet(0);
            HighOpacity = flagBits.IsBitSet(1);
            Flag3 = flagBits.IsBitSet(2);
            FollowsItem = flagBits.IsBitSet(3);

            SimultaneousParticles = Reader.ReadInt32();

            // Five always-zero DWORDs in Trilogy data (libeq speculates gravity/location/bbox reserved)
            Reader.BaseStream.Position += 5 * sizeof(int);

            SpawnRadius = Reader.ReadSingle();
            SpawnAngle = Reader.ReadSingle();
            SpawnLifespanMs = Reader.ReadUInt32();
            SpawnVelocity = Reader.ReadSingle();

            float normalZ = Reader.ReadSingle();
            float normalX = Reader.ReadSingle();
            float normalY = Reader.ReadSingle();
            SpawnNormal = new vec3(normalX, normalY, normalZ);

            SpawnRateMs = Reader.ReadUInt32();
            SpawnScale = Reader.ReadSingle();

            // Tint color stored as BGRX bytes (NOT a float).
            byte tintB = Reader.ReadByte();
            byte tintG = Reader.ReadByte();
            byte tintR = Reader.ReadByte();
            byte tintX = Reader.ReadByte();
            Tint = new Color(tintR, tintG, tintB, tintX);

            ParticleSprite = fragments[Reader.ReadInt32() - 1] as ParticleSprite;
        }
    }
}