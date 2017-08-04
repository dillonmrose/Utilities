using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Tools
{
    class AscendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return x.CompareTo(y);
        }
    }
    class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return y.CompareTo(x);
        }
    }

    public class StringLengthComparer : IComparer<String>
    {
        public int Compare(String x, String y)
        {
            int diff = y.Length - x.Length;
            if (diff == 0)
            {
                return x.CompareTo(y);
            }
            return diff;
        }
    }

    class Utils
    {
        public static Dictionary<string, int> getColIndexMap(string header)
        {
            return getColIndexMap(header, '\t');
        }
        public static Dictionary<string, int> getColIndexMap(string header, char delimiter)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();
            string[] headerSplit = header.Split(delimiter);
            for (int i = 0; i < headerSplit.Length; i++)
            {
                map[headerSplit[i]] = i;
            }
            return map;
        }
    }
    public class StringUtils
    {
        public static string RemovePunctuation(string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        
        public static string Normalize(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return "";
            }
            string normalizedLine = line;

            normalizedLine = RemovePunctuation(normalizedLine);

            normalizedLine = RemoveDiacritics(normalizedLine);

            normalizedLine = normalizedLine.ToLower(); // TODO consider -- use appropriate culture

            normalizedLine = normalizedLine.Replace("º", "º "); // note: if space was unnecessary, will get removed in next steps

            normalizedLine = Regex.Replace(normalizedLine, @"\s+", " ");

            normalizedLine = normalizedLine.Trim();

            return normalizedLine;
        }
    }
}
