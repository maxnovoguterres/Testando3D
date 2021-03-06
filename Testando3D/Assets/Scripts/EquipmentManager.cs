﻿using Assets.Scripts.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

    #region Singleton
    public static EquipmentManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion
    
    public Equipment[] defaultItems;
    public SkinnedMeshRenderer targetMesh;
    public Transform gunsParent;
    Equipment[] currentEquipment;
    SkinnedMeshRenderer[] currentMeshes;

    GunComponent dropGunComponent;

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    //Inventory inventory;

    private void Start()
    {
        //inventory = Inventory.instance;
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
        currentMeshes = new SkinnedMeshRenderer[numSlots];
        
    }

    public void Equip(Equipment newItem, GameObject player)
    {
        int slotIndex = (int)newItem.equipSlot;
        Equipment oldItem = Unequip(slotIndex);

        //if (gunHover.transform.childCount != 0)
        //    dropGunComponent = gunHover.GetComponentsInChildren<GunComponent>()[0];

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        SetEquipmentBlendShapes(newItem, 100);

        currentEquipment[slotIndex] = newItem;

        if (newItem.equipSlot == EquipmentSlot.Gun)
        {
            var camera = player.transform.Find("FirstPersonCamera");
            var gunCamera = camera.Find("GunCamera").gameObject;
            var gunHolder = camera.Find("GunHolder").gameObject;
            var childs = gunHolder.GetComponentsInChildren<GunComponent>();

            var ob = Instantiate(newItem.ob);
            ob.transform.parent = gunHolder.transform;
            ob.transform.localPosition = new Vector3(0, 0, 0);
            ob.transform.localRotation = Quaternion.identity;
            ob.GetComponent<GunComponent>().player = player;
            ob.GetComponent<GunComponent>().playerEntity = player.GetComponent<GameObjectEntity>().Entity;
            ob.GetComponent<GunComponent>().animator = gunHolder.GetComponent<Animator>();
            ob.GetComponent<GunComponent>().firstPersonCamera = camera.GetComponent<Camera>();
            ob.GetComponent<GunComponent>().gunCamera = gunCamera;
            GameManager.Instance.EnableRedDot(true);

            if (oldItem != null)
            {
                //oldItem.ob.GetComponent<GunComponent>().CurrentAmmo = dropGunComponent.CurrentAmmo;
                //oldItem.ob.GetComponent<GunComponent>().ExtraAmmo = dropGunComponent.ExtraAmmo;

                var _ob = Instantiate(oldItem.pickUpOb, new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z), Quaternion.identity);
                _ob.transform.parent = gunsParent;
                _ob.name = oldItem.pickUpOb.name;
                _ob.layer = 9;

                foreach (var child in childs)
                    Destroy(child.gameObject);
            }
        }
        else
        {

            SkinnedMeshRenderer newMesh = Instantiate(newItem.mesh);

            newMesh.transform.parent = targetMesh.transform;
            newMesh.transform.localPosition = new Vector3(0.278f, 0.703f, 0.79f);

            newMesh.GetComponent<GunComponent>().player = player;

            newMesh.bones = targetMesh.bones;
            newMesh.rootBone = targetMesh.rootBone;
            currentMeshes[slotIndex] = newMesh;
        }
    }

    public Equipment Unequip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            if (currentMeshes[slotIndex] != null)
            {
                Destroy(currentMeshes[slotIndex].gameObject);
            }

            Equipment oldItem = currentEquipment[slotIndex];
            SetEquipmentBlendShapes(oldItem, 0);

            currentEquipment[slotIndex] = null;

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }
            return oldItem;
        }
        return null;
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
        EquipDefaultItems();
    }

    void SetEquipmentBlendShapes(Equipment item, int weight)
    {
        foreach(EquipmentMeshRegion blendShape in item.coveredMeshRegions)
        {
            targetMesh.SetBlendShapeWeight((int)blendShape, weight);
        }
    }

    void EquipDefaultItems()
    {
        foreach (Equipment item in defaultItems)
        {
            //Equip(item);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnequipAll();
        }
    }
}
