using System.IO;
using System.Text;

namespace LanternExtractor.Infrastructure
{
    public static class ExtensionMethods
    {
        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            StringBuilder builder = new StringBuilder();
            char nextCharacter;
            while ((nextCharacter = reader.ReadChar()) != 0)
            {
                builder.Append(nextCharacter);
            }
            return builder.ToString();
        }
    }
}
