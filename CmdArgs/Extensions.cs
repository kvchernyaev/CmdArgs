#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    internal static class Extensions
    {
        public static void SplitPairByEquality(this string s, out string name, out string value)
        {
            if (s == null)
            {
                name = value = null;
                return;
            }
            int ieq = s.IndexOf("=");
            if (ieq < 0)
            {
                name = s;
                value = null;
            }
            else
            {
                name = s.Substring(0, ieq);
                value = s.Substring(ieq + 1);
            }
        }
    }
}