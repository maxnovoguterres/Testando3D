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
    public class SetTimeSystem : JobComponentSystem
    {
        public struct SetTime : IJobProcessComponentDataWithEntity<Components.DestroyAfterTime>
        {
            public float deltaTime;

            public void Execute(Entity entity, int index, ref DestroyAfterTime data)
            {
                Debug.Log("sdasdasd");
                data.lifeTime += deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new SetTime
            {
                deltaTime = Time.deltaTime
            }.ScheduleSingle(this, inputDeps);
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