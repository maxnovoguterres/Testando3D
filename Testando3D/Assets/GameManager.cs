using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static EntityManager entityManager;
    public static EntityManager deleteEntityManager;
    public static World deleteWorld;

    [Header("Prefabs")]
    public Mesh bullet;

    [Header("Canvas")]
    public Image redDot;
    public Image scopeOverlay;
    public Text pickUpText;

    public Equipment gunToEquip;
    public bool canEquip;
    public GameObject gunToDestroy;

    public static float GravityAceleration = 9.81f;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);

        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        deleteWorld = new World("deleteWorld");
        deleteEntityManager = deleteWorld.GetOrCreateManager<EntityManager>();

        Physics.IgnoreLayerCollision(8, 9, true);
        //bullet = Resources.Load("Prefabs/Bullet") as GameObject;
    }
}
