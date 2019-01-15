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
using System.Collections.Generic;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    public class GunSystem : ComponentSystem
    {
        CountDown timer = new CountDown(.15f, true);
        float reloadTimer = 0;

        public struct Gun
        {
            public ComponentArray<GunComponent> gunComponent;
            public ComponentArray<Transform> transform;
            public EntityArray Entities;
            public readonly int Length;
        }

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
            if (!GameManager.Instance.ammoImage.enabled)
            {
                GameManager.Instance.ammoImage.enabled = true;
                GameManager.Instance.ammoText.enabled = true;
            }

            Aim();
            Fire();
            Reload();

            var ca = new NativeArray<float>(gun.Length, Allocator.TempJob);

            for (var i = 0; i < gun.Length; i++)
            {
                ca[i] = gun.gunComponent[i].CurrentAccuracy;
                GameManager.Instance.ammoImage.sprite = gun.gunComponent[i].AmmoImage;
                GameManager.Instance.ammoText.text = gun.gunComponent[i].CurrentAmmo.ToString() + "/" + gun.gunComponent[i].ExtraAmmo.ToString();
            }

            var IncreaseAccurrency = new UpdateAccuracy
            {
                ca = ca,
                dTime = Time.deltaTime
            }.Schedule(gun.Length, 32);

            IncreaseAccurrency.Complete();

            ca.Dispose();
        }

        void Reload()
        {
            for (int i = 0; i < gun.Length; i++)
            {
                if (gun.gunComponent[i].player == null) continue;

                if (gun.gunComponent[i].ExtraAmmo <= 0)
                    gun.gunComponent[i].player.GetComponent<InputComponent>().isReloading = false;

                var IsReloading = gun.gunComponent[i].player.GetComponent<InputComponent>().isReloading;

                if (IsReloading && gun.gunComponent[i].CurrentAmmo < gun.gunComponent[i].MaxAmmo && gun.gunComponent[i].ExtraAmmo != 0)
                {
                    reloadTimer += Time.deltaTime;
                    if (reloadTimer >= 1.5f)
                    {
                        if (gun.gunComponent[i].CurrentAmmo + gun.gunComponent[i].ExtraAmmo < 30)
                        {
                            gun.gunComponent[i].CurrentAmmo += gun.gunComponent[i].ExtraAmmo;
                            gun.gunComponent[i].ExtraAmmo = 0;
                        }
                        else
                        {
                            gun.gunComponent[i].ExtraAmmo -= gun.gunComponent[i].MaxAmmo - gun.gunComponent[i].CurrentAmmo;
                            gun.gunComponent[i].CurrentAmmo += gun.gunComponent[i].MaxAmmo - gun.gunComponent[i].CurrentAmmo;
                        }

                        GameManager.Instance.ammoText.text = gun.gunComponent[i].CurrentAmmo.ToString() + "/" + gun.gunComponent[i].ExtraAmmo.ToString();
                        Debug.Log("Arma Recarregada!");
                        gun.gunComponent[i].player.GetComponent<InputComponent>().isReloading = false;
                        reloadTimer = 0;
                    }
                }
                else if (gun.gunComponent[i].CurrentAmmo == gun.gunComponent[i].MaxAmmo)
                {
                    IsReloading = false;
                    gun.gunComponent[i].player.GetComponent<InputComponent>().isReloading = false;
                }

                if (gun.gunComponent[i].animator != null)
                    gun.gunComponent[i].animator.SetBool("isReloading", IsReloading);
            }
        }

        public void Aim()
        {
            GameManager.Instance.EnableRedDot(false);
            for (var i = 0; i < gun.Length; i++)
            {
#if DEBUG
                if (gun.gunComponent[i].player == null) continue;
#endif
                var aim = gun.gunComponent[i].player.GetComponent<InputComponent>().Aim;
                if (aim)
                {
                    GameManager.Instance.EnableRedDot(false);
                    if (timer.ReturnedToZero)
                        timer.StartToCount();

                    timer.DecreaseTime();
                    if (timer.ReturnedToZero && Camera.main.fieldOfView != gun.gunComponent[i].scopedFOV)
                        OnScoped(gun.gunComponent[i]);
                }
                else
                {
                    GameManager.Instance.EnableRedDot(true);
                    timer.Zero();
                    if (Camera.main.fieldOfView != gun.gunComponent[i].normalFOV)
                        OnUnscoped(gun.gunComponent[i]);
                }

                if (gun.gunComponent[i].animator != null)
                    gun.gunComponent[i].animator.SetBool("Scoped", aim);
            }
        }

        void Fire()
        {
            List<Vector3> _p = new List<Vector3>();
            List<float> _s = new List<float>();
            List<Quaternion> _r = new List<Quaternion>();
            List<Entity> _e = new List<Entity>();
            List<float> _a = new List<float>();
            List<float> _ca = new List<float>();

            for (var i = 0; i < gun.Length; i++)
            {
                if (gun.gunComponent[i].CurrentAmmo <= 0)
                    gun.gunComponent[i].player.GetComponent<InputComponent>().Shoot = false;

                if (gun.gunComponent[i].countDown == null)
                    gun.gunComponent[i].countDown = new CountDown(gun.gunComponent[i].countDownRate);

#if DEBUG
                if (gun.gunComponent[i].player == null) continue;
#endif
                CountDown.DecreaseTime(gun.gunComponent[i].countDown);

                if (gun.gunComponent[i].countDown.ReturnedToZero && gun.gunComponent[i].player.GetComponent<InputComponent>().Shoot)
                {
                    _p.Add(gun.gunComponent[i].bocal.position);
                    _r.Add(gun.gunComponent[i].player.transform.Find("FirstPersonCamera").rotation);
                    _s.Add(gun.gunComponent[i].bulletSpeed);
                    _a.Add(gun.gunComponent[i].Accuracy);
                    _ca.Add(gun.gunComponent[i].CurrentAccuracy);
                    gun.gunComponent[i].countDown.StartToCount();
                    gun.gunComponent[i].CurrentAmmo -= 1;
                    if (gun.gunComponent[i].scopedFOV == 15)
                        gun.gunComponent[i].player.GetComponent<InputComponent>().isReloading = true;
                    GameManager.Instance.ammoText.text = gun.gunComponent[i].CurrentAmmo.ToString() + "/" + gun.gunComponent[i].ExtraAmmo.ToString();
                    if (gun.gunComponent[i].CurrentAmmo <= 0)
                        gun.gunComponent[i].player.GetComponent<InputComponent>().Shoot = false;
                }
            }

            if (_p.Count == 0) return;

            NativeArray<Vector3> p = new NativeArray<Vector3>(_p.Count, Allocator.TempJob);
            NativeArray<float> s = new NativeArray<float>(_s.Count, Allocator.TempJob);
            NativeArray<Quaternion> r = new NativeArray<Quaternion>(_r.Count, Allocator.TempJob);
            NativeArray<Entity> e = new NativeArray<Entity>(_p.Count, Allocator.TempJob);
            NativeArray<float> a = new NativeArray<float>(_r.Count, Allocator.TempJob);
            NativeArray<float> ca = new NativeArray<float>(_p.Count, Allocator.TempJob);
            GameManager.entityManager.CreateEntity(EntityArchetypes.bullet, e);

            for (var i = 0; i < _p.Count; i++)
            {
                p[i] = _p[i];
                r[i] = _r[i];
                s[i] = _s[i];
                a[i] = _a[i];
                ca[i] = _ca[i];

                GameManager.entityManager.SetSharedComponentData(e[i], new MeshInstanceRenderer { mesh = GameManager.Instance.bullet, material = (Material)Resources.Load("Material/bulletMAT") });
            }

            var bulletDependency = new BulletSpawn
            {
                p = p,
                r = r,
                s = s,
                e = e,
                dTime = Time.deltaTime
            }.Schedule(p.Length, 32);

            var increaseAccuracy = new IncreaseAccuracy
            {
                a = a,
                ca = ca
            }.Schedule(p.Length, 32, bulletDependency);

            increaseAccuracy.Complete();

            p.Dispose();
            s.Dispose();
            r.Dispose();
            e.Dispose();
            a.Dispose();
            ca.Dispose();
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

                var nextPos = pos + (s[i] * math.forward(r[i]) * dTime);

                GameManager.entityManager.SetComponentData(e[i], new DestroyAfterTime { lifeTime = 0f, timeToDestroy = 1f });
                GameManager.entityManager.SetComponentData(e[i], new Position { Value = pos });
                GameManager.entityManager.SetComponentData(e[i], new Rotation { Value = r[i] });
                GameManager.entityManager.SetComponentData(e[i], new Speed { Value = s[i] });
                GameManager.entityManager.SetComponentData(e[i], new Components.Collision { Radius = 0.1f });
                GameManager.entityManager.SetComponentData(e[i], new Scale { Value = new float3(0.02f, 0.02f, 0.02f) });
                GameManager.entityManager.SetComponentData(e[i], new Gravity { InitPosY = pos.y, InitVel = (nextPos.y - pos.y) / dTime, Mass = 0.1f, Time = 0 });

                var bufferArray = EntityBufferUtils.BufferValues<DirectionBuffer>(Direction.X, Direction.Z);
                GameManager.entityManager.GetBuffer<DirectionBuffer>(e[i]).AddRange(bufferArray);

                CollisionSystem.entities.Add(e[i]);

                bufferArray.Dispose();
            }
        }

        struct IncreaseAccuracy : IJobParallelFor
        {
            public NativeArray<float> a;
            public NativeArray<float> ca;

            public void Execute(int i)
            {
               ca[i] = (ca[i] >= 1 || ca[i] + a[i] > 1)? 1 : a[i];
            }
        }
        struct UpdateAccuracy : IJobParallelFor
        {
            public float dTime;
            public NativeArray<float> ca;

            public void Execute(int i)
            {
                ca[i] -= dTime;
            }
        }

        void OnScoped(GunComponent gunComponent)
        {
            GameManager.Instance.scopeOverlay.enabled = true;
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Guns");
            gunComponent.normalFOV = Camera.main.fieldOfView;
            Camera.main.fieldOfView = gunComponent.scopedFOV;
        }

        void OnUnscoped(GunComponent gunComponent)
        {
            GameManager.Instance.scopeOverlay.enabled = false;
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Guns");
            Camera.main.fieldOfView = gunComponent.normalFOV;
        }
    }
}
