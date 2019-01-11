using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static EntityManager entityManager;
    [Header("Prefabs")]
    public Mesh bullet;

    [Header("Canvas")]
    public Image scopeOverlay;
    public Text pickUpText;

    public Equipment gunToEquip;
    public bool canEquip;
    public GameObject gunToDestroy;

    public bool test;
    private float jumpOffSet = 0f;
    public static float GravityAceleration = 9.81f;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);

        entityManager = World.Active.GetOrCreateManager<EntityManager>();

        Physics.IgnoreLayerCollision(8, 9, true);
        //bullet = Resources.Load("Prefabs/Bullet") as GameObject;
    }

    private void Update()
    {
        if (test)
        {
            StartCoroutine(DoBobCycle());
            test = false;
        }
    }

    public float JumpOffSet()
    {
        return jumpOffSet;
    }

    public IEnumerator DoBobCycle()
    {
        // make the camera move down slightly
        float t = 0f;
        while (t < 0.2f)
        {
            jumpOffSet = Mathf.Lerp(0f, 0.1f, t / 0.2f);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        // make it move back to neutral
        t = 0f;
        while (t < 0.1f)
        {
            jumpOffSet = Mathf.Lerp(0.1f, 0f, t / 0.2f);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        jumpOffSet = 0f;
    }
}
