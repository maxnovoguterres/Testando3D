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
        public float distance;
        public BoxCollider boxCollider;
        public Equipment equipment;
    }
}
