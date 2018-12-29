using Assets.Scripts.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static EntityManager entityManager;
    [Header("Prefabs")]
    public static GameObject bullet;


    public static EntityArchetype KineticRigidBodyArchetype;
    public static EntityArchetype KinematicRigidBodyArchetype;
    public static EntityArchetype SphereColliderArchetype;
    public static EntityArchetype SimpleRendererArchetype;

    void Start()
    {
        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        bullet = Resources.Load("Prefabs/Bullet") as GameObject;

        KineticRigidBodyArchetype = PhysicsEntityFactory.CreateKineticRigidbodyArchetype(GameManager.entityManager);
        KinematicRigidBodyArchetype = PhysicsEntityFactory.CreateKinematicRigidbodyArchetype(GameManager.entityManager);
        SphereColliderArchetype = PhysicsEntityFactory.CreateSphereColliderArchetype(GameManager.entityManager);

        SimpleRendererArchetype = GameManager.entityManager.CreateArchetype(
            typeof(Unity.Transforms.Position),
            typeof(Unity.Transforms.Rotation),
            typeof(Unity.Transforms.Scale),
            typeof(MeshInstanceRenderer),
            typeof(PhysicsEngine.FollowRigidBody)
            );
    }
}
