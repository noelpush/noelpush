namespace NoelPush.Objects.ViewModel
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;


    namespace Itron.Mcn.Utils.Extension
    {
        /// <summary>
        /// Utility class that contains extensions method for strings
        /// </summary>
        public static partial class StringExtension
        {
            private static readonly Regex cutter = new Regex("`", RegexOptions.Compiled);

            /// <summary>
            /// Gets the number of digits contained whithin the given string
            /// </summary>
            /// <param name="value">The string to count the digits from</param>
            /// <returns>The number of digits found in the given string</returns>
            public static int GetNumberOfDigits(this string value)
            {
                return (from character in value where char.IsDigit(character) select character).Count();
            }

            /// <summary>
            /// Remove the generic part from the given string (which should be a class name)
            /// </summary>
            /// <param name="value">The string to clean</param>
            /// <returns>The cleaned string from the generic part</returns>
            public static string RemoveGenericPart(this string value)
            {
                return cutter.Split(value)[0];
            }

            /// <summary>
            /// Add n times the given character to the string (same as FillWithCharacter(characters, number, append) with append setted to false
            /// </summary>
            /// <param name="value">The string to modify</param>
            /// <param name="characterToFill">The character to add</param>
            /// <param name="numberOfTimes">The number of characters to add</param>
            /// <returns>A new string with the number of characters added to the value</returns>
            public static string FillWithCharacter(this string value, char characterToFill, int numberOfTimes)
            {
                return new StringBuilder().Append(characterToFill, numberOfTimes).Append(value).ToString();
            }

            /// <summary>
            /// Add n times the given character to the string
            /// </summary>
            /// <param name="value">The string to modify</param>
            /// <param name="characterToFill">The character to add</param>
            /// <param name="numberOfTimes">The number of characters to add</param>
            /// <param name="append">true to append the characters to the strings otherwise false</param>
            /// <returns>A new string with the number of characters added to the value</returns>
            public static string FillWithCharacter(this string value, char characterToFill, int numberOfTimes, bool append)
            {
                return append ?
                    new StringBuilder().Append(value).Append(characterToFill, numberOfTimes).ToString() :
                    value.FillWithCharacter(characterToFill, numberOfTimes);
            }

            /// <summary>
            /// Convert a string to a byte array reprensentation
            /// </summary>
            /// <param name="value">the hexadecimal values as string</param>
            /// <returns>the hexadecimal values as string byte array reprensentation</returns>
            public static byte[] GetByteValue(this string value)
            {
                if (value.Length % 2 != 0)
                    value = "0" + value;

                var result = new List<byte>(value.Length / 2);

                for (int i = 0; i < value.Length; i++)
                {
                    var byteAsString = value[i] + value[i + 1].ToString();
                    var byteValue = byte.Parse(byteAsString, NumberStyles.HexNumber);

                    result.Add(byteValue);
                    i++;
                }

                return result.ToArray();
            }
        }
    }

}
