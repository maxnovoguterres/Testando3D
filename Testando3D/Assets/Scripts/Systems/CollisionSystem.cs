using System.Collections.Generic;
using Unity.Jobs;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    public class CollisionSystem : MonoBehaviour
    {
        public static CollisionSystem Instance;

        public static List<Entity> entities;
        NativeArray<Vector3> PreviousPos;
        List<Vector3> _PreviousPos = new List<Vector3>();
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

        struct IntegratePhysics : IJobParallelFor
        {
            public NativeArray<RaycastHit> Hits;

            public void Execute(int i)
            {
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
                    _PreviousPos.Add(GameManager.entityManager.GetComponentData<Position>(entities[i]).Value);
                return;
            }

            PreviousPos = new NativeArray<Vector3>(_PreviousPos.Count, Allocator.TempJob);
            Pos = new NativeArray<Vector3>(_PreviousPos.Count, Allocator.TempJob);

            for (var i = 0; i < _PreviousPos.Count; i++)
            {
                PreviousPos[i] = _PreviousPos[i];
                Pos[i] = GameManager.entityManager.GetComponentData<Position>(entities[i]).Value;
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

            var _Collision = new IntegratePhysics()
            {
                Hits = raycastHits
            }.Schedule(PreviousPos.Length, 32, raycastDependency);

            _Collision.Complete();

            for (var i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].normal != Vector3.zero)
                {
                    GameManager.entityManager.DestroyEntity(entities[i]);
                    entities.RemoveAt(i);
                }
            }

            raycastCommands.Dispose();
            raycastHits.Dispose();
            Pos.Dispose();
            PreviousPos.Dispose();

            _PreviousPos = new List<Vector3>();
            for (var i = 0; i < entities.Count; i++)
                _PreviousPos.Add(GameManager.entityManager.GetComponentData<Position>(entities[i]).Value);
        }

        private void OnDisable()
        {
            //Pos.Dispose();
            //PreviousPos.Dispose();
        }

    }
    //[BurstCompile]
    //public class CollisionSystem : ComponentSystem
    //{
    //    public struct Item
    //    {
    //        public ComponentDataArray<Components.Collision> collision;
    //        public ComponentDataArray<Position> position;
    //        public EntityArray Entities;
    //        public readonly int Length;
    //    }

    //    [Inject] Item itens;

    //    protected override void OnUpdate()
    //    {
    //        Debug.Log(itens.Length);

    //        NativeArray<JobHandle> rayCastJobs = new NativeArray<JobHandle>();
    //        for (var i = 0; i < itens.Length; i++)
    //        {
    //            Debug.Log("asd");
    //            var col = itens.collision[i];
    //            var pos = itens.position[i].Value;
    //            if (i == 0)
    //            {
    //                rayCastJobs[i] = new RayCastJob()
    //                {
    //                    startPos = pos,
    //                    endPos = col.PreviousPos
    //                }.Schedule(itens.Length, 32);
    //            }
    //            else
    //            {
    //                rayCastJobs[i] = new RayCastJob()
    //                {
    //                    startPos = pos,
    //                    endPos = col.PreviousPos
    //                }.Schedule(itens.Length, 32, rayCastJobs[i - 1]);
    //            }
    //            //if (!col.PreviousPos.Equals(new float3()) && !col.PreviousPos.Equals(pos.Value))
    //            //    DetectMovingCollision(pos.Value, col.PreviousPos, itens.Entities[i]);
    //            //else
    //            //    DetectCollision(pos.Value, col.Radius);
    //        }
    //        rayCastJobs.Dispose();

    //    }

    //    struct RayCastJob : IJobParallelFor
    //    {
    //        public Vector3 startPos;
    //        public Vector3 endPos;

    //        public void Execute(int index)
    //        {
    //            var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
    //            var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);

    //            commands[0] = new RaycastCommand(startPos, endPos);

    //            var handle = RaycastCommand.ScheduleBatch(commands, results, 1);

    //            handle.Complete();

    //            RaycastHit batchedHit = results[0];
    //            Debug.Log(batchedHit);

    //            results.Dispose();
    //            commands.Dispose();
    //        }
    //    }

    //public void DetectCollision(Vector3 pos, float radius)
    //{
    //    Debug.Log("Collision Method 1");
    //    Collider[] hits = Physics.OverlapSphere(pos, radius);
    //    if (hits.Length > 0)
    //    {
    //        for (var i = 0; i < hits.Length; i++)
    //        {
    //            Debug.Log(hits[i].name);
    //        }
    //    }
    //}
    //public void DetectMovingCollision(Vector3 startPos, Vector3 endPos, Entity entity)
    //{
    //    Debug.Log("Collision Method 2");

    //    RaycastHit hit;
    //    if (Physics.Linecast(startPos, endPos, out hit))
    //    {
    //        Debug.Log("Destroyed");
    //        GameManager.entityManager.DestroyEntity(entity);
    //    }
    //}
    //}

    //[BurstCompile]
    //public class CollisionSystem : JobComponentSystem
    //{
    //    public struct CollisionObjects : IJobProcessComponentDataWithEntity<Components.Collision, Position>
    //    {
    //        public float deltaTime;
    //        public void Execute(Entity entity, int index, ref Components.Collision col, ref Position pos)
    //        {
    //            if (!col.PreviousPos.Equals(new float3()) && !col.PreviousPos.Equals(pos.Value))
    //                DetectMovingCollision(pos.Value, col.PreviousPos, entity);
    //            else
    //                DetectCollision(pos.Value, col.Radius);

    //            col.PreviousPos = pos.Value;
    //        }

    //        public void DetectCollision(Vector3 pos, float radius)
    //        {
    //            Debug.Log("Collision Method 1");

    //            //var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
    //            //var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);

    //            //Vector3 direction = Vector3.forward;

    //            //commands[0] = new RaycastCommand(pos, direction);

    //            //var handle = RaycastCommand.ScheduleBatch(commands, results, 1);

    //            //handle.Complete();

    //            //RaycastHit batchedHit = results[0];

    //            //results.Dispose();
    //            //commands.Dispose();

    //            //Collider[] hits = Physics.OverlapSphere(pos, radius);
    //            //if (hits.Length > 0)
    //            //{
    //            //    for (var i = 0; i < hits.Length; i++)
    //            //    {
    //            //        Debug.Log(hits[i].name);
    //            //    }
    //            //}
    //        }
    //        public void DetectMovingCollision(Vector3 startPos, Vector3 endPos, Entity entity)
    //        {
    //            Debug.Log("Collision Method 2");


    //            //var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
    //            //var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);

    //            //commands[0] = new RaycastCommand(startPos, endPos);

    //            //var handle = RaycastCommand.ScheduleBatch(commands, results, 1);

    //            //handle.Complete();

    //            //RaycastHit batchedHit = results[0];

    //            //results.Dispose();
    //            //commands.Dispose();

    //            //RaycastHit hit;
    //            //if (Physics.Linecast(startPos, endPos, out hit))
    //            //{
    //            //    Debug.Log("Destroyed");
    //            //    GameManager.entityManager.DestroyEntity(entity);
    //            //}
    //        }
    //    }

    //    protected override JobHandle OnUpdate(JobHandle inputDeps)
    //    {
    //        return new CollisionObjects
    //        {
    //            deltaTime = Time.deltaTime
    //        }.ScheduleSingle(this, inputDeps);
    //    }
    //}
}