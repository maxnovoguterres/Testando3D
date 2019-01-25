using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;

namespace Assets.Scripts.Components
{
    [Serializable]
    public struct _Input : IComponentData
    {
        [HideInInspector]
        public float3 movement;
        public byte shoot;
        public byte aim;
        public byte isReloading;
        public byte crouch;
    }

    public class InputComponent : ComponentDataWrapper<_Input> { }
}
