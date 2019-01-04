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
    public class InputMovementSystem : ComponentSystem
    {
        public struct Player
        {
            public InputComponent inputComponent;
            public MovementComponent movementComponent;
            public Transform transform;
            public Animator animator;
            public Rigidbody rb;
            public CharacterController characterController;
        }
        public float jumpOffset = 0f;

        protected override void OnUpdate()
        {
            var item = GetEntities<Player>()[0];

            if (!item.movementComponent.previouslyGrounded && item.characterController.isGrounded)
            {
                DoBobCycle();
                //PlayLandingSound();
                item.inputComponent.movement.y = 0f;
                item.movementComponent.jumping = false;
            }
            if (!item.characterController.isGrounded && !item.movementComponent.jumping && item.movementComponent.previouslyGrounded)
            {
                item.inputComponent.movement.y = 0f;
            }

            item.movementComponent.previouslyGrounded = item.characterController.isGrounded;

            Vector3 moveDir = new Vector3();

            moveDir = new float3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));

            Vector3 desiredMove = item.transform.forward * moveDir.x + item.transform.right * moveDir.z;
            RaycastHit hitInfo;
            Physics.SphereCast(item.transform.position, item.characterController.radius, Vector3.down, out hitInfo,
                               item.characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            item.movementComponent.isWalking = !Input.GetButton("Sprint");

            if (Input.GetButtonDown("Crouch"))
            {
                item.movementComponent.isCrouching = true;
                item.characterController.center = new Vector3(item.characterController.center.x, 0.465f, item.characterController.center.z);
                item.characterController.height = 0.855f;
            }
            if (Input.GetButtonUp("Crouch"))
            {
                item.movementComponent.isCrouching = false;
                item.characterController.center = new Vector3(item.characterController.center.x, 0.93f, item.characterController.center.z);
                item.characterController.height = 1.71f;
            }
            if (item.movementComponent.isCrouching)
            {
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, new Vector3(Camera.main.transform.localPosition.x, 0.7265f, Camera.main.transform.localPosition.z), Time.deltaTime * 6);
                item.movementComponent.speed = item.movementComponent.crouchSpeed;
            }
            else
            {
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, new Vector3(Camera.main.transform.localPosition.x, 1.453f, Camera.main.transform.localPosition.z), Time.deltaTime * 6);
                item.movementComponent.speed = item.movementComponent.isWalking ? item.movementComponent.walkSpeed : item.movementComponent.runSpeed;
            }

            item.inputComponent.movement.x = desiredMove.x * item.movementComponent.speed;
            item.inputComponent.movement.z = desiredMove.z * item.movementComponent.speed;

            if (item.characterController.isGrounded)
            {
                item.inputComponent.movement.y = Physics.gravity.y;

                if (Input.GetButtonDown("Jump") && !item.movementComponent.jumping)
                {
                    item.inputComponent.movement.y = item.movementComponent.jumpSpeed;
                    //PlayJumpSound();
                    item.movementComponent.jumping = true;
                }
            }
            else
            {
                item.inputComponent.movement += Physics.gravity * 2 * Time.fixedDeltaTime;
            }

            //ProgressStepCycle(speed);

            item.movementComponent.collisionFlags = item.characterController.Move(item.inputComponent.movement * Time.fixedDeltaTime);

            float speedPercent = item.characterController.velocity.magnitude / item.movementComponent.runSpeed;
            item.animator.SetFloat("speedPercent", speedPercent, .1f, Time.deltaTime);

            if (Input.GetButtonDown("Fire1"))
            {
                item.inputComponent.Shoot = true;
            }
            if (Input.GetButtonUp("Fire1"))
            {
                item.inputComponent.Shoot = false;
            }

            if (Input.GetButtonDown("Fire2"))
            {
                item.inputComponent.Aim = true;
            }
            if (Input.GetButtonUp("Fire2"))
            {
                item.inputComponent.Aim = false;
            }
        }

        public void DoBobCycle()
        {
            // make the camera move down slightly
            float t = 0f;
            while (t < 0.2f)
            {
                jumpOffset = Mathf.Lerp(0f, 0.1f, t / 0.2f);
                t += Time.deltaTime;
            }

            // make it move back to neutral
            t = 0f;
            while (t < 0.1f)
            {
                jumpOffset = Mathf.Lerp(0.1f, 0f, t / 0.2f);
                t += Time.deltaTime;
            }
            jumpOffset = 0f;
        }
    }
}
