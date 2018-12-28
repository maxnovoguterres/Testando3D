﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class MovementComponent : MonoBehaviour
    {
        public float speed;
        public float jumpSpeed;
        public bool jumping;
        public CollisionFlags collisionFlags;
        public bool previouslyGrounded;
    }
}