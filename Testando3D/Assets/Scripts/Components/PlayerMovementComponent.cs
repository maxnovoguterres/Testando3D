using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [Serializable]
    public struct PlayerMovement : IComponentData
    {
        public float walkSpeed;
        public float runSpeed;
        public float crouchSpeed;
        public float speed;
        public float jumpSpeed;
        [HideInInspector] public byte jumping;
        [HideInInspector] public byte previouslyGrounded;
        [HideInInspector] public byte isWalking;
        [HideInInspector] public byte isRunning;
        [HideInInspector] public byte isCrouching;
    }
    public class PlayerMovementComponent : ComponentDataWrapper<PlayerMovement> { }
}
