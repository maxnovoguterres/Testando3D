using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class InputComponent : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 movement;
        public bool Shoot;
        //public float speed;
    }
}
