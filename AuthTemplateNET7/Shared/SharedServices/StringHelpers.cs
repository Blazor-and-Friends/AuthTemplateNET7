using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.SharedServices;

//added
public static class StringHelpers
{
    static Random random;
    static Regex regex;

    /// <summary>
    /// Generates a random alphabetical string from upper and lowercase letters.
    /// </summary>
    /// <param name="length">How long the string should be</param>
    /// <returns>A random string</returns>
    public static string GenerateRandomString(int length = 7)
    {
        if (random == null) random = new Random();

        char[] result = new char[length];

        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        int lettersCount = letters.Length;

        for (int i = 0; i < length; i++)
        {
            result[i] = letters[random.Next(lettersCount)];
        }

        return new string(result);
    }

    public static string Shorten(this string s, int maxLength, bool addElipses = true)
    {
        if (string.IsNullOrEmpty(s) || s.Length <= maxLength) return s;

        if (addElipses) return s.Substring(0, maxLength - 3) + "...";
        return s.Substring(0, maxLength);
    }

    /// <summary>
    /// Turns property names into words, e.g. FirstName becomes First Name
    /// </summary>
    /// <param name="s">The string to convert</param>
    /// <returns>A string with a spce before any uppercase character (except the firt letter)</returns>
    public static string TitleCaseToWords(this string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        if (regex == null)
        {
            //courtesy https://stackoverflow.com/a/4489046/2816057
            regex = new Regex(@"
                    (?<=[A-Z])(?=[A-Z][a-z]) |
                    (?<=[^A-Z])(?=[A-Z]) |
                    (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
        }

        return regex.Replace(s, " ");
    }
}
