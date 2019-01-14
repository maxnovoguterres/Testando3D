using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Entities;
using Assets.Scripts.ECSWindows;
using Unity.Transforms;
using Unity.Rendering;
using System.IO;
using System.Text;
using Assets.Scripts.PureECS;

enum TransformType
{
    All = 0,
    Selecteds = 1
}

class ECSUtilsWindows : EditorWindow
{
    TransformType transformType = TransformType.Selecteds;
    private static GameObject ecsColliders;

    [MenuItem("Window/ECS Utils")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ECSUtilsWindows));
    }

    void OnGUI()
    {
        GUILayout.Label("Tranform Object in Entities", EditorStyles.boldLabel);
        var transformTypes = Enum.GetValues(typeof(TransformType)).Cast<TransformType>().Select(x => x.ToString()).ToArray();

        transformType = (TransformType)EditorGUILayout.Popup("Objects to tranform: ", (int)transformType, transformTypes);

        if (GUILayout.Button("Transfom"))
        {
            Transform();
        }
    }

    private void Transform()
    {
        ecsColliders = GameObject.Find("ECSColliders");
        if (transformType == TransformType.Selecteds)
        {
            var objs = Selection.gameObjects.ToList();
            if (objs.Count == 0)
            {
                Debug.LogError("Select at least one GameObject.");
                return;
            }

            _Transform(objs);
        }
        else if (transformType == TransformType.All)
        {
            Debug.Log("Not Implemented Yet.");
        }
    }

    private void _Transform(List<GameObject> objects)
    {
        var objInfo = new List<ObjectInfo>();
        GetObjetoInfo(objects, ref objInfo);

        NewDataText(objInfo);
        objInfo = new List<ObjectInfo>();
    }

    private static void GetObjetoInfo(List<GameObject> objs, ref List<ObjectInfo> objInfos, string parentName = null)
    {

        foreach (var obj in objs)
        {
            if (obj.transform.childCount > 0) GetObjetoInfo(obj.GetComponentsInChildren<Transform>().Where(c => c.gameObject != obj).Select(x => x.gameObject).ToList(), ref objInfos, string.IsNullOrWhiteSpace(parentName)? PrefabUtility.GetCorrespondingObjectFromSource(obj).name : parentName);

            var meshRender = obj.GetComponent<MeshRenderer>();
            var mesh = obj.GetComponent<MeshFilter>();

            if (meshRender == null || mesh == null) continue;

            var matName = meshRender.material.name.Split(' ')[0];
            var meshName = mesh.sharedMesh.name.Split(' ')[0];
            var prefabName = string.IsNullOrWhiteSpace(parentName) ? PrefabUtility.GetCorrespondingObjectFromSource(obj).name : parentName;

            objInfos.Add(new ObjectInfo
            {
                Name = obj.name,
                Position = obj.transform.position,
                Rotation = obj.transform.rotation,
                Scale = obj.transform.lossyScale,
                MaterialResource = matName,
                Mesh = new MeshInfo { PrefabName = prefabName, MeshName = meshName }
            });

            if (string.IsNullOrWhiteSpace(parentName))
                obj.transform.parent = ecsColliders.transform;

            DestroyImmediate(obj.GetComponent<MeshRenderer>());
            DestroyImmediate(obj.GetComponent<MeshFilter>());

            matName = "";
            prefabName = "";
        }
    }

    private void NewDataText(List<ObjectInfo> objectInfos)
    {
        StringBuilder archive = new StringBuilder();

        //getting data we already have.
        var existingPositions = SceneObjectsData.position;
        var existingRotations = SceneObjectsData.rotation;
        var existingScales = SceneObjectsData.scale;
        var existingMeshResources = SceneObjectsData.meshResource;
        var existingMaterialResources = SceneObjectsData.materialResource;

        //adding new data.
        existingPositions.AddRange(objectInfos.Select(x => x.Position));
        existingRotations.AddRange(objectInfos.Select(x => x.Rotation));
        existingScales.AddRange(objectInfos.Select(x => x.Scale));
        existingMeshResources.AddRange(objectInfos.Select(x => x.Mesh));
        existingMaterialResources.AddRange(objectInfos.Select(x => x.MaterialResource));

        using (var streamW = new StreamWriter("EcsData.txt"))
        {
            streamW.WriteLine("Positions:");
            streamW.WriteLine(string.Join(", ", existingPositions.Select(x => $"new float3({x.x.ToString().Replace(',', '.')}f, {x.y.ToString().Replace(',', '.')}f, {x.z.ToString().Replace(',', '.')}f)")));
            streamW.WriteLine("");

            streamW.WriteLine("Rotations:");
            streamW.WriteLine(string.Join(", ", existingRotations.Select(x => $"new quaternion({x.value.x.ToString().Replace(',', '.')}f, {x.value.y.ToString().Replace(',', '.')}f, {x.value.z.ToString().Replace(',', '.')}f, {x.value.w.ToString().Replace(',', '.')}f)")));
            streamW.WriteLine("");

            streamW.WriteLine("Scales:");
            streamW.WriteLine(string.Join(", ", existingScales.Select(x => $"new float3({x.x.ToString().Replace(',', '.')}f, {x.y.ToString().Replace(',', '.')}f, {x.z.ToString().Replace(',', '.')}f)")));
            streamW.WriteLine("");

            streamW.WriteLine("MeshResources:");
            streamW.WriteLine(string.Join(", ", existingMeshResources.Select(x => $@"new MeshInfo {{ PrefabName = ""{x.PrefabName}"", MeshName = ""{x.MeshName}""}}")));
            streamW.WriteLine("");

            streamW.WriteLine("MaterialResources:");
            streamW.WriteLine(string.Join(", ", existingMaterialResources.Select(x => $@"""{x}""")));
            streamW.WriteLine("");
        }
    }

}