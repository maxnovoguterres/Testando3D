using Assets.Scripts.Buffers;
using Assets.Scripts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

namespace Assets.Scripts.Helpers
{
    public static class EntityArchetypes
    {
        public static EntityArchetype bullet = GameManager.entityManager.CreateArchetype(ComponentType.Create<Position>(), ComponentType.Create<Rotation>(), ComponentType.Create<Speed>(), ComponentType.Create<Components.Collision>(), ComponentType.Create<Scale>(), ComponentType.Create<Gravity>(), ComponentType.Create<MeshInstanceRenderer>(), typeof(DirectionBuffer), ComponentType.Create<DestroyAfterTime>());
        public static EntityArchetype standardObject = GameManager.entityManager.CreateArchetype(ComponentType.Create<Position>(), ComponentType.Create<Rotation>(), ComponentType.Create<Scale>(), ComponentType.Create<MeshInstanceRenderer>(), ComponentType.Create<Scene>());
    }
}
