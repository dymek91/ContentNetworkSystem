using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentNetworkSystem
{
    public static class Extensions
    {
        public static IEnumerable<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }
        public static IEnumerable<int> AllIndexesOf(this string str, char value)
        { 
            for (int index = 0; ; index += 1)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }
    }
}
