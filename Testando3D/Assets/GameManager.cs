using Assets.Scripts.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static EntityManager entityManager;
    [Header("Prefabs")]
    public static GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        bullet = Resources.Load("Prefabs/Bullet") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
