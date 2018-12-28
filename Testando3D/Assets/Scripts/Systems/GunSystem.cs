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

namespace Assets.Scripts.Systems
{
    public class GunSystem : ComponentSystem
    {
        public struct Gun
        {
            public GunComponent gunComponent;
            public Transform transform;
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

        protected override void OnCreateManager()
        {

        }

        protected override void OnUpdate()
        {
            foreach (var gun in GetEntities<Gun>())
            {
                if (gun.gunComponent.player == null) continue;

                if (gun.gunComponent.player.GetComponent<InputComponent>().Shoot)
                {
                    NativeArray<Entity> bullet = new NativeArray<Entity>(1, Allocator.Temp);
                    GameManager.entityManager.Instantiate(GameManager.bullet, bullet);
                    GameManager.entityManager.AddComponentData(bullet[0], new SpeedComponent { Value = 0.1f });
                    GameManager.entityManager.SetComponentData(bullet[0], new Position { Value = gun.transform.transform.position });
                    GameManager.entityManager.SetComponentData(bullet[0], new Rotation { Value = GameManager.bullet.transform.rotation });
                    GameManager.entityManager.SetComponentData(bullet[0], new Scale { Value = GameManager.bullet.transform.localScale });

                    bullet.Dispose();
                    RaycastHit[] hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    hit = Physics.RaycastAll(ray);
                    foreach (var _item in hit)
                    {
                        Rigidbody body = _item.collider.attachedRigidbody;
                        if (body != null)
                        {
                            Debug.Log(_item.transform.name);
                            body.AddForceAtPosition(Vector3.forward, _item.point, ForceMode.Impulse);
                        }
                    }
                }
            }
        }

        void Picked(Transform transform, PickupComponent pickupComponent)
        {
            Collider[] hits;
            hits = Physics.OverlapSphere(transform.position, pickupComponent.radius);
            foreach(var hit in hits)
            {
                if (hit.GetComponent<InputComponent>() == null) continue;
                EquipmentManager.instance.Equip(pickupComponent.equipment);
                break;
            }
        }
    }
}
