using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class PlayerMovementComponent : MonoBehaviour
    {
        public float walkSpeed;
        public float runSpeed;
        public float crouchSpeed;
        public float speed;
        public float jumpSpeed;
        [HideInInspector] public bool jumping;
        [HideInInspector] public bool previouslyGrounded;
        [HideInInspector] public bool isWalking;
        [HideInInspector] public bool isCrouching;
    }
}
