using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Entities;
using Assets.Scripts.Components;
using Unity.Transforms;
using Unity.Collections;
using Assets.Scripts.Helpers;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Burst;
using Assets.Scripts.Utils;
using Assets.Scripts.Buffers;
using UnityEngine.UI;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    public class GunSystem : ComponentSystem
    {
        public struct Gun
        {
            public ComponentArray<GunComponent> gunComponent;
            public ComponentArray<Transform> transform;
            public ComponentArray<PickupComponent> pickupComponent;
            public EntityArray Entities;
            public readonly int Length;
        }
        float timer = 0;

        [Inject] Gun gun;

        protected override void OnCreateManager()
        {
            for (var i = 0; i < gun.Length; i++)
            {
                gun.gunComponent[i].countDown = new CountDown(gun.gunComponent[i].countDownRate);
                gun.gunComponent[i].animator = GameObject.Find("GunHolder").GetComponent<Animator>();
                gun.gunComponent[i].scopeOverlay = GameObject.Find("ScopeOverlay").GetComponent<Image>();
            }
        }

        protected override void OnUpdate()
        {
            for (var i = 0; i < gun.Length; i++)
            {
                if (gun.gunComponent[i].countDown == null)
                    gun.gunComponent[i].countDown = new CountDown(gun.gunComponent[i].countDownRate);

                //Fire(gun.gunComponent[i], gun.gunComponent[i].bocal);
                //Picked(gun.transform[i], gun.gunComponent[i], gun.pickupComponent[i]);

                if (Input.GetButtonDown("Fire2"))
                {
                    timer = 0;
                    gun.gunComponent[i].isScoped = true;
                    gun.gunComponent[i].animator.SetBool("Scoped", gun.gunComponent[i].isScoped);
                }
                if (Input.GetButtonUp("Fire2"))
                {
                    gun.gunComponent[i].isScoped = false;
                    OnUnscoped(gun.gunComponent[i]);
                }

                if (gun.gunComponent[i].isScoped)
                {
                    timer += Time.deltaTime;
                    if (timer >= 0.15f)
                    {
                        OnScoped(gun.gunComponent[i]);
                        gun.gunComponent[i].isScoped = false;
                    }
                }
            }
        }

        void Fire(GunComponent gunComponent, Transform bocalT)
        {
            CountDown.DecreaseTime(gunComponent.countDown);

            if (gunComponent.player == null || gunComponent.countDown.CoolDown > 0) return;

            if (gunComponent.player.GetComponent<InputComponent>().Shoot)
            {
                var rotation = gunComponent.player.transform.Find("FirstPersonCamera").rotation;
                float3 pos = bocalT.position;
                var _pos = pos + (gunComponent.bulletSpeed * math.forward(rotation) * Time.deltaTime);

                NativeArray<Entity> _bullet = new NativeArray<Entity>(1, Allocator.Temp);
                GameManager.entityManager.Instantiate(GameManager.bullet, _bullet);
                GameManager.entityManager.SetComponentData(_bullet[0], new Position { Value = bocalT.position });
                GameManager.entityManager.SetComponentData(_bullet[0], new Rotation { Value = rotation });
                GameManager.entityManager.SetComponentData(_bullet[0], new Speed { Value = gunComponent.bulletSpeed });
                GameManager.entityManager.SetComponentData(_bullet[0], new Components.Collision { Radius = 0.1f });
                GameManager.entityManager.AddBuffer<MoveForwardDirectionBuffer>(_bullet[0]);
                var buffer = GameManager.entityManager.GetBuffer<MoveForwardDirectionBuffer>(_bullet[0]);

                CollisionSystem.entities.Add(_bullet[0]);

                var moveForwardDirectionBuffer = new MoveForwardDirectionBuffer[2];
                moveForwardDirectionBuffer[0].Value = Direction.X;
                moveForwardDirectionBuffer[1].Value = Direction.Z;

                var bufferArray = new NativeArray<MoveForwardDirectionBuffer>(moveForwardDirectionBuffer, Allocator.TempJob);
                buffer.AddRange(bufferArray);

                GameManager.entityManager.SetComponentData(_bullet[0], new Scale { Value = new float3(0.01f, 0.02f, 0.02f) });
                var gravity = GameManager.entityManager.GetComponentData<Gravity>(_bullet[0]);

                GameManager.entityManager.SetComponentData(_bullet[0], new Gravity
                {
                    InitPosY = bocalT.position.y,
                    InitVel = (_pos.y - pos.y) / Time.deltaTime,
                    Mass = 0,//gravity.Mass,
                    Time = 0
                });

                _bullet.Dispose();
                bufferArray.Dispose();

                RaycastHit[] hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                hit = Physics.RaycastAll(ray);
                foreach (var _item in hit)
                {
                    Rigidbody body = _item.collider.attachedRigidbody;
                    if (body != null)
                    {
                        body.AddForceAtPosition(Vector3.forward, _item.point, ForceMode.Impulse);
                    }
                }

                gunComponent.countDown.StartToCount();
            }
        }

        void Picked(Transform transform, GunComponent gunComponent, PickupComponent pickupComponent)
        {
            if (gunComponent.player != null) return;

            UnityEngine.Collider[] hits;
            hits = Physics.OverlapSphere(transform.position, pickupComponent.radius);
            foreach (var hit in hits)
            {
                if (hit.GetComponent<InputComponent>() == null) continue;

                EquipmentManager.instance.Equip(pickupComponent.equipment, hit.gameObject);
                break;
            }
            //StandardMethods.Destroy(transform.gameObject);
        }

        void OnScoped(GunComponent gunComponent)
        {
            gunComponent.scopeOverlay.enabled = true;
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Guns");
            gunComponent.normalFOV = Camera.main.fieldOfView;
            Camera.main.fieldOfView = gunComponent.scopedFOV;
        }

        void OnUnscoped(GunComponent gunComponent)
        {
            gunComponent.scopeOverlay.enabled = false;
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Guns");
            Camera.main.fieldOfView = gunComponent.normalFOV;
        }
    }
}
