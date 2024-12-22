using System.Linq;
using System.Text.RegularExpressions;

namespace LanternExtractor.EQ
{
    public static class ShortnameHelper
    {
        public static string GetCorrectZoneShortname(string shortName)
        {
            if (char.IsDigit(shortName.Last()))
            {
                if (shortName != "qeynos2" && shortName != "qey2hh1" && !shortName.StartsWith("global"))
                {
                    shortName = Regex.Replace(shortName, @"[\d-]", string.Empty);
                }
            }

            return shortName;
        }
    }
}