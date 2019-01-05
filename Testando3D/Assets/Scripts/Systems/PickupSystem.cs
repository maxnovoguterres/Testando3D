using Assets.Scripts.Components;
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
                orientation[i] = Quaternion.LookRotation(new Vector3(0, 1, 0));
                direction[i] = new Vector3(0, 1, 0);
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
                if (results[i].normal != Vector3.zero)
                {
                    if (results[i].collider.GetComponent<InputComponent>() == null) continue;

                    EquipmentManager.instance.Equip(gun.pickupComponent[i].equipment, results[i].collider.gameObject);
                    Helpers.StandardMethods.Destroy(gun.transform[i].gameObject);
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
