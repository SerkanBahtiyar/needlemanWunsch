using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biyoinformatik
{
    public static class reverse
    {
        public static string Reverse(this string gelen)
        {
            char[] sb = gelen.ToCharArray();
            Array.Reverse(sb);
            return new string(sb);
        }
    }
}
