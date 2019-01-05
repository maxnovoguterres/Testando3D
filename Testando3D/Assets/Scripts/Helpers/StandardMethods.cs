using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class StandardMethods : MonoBehaviour
    {
        public static void _Destroy(GameObject gameObject)
        {
            Destroy(gameObject);
        }
    }
}
