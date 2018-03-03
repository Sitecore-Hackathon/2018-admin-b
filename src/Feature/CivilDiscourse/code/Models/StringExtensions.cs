using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AdminB.Feature.CivilDiscourse.Models
{
    public static class StringExtension
    {
        public static string SanitizeToItemName(this string possibleName)
        {
            return SanitizeToItemName(possibleName, Sitecore.Configuration.Settings.InvalidItemNameChars);
        }

        public static string SanitizeToItemName(this string possibleName, char[] invalidCharacters)
        {
            return string.Concat(possibleName.Trim().Split(invalidCharacters));
        }
        public static int CountMatches(this string source, string searchText)
        {

            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(searchText))
                return 0;

            source = source.ToLower();
            searchText = searchText.ToLower();

            int counter = 0;
            int startIndex = -1;
            while ((startIndex = (source.IndexOf(searchText, startIndex + 1))) != -1)
                counter++;
            return counter;
        }

        public static string CapitalizeFirst(this string s)
        {
            bool IsNewSentense = true;
            var result = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (IsNewSentense && char.IsLetter(s[i]))
                {
                    result.Append(char.ToUpper(s[i]));
                    IsNewSentense = false;
                }
                else
                    result.Append(s[i]);

                if (s[i] == '!' || s[i] == '?' || s[i] == '.')
                {
                    IsNewSentense = true;
                }
            }

            return result.ToString();
        }
    }
}