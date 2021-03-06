﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Entities;
using Assets.Scripts.Helpers;
using Unity.Mathematics;

namespace Assets.Scripts.Components
{
    [Serializable]
    public struct Speed : IComponentData
    {
        public float Value;
    }
    public class SpeedComponent : ComponentDataWrapper<Speed> { }

}
