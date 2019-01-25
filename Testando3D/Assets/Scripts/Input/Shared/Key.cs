using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Input.Shared
{
    public class Key
    {
        public static Key KeyNull { get { return new Key { Name = null, Type = null, Bit = 0 }; } }

        public string Name { get; set; }
        public Type Type { get; set; }
        public int Bit { get; set; }
    }

}
