using Assets.Scripts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

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

        public struct Pick : IJobParallelFor
        {
            public NativeArray<Vector3> Center;
            public NativeArray<Vector3> HalfExtents;
            public NativeArray<Quaternion> Orientation;
            public NativeArray<Vector3> Direction;
            public NativeArray<BoxcastCommand> commands;

            public void Execute(int i)
            {
                commands[i] = new BoxcastCommand(Center[i], HalfExtents[i], Orientation[i], Direction[i]);
            }
        }

        protected override void OnUpdate()
        {
            var center = new NativeArray<Vector3>(gun.Length, Allocator.TempJob);
            var halfExtents = new NativeArray<Vector3>(gun.Length, Allocator.TempJob);
            var orientation = new NativeArray<Quaternion>(gun.Length, Allocator.TempJob);
            var direction = new NativeArray<Vector3>(gun.Length, Allocator.TempJob);
            var commands = new NativeArray<BoxcastCommand>(gun.Length, Allocator.TempJob);
            var results = new NativeArray<RaycastHit>(gun.Length, Allocator.TempJob);

            for (var i = 0; i < gun.Length; i++)
            {
                center[i] = gun.pickupComponent[i].boxCollider.bounds.center;
                halfExtents[i] = gun.pickupComponent[i].boxCollider.bounds.size;
                orientation[i] = Quaternion.LookRotation(Vector3.right);//gun.transform[i].rotation;
                direction[i] = gun.transform[i].right;//Vector3.one * .5f;//gun.pickupComponent[i].direction;
                //Picked(gun.transform[i], gun.pickupComponent[i]);
            }

            var pickDependency = new Pick
            {
                Center = center,
                Direction = direction,
                HalfExtents = halfExtents,
                Orientation = orientation,
                commands = commands
            }.Schedule(gun.Length, 32);

            var handle = BoxcastCommand.ScheduleBatch(commands, results, 32, pickDependency);

            handle.Complete();

            for (var i = 0; i < results.Length; i++)
            {
                //Debug.Log(results[i].normal);
                if (results[i].normal != Vector3.zero)
                {
                    if (results[i].collider.GetComponent<InputComponent>() == null) continue;
                    Debug.Log("asd");

                    EquipmentManager.instance.Equip(gun.pickupComponent[i].equipment, results[i].collider.gameObject);
                    break;
                }
            }

            center.Dispose();
            halfExtents.Dispose();
            orientation.Dispose();
            direction.Dispose();
            commands.Dispose();
            results.Dispose();
        }

        //void Picked(Transform transform, PickupComponent pickupComponent)
        //{
        //    UnityEngine.Collider[] hits;
        //    hits = Physics.OverlapSphere(transform.position, pickupComponent.radius);
        //    foreach (var hit in hits)
        //    {
        //        if (hit.GetComponent<InputComponent>() == null) continue;

        //        EquipmentManager.instance.Equip(pickupComponent.equipment, hit.gameObject);
        //        break;
        //    }
        //    //StandardMethods.Destroy(transform.gameObject);
        //}
    }
}
