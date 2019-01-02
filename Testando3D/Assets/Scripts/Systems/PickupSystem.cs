using Assets.Scripts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class PickupSystem : ComponentSystem
    {
        public struct Gun
        {
            public ComponentArray<Transform> transform;
            public ComponentArray<PickupComponent> pickupComponent;
            public EntityArray Entities;
            public readonly int Length;
        }

        [Inject] Gun gun;

        protected override void OnUpdate()
        {
            for (var i = 0; i < gun.Length; i++)
            {
                Picked(gun.transform[i], gun.pickupComponent[i]);
            }
        }

        void Picked(Transform transform, PickupComponent pickupComponent)
        {
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
