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
    public struct DestroyAfterTime : IComponentData
    {
        public float lifeTime;
        public float timeToDestroy;
        public bool delete { get { return lifeTime >= timeToDestroy; } }
    }
    public class DestroyAfterTimeComponent : ComponentDataWrapper<DestroyAfterTime> { }
}