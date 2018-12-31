using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [Serializable]
    public struct Gravity : IComponentData
    {
        public float Mass;
        [HideInInspector] public float InitPosY;
        [HideInInspector] public float InitVel;
        [HideInInspector]public float Time;
        //public float Vel;

    }
    public class GravityComponent : ComponentDataWrapper<Gravity> { }
}
