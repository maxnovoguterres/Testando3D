using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static EntityManager entityManager;
    [Header("Prefabs")]
    public GameObject bullet;

    [Header("Canvas")]
    public Image scopeOverlay;

    public static float GravityAceleration = 9.81f;


    void Start()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);

        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        bullet = Resources.Load("Prefabs/Bullet") as GameObject;
    }
}
