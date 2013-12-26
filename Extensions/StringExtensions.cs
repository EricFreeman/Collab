using System.Web;

namespace Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotNullOrEmpty(this string s)
        {
            return !s.IsNullOrEmpty();
        }

        public static string ToFormat(this string s, params object[] paramList)
        {
            return string.Format(s, paramList);
        }

        public static string RemoveBefore(this string s, string find)
        {
            var index = s.IndexOf(find, System.StringComparison.Ordinal);
            var rtn = s.Substring(index, s.Length - index);

            return rtn;
        }

        public static string ToMapPath(this string s)
        {
            return HttpContext.Current.Server.MapPath(s);
        }
    }
}
