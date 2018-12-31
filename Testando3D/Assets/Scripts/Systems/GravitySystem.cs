using Assets.Scripts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    public class GravitySystem : JobComponentSystem
    {
        private struct ItemsJob : IJobProcessComponentData<Position, Gravity>
        {
            public float deltaTime;
            
            public void Execute(ref Position position, ref Gravity gravity)
            {
                gravity.Time = gravity.Time + deltaTime;
                var pos = position.Value;
                pos.y = gravity.InitPosY + gravity.InitVel * gravity.Time - .5f * GameManager.GravityAceleration * math.pow(gravity.Time, 2);
                position.Value = pos;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new ItemsJob
            {
                deltaTime = Time.deltaTime
            }.ScheduleSingle(this, inputDeps);
        }
    }
}
