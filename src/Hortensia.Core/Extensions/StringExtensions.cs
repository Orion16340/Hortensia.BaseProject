using Hortensia.Core.Threads;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Hortensia.Core.Extensions
{
    public static class StringExtensions
    {
        public static string DeleteLastLetter(this string str) => str.Length < 2 ? str : str[0..^1];
        public static int CountOccurences(this string str, char chr) => str.CountOccurences(chr, 0, str.Length);
        public static string EscapeString(this string str) => str == null ? null : Regex.Replace(str, "[\\r\\n\\x00\\x1a\\\\'\"]", "\\$0");
        public static string HtmlEntities(this string str) => str.Replace("&", "&amp").Replace("<", "&lt").Replace(">", "&gt");

        private static int CountOccurences(this string str, char chr, int startIndex, int count)
        {
            int occurences = 0;

            for (int i = startIndex; i < startIndex + count; i++)
                if (str[i] == chr)
                    occurences++;

            return occurences;
        }

        public static string FirstLetterUpper(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            char[] chArray = str.ToCharArray();
            chArray[0] = char.ToUpper(chArray[0]);

            return new string(chArray);
        }

        public static string UpperAfterChar(this string @string, char @char)
        {
            var stringBuilder = new StringBuilder(@string.Length);
            bool flag = true;
            string str = @string;

            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];

                if (!flag)
                    stringBuilder.Append(chr);
                else
                    stringBuilder.Append(char.ToUpper(chr));

                flag = false;

                if (chr == @char)
                    flag = true;
            }
            return stringBuilder.ToString();
        }

        public static string RandomString(this Random random, int size)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < size; i++)
            {
                //26 letters in the alphabet, ascii + 65 for the capital letters
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))));
            }
            return builder.ToString();
        }

        public static string RandomName()
        {
            string[] syllables =
                {
                        "la", "le", "ly", "lu", "li", "lo",
                        "za", "ze", "zy", "zu", "zi", "zo",
                        "ra", "re", "ry", "ru", "ri", "ro",
                        "ta", "te", "ty", "tu", "ti", "to",
                        "pa", "pe", "py", "pu", "pi", "po",
                        "qa", "qe", "qy", "qu", "qi", "qo",
                        "sa", "se", "sy", "su", "si", "so",
                        "da", "de", "dy", "du", "di", "do",
                        "fa", "fe", "fy", "fu", "fi", "fo",
                        "ga", "ge", "gy", "gu", "gi", "go",
                        "ha", "he", "hy", "hu", "hi", "ho",
                        "ja", "je", "jy", "ju", "ji", "jo",
                        "ka", "ke", "ky", "ku", "ki", "ko",
                        "na", "ne", "ny", "nu", "ni", "no",
                        "ma", "me", "my", "mu", "mi", "mo",
                        "ca", "ce", "cy", "cu", "ci", "co",
                        "ba", "be", "by", "bu", "bi", "bo",
                        "va", "ve", "vy", "vu", "vi", "vo",
                        "xa", "xe", "xy", "xu", "xi", "xo",
                };
            AsyncRandom random = new();
            string result = "";

            for (int i = 0; i < random.Next(2, 4); i++)
                result += syllables[random.Next(0, syllables.Length)];

            if (random.Next(2) == 0)
            {
                result += "-";

                for (int i = 0; i < random.Next(1, 3); i++)
                    result += syllables[random.Next(0, syllables.Length)];
            }
            result = char.ToUpper(result[0]) + result[1..];
            return result;
        }
    }
}
