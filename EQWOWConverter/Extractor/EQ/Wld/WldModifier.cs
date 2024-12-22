namespace LanternExtractor.EQ.Wld
{
    public class WldModifier
    {
        /*if (fragId == FragmentType.BspRegion)
{
    _bspRegions.Add(newFragment as BspRegion);
}*/
/*
                long cachedPosition = reader.BaseStream.Position;
                // Create data mods class
                if (_wldType == WldType.Zone && newFragment.Type == FragmentType.Mesh)
                {
                    // Get vertex color count
                    int skip = 18 * 4 + 3 * 2;

                    long colorCountLocation = readPosition + skip;

                    reader.BaseStream.Position = colorCountLocation;

                    int count = reader.ReadInt16();

                    long colorsLocation = readPosition + (newFragment as Mesh).ColorStart;
                    reader.BaseStream.Position = colorsLocation;
                    writer.BaseStream.Position = colorsLocation;

                    Random random = new Random();
                    for (int j = 0; j < count; ++j)
                    {
                        var dwordValue = reader.ReadInt32();
                        var bytesValue = BitConverter.GetBytes(dwordValue);
                        byte amount = 255;//(byte)((random.Next(int.MaxValue) % 2 == 0) ? 0 : 255);
                        bytesValue[3] = amount;
                        writer.Write(bytesValue);
                    }
                }

                reader.BaseStream.Position = cachedPosition;*/

        //_fragmentTypeDictionary[fragId].Add(newFragment);
    }
}