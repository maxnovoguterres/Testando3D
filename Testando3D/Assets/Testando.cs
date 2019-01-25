using Assets.Scripts.Components;
using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Testando : MonoBehaviour
{

    public enum ActionMap
    {
        Player1 = 1,
        Player2 = 2
    }


    public InputMaster controls;

    public static Dictionary<ActionMap, Entity> entityAction;
    static Testando()
    {

        //controls.Player2.Crouch.performed += x => Crouch(ActionMap.Player2);
    }

    private void Start()
    {
        controls = new InputMaster();
        entityAction = new Dictionary<ActionMap, Entity>();
        controls.Player1.Crouch.performed += x => Crouch(ActionMap.Player1);
        controls.Player1.Aim.performed += x => Aim(ActionMap.Player1);
        controls.Player1.Fire.performed += x => Fire(ActionMap.Player1);
        controls.Player1.Horizontal.performed += x => Horizontal(ActionMap.Player1);
        controls.Player1.Jump.performed += x => Jump(ActionMap.Player1);
        controls.Player1.Reload.performed += x => Reload(ActionMap.Player1);
        controls.Player1.Run.performed += x => Run(ActionMap.Player1);
        controls.Player1.Vertical.performed += x => Vertical(ActionMap.Player1);
        controls.Player1.PickupItemsAndEquipments.performed += x => PickupItemsAndEquipments(ActionMap.Player1);
    }
    //protected override void OnCreateManager()
    //{
    //}
    static void Crouch(ActionMap action)
    {
        if (!entityAction.ContainsKey(action)) return;
        Entity e = entityAction[action];
        e.SetComponentData<_Input>(null, new KeyValuePair<string, object>("crouch", 1));
    }
    static void Aim(ActionMap action)
    {
        if (!entityAction.ContainsKey(action)) return;
        Entity e = entityAction[action];
        e.SetComponentData<_Input>(null, new KeyValuePair<string, object>("aim", true));
    }
    static void Fire(ActionMap action)
    {
        if (!entityAction.ContainsKey(action)) return;
        Entity e = entityAction[action];
    }
    static void Horizontal(ActionMap action)
    {
        if (!entityAction.ContainsKey(action)) return;
        Entity e = entityAction[action];
    }
    static void Jump(ActionMap action)
    {
        if (!entityAction.ContainsKey(action)) return;
        Entity e = entityAction[action];
    }
    static void Reload(ActionMap action)
    {
        if (!entityAction.ContainsKey(action)) return;
        Entity e = entityAction[action];
    }
    static void Run(ActionMap action)
    {
        if (!entityAction.ContainsKey(action)) return;
        Entity e = entityAction[action];
    }
    static void Vertical(ActionMap action)
    {
        if (!entityAction.ContainsKey(action)) return;
        Entity e = entityAction[action];
    }
    static void PickupItemsAndEquipments(ActionMap action)
    {
        if (!entityAction.ContainsKey(action)) return;
        Entity e = entityAction[action];
    }
}
