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
                var _pos = speed.Value * math.forward(rotation.Value) * deltaTime;

                var directionBuffer = GameManager.entityManager.GetBuffer<DirectionBuffer>(entity);

                if (directionBuffer.Length > 0)
                    for (var i = 0; i < directionBuffer.Length; i++)
                    {
                        if (directionBuffer[i].Value == DirectionBuffer.MoveFowardDirectionX) pos.x += _pos.x;
                        if (directionBuffer[i].Value == DirectionBuffer.MoveFowardDirectionY) pos.y += _pos.y;
                        if (directionBuffer[i].Value == DirectionBuffer.MoveFowardDirectionZ) pos.z += _pos.z;
                    }
                else
                    pos += _pos;

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
