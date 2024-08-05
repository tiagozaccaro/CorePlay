using System.Text.RegularExpressions;

namespace CorePlay.SDK.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveTrademarks(this string str, string remplacement = "")
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            return Regex.Replace(str, @"[™©®]", remplacement);
        }
    }
}
