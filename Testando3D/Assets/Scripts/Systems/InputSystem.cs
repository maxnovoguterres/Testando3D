using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Assets.Scripts.Components;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
    public class InputSystem : ComponentSystem
    {
        public struct Player
        {
            public InputComponent inputComponent;
            public PlayerMovementComponent movementComponent;
            public Transform transform;
            public Animator animator;
            public Rigidbody rb;
            public CharacterController characterController;
        }
        public float playerHeight = 0f;
        public float3 playerCenter = 0f;
        public float cameraY = 0f;
        Player? player;

        protected override void OnUpdate()
        {
            if (player == null)
            {
                player = GetEntities<Player>()[0];
                playerHeight = player.Value.characterController.height;
                playerCenter = player.Value.characterController.center;
                cameraY =Camera.main.transform.position.y;
            }

            if (!player.Value.movementComponent.previouslyGrounded && player.Value.characterController.isGrounded)
            {
                //PlayLandingSound();
                player.Value.inputComponent.movement.y = 0f;
                player.Value.movementComponent.jumping = false;
            }
            if (!player.Value.characterController.isGrounded && !player.Value.movementComponent.jumping && player.Value.movementComponent.previouslyGrounded)
            {
                player.Value.inputComponent.movement.y = 0f;
            }

            player.Value.movementComponent.previouslyGrounded = player.Value.characterController.isGrounded;

            Vector3 moveDir = new Vector3();

            moveDir = new float3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));

            Vector3 desiredMove = player.Value.transform.forward * moveDir.x + player.Value.transform.right * moveDir.z;
            RaycastHit hitInfo;
            Physics.SphereCast(player.Value.transform.position, player.Value.characterController.radius, Vector3.down, out hitInfo,
                               player.Value.characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            player.Value.movementComponent.isWalking = !Input.GetButton("Sprint");

            if (Input.GetButtonDown("Crouch"))
            {
                player.Value.movementComponent.isCrouching = true;
                player.Value.characterController.center = new Vector3(playerCenter.x, playerCenter.y / 2, playerCenter.z);
                player.Value.characterController.height = playerHeight / 2;
            }
            if (Input.GetButtonUp("Crouch"))
            {
                player.Value.movementComponent.isCrouching = false;
                player.Value.characterController.center = playerCenter;
                player.Value.characterController.height = playerHeight;
            }
            if (player.Value.movementComponent.isCrouching)
            {
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, new Vector3(Camera.main.transform.localPosition.x, cameraY / 2, Camera.main.transform.localPosition.z), Time.deltaTime * 6);
                player.Value.movementComponent.speed = player.Value.movementComponent.crouchSpeed;
            }
            else
            {
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, new Vector3(Camera.main.transform.localPosition.x, cameraY, Camera.main.transform.localPosition.z), Time.deltaTime * 6);
                player.Value.movementComponent.speed = player.Value.movementComponent.isWalking ? player.Value.movementComponent.walkSpeed : player.Value.movementComponent.runSpeed;
            }

            player.Value.inputComponent.movement.x = desiredMove.x * player.Value.movementComponent.speed;
            player.Value.inputComponent.movement.z = desiredMove.z * player.Value.movementComponent.speed;

            if (player.Value.characterController.isGrounded)
            {
                player.Value.inputComponent.movement.y = Physics.gravity.y;

                if (Input.GetButtonDown("Jump") && !player.Value.movementComponent.jumping)
                {
                    player.Value.inputComponent.movement.y = player.Value.movementComponent.jumpSpeed;
                    //PlayJumpSound();
                    player.Value.movementComponent.jumping = true;
                }
            }
            else
            {
                player.Value.inputComponent.movement += Physics.gravity * 2 * Time.fixedDeltaTime;
            }

            player.Value.characterController.Move(player.Value.inputComponent.movement * Time.fixedDeltaTime);

            //ProgressStepCycle(speed);

            float speedPercent = player.Value.characterController.velocity.magnitude / player.Value.movementComponent.runSpeed;
            player.Value.animator.SetFloat("speedPercent", speedPercent, .1f, Time.deltaTime);

            if (Input.GetButtonDown("Fire1"))
            {
                player.Value.inputComponent.Shoot = true;
            }
            if (Input.GetButtonUp("Fire1"))
            {
                player.Value.inputComponent.Shoot = false;
            }

            if (Input.GetButtonDown("Fire2"))
            {
                player.Value.inputComponent.Aim = true;
            }
            if (Input.GetButtonUp("Fire2"))
            {
                player.Value.inputComponent.Aim = false;
            }
        }
    }
}
