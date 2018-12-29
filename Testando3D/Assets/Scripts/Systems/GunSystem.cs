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
            for(var i = 0; i < gun.Length; i++)
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
                NativeArray<Entity> _bullet = new NativeArray<Entity>(1, Allocator.Temp);
                GameManager.entityManager.Instantiate(GameManager.bullet, _bullet);
                GameManager.entityManager.SetComponentData(_bullet[0], new Position { Value = bocalT.position });
                GameManager.entityManager.SetComponentData(_bullet[0], new Rotation { Value = gunComponent.player.transform.Find("FisrtPersonCamera").rotation });
                _bullet.Dispose();

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

            Collider[] hits;
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
