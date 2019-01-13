using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Assets.Scripts.Components
{
    public struct Scene : IComponentData
    {
    }
    public class SceneComponent : ComponentDataWrapper<Scene>{ }
}
