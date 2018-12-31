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

    public static float GravityAceleration = 9.81f;


    void Start()
    {
        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        bullet = Resources.Load("Prefabs/Bullet") as GameObject;
    }
}
