using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Input;

namespace Assets.Scripts.Components
{
    //[Serializable]
    public class PlayerComponent : MonoBehaviour//: IComponentData
    {
        public int PlayerLocalID;
        public float playerHeight;
        public float3 playerCenter;
        public float cameraY;
        public Gamepad gamepad = null;
        public Keyboard keyboard = null;
        public Mouse mouse = null;
    }
    //public class PlayerComponent : ComponentDataWrapper<_Player> { }
}
