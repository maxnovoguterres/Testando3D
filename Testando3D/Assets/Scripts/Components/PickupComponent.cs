using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class PickupComponent : MonoBehaviour
    {
        //public Vector3 center;
        //public Vector3 halfExtents;
        public BoxCollider boxCollider;
        public Quaternion orientation;
        public Vector3 direction;
        public Equipment equipment;
    }
}
