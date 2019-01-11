using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [Serializable]
    public struct Collision : IComponentData
    {
        public float Radius;
        [HideInInspector] public float3 PreviousPos;
    }
    public class CollisionComponent : ComponentDataWrapper<Collision> { }
}
