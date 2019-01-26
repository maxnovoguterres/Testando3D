using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.ECSWindows
{
    public class ObjectInfo
    {
        public string Name { get; set; }
        public float3 Position { get; set; }
        public quaternion Rotation { get; set; }
        public float3 Scale { get; set; }
        public MeshInfo Mesh { get; set; }
        public string MaterialResource { get; set; }
    }
}
