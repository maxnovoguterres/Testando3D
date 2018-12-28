using Assets.Scripts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

namespace Assets.Scripts.Systems
{
    public class MoveForwardSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct ItemsJob : IJobProcessComponentData<Position, _SpeedComponent, Rotation>
        {
            public float deltaTime;

            public void Execute(ref Position position, [ReadOnly]ref _SpeedComponent speed, ref Rotation rotation)
            {
                var _position = position.Value;
                _position += speed.Value * math.forward(rotation.Value) * deltaTime;
                position.Value = _position;
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
