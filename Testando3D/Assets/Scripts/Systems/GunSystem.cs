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
        #region [GUN Struct]
        public struct Gun
        {
            public ComponentArray<GunComponent> gunComponent;
            public ComponentArray<Transform> transform;
            public EntityArray Entities;
            public readonly int Length;
        }

        [Inject] Gun gun;
        #endregion

        #region [Native Arrays]
        //Accuracy
        NativeArray<float> ca; //currentAccuracy
        NativeArray<float> ia; //increaseAccuracy
        NativeArray<float> a; //accuracy
        NativeArray<float> rca; //random bullet's direction
        NativeArray<int> ru; //randomUp -> 0 = up | 1 = down
        NativeArray<int> rl; //randomLeft -> 0 = left | 1 = right

        //Recoil
        NativeArray<float> re; //recoil
        NativeArray<float> iRe; //increaseRecoil

        //Extra datas used in Accurracy and Recoil
        NativeArray<float> m; //Modifier 
        NativeArray<int> iUsed; //Index of Native Arrays used in shoot

        //Ammo
        NativeArray<int> cAmmo; //currentAmmo
        NativeArray<int> ea; //extraAmmo
        NativeArray<int> ma; //maxAmmo
        NativeArray<float> rt; //reloadTimer

        //bullet
        NativeArray<quaternion> r; //rotation
        NativeArray<Vector3> p; // position
        NativeArray<float> s; //scale
        NativeArray<Entity> e; //entity

        //Player Components
        NativeArray<_Input> input;
        NativeArray<PlayerMovement> pmc;

        //Communs
        NativeArray<float> fcd; //fireCoolDown
        #endregion

        protected override void OnCreateManager()
        {
            for (var i = 0; i < gun.Length; i++)
            {
                gun.gunComponent[i].fireCoolDown = gun.gunComponent[i].FireRate;
                gun.gunComponent[i].animator = GameObject.Find("GunHolder").GetComponent<Animator>();
                gun.gunComponent[i].timer = new CountDown(.15f, true);
            }
        }

        protected override void OnUpdate()
        {
            GetDatas();

            if (!GameManager.Instance.ammoImage.enabled)
            {
                GameManager.Instance.ammoImage.enabled = true;
                GameManager.Instance.ammoText.enabled = true;
            }

            Aim();

            #region [Commun]
            var UpdateRate = new UpdateRate
            {
                dTime = Time.deltaTime * .5f,
                fcd = fcd
            }.Schedule(gun.Length, 32);
            #endregion

            #region [Fire]
            JobHandle? increaseRecoil = null;
            if (p.Length > 0)
            {
                var bulletDependency = new BulletSpawn
                {
                    p = p,
                    r = r,
                    s = s,
                    e = e,
                    rca = rca,
                    rl = rl,
                    ru = ru,
                    input = input,
                    dTime = Time.deltaTime,
                }.Schedule(p.Length, 32, UpdateRate);

                var increaseAccuracy = new IncreaseAccuracy
                {
                    ia = ia,
                    ca = ca,
                    m = m,
                    iUsed = iUsed,
                }.Schedule(ia.Length, 32, bulletDependency);

                increaseRecoil = new IncreaseRecoil
                {
                    ro = r,
                    re = re,
                    iRe = iRe,
                    m = m,
                    iUsed = iUsed,
                }.Schedule(ia.Length, 32, increaseAccuracy);
            }
            #endregion

            #region [Reload]
            var reload = new Reload
            {
                ca = cAmmo,
                ea = ea,
                input = input,
                ma = ma,
                rt = rt,
                dTime = Time.deltaTime,
            }.Schedule(gun.Length, 32, increaseRecoil ?? UpdateRate);
            #endregion

            #region [Update Accuracy]
            var updateAccuracy = new UpdateAccuracy
            {
                ca = ca,
                dTime = Time.deltaTime * .5f,
                a = a,
                m = m,
            }.Schedule(gun.Length, 32, reload);
            updateAccuracy.Complete();
            #endregion

            for (var i = 0; i < gun.Length; i++)
            {
                //if (p.Length > 0)
                //{
                //    gun.gunComponent[i].player.transform.Find("FirstPersonCamera").transform.rotation = r[i];
                //}
                if (gun.gunComponent[i].animator != null)
                    gun.gunComponent[i].animator.SetBool("isReloading", input[i].isReloading == 1);

                GameManager.Instance.ammoImage.sprite = gun.gunComponent[i].AmmoImage;
                GameManager.Instance.ammoText.text = gun.gunComponent[i].CurrentAmmo.ToString() + "/" + gun.gunComponent[i].ExtraAmmo.ToString();

                UpdateArrows(gun.gunComponent[i].CurrentAccuracy);

                #region [Setting Data]
                gun.gunComponent[i].CurrentAccuracy = ca[i];
                gun.gunComponent[i].fireCoolDown = fcd[i];
                gun.gunComponent[i].CurrentAmmo = cAmmo[i];
                gun.gunComponent[i].ExtraAmmo = ea[i];
                gun.gunComponent[i].MaxAmmo = ma[i];
                gun.gunComponent[i].reloadTimer = rt[i];

                gun.gunComponent[i].playerEntity.SetComponentData<_Input>(null, new KeyValuePair<string, object>("isReloading", input[i].isReloading));
                #endregion
            }

            #region [Dispose]
            a.Dispose();
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
            input.Dispose();
            cAmmo.Dispose();
            ea.Dispose();
            ma.Dispose();
            rt.Dispose();
            iUsed.Dispose();
            fcd.Dispose();
            pmc.Dispose();
            #endregion

        }

        void UpdateArrows(float ca)
        {
            Vector3[] v = new Vector3[4] { Vector3.up, Vector3.down, Vector3.right, Vector3.left };
            for (var i = 0; i < 4; i++)
                GameManager.Instance.arrows[i].transform.position = ArrowNewPos(GameManager.Instance.arrowsPos[i], v[i], ca);
        }

        public Vector3 ArrowNewPos(Vector3 pos, Vector3 dir, float cA)
        {
            return pos + cA * 70 * dir;
        }

        public void Aim()
        {
            GameManager.Instance.EnableRedDot(false);
            for (var i = 0; i < gun.Length; i++)
            {
                var aim = input[i].aim == 1;
                if (aim)
                {
                    GameManager.Instance.EnableRedDot(false);
                    if (gun.gunComponent[i].timer.ReturnedToZero)
                        gun.gunComponent[i].timer.StartToCount();

                    gun.gunComponent[i].timer.DecreaseTime();
                    if (gun.gunComponent[i].timer.ReturnedToZero && gun.gunComponent[i].firstPersonCamera.fieldOfView != gun.gunComponent[i].scopedFOV)
                        OnScoped(gun.gunComponent[i]);
                }
                else
                {
                    GameManager.Instance.EnableRedDot(true);
                    gun.gunComponent[i].timer.Zero();
                    if (gun.gunComponent[i].firstPersonCamera.fieldOfView != gun.gunComponent[i].normalFOV)
                        OnUnscoped(gun.gunComponent[i]);
                }

                if (gun.gunComponent[i].animator != null)
                    gun.gunComponent[i].animator.SetBool("Scoped", aim);
            }
        }

        struct UpdateRate : IJobParallelFor
        {
            public float dTime;
            public NativeArray<float> fcd;

            public void Execute(int i)
            {
                if (fcd[i] > 0)
                    fcd[i] -= dTime;
            }
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
            public NativeArray<_Input> input;
            public float dTime;

            public void Execute(int i)
            {
                float3 pos = p[i];

                var nextPos = pos + (s[i] * math.forward(r[i]) * dTime);
                var rM = rca[i] * (input[i].aim == 1 ? 0.01f : .02f);
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
            public NativeArray<int> iUsed;

            public void Execute(int i)
            {
                var _i = iUsed[i];
                var _ia = ia[i] * m[_i];
                ca[iUsed[i]] = (ca[iUsed[i]] >= 1 || ca[iUsed[i]] + _ia > 1) ? 1 : ca[iUsed[i]] + _ia;
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
            public NativeArray<int> iUsed;

            public void Execute(int i)
            {
                var _i = iUsed[i];
                var _iRe = iRe[i] * m[_i];
                var _ro = ro[i];
                _ro.value.x -= (re[i] >= .002f || re[i] + _iRe > .002f) ? .002f : re[i] + _iRe;
                ro[i] = _ro;
            }
        }
        //struct UpdateRecoil : IJobParallelFor
        //{
        //    public float dTime;
        //    public NativeArray<quaternion> ro;
        //    public NativeArray<float> re;
        //    public NativeArray<float> m;

        //    public void Execute(int i)
        //    {
        //        var _d = dTime * m[i];
        //        var _ro = ro[i];
        //        _ro.value.x = (re[i] < 0 || re[i] - _d < 0) ? 0 : re[i] - _d;
        //        ro[i] = _ro;
        //    }
        //}

        struct Reload : IJobParallelFor
        {
            public float dTime;
            public NativeArray<int> ea;
            public NativeArray<int> ca;
            public NativeArray<int> ma;
            public NativeArray<float> rt;
            public NativeArray<_Input> input;

            public void Execute(int i)
            {
                var _input = input[i];

                if (ea[i] <= 0) _input.isReloading = 0;

                var IsReloading = _input.isReloading == 1;

                if (IsReloading && ca[i] < ma[i] && ea[i] != 0)
                {
                    rt[i] += dTime;
                    if (rt[i] >= 1.5f)
                    {
                        if (ca[i] + ea[i] < ma[i])
                        {
                            ca[i] += ea[i];
                            ea[i] = 0;
                        }
                        else
                        {
                            ea[i] -= ma[i] - ca[i];
                            ca[i] += ma[i] - ca[i];
                        }

                        _input.isReloading = 0;
                        rt[i] = 0;
                    }
                }
                else if (ca[i] == ma[i])
                {
                    IsReloading = false;
                    _input.isReloading = 0;
                }
                input[i] = _input;
            }
        }

        struct GetModifier : IJobParallelFor
        {
            public NativeArray<PlayerMovement> pm;
            public NativeArray<float> m;

            public void Execute(int i)
            {
                m[i] = pm[i].isCrouching == 1 ? .5f : pm[i].jumping == 1 ? 2 : pm[i].isWalking == 1 ? 1.2f : pm[i].isRunning == 1 ? 1.5f : 1;
            }
        }

        void OnScoped(GunComponent gunComponent)
        {
            GameManager.Instance.scopeOverlay.enabled = true;
            gunComponent.gunCamera.SetActive(false);
            gunComponent.normalFOV = gunComponent.firstPersonCamera.fieldOfView;
            gunComponent.firstPersonCamera.fieldOfView = gunComponent.scopedFOV;
        }

        void OnUnscoped(GunComponent gunComponent)
        {
            GameManager.Instance.scopeOverlay.enabled = false;
            gunComponent.gunCamera.SetActive(true);
            gunComponent.firstPersonCamera.fieldOfView = gunComponent.normalFOV;
        }

        void GetDatas()
        {
            #region [Native Array Instances]
            input = new NativeArray<_Input>(gun.Length, Allocator.TempJob);
            pmc = new NativeArray<PlayerMovement>(gun.Length, Allocator.TempJob);
            cAmmo = new NativeArray<int>(gun.Length, Allocator.TempJob);
            ea = new NativeArray<int>(gun.Length, Allocator.TempJob);
            ma = new NativeArray<int>(gun.Length, Allocator.TempJob);
            rt = new NativeArray<float>(gun.Length, Allocator.TempJob);
            ca = new NativeArray<float>(gun.Length, Allocator.TempJob);
            a = new NativeArray<float>(gun.Length, Allocator.TempJob);
            m = new NativeArray<float>(gun.Length, Allocator.TempJob);
            fcd = new NativeArray<float>(gun.Length, Allocator.TempJob);
            #endregion

            #region [Temp List]
            List<Vector3> _p = new List<Vector3>();
            List<float> _s = new List<float>();
            List<Quaternion> _r = new List<Quaternion>();
            List<Entity> _e = new List<Entity>();
            List<float> _ia = new List<float>();
            List<int> _iUsed = new List<int>();
            List<float> _rca = new List<float>();
            List<int> _ru = new List<int>();
            List<int> _rl = new List<int>();
            List<float> _m = new List<float>();
            List<float> _re = new List<float>();
            List<float> _iRe = new List<float>();
            #endregion

            for (var i = 0; i < gun.Length; i++)
            {
                if(gun.gunComponent[i].timer == null) gun.gunComponent[i].timer = new CountDown(.15f, true);

                #region [PlayerComponents]
                input[i] = GameManager.entityManager.GetComponentData<_Input>(gun.gunComponent[i].playerEntity);
                pmc[i] = GameManager.entityManager.GetComponentData<PlayerMovement>(gun.gunComponent[i].playerEntity);
                #endregion

                #region [Accuracy]
                ca[i] = gun.gunComponent[i].CurrentAccuracy;
                a[i] = gun.gunComponent[i].Accuracy;
                #endregion

                #region [ShootDatas]
                if (gun.gunComponent[i].fireCoolDown <= 0 && input[i].shoot == 1 && gun.gunComponent[i].CurrentAmmo > 0)
                {
                    _ia.Add(gun.gunComponent[i].IncreaseAccuracy);
                    _re.Add(gun.gunComponent[i].Recoil);
                    _iRe.Add(gun.gunComponent[i].IncreaseRecoil);
                    _iUsed.Add(i);
                    gun.gunComponent[i].fireCoolDown = gun.gunComponent[i].FireRate;
                    gun.gunComponent[i].CurrentAmmo -= 1;

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
                #endregion

                #region [Ammo]
                cAmmo[i] = gun.gunComponent[i].CurrentAmmo;
                ea[i] = gun.gunComponent[i].ExtraAmmo;
                ma[i] = gun.gunComponent[i].MaxAmmo;
                rt[i] = gun.gunComponent[i].reloadTimer;
                #endregion

                #region [Commun]
                fcd[i] = gun.gunComponent[i].fireCoolDown;
                #endregion
            }

            #region [SettingBulletNativeArraysData]

            p = new NativeArray<Vector3>(_p.Count, Allocator.TempJob);
            s = new NativeArray<float>(_s.Count, Allocator.TempJob);
            r = new NativeArray<quaternion>(_r.Count, Allocator.TempJob);
            e = new NativeArray<Entity>(_p.Count, Allocator.TempJob);
            ia = new NativeArray<float>(_ia.Count, Allocator.TempJob);
            rca = new NativeArray<float>(_rca.Count, Allocator.TempJob);
            ru = new NativeArray<int>(_ru.Count, Allocator.TempJob);
            rl = new NativeArray<int>(_rl.Count, Allocator.TempJob);
            re = new NativeArray<float>(_re.Count, Allocator.TempJob);
            iRe = new NativeArray<float>(_iRe.Count, Allocator.TempJob);
            iUsed = new NativeArray<int>(_iUsed.Count, Allocator.TempJob);
            GameManager.entityManager.CreateEntity(EntityArchetypes.bullet, e);

            for (var i = 0; i < _ia.Count; i++)
            {
                ia[i] = _ia[i];
                iUsed[i] = _iUsed[i];
                rca[i] = _rca[i];
                ru[i] = _ru[i];
                rl[i] = _rl[i];
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
            #endregion

            #region [Modifier]
            var getModifier = new GetModifier
            {
                pm = pmc,
                m = m
            }.Schedule(gun.Length, 32);
            getModifier.Complete();
            #endregion
        }
    }
}
