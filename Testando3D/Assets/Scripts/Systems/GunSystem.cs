using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Entities;
using Assets.Scripts.Components;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
    public class GunSystem : ComponentSystem
    {
        public struct Gun
        {
            public GunComponent gunComponent;
        }

        public struct Player
        {

        }

        protected override void OnCreateManager()
        {

        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> bullet = new NativeArray<Entity>(1, Allocator.Temp);
            GameManager.entityManager.Instantiate(GameManager.bullet, bullet);
            GameManager.entityManager.AddComponentData(bullet[0], new SpeedComponent { Value = 0.1f });
            GameManager.entityManager.SetComponentData(bullet[0], new Position { Value = item.transform.position });
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
