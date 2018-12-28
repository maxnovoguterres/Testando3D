using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Entities;

namespace Assets.Scripts.Components
{
    public class GunComponent : MonoBehaviour
    {
        public CountDown countDown;
        public GameObject player;
        public Transform bocal;
    }
}
