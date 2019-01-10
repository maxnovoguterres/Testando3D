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
        public struct Player
        {
            public ComponentArray<InputComponent> inputComponent;
            public ComponentArray<MovementComponent> movementComponent;
            public ComponentArray<Transform> transform;
            public ComponentArray<Animator> animator;
            public ComponentArray<Rigidbody> rb;
            public ComponentArray<CharacterController> characterController;
        }

        [Inject] Gun gun;
        [Inject] Player player;

        public struct CheckPos : IJobParallelFor
        {
            public NativeArray<float> Dist;
            public NativeArray<Vector3> GunPos;
            public NativeArray<Vector3> CharPos;
            public NativeArray<byte> CanPickUp;

            public void Execute(int i)
            {
                CanPickUp[i] = (byte)(Vector3.Distance(GunPos[i], CharPos[i]) <= Dist[i] ? 1 : 0);
            }
        }

        public struct CheckPath : IJobParallelFor
        {
            public NativeArray<Vector3> GunPos;
            public NativeArray<Vector3> CharPos;
            public NativeArray<byte> CanPickUp;
            public NativeArray<RaycastCommand> commands;

            public void Execute(int i)
            {
                if (CanPickUp[i] == 1)
                    commands[i] = new RaycastCommand(GunPos[i], CharPos[i]);
                else
                    commands[i] = new RaycastCommand();
            }
        }

        protected override void OnUpdate()
        {
            var gunPos = new NativeArray<Vector3>(gun.Length, Allocator.TempJob);
            var charPos = new NativeArray<Vector3>(gun.Length, Allocator.TempJob);
            var dist = new NativeArray<float>(gun.Length, Allocator.TempJob);
            var canPickUp = new NativeArray<byte>(gun.Length, Allocator.TempJob);

            for (var i = 0; i < gun.Length; i++)
            {
                gunPos[i] = gun.pickupComponent[i].boxCollider.bounds.center;
                dist[i] = gun.pickupComponent[i].distance;
                charPos[i] = player.transform[0].position;
                Debug.DrawRay(gunPos[i], charPos[i], Color.red);
            }

            var checkPosDep = new CheckPos
            {
                GunPos = gunPos,
                CharPos = charPos,
                CanPickUp = canPickUp,
                Dist = dist
            }.Schedule(gun.Length, 32);

            var commands = new NativeArray<RaycastCommand>(gun.Length, Allocator.TempJob);
            var results = new NativeArray<RaycastHit>(gun.Length, Allocator.TempJob);

            var checkPathDep = new CheckPath
            {
                GunPos = gunPos,
                CharPos = charPos,
                CanPickUp = canPickUp,
                commands = commands
            }.Schedule(gun.Length, 32, checkPosDep);

            var handle = RaycastCommand.ScheduleBatch(commands, results, 32, checkPathDep);

            handle.Complete();

            for (var i = 0; i < results.Length; i++)
            {
                if (results[i].normal == Vector3.zero && canPickUp[i] == 1)
                {
                    //if (results[i].collider.GetComponent<InputComponent>() == null) continue;

                    Debug.Log(gun.pickupComponent[i].gameObject);
                    //EquipmentManager.instance.Equip(gun.pickupComponent[i].equipment, results[i].collider.gameObject);
                    //Object.Destroy(gun.transform[i].gameObject);
                    continue;
                }
            }

            gunPos.Dispose();
            charPos.Dispose();
            commands.Dispose();
            results.Dispose();
            canPickUp.Dispose();
            dist.Dispose();
        }
    }
}
