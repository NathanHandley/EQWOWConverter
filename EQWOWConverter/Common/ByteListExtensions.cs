// Claude Code (Opus) actually wrote this on 7/8 to help support some performance tuning activities

namespace EQWOWConverter.Common
{
    // Appends little-endian primitives directly to a byte list without the per-value throwaway array that
    // BitConverter.GetBytes(...) + AddRange allocates. Byte layout is identical to BitConverter.GetBytes on
    // this (little-endian) platform. Used on the hottest serialization paths (M2 animation tracks) to cut
    // GC pressure. stackalloc keeps the scratch buffer off the heap.
    internal static class ByteListExtensions
    {
        public static void AddSingleLE(this List<byte> list, float value)
        {
            Span<byte> tmp = stackalloc byte[4];
            BitConverter.TryWriteBytes(tmp, value);
            list.Add(tmp[0]); list.Add(tmp[1]); list.Add(tmp[2]); list.Add(tmp[3]);
        }

        public static void AddUInt16LE(this List<byte> list, ushort value)
        {
            list.Add((byte)(value & 0xFF));
            list.Add((byte)((value >> 8) & 0xFF));
        }

        public static void AddUInt32LE(this List<byte> list, uint value)
        {
            list.Add((byte)(value & 0xFF));
            list.Add((byte)((value >> 8) & 0xFF));
            list.Add((byte)((value >> 16) & 0xFF));
            list.Add((byte)((value >> 24) & 0xFF));
        }
    }
}
