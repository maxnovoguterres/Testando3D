using UnityEngine;
using Unity.Entities;
using Assets.Scripts.Components;
using Unity.Transforms;
using Unity.Collections;
using Assets.Scripts.Helpers;
using Unity.Mathematics;
using Unity.Burst;
using Assets.Scripts.Buffers;
using Unity.Rendering;
using Unity.Jobs;
using System.Linq;

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
        CountDown timer = new CountDown(.15f);

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
            NativeArray<Vector3> p = new NativeArray<Vector3>(gun.Length, Allocator.TempJob);
            NativeArray<float> s = new NativeArray<float>(gun.Length, Allocator.TempJob);
            NativeArray<Quaternion> r = new NativeArray<Quaternion>(gun.Length, Allocator.TempJob);
            NativeArray<Entity> e = new NativeArray<Entity>(gun.Length, Allocator.TempJob);
            GameManager.entityManager.CreateEntity(EntityArchetypes.bullet, e);

            for (var i = 0; i < gun.Length; i++)
            {
                if (gun.gunComponent[i].countDown == null)
                    gun.gunComponent[i].countDown = new CountDown(3); // gun.gunComponent[i].countDownRate


                CountDown.DecreaseTime(gun.gunComponent[i].countDown);

                if (gun.gunComponent[i].countDown.ReturnedToZero && gun.gunComponent[i].player.GetComponent<InputComponent>().Shoot)
                {
                    p[i] = gun.gunComponent[i].bocal.position;
                    r[i] = gun.gunComponent[i].player.transform.Find("FirstPersonCamera").rotation;
                    s[i] = gun.gunComponent[i].bulletSpeed;
                    GameManager.entityManager.SetSharedComponentData(e[i], new MeshInstanceRenderer { mesh = GameManager.Instance.bullet, material = (Material)Resources.Load("Material/bulletMAT") });

                    gun.gunComponent[i].countDown.StartToCount();
                }

                //var aim = gun.gunComponent[i].player.GetComponent<InputComponent>();

                //if (aim && !timer.Flag)
                //{
                //    timer.StartToCount();
                //}
                //else if (timer.Flag)
                //{
                //    OnUnscoped(gun.gunComponent[i]);
                //}

                //if (aim)
                //{
                //    CountDown.DecreaseTime(timer);
                //    if (timer.ReturnedToZero && Camera.main.fieldOfView != gun.gunComponent[i].scopedFOV)
                //    {
                //        OnScoped(gun.gunComponent[i]);
                //    }
                //}

                //gun.gunComponent[i].animator.SetBool("Scoped", aim);

                var bulletDependency = new BulletSpawn
                {
                    p = p,
                    r = r,
                    s = s,
                    e = e,
                    dTime = Time.deltaTime
                }.Schedule(gun.Length, 32);

                bulletDependency.Complete();


                p.Dispose();
                s.Dispose();
                r.Dispose();
                e.Dispose();
            }
        }

        struct BulletSpawn : IJobParallelFor
        {
            public NativeArray<Vector3> p;
            public NativeArray<float> s;
            public NativeArray<Quaternion> r;
            public NativeArray<Entity> e;
            public float dTime;

            public void Execute(int i)
            {
                float3 pos = p[i];

                if (p[i] == Vector3.zero) return;

                var nextPos = pos + (s[i] * math.forward(r[i]) * dTime);

                GameManager.entityManager.SetComponentData(e[i], new Position { Value = pos });
                GameManager.entityManager.SetComponentData(e[i], new Rotation { Value = r[i] });
                GameManager.entityManager.SetComponentData(e[i], new Speed { Value = s[i] });
                GameManager.entityManager.SetComponentData(e[i], new Components.Collision { Radius = 0.1f });
                GameManager.entityManager.SetComponentData(e[i], new Scale { Value = new float3(0.01f, 0.02f, 0.02f) });
                GameManager.entityManager.SetComponentData(e[i], new Gravity { InitPosY = pos.y, InitVel = (nextPos.y - pos.y) / dTime, Mass = 0.1f, Time = 0 });

                var bufferArray = EntityBufferUtils.BufferValues<MoveForwardDirectionBuffer>(Direction.X, Direction.Z);
                GameManager.entityManager.GetBuffer<MoveForwardDirectionBuffer>(e[i]).AddRange(bufferArray);

                CollisionSystem.entities.Add(e[i]);

                bufferArray.Dispose();
            }
        }

        void Fire()
        {
            NativeArray<Vector3> p = new NativeArray<Vector3>(gun.Length, Allocator.TempJob);
            NativeArray<float> s = new NativeArray<float>(gun.Length, Allocator.TempJob);
            NativeArray<Quaternion> r = new NativeArray<Quaternion>(gun.Length, Allocator.TempJob);
            NativeArray<Entity> e = new NativeArray<Entity>(gun.Length, Allocator.TempJob);
            GameManager.entityManager.CreateEntity(EntityArchetypes.bullet, e);

            for (var i = 0; i < gun.Length; i++)
            {
                if (gun.gunComponent[i].countDown == null)
                    gun.gunComponent[i].countDown = new CountDown(3); // gun.gunComponent[i].countDownRate


                CountDown.DecreaseTime(gun.gunComponent[i].countDown);

                if (gun.gunComponent[i].countDown.ReturnedToZero && gun.gunComponent[i].player.GetComponent<InputComponent>().Shoot)
                {
                    p[i] = gun.gunComponent[i].bocal.position;
                    r[i] = gun.gunComponent[i].player.transform.Find("FirstPersonCamera").rotation;
                    s[i] = gun.gunComponent[i].bulletSpeed;
                    GameManager.entityManager.SetSharedComponentData(e[i], new MeshInstanceRenderer { mesh = GameManager.Instance.bullet, material = (Material)Resources.Load("Material/bulletMAT") });
                }
            }
        }

        void OnScoped(GunComponent gunComponent)
        {
            timer.F(true);
            GameManager.Instance.scopeOverlay.enabled = true;
            //gunComponent.scopeOverlay.enabled = true; 
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Guns");
            gunComponent.normalFOV = Camera.main.fieldOfView;
            Camera.main.fieldOfView = gunComponent.scopedFOV;
        }

        void OnUnscoped(GunComponent gunComponent)
        {
            timer.F(false);
            GameManager.Instance.scopeOverlay.enabled = false;
            //gunComponent.scopeOverlay.enabled = false;
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Guns");
            Camera.main.fieldOfView = gunComponent.normalFOV;
        }
    }
}
