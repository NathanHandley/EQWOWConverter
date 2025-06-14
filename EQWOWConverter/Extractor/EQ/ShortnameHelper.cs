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
                switch (shortName)
                {
                    case "load2":
                    case "qey2hh1":
                    case "qeynos2":
                    case var s when s.StartsWith("global"):
                        return shortName;
                    default:
                        shortName = Regex.Replace(shortName, @"[\d-]", string.Empty);
                        break;
                }
            }

            return shortName;
        }
    }
}