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
        Dictionary<GameObject, PlayerMovementComponent> playerMovementComponent = new Dictionary<GameObject, PlayerMovementComponent>();
        Dictionary<GameObject, InputComponent> inputComponent = new Dictionary<GameObject, InputComponent>();
        NativeArray<float> ca = new NativeArray<float>();
        NativeArray<float> m = new NativeArray<float>();

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
            playerMovementComponent = new Dictionary<GameObject, PlayerMovementComponent>();
            inputComponent = new Dictionary<GameObject, InputComponent>();

            if (!GameManager.Instance.ammoImage.enabled)
            {
                GameManager.Instance.ammoImage.enabled = true;
                GameManager.Instance.ammoText.enabled = true;
            }

            Aim();
            Fire();
            Reload();

            ca = new NativeArray<float>(gun.Length, Allocator.TempJob);
            var a = new NativeArray<float>(gun.Length, Allocator.TempJob);
            m = new NativeArray<float>(gun.Length, Allocator.TempJob);

            for (var i = 0; i < gun.Length; i++)
            {
                PlayerMovementComponent pmc;
                if (!playerMovementComponent.ContainsKey(gun.gunComponent[i].player)) playerMovementComponent.Add(gun.gunComponent[i].player, gun.gunComponent[i].player.GetComponent<PlayerMovementComponent>());
                pmc = playerMovementComponent[gun.gunComponent[i].player];

                ca[i] = gun.gunComponent[i].CurrentAccuracy;
                a[i] = gun.gunComponent[i].Accuracy;
                m[i] = pmc.isCrouching? .5f : pmc.jumping? 2 : pmc.isWalking? 1.2f : pmc.isRunning? 1.5f : 1;
                GameManager.Instance.ammoImage.sprite = gun.gunComponent[i].AmmoImage;
                GameManager.Instance.ammoText.text = gun.gunComponent[i].CurrentAmmo.ToString() + "/" + gun.gunComponent[i].ExtraAmmo.ToString();
            }

            var updateAccuracy = new UpdateAccuracy
            {
                ca = ca,
                dTime = Time.deltaTime * .5f,
                a = a,
                m = m,
            }.Schedule(gun.Length, 32);

            updateAccuracy.Complete();

            for (var i = 0; i < gun.Length; i++)
            {
                gun.gunComponent[i].CurrentAccuracy = ca[i];
                UpdateArrows(gun.gunComponent[i]);
            }

            ca.Dispose();
            a.Dispose();
            m.Dispose();
        }

        void UpdateArrows(GunComponent gunC)
        {
            Vector3[] v = new Vector3[4] { Vector3.up, Vector3.down, Vector3.right, Vector3.left };
            for (var i = 0; i < 4; i++)
                GameManager.Instance.arrows[i].transform.position = ArrowNewPos(GameManager.Instance.arrowsPos[i], v[i], gunC.CurrentAccuracy);
        }

        public Vector3 ArrowNewPos(Vector3 pos, Vector3 dir, float cA)
        {
            return pos + cA * 70 * dir;
        }

        void Reload()
        {
            for (int i = 0; i < gun.Length; i++)
            {
                if (gun.gunComponent[i].player == null) continue;

                InputComponent inputC;
                if (!inputComponent.ContainsKey(gun.gunComponent[i].player)) inputComponent.Add(gun.gunComponent[i].player, gun.gunComponent[i].player.GetComponent<InputComponent>());
                inputC = inputComponent[gun.gunComponent[i].player];

                if (gun.gunComponent[i].ExtraAmmo <= 0)
                    inputC.isReloading = false;

                var IsReloading = inputC.isReloading;

                if (IsReloading && gun.gunComponent[i].CurrentAmmo < gun.gunComponent[i].MaxAmmo && gun.gunComponent[i].ExtraAmmo != 0)
                {
                    reloadTimer += Time.deltaTime;
                    if (reloadTimer >= 1.5f)
                    {
                        if (gun.gunComponent[i].CurrentAmmo + gun.gunComponent[i].ExtraAmmo < gun.gunComponent[i].MaxAmmo)
                        {
                            gun.gunComponent[i].CurrentAmmo += gun.gunComponent[i].ExtraAmmo;
                            gun.gunComponent[i].ExtraAmmo = 0;
                        }
                        else
                        {
                            gun.gunComponent[i].ExtraAmmo -= gun.gunComponent[i].MaxAmmo - gun.gunComponent[i].CurrentAmmo;
                            gun.gunComponent[i].CurrentAmmo += gun.gunComponent[i].MaxAmmo - gun.gunComponent[i].CurrentAmmo;
                        }
                        
                        inputC.isReloading = false;
                        reloadTimer = 0;
                    }
                }
                else if (gun.gunComponent[i].CurrentAmmo == gun.gunComponent[i].MaxAmmo)
                {
                    IsReloading = false;
                    inputC.isReloading = false;
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
                InputComponent inputC;
                if (!inputComponent.ContainsKey(gun.gunComponent[i].player)) inputComponent.Add(gun.gunComponent[i].player, gun.gunComponent[i].player.GetComponent<InputComponent>());
                inputC = inputComponent[gun.gunComponent[i].player];

                var aim = inputC.Aim;
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
            List<float> _ia = new List<float>();
            List<float> _ca = new List<float>();
            List<float> _rca = new List<float>();
            List<int> _ru = new List<int>();
            List<int> _rl = new List<int>();
            List<float> _m = new List<float>();
            List<float> _re = new List<float>();
            List<float> _iRe = new List<float>();

            for (var i = 0; i < gun.Length; i++)
            {
#if DEBUG
                if (gun.gunComponent[i].player == null) continue;
#endif
                InputComponent inputC;
                if (!inputComponent.ContainsKey(gun.gunComponent[i].player)) inputComponent.Add(gun.gunComponent[i].player, gun.gunComponent[i].player.GetComponent<InputComponent>());
                inputC = inputComponent[gun.gunComponent[i].player];

                if (gun.gunComponent[i].CurrentAmmo <= 0)
                    inputC.Shoot = false;

                if (gun.gunComponent[i].countDown == null)
                    gun.gunComponent[i].countDown = new CountDown(gun.gunComponent[i].countDownRate);

                CountDown.DecreaseTime(gun.gunComponent[i].countDown);

                if (gun.gunComponent[i].countDown.ReturnedToZero && inputC.Shoot)
                {
                    PlayerMovementComponent pmc;
                    if (!playerMovementComponent.ContainsKey(gun.gunComponent[i].player)) playerMovementComponent.Add(gun.gunComponent[i].player, gun.gunComponent[i].player.GetComponent<PlayerMovementComponent>());
                    pmc = playerMovementComponent[gun.gunComponent[i].player];

                    _ia.Add(gun.gunComponent[i].IncreaseAccuracy);
                    _ca.Add(gun.gunComponent[i].CurrentAccuracy);
                    _m.Add(pmc.isCrouching ? .5f : pmc.jumping ? 2 : pmc.isWalking ? 1.2f : pmc.isRunning ? 1.5f : 1);
                    _re.Add(gun.gunComponent[i].Recoil);
                    _iRe.Add(gun.gunComponent[i].IncreaseRecoil);
                    gun.gunComponent[i].countDown.StartToCount();
                    gun.gunComponent[i].CurrentAmmo -= 1;
                    if (gun.gunComponent[i].scopedFOV == 15)
                        inputC.isReloading = true;
                    if (gun.gunComponent[i].CurrentAmmo <= 0)
                        inputC.Shoot = false;

                    for (int j = 0; j < gun.gunComponent[i].qtdProjectile; j++)
                    {
                        _p.Add(gun.gunComponent[i].bocal.position);
                        _r.Add(gun.gunComponent[i].player.transform.Find("FirstPersonCamera").rotation);
                        _s.Add(gun.gunComponent[i].bulletSpeed);
                        _rca.Add(UnityEngine.Random.Range(0, gun.gunComponent[i].CurrentAccuracy));
                        _ru.Add(UnityEngine.Random.Range(0, 2));
                        _rl.Add(UnityEngine.Random.Range(0, 2));
                    }
                }
            }

            if (_p.Count == 0) return;

            NativeArray<Vector3> p = new NativeArray<Vector3>(_p.Count, Allocator.TempJob);
            NativeArray<float> s = new NativeArray<float>(_s.Count, Allocator.TempJob);
            NativeArray<quaternion> r = new NativeArray<quaternion>(_r.Count, Allocator.TempJob);
            NativeArray<Entity> e = new NativeArray<Entity>(_p.Count, Allocator.TempJob);
            NativeArray<float> ia = new NativeArray<float>(_ia.Count, Allocator.TempJob);
            ca = new NativeArray<float>(_ca.Count, Allocator.TempJob);
            NativeArray<float> rca = new NativeArray<float>(_rca.Count, Allocator.TempJob);
            NativeArray<int> ru = new NativeArray<int>(_ru.Count, Allocator.TempJob);
            NativeArray<int> rl = new NativeArray<int>(_rl.Count, Allocator.TempJob);
            m = new NativeArray<float>(_m.Count, Allocator.TempJob);
            NativeArray<float> re = new NativeArray<float>(_re.Count, Allocator.TempJob);
            NativeArray<float> iRe = new NativeArray<float>(_iRe.Count, Allocator.TempJob);
            GameManager.entityManager.CreateEntity(EntityArchetypes.bullet, e);

            for (var i = 0; i < _ia.Count; i++)
            {
                ia[i] = _ia[i];
                ca[i] = _ca[i];
                rca[i] = _rca[i];
                ru[i] = _ru[i];
                rl[i] = _rl[i];
                m[i] = _m[i];
                re[i] = _re[i];
                iRe[i] = _iRe[i];
            }
            for (int j = 0; j < _p.Count; j++)
            {
                p[j] = _p[j];
                r[j] = _r[j];
                s[j] = _s[j];
                GameManager.entityManager.SetSharedComponentData(e[j], new MeshInstanceRenderer { mesh = GameManager.Instance.bullet, material = (Material)Resources.Load("Material/bulletMAT") });
            }

            var bulletDependency = new BulletSpawn
            {
                p = p,
                r = r,
                s = s,
                e = e,
                rca = rca,
                rl = rl,
                ru = ru,
                dTime = Time.deltaTime,
            }.Schedule(p.Length, 32);

            var increaseAccuracy = new IncreaseAccuracy
            {
                ia = ia,
                ca = ca,
                m = m
            }.Schedule(ia.Length, 32, bulletDependency);

            var increaseRecoil = new IncreaseRecoil
            {
                ro = r,
                re = re,
                iRe = iRe,
                m = m,
            }.Schedule(ia.Length, 32, increaseAccuracy);

            increaseRecoil.Complete();

            for (var i = 0; i < ca.Length; i++)
            {
                gun.gunComponent[i].player.transform.Find("FirstPersonCamera").transform.rotation = r[i];
                gun.gunComponent[i].CurrentAccuracy = ca[i];
            }
            p.Dispose();
            s.Dispose();
            r.Dispose();
            e.Dispose();
            ia.Dispose();
            ca.Dispose();
            rca.Dispose();
            ru.Dispose();
            rl.Dispose();
            m.Dispose();
            re.Dispose();
            iRe.Dispose();
        }

        struct BulletSpawn : IJobParallelFor
        {
            public NativeArray<Vector3> p;
            public NativeArray<float> s;
            public NativeArray<quaternion> r;
            public NativeArray<Entity> e;
            public NativeArray<float> rca;
            public NativeArray<int> ru;
            public NativeArray<int> rl;
            public float dTime;

            public void Execute(int i)
            {
                float3 pos = p[i];

                var nextPos = pos + (s[i] * math.forward(r[i]) * dTime);
                var rM = (rca[i] * .02f);
                GameManager.entityManager.SetComponentData(e[i], new DestroyAfterTime { lifeTime = 0f, timeToDestroy = 1f });
                GameManager.entityManager.SetComponentData(e[i], new Position { Value = pos });
                GameManager.entityManager.SetComponentData(e[i], new Rotation { Value = new quaternion(r[i].value.x, r[i].value.y + rM * (rl[i] % 2 == 0 ? +1 : -1), r[i].value.z + rM * 2 * (ru[i] % 2 == 0 ? +1 : -1), r[i].value.w) });
                GameManager.entityManager.SetComponentData(e[i], new Speed { Value = s[i] });
                GameManager.entityManager.SetComponentData(e[i], new Components.Collision { Radius = 0.1f });
                GameManager.entityManager.SetComponentData(e[i], new Scale { Value = new float3(0.02f, 0.02f, 0.02f) });
                //GameManager.entityManager.SetComponentData(e[i], new Gravity { InitPosY = pos.y, InitVel = (nextPos.y - pos.y) / dTime, Mass = 0.1f, Time = 0 });

                var bufferArray = EntityBufferUtils.BufferValues<DirectionBuffer>(Direction.X, Direction.Y, Direction.Z);
                GameManager.entityManager.GetBuffer<DirectionBuffer>(e[i]).AddRange(bufferArray);

                CollisionSystem.entities.Add(e[i]);

                bufferArray.Dispose();
            }
        }

        struct IncreaseAccuracy : IJobParallelFor
        {
            public NativeArray<float> ia;
            public NativeArray<float> ca;
            public NativeArray<float> m;

            public void Execute(int i)
            {
                var _ia = ia[i] * m[i];
                ca[i] = (ca[i] >= 1 || ca[i] + _ia > 1) ? 1 : ca[i] + _ia;
            }
        }
        struct UpdateAccuracy : IJobParallelFor
        {
            public float dTime;
            public NativeArray<float> a;
            public NativeArray<float> ca;
            public NativeArray<float> m;

            public void Execute(int i)
            {
                var _a = a[i] * m[i];
                ca[i] = (ca[i] < _a || ca[i] - dTime < _a) ? _a : ca[i] - dTime;
            }
        }

        struct IncreaseRecoil : IJobParallelFor
        {
            public NativeArray<quaternion> ro;
            public NativeArray<float> re;
            public NativeArray<float> iRe;
            public NativeArray<float> m;

            public void Execute(int i)
            {
                var _iRe = iRe[i] * m[i];
                var _ro = ro[i];
                _ro.value.x -= (re[i] >= .002f || re[i] + _iRe > .002f) ? .002f : re[i] + _iRe;
                ro[i] = _ro;
            }
        }
        struct UpdateRecoil : IJobParallelFor
        {
            public float dTime;
            public NativeArray<quaternion> ro;
            public NativeArray<float> re;
            public NativeArray<float> m;

            public void Execute(int i)
            {
                var _d = dTime * m[i];
                var _ro = ro[i];
                _ro.value.x = (re[i] < 0 || re[i] - _d < 0) ? 0 : re[i] - _d;
                ro[i] = _ro;
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
