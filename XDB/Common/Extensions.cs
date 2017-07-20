using System;
using System.Collections.Generic;
using System.Text;

namespace XDB.Common
{
    public static class Extensions
    {
        public static string Replace(this string s, char[] separators, string newVal)
        {
            string[] temp;

            temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(newVal, temp);
        }
    }
}
