using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public static class EnumUtilities
    {
        public static int[] GetEnumNumber<T>(this List<T> me)
        {
            var data = new int[3];
            for (var i = 0; i < me.Count; i++)
                data[i] = int.Parse(me[i].ToString());

            return data;
        }
    }
}
