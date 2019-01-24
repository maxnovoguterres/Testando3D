using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Components
{
    [Serializable]
    public struct _Player : IComponentData
    {
        public int PlayerLocalID;
        public float playerHeight;
        public float3 playerCenter;
        public float cameraY;
    }
    public class PlayerComponent : ComponentDataWrapper<_Player> { }
}
