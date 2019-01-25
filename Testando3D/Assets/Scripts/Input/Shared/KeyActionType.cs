using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.Input.Controls;

namespace Assets.Scripts.Input.Shared
{
    public class KeyActionType
    {
        public Key Keyboard { get; set; }
        public Key Gamepad { get; set; }
        public Key Mouse { get; set; }

        public KeyActionType(Key k, Key g, Key m)
        {
            Keyboard = k;
            Gamepad = g;
            Mouse = m;
        }
    }
}
