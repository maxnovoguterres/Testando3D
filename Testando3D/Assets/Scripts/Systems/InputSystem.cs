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
using Assets.Scripts.Input.Shared;

namespace Assets.Scripts.Systems
{
    public class InputSystem : ComponentSystem
    {
        public struct Player
        {
            public ComponentDataArray<PlayerMovement> movementComponent;
            public ComponentArray<PlayerComponent> playerComponent;
            public ComponentArray<Transform> transform;
            public ComponentArray<Animator> animator;
            public ComponentArray<Rigidbody> rb;
            public ComponentArray<CharacterController> characterController;
            public ComponentDataArray<_Input> inputComponent;
            public EntityArray Entities;
            public readonly int Length;
        }
        [Inject] Player player;

        protected override void OnUpdate()
        {
            for (int i = 0; i < player.Length; i++)
            {
                if (player.Length == 0) return;

                var _player = player;
                //var player.playerComponent[i] = _player.player.playerComponent[i][i];

                if (player.playerComponent[i].playerHeight == 0)
                {
                    player.playerComponent[i].playerHeight = player.characterController[i].height;
                    player.playerComponent[i].playerCenter = player.characterController[i].center;
                    player.playerComponent[i].cameraY = player.transform[i].Find("FirstPersonCamera").transform.position.y;
                }

                var playerInput = _player.inputComponent[i];
                var playerMovement = _player.movementComponent[i];

                if (_player.movementComponent[i].previouslyGrounded == 0 && _player.characterController[i].isGrounded)
                {
                    //PlayLandingSound();
                    playerInput.movement.y = 0f;
                    playerMovement.jumping = 0;
                }
                if (!_player.characterController[i].isGrounded && _player.movementComponent[i].jumping == 0 && _player.movementComponent[i].previouslyGrounded == 1)
                {
                    playerInput.movement.y = 0f;
                }

                playerMovement.previouslyGrounded = (byte)(_player.characterController[i].isGrounded ? 1 : 0);

                Vector3 moveDir = new Vector3();
                //var ver = Input.GetAxis("Vertical");
                //var hor = Input.GetAxis("Horizontal");
                var ver = player.playerComponent[i].GetAxis("Vertical");
                var hor = player.playerComponent[i].GetAxis("Horizontal");
                moveDir = new float3(ver, 0, hor);

                Vector3 desiredMove = _player.transform[i].forward * moveDir.x + _player.transform[i].right * moveDir.z;
                RaycastHit hitInfo;
                Physics.SphereCast(_player.transform[i].position, _player.characterController[i].radius, Vector3.down, out hitInfo,
                                   _player.characterController[i].height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                var wal = ver != 0 || hor != 0;
                //var spr = Input.GetButton("Sprint");
                //var spr = NewInputManager.kb.leftShiftKey.isPressed;
                var spr = player.playerComponent[i].GetButton("Run");
                playerMovement.isWalking = (byte)(wal && !spr ? 1 : 0);
                playerMovement.isRunning = (byte)(wal && spr ? 1 : 0);


                //if (Input.GetButtonDown("Crouch"))
                if (player.playerComponent[i].GetButtonDown("Crouch"))
                {
                    playerMovement.isCrouching = 1;
                    _player.characterController[i].center = new Vector3(player.playerComponent[i].playerCenter.x, player.playerComponent[i].playerCenter.y / 2, player.playerComponent[i].playerCenter.z);
                    _player.characterController[i].height = player.playerComponent[i].playerHeight / 2;
                }
                //if (Input.GetButtonUp("Crouch"))
                if (player.playerComponent[i].GetButtonUp("Crouch"))
                {
                    playerMovement.isCrouching = 0;
                    _player.characterController[i].center = player.playerComponent[i].playerCenter;
                    _player.characterController[i].height = player.playerComponent[i].playerHeight;
                }
                if (_player.movementComponent[i].isCrouching == 1)
                {
                    player.transform[i].Find("FirstPersonCamera").transform.localPosition = Vector3.Lerp(player.transform[i].Find("FirstPersonCamera").transform.localPosition, new Vector3(player.transform[i].Find("FirstPersonCamera").transform.localPosition.x, player.playerComponent[i].cameraY / 2, player.transform[i].Find("FirstPersonCamera").transform.localPosition.z), Time.deltaTime * 6);
                    playerMovement.speed = _player.movementComponent[i].crouchSpeed;
                }
                else
                {
                    player.transform[i].Find("FirstPersonCamera").transform.localPosition = Vector3.Lerp(player.transform[i].Find("FirstPersonCamera").transform.localPosition, new Vector3(player.transform[i].Find("FirstPersonCamera").transform.localPosition.x, player.playerComponent[i].cameraY, player.transform[i].Find("FirstPersonCamera").transform.localPosition.z), Time.deltaTime * 6);
                    playerMovement.speed = _player.movementComponent[i].isWalking == 1 ? _player.movementComponent[i].walkSpeed : _player.movementComponent[i].runSpeed;
                }

                playerInput.movement.x = desiredMove.x * _player.movementComponent[i].speed;
                playerInput.movement.z = desiredMove.z * _player.movementComponent[i].speed;

                if (_player.characterController[i].isGrounded)
                {
                    playerInput.movement.y = Physics.gravity.y;

                    //if (Input.GetButtonDown("Jump") && _player.movementComponent[i].jumping == 0)
                    if (player.playerComponent[i].GetButtonDown("Jump") && _player.movementComponent[i].jumping == 0)
                    {
                        playerInput.movement.y = _player.movementComponent[i].jumpSpeed;
                        //PlayJumpSound();
                        playerMovement.jumping = 1;
                    }
                }
                else
                {
                    playerInput.movement += new float3(0, -9.81f, 0) * 2 * Time.fixedDeltaTime;
                }

                _player.characterController[i].Move(playerInput.movement * Time.fixedDeltaTime);

                //ProgressStepCycle(speed);

                float speedPercent = _player.characterController[i].velocity.magnitude / _player.movementComponent[i].runSpeed;
                _player.animator[i].SetFloat("speedPercent", speedPercent, .1f, Time.deltaTime);

                //if (Input.GetButtonDown("Fire1") && playerInput.isReloading == 0)
                if (player.playerComponent[i].GetButton("Fire"))
                {
                    playerInput.shoot = 1;
                }
                //if (Input.GetButtonUp("Fire1"))
                if (player.playerComponent[i].GetButtonUp("Fire"))
                {
                    playerInput.shoot = 0;
                }

                //if (Input.GetButtonDown("Fire2") && playerInput.isReloading == 0)
                if (player.playerComponent[i].GetButtonDown("Aim"))
                {
                    Debug.Log("mirando");
                    playerInput.aim = 1;
                }
                //if (Input.GetButtonUp("Fire2"))
                if (player.playerComponent[i].GetButtonUp("Aim"))
                {
                    Debug.Log("parou de mirar");
                    playerInput.aim = 0;
                }

                //if (Input.GetKeyDown(KeyCode.R) && playerInput.aim == 0)
                if (player.playerComponent[i].GetButtonDown("Reload") && playerInput.aim == 0)
                {
                    Debug.Log("reloading");
                    playerInput.isReloading = 1;
                }

                if (player.playerComponent[i].GetButtonDown("Interactions") && GameManager.Instance.canEquip)
                {
                    EquipmentManager.instance.Equip(GameManager.Instance.gunToEquip, player.transform[0].gameObject);
                    UnityEngine.Object.Destroy(GameManager.Instance.gunToDestroy);
                    GameManager.Instance.pickUpText.text = "";
                    GameManager.Instance.canEquip = false;
                }

                _player.inputComponent[i] = playerInput;
                _player.movementComponent[i] = playerMovement;
                //_player.player.playerComponent[i][i] = player.playerComponent[i];
                player = _player;
            }
        }
    }
}
