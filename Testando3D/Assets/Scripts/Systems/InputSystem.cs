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
            public ComponentArray<PlayerMovementComponent> movementComponent;
            public ComponentArray<Transform> transform;
            public ComponentArray<Animator> animator;
            public ComponentArray<Rigidbody> rb;
            public ComponentArray<CharacterController> characterController;
            public ComponentDataArray<_Input> inputComponent;
            public EntityArray Entities;
            public readonly int Length;
        }
        [Inject] Player player;

        public float playerHeight = 0f;
        public float3 playerCenter = 0f;
        public float cameraY = 0f;

        protected override void OnUpdate()
        {
            if (player.Length == 0) return;

            if (playerHeight == 0)
            {
                playerHeight = player.characterController[0].height;
                playerCenter = player.characterController[0].center;
                cameraY =Camera.main.transform.position.y;
            }

            var _player = player;
            var playerInput = _player.inputComponent[0];

            if (!_player.movementComponent[0].previouslyGrounded && _player.characterController[0].isGrounded)
            {
                //PlayLandingSound();
                playerInput.movement.y = 0f;
                _player.movementComponent[0].jumping = false;
            }
            if (!_player.characterController[0].isGrounded && !_player.movementComponent[0].jumping && _player.movementComponent[0].previouslyGrounded)
            {
                playerInput.movement.y = 0f;
            }

            _player.movementComponent[0].previouslyGrounded = _player.characterController[0].isGrounded;

            Vector3 moveDir = new Vector3();
            var ver = Input.GetAxis("Vertical");
            var hor = Input.GetAxis("Horizontal");
            moveDir = new float3(ver, 0, hor);

            Vector3 desiredMove = _player.transform[0].forward * moveDir.x + _player.transform[0].right * moveDir.z;
            RaycastHit hitInfo;
            Physics.SphereCast(_player.transform[0].position, _player.characterController[0].radius, Vector3.down, out hitInfo,
                               _player.characterController[0].height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            var wal = ver != 0 || hor != 0;
            var spr = Input.GetButton("Sprint");
            _player.movementComponent[0].isWalking = wal && !spr;
            _player.movementComponent[0].isRunning = wal && spr;


            if (Input.GetButtonDown("Crouch"))
            {
                _player.movementComponent[0].isCrouching = true;
                _player.characterController[0].center = new Vector3(playerCenter.x, playerCenter.y / 2, playerCenter.z);
                _player.characterController[0].height = playerHeight / 2;
            }
            if (Input.GetButtonUp("Crouch"))
            {
                _player.movementComponent[0].isCrouching = false;
                _player.characterController[0].center = playerCenter;
                _player.characterController[0].height = playerHeight;
            }
            if (_player.movementComponent[0].isCrouching)
            {
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, new Vector3(Camera.main.transform.localPosition.x, cameraY / 2, Camera.main.transform.localPosition.z), Time.deltaTime * 6);
                _player.movementComponent[0].speed = _player.movementComponent[0].crouchSpeed;
            }
            else
            {
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, new Vector3(Camera.main.transform.localPosition.x, cameraY, Camera.main.transform.localPosition.z), Time.deltaTime * 6);
                _player.movementComponent[0].speed = _player.movementComponent[0].isWalking ? _player.movementComponent[0].walkSpeed : _player.movementComponent[0].runSpeed;
            }

            playerInput.movement.x = desiredMove.x * _player.movementComponent[0].speed;
            playerInput.movement.z = desiredMove.z * _player.movementComponent[0].speed;

            if (_player.characterController[0].isGrounded)
            {
                playerInput.movement.y = Physics.gravity.y;

                if (Input.GetButtonDown("Jump") && !_player.movementComponent[0].jumping)
                {
                    playerInput.movement.y = _player.movementComponent[0].jumpSpeed;
                    //PlayJumpSound();
                    _player.movementComponent[0].jumping = true;
                }
            }
            else
            {
                playerInput.movement += new float3(0, -9.81f, 0) * 2 * Time.fixedDeltaTime;
            }

            _player.characterController[0].Move(playerInput.movement * Time.fixedDeltaTime);

            //ProgressStepCycle(speed);

            float speedPercent = _player.characterController[0].velocity.magnitude / _player.movementComponent[0].runSpeed;
            _player.animator[0].SetFloat("speedPercent", speedPercent, .1f, Time.deltaTime);

            if (Input.GetButtonDown("Fire1") && playerInput.isReloading == 0)
            {
                playerInput.shoot = 1;
            }
            if (Input.GetButtonUp("Fire1"))
            {
                playerInput.shoot = 0;
            }

            if (Input.GetButtonDown("Fire2") && playerInput.isReloading == 0)
            {
                playerInput.aim = 1;
            }
            if (Input.GetButtonUp("Fire2"))
            {
                playerInput.aim = 0;
            }

            if (Input.GetKeyDown(KeyCode.R) && playerInput.aim == 0)
            {
                playerInput.isReloading = 1;
            }

            _player.inputComponent[0] = playerInput;
            player = _player;
            //Debug.Log(player.inputComponent[0].isReloading);
        }
    }
}
