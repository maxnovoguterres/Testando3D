using Assets.Scripts.ECSWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace Assets.Scripts.PureECS
{
    public static class SceneObjectsData
    {
        public static List<float3> position = new List<float3>
        {
            new float3(11.60457f, 1.254109f, -0.6889542f), new float3(11.49394f, 0.6280944f, -0.8644481f), new float3(13.222f, 0.3717006f, -2.159503f), new float3(14.4f, 0.71f, -5.6541f), new float3(13.3391f, 0.399f, -6.11f)
        };

        public static List<quaternion> rotation = new List<quaternion>
        {
            new quaternion(-0.7071068f, 0f, 0f, 0.7071068f), new quaternion(-0.7071068f, 0f, 0f, 0.7071068f), new quaternion(0.5f, 0.5f, 0.5f, -0.5f), new quaternion(0f, 0f, 0f, 1f), new quaternion(0f, 0f, 0f, 1f)

        };

        public static List<float3> scale = new List<float3>
        {
            new float3(0.6666667f, 0.6666668f, 0.6666668f), new float3(0.6666667f, 0.6666668f, 0.6666668f), new float3(0.3180982f, 0.2981745f, 0.03333334f), new float3(0.5823437f, 0.5823437f, 0.5823437f), new float3(0.5424f, 0.5424f, 0.5424f)
        };

        public static List<MeshInfo> meshResource = new List<MeshInfo>
        {
            new MeshInfo { PrefabName = "Mesa", MeshName = "Plane"}, new MeshInfo { PrefabName = "Mesa", MeshName = "Plane_001"}, new MeshInfo { PrefabName = "cadeira", MeshName = "Cube_001"}, new MeshInfo { PrefabName = "fogao2", MeshName = "Cube"}, new MeshInfo { PrefabName = "pia2", MeshName = "Cube"}
        };

        public static List<string> materialResource = new List<string>
        {
            "panoMesaMat", "mesaMat", "cadeiraMat", "Fogao.Mat", "Pia2Mat"
        };
    }
}
