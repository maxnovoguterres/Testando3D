using System.Collections.Generic;
using Unity.Jobs;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    public class CollisionSystem : MonoBehaviour
    {
        public static CollisionSystem Instance;

        public static List<Entity> entities;
        NativeArray<Vector3> PreviousPos;
        List<Vector3> _PreviousPos = new List<Vector3>();
        List<Quaternion> Rot = new List<Quaternion>();
        NativeArray<Vector3> Pos;
        

        struct PrepareRaycastCommands : IJobParallelFor
        {
            [Unity.Collections.ReadOnly]
            public NativeArray<Vector3> startPos;
            [Unity.Collections.ReadOnly]
            public NativeArray<Vector3> endPos;
            public NativeArray<RaycastCommand> Raycasts;

            public void Execute(int i)
            {
                Raycasts[i] = new RaycastCommand(startPos[i], endPos[i]);
            }
        }

        private void Start()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(this);

            entities = new List<Entity>();
        }

        private void Update()
        {
            if (entities.Count == 0) return;

            if (_PreviousPos.Count == 0)
            {
                for (var i = 0; i < entities.Count; i++)
                {
                    Rot.Add(GameManager.entityManager.GetComponentData<Rotation>(entities[i]).Value);
                    _PreviousPos.Add(GameManager.entityManager.GetComponentData<Position>(entities[i]).Value);
                }
                return;
            }

            PreviousPos = new NativeArray<Vector3>(_PreviousPos.Count, Allocator.TempJob);
            Pos = new NativeArray<Vector3>(_PreviousPos.Count, Allocator.TempJob);

            for (var i = 0; i < _PreviousPos.Count; i++)
            {
                PreviousPos[i] = _PreviousPos[i];
                Pos[i] = GameManager.entityManager.GetComponentData<Position>(entities[i]).Value - (float3)PreviousPos[i];
                Debug.DrawRay(PreviousPos[i], Pos[i], Color.red);
            }

            var raycastCommands = new NativeArray<RaycastCommand>(PreviousPos.Length, Allocator.TempJob);
            var raycastHits = new NativeArray<RaycastHit>(PreviousPos.Length, Allocator.TempJob);

            var setupDependency = new PrepareRaycastCommands()
            {
                startPos = PreviousPos,
                endPos = Pos,
                Raycasts = raycastCommands
            }.Schedule(PreviousPos.Length, 32);

            var raycastDependency = RaycastCommand.ScheduleBatch(raycastCommands, raycastHits, 32, setupDependency);

            raycastDependency.Complete();

            for (var i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].normal != Vector3.zero)
                {
                    Rigidbody body = raycastHits[i].collider.attachedRigidbody;
                    if (body != null)
                        body.AddForceAtPosition(math.forward(Rot[i]) * 2, raycastHits[i].point, ForceMode.Impulse);

                    GameManager.entityManager.DestroyEntity(entities[i]);
                    entities.RemoveAt(i);
                }
            }

            raycastCommands.Dispose();
            raycastHits.Dispose();
            Pos.Dispose();
            PreviousPos.Dispose();

            Rot = new List<Quaternion>();
            _PreviousPos = new List<Vector3>();
            for (var i = 0; i < entities.Count; i++)
            {
                _PreviousPos.Add(GameManager.entityManager.GetComponentData<Position>(entities[i]).Value);
                Rot.Add(GameManager.entityManager.GetComponentData<Rotation>(entities[i]).Value);
            }
        }
    }
}