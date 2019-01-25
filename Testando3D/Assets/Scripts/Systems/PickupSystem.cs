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
            public ComponentDataArray<_Input> inputComponent;
            public ComponentDataArray<PlayerMovement> movementComponent;
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
            public NativeArray<float> _Dist;
            public NativeArray<Vector3> GunPos;
            public NativeArray<Vector3> CharPos;
            public NativeArray<byte> CanPickUp;

            public void Execute(int i)
            {
                _Dist[i] = Vector3.Distance(GunPos[i], CharPos[i]);
                CanPickUp[i] = (byte)(_Dist[i] <= Dist[i] ? 1 : 0);
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

        public struct NearstGun : IJobParallelFor
        {
            public NativeArray<float> Dist;
            public NativeArray<byte> CanPick;
            public NativeArray<int> Index;
            public NativeArray<float> MinDist;

            public void Execute(int i)
            {
                if (CanPick[i] == 1 && Dist[i] <= MinDist[0])
                {
                    Index[0] = i;
                    MinDist[0] = Dist[i];
                }
            }
        }

        protected override void OnUpdate()
        {
            var gunPos = new NativeArray<Vector3>(gun.Length, Allocator.TempJob);
            var charPos = new NativeArray<Vector3>(gun.Length, Allocator.TempJob);
            var charPosRay = new NativeArray<Vector3>(gun.Length, Allocator.TempJob);
            var dist = new NativeArray<float>(gun.Length, Allocator.TempJob);
            var _dist = new NativeArray<float>(gun.Length, Allocator.TempJob);
            var canPickUp = new NativeArray<byte>(gun.Length, Allocator.TempJob);

            for (var i = 0; i < gun.Length; i++)
            {
                gunPos[i] = gun.pickupComponent[i].boxCollider.bounds.center;
                dist[i] = gun.pickupComponent[i].distance;
                charPos[i] = player.transform[0].Find("FirstPersonCamera").position;
                charPosRay[i] = player.transform[0].Find("FirstPersonCamera").position - gunPos[i];
                Debug.DrawRay(gunPos[i], charPosRay[i], Color.red);
            }

            var checkPos = new CheckPos
            {
                GunPos = gunPos,
                CharPos = charPos,
                CanPickUp = canPickUp,
                Dist = dist,
                _Dist = _dist
            }.Schedule(gun.Length, 32);


            var commands = new NativeArray<RaycastCommand>(gun.Length, Allocator.TempJob);
            var results = new NativeArray<RaycastHit>(gun.Length, Allocator.TempJob);

            var checkPath = new CheckPath
            {
                GunPos = gunPos,
                CharPos = charPosRay,
                CanPickUp = canPickUp,
                commands = commands
            }.Schedule(gun.Length, 32, checkPos);

            var ray = RaycastCommand.ScheduleBatch(commands, results, 32, checkPath);

            ray.Complete();

            gunPos.Dispose();
            charPosRay.Dispose();
            canPickUp.Dispose();
            commands.Dispose();
            charPos.Dispose();
            dist.Dispose();

            var canPick = new NativeArray<byte>(gun.Length, Allocator.TempJob);

            for (var i = 0; i < gun.Length; i++)
            {
                canPick[i] = (byte)(results[i].normal == Vector3.zero || (results[i].collider != null && results[i].collider.tag != "Untagged") ? 1 : 0);
            }

            results.Dispose();

            var gunIndex = new NativeArray<int>(1, Allocator.TempJob);
            var minDist = new NativeArray<float>(1, Allocator.TempJob);
            gunIndex[0] = -1;
            minDist[0] = 100;

            var nearstGun = new NearstGun
            {
                Dist = _dist,
                CanPick = canPick,
                Index = gunIndex,
                MinDist = minDist
            }.Schedule(gun.Length, 32, ray);

            nearstGun.Complete();

            canPick.Dispose();
            minDist.Dispose();
            _dist.Dispose();

            var _i = gunIndex[0];

            if (_i != -1)
            {
                GameManager.Instance.pickUpText.text = "Press T to PICK " + gun.transform[_i].name;
                GameManager.Instance.gunToEquip = gun.pickupComponent[_i].equipment;
                GameManager.Instance.canEquip = true;
                GameManager.Instance.gunToDestroy = gun.transform[_i].gameObject;
            }
            else
            {
                GameManager.Instance.pickUpText.text = "";
                GameManager.Instance.canEquip = false;
            }

            gunIndex.Dispose();

            //if (Input.GetKeyDown(KeyCode.T) && GameManager.Instance.canEquip)
            //{
            //    EquipmentManager.instance.Equip(GameManager.Instance.gunToEquip, player.transform[0].gameObject);
            //    Object.Destroy(GameManager.Instance.gunToDestroy);
            //    GameManager.Instance.pickUpText.text = "";
            //    GameManager.Instance.canEquip = false;
            //}

        }
    }
}
