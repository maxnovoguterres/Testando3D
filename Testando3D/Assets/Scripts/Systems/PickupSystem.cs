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
            public EntityArray Entities;
            public readonly int Length;
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
            public NativeArray<int> GunIndex;
            public NativeArray<float> MinDist;
            public NativeArray<int> PlayerIndexRef;
            public int gunLength;

            public void Execute(int i)
            {
                if (CanPick[i] == 1 && Dist[i] <= MinDist[PlayerIndexRef[i]])
                {
                    GunIndex[PlayerIndexRef[i]] = i - (PlayerIndexRef[i] * gunLength);
                    MinDist[PlayerIndexRef[i]] = Dist[i];
                }
            }
        }
        NativeArray<Vector3> gunPos;
        NativeArray<Vector3> charPos;
        NativeArray<Vector3> charPosRay;
        NativeArray<float> dist;
        NativeArray<float> _dist;
        NativeArray<byte> canPickUp;
        NativeArray<int> playerIndexRef;


        protected override void OnUpdate()
        {
            Debug.Log(player.Length);

            int _length = gun.Length * player.Length;
            gunPos = new NativeArray<Vector3>(_length, Allocator.TempJob);
            charPos = new NativeArray<Vector3>(_length, Allocator.TempJob);
            charPosRay = new NativeArray<Vector3>(_length, Allocator.TempJob);
            dist = new NativeArray<float>(_length, Allocator.TempJob);
            _dist = new NativeArray<float>(_length, Allocator.TempJob);
            canPickUp = new NativeArray<byte>(_length, Allocator.TempJob);
            playerIndexRef = new NativeArray<int>(_length, Allocator.TempJob);

            for (int j = 0; j < player.Length; j++)
            {
                for (var i = 0; i < gun.Length; i++)
                {
                    playerIndexRef[i + (j * gun.Length)] = j;
                    gunPos[i + (j * gun.Length)] = gun.pickupComponent[i].boxCollider.bounds.center;
                    dist[i + (j * gun.Length)] = gun.pickupComponent[i].distance;
                    charPos[i + (j * gun.Length)] = player.transform[j].Find("FirstPersonCamera").position;
                    charPosRay[i + (j * gun.Length)] = player.transform[j].Find("FirstPersonCamera").position - gunPos[i];
                    Debug.DrawRay(gunPos[i], charPosRay[i], Color.red);
                }
            }

            var checkPos = new CheckPos
            {
                GunPos = gunPos,
                CharPos = charPos,
                CanPickUp = canPickUp,
                Dist = dist,
                _Dist = _dist
            }.Schedule(_length, 32);


            var commands = new NativeArray<RaycastCommand>(_length, Allocator.TempJob);
            var results = new NativeArray<RaycastHit>(_length, Allocator.TempJob);

            var checkPath = new CheckPath
            {
                GunPos = gunPos,
                CharPos = charPosRay,
                CanPickUp = canPickUp,
                commands = commands
            }.Schedule(_length, 32, checkPos);

            var ray = RaycastCommand.ScheduleBatch(commands, results, 32, checkPath);

            ray.Complete();

            gunPos.Dispose();
            charPosRay.Dispose();
            canPickUp.Dispose();
            commands.Dispose();
            charPos.Dispose();
            dist.Dispose();

            var canPick = new NativeArray<byte>(_length, Allocator.TempJob);

            for (var i = 0; i < _length; i++)
            {
                canPick[i] = (byte)(results[i].normal == Vector3.zero || (results[i].collider != null && results[i].collider.tag != "Untagged") ? 1 : 0);
            }

            results.Dispose();

            var gunIndex = new NativeArray<int>(player.Length, Allocator.TempJob);
            var minDist = new NativeArray<float>(player.Length, Allocator.TempJob);

            for (int i = 0; i < player.Length; i++)
            {
                gunIndex[i] = -1;
                minDist[i] = 100;
            }
            var nearstGun = new NearstGun
            {
                Dist = _dist,
                CanPick = canPick,
                PlayerIndexRef = playerIndexRef,
                MinDist = minDist,
                GunIndex = gunIndex,
                gunLength = gun.Length

            }.Schedule(_length, 32, ray);

            nearstGun.Complete();

            canPick.Dispose();
            minDist.Dispose();
            _dist.Dispose();
            playerIndexRef.Dispose();

            for (int i = 0; i < player.Length; i++)
            {

                var _i = gunIndex[i];
                if (_i != -1)
                {
                    Debug.Log($"Player {i} Near of {gun.transform[_i].name}");
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
