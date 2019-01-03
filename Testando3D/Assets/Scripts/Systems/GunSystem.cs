﻿using UnityEngine;
using Unity.Entities;
using Assets.Scripts.Components;
using Unity.Transforms;
using Unity.Collections;
using Assets.Scripts.Helpers;
using Unity.Mathematics;
using Unity.Burst;
using Assets.Scripts.Buffers;
using Unity.Rendering;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    public class GunSystem : ComponentSystem
    {
        public struct Gun
        {
            public ComponentArray<GunComponent> gunComponent;
            public ComponentArray<Transform> transform;
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
            }
        }

        protected override void OnUpdate()
        {
            for (var i = 0; i < gun.Length; i++)
            {
                if (gun.gunComponent[i].countDown == null)
                    gun.gunComponent[i].countDown = new CountDown(gun.gunComponent[i].countDownRate);

                Fire(gun.gunComponent[i], gun.gunComponent[i].bocal);

                if (Input.GetButtonDown("Fire2"))
                {
                    timer = 0;
                    gun.gunComponent[i].isScoped = true;
                }
                if (Input.GetButtonUp("Fire2"))
                {
                    gun.gunComponent[i].isScoped = false;
                    OnUnscoped(gun.gunComponent[i]);
                }

                if (gun.gunComponent[i].isScoped)
                {
                    timer += Time.deltaTime;
                    if (timer >= 0.15f && Camera.main.fieldOfView != gun.gunComponent[i].scopedFOV)
                    {
                        OnScoped(gun.gunComponent[i]);
                    }
                }

                gun.gunComponent[i].animator.SetBool("Scoped", gun.gunComponent[i].isScoped);
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

                NativeArray<Entity> bullet = new NativeArray<Entity>(1, Allocator.Temp);
                GameManager.entityManager.CreateEntity(EntityArchetypes.bullet, bullet);
                GameManager.entityManager.SetComponentData(bullet[0], new Position { Value = bocalT.position });
                GameManager.entityManager.SetComponentData(bullet[0], new Rotation { Value = rotation });
                GameManager.entityManager.SetComponentData(bullet[0], new Speed { Value = gunComponent.bulletSpeed });
                GameManager.entityManager.SetComponentData(bullet[0], new Components.Collision { Radius = 0.1f });
                GameManager.entityManager.SetComponentData(bullet[0], new Scale { Value = new float3(0.01f, 0.02f, 0.02f) });
                GameManager.entityManager.SetComponentData(bullet[0], new Gravity { InitPosY = bocalT.position.y, InitVel = (_pos.y - pos.y) / Time.deltaTime, Mass = 0, Time = 0 });
                GameManager.entityManager.SetSharedComponentData(bullet[0], new MeshInstanceRenderer { mesh = (Mesh)Resources.Load("Mesh/Bullet"), material = (Material)Resources.Load("Material/BulletMAT") });

                var bufferArray = EntityBufferUtils.BufferValues<MoveForwardDirectionBuffer>(Direction.X, Direction.Z);
                GameManager.entityManager.GetBuffer<MoveForwardDirectionBuffer>(bullet[0]).AddRange(bufferArray);

                CollisionSystem.entities.Add(bullet[0]);

                bullet.Dispose();
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

        void OnScoped(GunComponent gunComponent)
        {
            GameManager.Instance.scopeOverlay.enabled = true;
            //gunComponent.scopeOverlay.enabled = true; 
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Guns");
            gunComponent.normalFOV = Camera.main.fieldOfView;
            Camera.main.fieldOfView = gunComponent.scopedFOV;
        }

        void OnUnscoped(GunComponent gunComponent)
        {
            GameManager.Instance.scopeOverlay.enabled = false;
            //gunComponent.scopeOverlay.enabled = false;
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Guns");
            Camera.main.fieldOfView = gunComponent.normalFOV;
        }
    }
}
