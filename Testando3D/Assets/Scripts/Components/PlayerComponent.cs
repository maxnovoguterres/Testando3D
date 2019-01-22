﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Assets.Scripts.Components
{
    [Serializable]
    public struct _Player : IComponentData
    {
        public int PlayerLocalID;
    }
    public class PlayerComponent : ComponentDataWrapper<_Player> { }
}
