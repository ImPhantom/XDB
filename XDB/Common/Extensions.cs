using System;

namespace XDB.Common
{
    public static class Extensions
    {
        private const long kb = 1024;
        private const long mb = kb * 1024;
        private const long gb = mb * 1024;
        private const long tb = gb * 1024;

        public static string ToFormalSize(this int value, int places = 0)
        {
            return ((long)value).ToFormalSize(places);
        }

        public static string ToFormalSize(this long value, int places = 0)
        {
            var asTb = Math.Round((double)value / tb, places);
            var asGb = Math.Round((double)value / gb, places);
            var asMb = Math.Round((double)value / mb, places);
            var asKb = Math.Round((double)value / kb, places);
            string chosenValue = asTb > 1 ? string.Format("{0}TB", asTb)
                : asGb > 1 ? string.Format("{0}GB", asGb)
                : asMb > 1 ? string.Format("{0}MB", asMb)
                : asKb > 1 ? string.Format("{0}KB", asKb)
                : string.Format("{0}B", Math.Round((double)value, places));
            return chosenValue;
        }

        public static string Replace(this string s, char[] separators, string newVal)
        {
            string[] temp;

            temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(newVal, temp);
        }

        
    }
}
