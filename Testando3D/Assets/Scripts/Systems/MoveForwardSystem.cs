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
using Assets.Scripts.Helpers;
using Assets.Scripts.Buffers;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    public class MoveForwardSystem : JobComponentSystem
    {
        private struct ItemsJob : IJobProcessComponentDataWithEntity<Position, Speed, Rotation>
        {
            public float deltaTime;

            public void Execute(Entity entity, int index, ref Position position, ref Speed speed, ref Rotation rotation)
            {
                var pos = position.Value;
                pos += speed.Value * math.forward(rotation.Value) * deltaTime;

                var directionBuffer = GameManager.entityManager.GetBuffer<MoveForwardDirectionBuffer>(entity);

                if (directionBuffer.Length > 0)
                    for (var i = 0; i < directionBuffer.Length; i++)
                    {
                        if (directionBuffer[i].Value == MoveForwardDirectionBuffer.MoveFowardDirectionX) pos.x += (speed.Value * math.forward(rotation.Value) * deltaTime).x;
                        if (directionBuffer[i].Value == MoveForwardDirectionBuffer.MoveFowardDirectionY) pos.y += (speed.Value * math.forward(rotation.Value) * deltaTime).y;
                        if (directionBuffer[i].Value == MoveForwardDirectionBuffer.MoveFowardDirectionZ) pos.z += (speed.Value * math.forward(rotation.Value) * deltaTime).z;
                    }
                else
                    pos += speed.Value * math.forward(rotation.Value) * deltaTime;

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
