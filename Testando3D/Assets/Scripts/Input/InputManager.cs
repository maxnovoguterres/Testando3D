using Assets.Scripts.Components;
using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Input
{
    public enum ActionMap
    {
        Player1 = 1,
        Player2 = 2
    }

    [InitializeOnLoad]
    public class InputManager //: JobComponentSystem
    {
        public static InputMaster controls;
        public static Dictionary<ActionMap, Entity> entityAction;

        static InputManager()
        {
            controls = new InputMaster();
            entityAction = new Dictionary<ActionMap, Entity>();
            controls.Player.Crouch.performed += x => Crouch(ActionMap.Player1);
            controls.Player.Aim.performed += x => Aim(ActionMap.Player1);
            controls.Player.Fire.performed += x => Fire(ActionMap.Player1);
            controls.Player.Horizontal.performed += x => Horizontal(ActionMap.Player1);
            controls.Player.Jump.performed += x => Jump(ActionMap.Player1);
            controls.Player.Reload.performed += x => Reload(ActionMap.Player1);
            controls.Player.Run.performed += x => Run(ActionMap.Player1);
            controls.Player.Vertical.performed += x => Vertical(ActionMap.Player1);
            controls.Player.PickupItemsAndEquipments.performed += x => PickupItemsAndEquipments(ActionMap.Player1);

            //controls.Player2.Crouch.performed += x => Crouch(ActionMap.Player2);
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
}
