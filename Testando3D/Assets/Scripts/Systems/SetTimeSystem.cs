using System.Collections.Generic;
using Unity.Jobs;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Assets.Scripts.Components;

namespace Assets.Scripts.Systems
{
    public class Barrier : BarrierSystem { }

    public class SetTimeSystem : JobComponentSystem
    {
        public struct SetTime : IJobParallelFor
        {
            public float deltaTime;
            [ReadOnly] public EntityCommandBuffer Cmd;
            public EntityArray entity;
            public NativeArray<DestroyAfterTime> dAT;

            public void Execute(int i)
            {
                //dAT[i].lifeTime += deltaTime;
                //if (dAT[i].delete)
                //if (GameManager.entityManager.Exists(entity[i]))
                //    Cmd.DestroyEntity(entity[i]);
                //GameManager.entityManager.DestroyEntity(entity[i]);
            }
        }

        public struct ObjectsToDestroy
        {
            public ComponentDataArray<DestroyAfterTime> destroyAfterTime;
            public EntityArray e;
            public readonly int Length;
        }

        [Inject] ObjectsToDestroy objectsToDestroy;

        [Inject] private Barrier m_Barrier;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var dAT = new NativeArray<DestroyAfterTime>(objectsToDestroy.Length, Allocator.TempJob);
            for (int i = 0; i < objectsToDestroy.Length; i++)
            {
                //objectsToDestroy.destroyAfterTime[i].lifeTime += Time.deltaTime;
                dAT[i] = objectsToDestroy.destroyAfterTime[i];
            }
            var setTime = new SetTime
            {
                deltaTime = Time.deltaTime,
                Cmd = m_Barrier.CreateCommandBuffer(),
                dAT = dAT,
                entity = objectsToDestroy.e
            }.Schedule(objectsToDestroy.Length, 32);
            setTime.Complete();
            dAT.Dispose();

            return setTime;
        }

        //protected override void OnStopRunning()
        //{
        //    for (int i = 0; i < objectsToDestroy.Lenght; i++)
        //    {
        //        if (objectsToDestroy.destroyAfterTime[i].delete)
        //        {
        //            GameManager.entityManager.DestroyEntity(objectsToDestroy.e[i]);
        //        }
        //    }
        //}
    }
}