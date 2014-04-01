using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QSI.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceCharAt(this string input, int index, char replacementChar)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            char[] inputAsChars = input.ToCharArray();
            inputAsChars[index] = replacementChar;
            return new string(inputAsChars);
        }

        public static bool IsAlphaNumericNoWhiteSpace(this string s)
        {
            Regex regEx = new Regex(@"^[a-zA-Z0-9]*$");
            return regEx.IsMatch(s);
        }
    }
}
