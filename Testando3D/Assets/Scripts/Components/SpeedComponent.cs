using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Entities;

namespace Assets.Scripts.Components
{
    [Serializable]
    public struct _SpeedComponent : IComponentData
    {
        public float Value;

    }
    public class SpeedComponent : ComponentDataWrapper<_SpeedComponent> { }

}
