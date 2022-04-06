using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSNTools.Utils
{
    public static class Extensions
    {
        public static bool Contains(this string[] vs, string name)
        {
            foreach (string str in vs)
            {
                if (str == name)
                    return true;
            }
            return false;
        }
    }
}
