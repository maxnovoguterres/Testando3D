using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot equipSlot;
    public SkinnedMeshRenderer mesh;
    public GameObject ob;
    public GameObject pickUpOb;
    public EquipmentMeshRegion[] coveredMeshRegions;
    public int armorModifier;
    public int damageModifier;

    public override void Use()
    {
        base.Use();
        //EquipmentManager.instance.Equip(this);
    }
}

public enum EquipmentSlot { Head, Chest, Legs, Weapon, Shield, Feet , Gun}
public enum EquipmentMeshRegion { Legs, Arms, Torso };
