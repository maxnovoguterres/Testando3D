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
    public class MyBarrier : BarrierSystem { }
    [BurstCompile]
    public class DestroyAfterTimeSystem : ComponentSystem
    {
        public struct ObjectsToDestroy
        {
            public ComponentDataArray<DestroyAfterTime> destroyAfterTime;
            public EntityArray e;
            public readonly int Length;
        }

        [Inject] ObjectsToDestroy objectsToDestroy;

        [Inject] private MyBarrier m_Barrier;

        protected override void OnUpdate()
        {
            //Debug.Log(objectsToDestroy.Length);
            //List<Entity> e = new List<Entity>();

            //for (int i = 0; i < objectsToDestroy.Length; i++)
            //{
            //    if (objectsToDestroy.destroyAfterTime[i].delete)
            //    {
            //        e.Add(objectsToDestroy.e[i]);
            //    }
            //}
            //NativeArray<Entity> _e = new NativeArray<Entity>(e.Count, Allocator.Temp);
            //for (int i = 0; i < e.Count; i++)
            //{
            ////PostUpdateCommands.DestroyEntity(e[i]);
            //    _e[i] = e[i];
            //}
            //GameManager.entityManager.DestroyEntity(_e);
            //_e.Dispose();
        }
    }
}