using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Common
{
    public static class Common
    {
        public static int ExtractNumberBeforeUnderscore(string input)
        {
            // Define a regular expression pattern to match the number before the underscore
            string pattern = @"(\d+)_";

            // Create a regex object and find the first match in the input string
            Match match = Regex.Match(input, pattern);

            // Check if a match is found and extract the number
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int number))
                {
                    return number;
                }
            }

            // Return a default value (you may choose a different approach depending on your requirements)
            return -1;
        }
    }
}
