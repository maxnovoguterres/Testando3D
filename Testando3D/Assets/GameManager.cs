using Assets.Scripts.ECSWindows;
using Assets.Scripts.Helpers;
using Assets.Scripts.PureECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static EntityManager entityManager;

    //[Header("Prefabs")]
    [HideInInspector] public Mesh bullet;

    [Header("Canvas")]
    public Image redDot;
    public Image scopeOverlay;
    public Text pickUpText;

    public GameObject ECSColliders;

    [HideInInspector] public Equipment gunToEquip;
    [HideInInspector] public bool canEquip;
    [HideInInspector] public GameObject gunToDestroy;

    public static float GravityAceleration = 9.81f;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);

        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        bullet = ((GameObject)Resources.Load("Mesh/bullet")).GetComponent<MeshFilter>().sharedMesh;
        Physics.IgnoreLayerCollision(8, 9, true);
        //bullet = Resources.Load("Prefabs/Bullet") as GameObject;
        SetObjectInScene();
    }

    void SetObjectInScene()
    {
        var meshes = new Dictionary<MeshInfo, Mesh>();
        var materials = new Dictionary<string, Material>();

        NativeArray<Entity> e = new NativeArray<Entity>(SceneObjectsData.position.Count, Allocator.TempJob);
        GameManager.entityManager.CreateEntity(EntityArchetypes.standardObject, e);

        for (var i = 0; i < SceneObjectsData.position.Count; i++)
        {
            Mesh mesh;
            if (meshes.ContainsKey(SceneObjectsData.meshResource[i])) mesh = meshes[SceneObjectsData.meshResource[i]];
            else
            {
                var _meshes = ((GameObject)Resources.Load("Mesh/" + SceneObjectsData.meshResource[i].PrefabName)).GetComponentsInChildren<MeshFilter>().Select(x => x.sharedMesh);
                mesh = _meshes.Single(x => x.name == SceneObjectsData.meshResource[i].MeshName);
            }

            Material material;
            if (materials.ContainsKey(SceneObjectsData.materialResource[i])) material = materials[SceneObjectsData.materialResource[i]];
            else material = (Material)Resources.Load("Material/" + SceneObjectsData.materialResource[i]);

            GameManager.entityManager.SetComponentData(e[i], new Position { Value = SceneObjectsData.position[i] });
            GameManager.entityManager.SetComponentData(e[i], new Rotation { Value = SceneObjectsData.rotation[i] });
            GameManager.entityManager.SetComponentData(e[i], new Scale { Value = SceneObjectsData.scale[i] });
            GameManager.entityManager.SetSharedComponentData(e[i], new MeshInstanceRenderer { mesh = mesh, material = material });
        }

        e.Dispose();
    }
}
