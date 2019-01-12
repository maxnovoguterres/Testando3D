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

        protected override void OnUpdate()
        {
            Debug.Log(objectsToDestroy.Length);
            List<Entity> e = new List<Entity>();

            for (int i = 0; i < objectsToDestroy.Length; i++)
            {
                if (objectsToDestroy.destroyAfterTime[i].delete)
                {
                    e.Add(objectsToDestroy.e[i]);
                }
            }
            for (int i = 0; i < e.Count; i++)
            {
                GameManager.entityManager.DestroyEntity(e[i]);
            }
        }
    }
}

