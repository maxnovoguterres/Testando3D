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
using PhysicsEngine;
using Unity.Rendering;

namespace Assets.Scripts.Systems
{
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

        public struct Player
        {
            public InputComponent inputComponent;
            public MovementComponent movementComponent;
            public Transform transform;
            public Animator animator;
            public Rigidbody rb;
            public CharacterController characterController;
        }

        [Inject] Gun gun;

        protected override void OnCreateManager()
        {
            for (var i = 0; i < gun.Length; i++)
            {
                gun.gunComponent[i].countDown = new CountDown(gun.gunComponent[i].countDownRate);
            }
        }

        protected override void OnUpdate()
        {
            for (var i = 0; i < gun.Length; i++)
            {
                if (gun.gunComponent[i].countDown == null)
                    gun.gunComponent[i].countDown = new CountDown(gun.gunComponent[i].countDownRate);

                Fire(gun.gunComponent[i], gun.gunComponent[i].bocal);
                Picked(gun.transform[i], gun.gunComponent[i], gun.pickupComponent[i]);
            }
        }

        void Fire(GunComponent gunComponent, Transform bocalT)
        {
            CountDown.DecreaseTime(gunComponent.countDown);

            if (gunComponent.player == null || gunComponent.countDown.CoolDown > 0) return;

            if (gunComponent.player.GetComponent<InputComponent>().Shoot)
            {
                //NativeArray<Entity> _bullet = new NativeArray<Entity>(1, Allocator.Temp);
                //GameManager.entityManager.Instantiate(GameManager.bullet, _bullet);
                //GameManager.entityManager.SetComponentData(_bullet[0], new Position { Value = bocalT.position });
                //GameManager.entityManager.SetComponentData(_bullet[0], new Rotation { Value = gunComponent.player.transform.Find("FisrtPersonCamera").rotation });

                Entity rigidBody = GameManager.entityManager.CreateEntity(GameManager.KineticRigidBodyArchetype);
                GameManager.entityManager.SetComponentData(rigidBody, new Position { Value = bocalT.position });
                GameManager.entityManager.SetComponentData(rigidBody, new RigidBody { IsKinematic = 0, InverseMass = 1f, MomentOfInertia = float3x3.identity });
                GameManager.entityManager.SetComponentData(rigidBody, new Velocity { Value = 0 });
                GameManager.entityManager.SetComponentData(rigidBody, new Rotation { Value = gunComponent.player.transform.Find("FisrtPersonCamera").rotation });
                GameManager.entityManager.SetComponentData(rigidBody, new Scale { Value = new float3(0.01f, 0.02f, 0.02f) });
                //GameManager.entityManager.AddComponentData(rigidBody, new _SpeedComponent { Value = 100 });

                Entity sphereCollider = GameManager.entityManager.CreateEntity(GameManager.SphereColliderArchetype);
                GameManager.entityManager.SetComponentData(sphereCollider, new PhysicsEngine.Collider { RigidBodyEntity = rigidBody });
                GameManager.entityManager.SetComponentData(sphereCollider, new PhysicsEngine.SphereCollider { Radius = 0.5f });
                GameManager.entityManager.SetComponentData(sphereCollider, new PhysicsEngine.ColliderPhysicsProperties { Friction = 0f, CoefficientOfRestitution = 0f });

                Entity sphereRender = GameManager.entityManager.CreateEntity(GameManager.SimpleRendererArchetype);
                GameManager.entityManager.SetSharedComponentData(sphereRender, GameManager.bullet.GetComponent<MeshInstanceRendererComponent>().Value);
                GameManager.entityManager.SetComponentData(sphereRender, new Unity.Transforms.Position { Value = bocalT.position });
                GameManager.entityManager.SetComponentData(sphereRender, new FollowRigidBody { RigidBodyEntity = rigidBody });
                GameManager.entityManager.SetComponentData(sphereRender, new Rotation { Value = gunComponent.player.transform.Find("FisrtPersonCamera").rotation });
                GameManager.entityManager.SetComponentData(sphereRender, new Scale { Value = new float3(0.01f, 0.02f, 0.02f) });
                //GameManager.entityManager.AddComponentData(sphereRender, new _SpeedComponent { Value = 100 });

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
    }
}
