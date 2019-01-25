using Assets.Scripts.Components;
using Assets.Scripts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class NewInputManager : MonoBehaviour
{

    //public enum ActionMap
    //{
    //    Player1 = 1,
    //    Player2 = 2
    //}


    //public InputMaster controls;
    public Teste2 controls2;
    Mouse m;
    public static Keyboard kb;
    //public static Dictionary<ActionMap, Entity> entityAction;

    public static float horizontal;
    public static float vertical;
    public static float crouch;
    public static float jump;
    public static float fire;
    public static float aim = 2;
    public static float reload;
    public static float interactions;
    public static bool run;
    float _run;
    public static float mouseX;
    public static float mouseY;

    private void Awake()
    {
        m = InputSystem.GetDevice<Mouse>();
        kb = InputSystem.GetDevice<Keyboard>();
        //entityAction = new Dictionary<ActionMap, Entity>();
        //controls.Player1.Crouch.performed += x => Crouch(ActionMap.Player1);
        //controls.Player1.Aim.performed += x => Aim(ActionMap.Player1);
        ////controls.Player1.Fire.performed += x => Fire(ActionMap.Player1);
        //controls.Player1.Fire.performed += x => Teste();
        //controls.Player1.Horizontal.performed += x => Horizontal(ActionMap.Player1);
        //controls.Player1.Jump.performed += x => Jump(ActionMap.Player1);
        //controls.Player1.Reload.performed += x => Reload(ActionMap.Player1);
        //controls.Player1.Run.performed += x => Run(ActionMap.Player1);
        //controls.Player1.Vertical.performed += x => Vertical(ActionMap.Player1);
        //controls.Player1.Interactions.performed += x => Interactions(ActionMap.Player1);
        controls2.Player.Horizontal.performed += x => { horizontal = x.ReadValue<float>(); };
        controls2.Player.Vertical.performed += x => { vertical = x.ReadValue<float>(); };
        controls2.Player.Crouch.performed += x => { crouch = x.ReadValue<float>(); };
        controls2.Player.Jump.performed += x => { jump = x.ReadValue<float>(); };
        //controls2.Player.Fire.performed += x => Teste(x.ReadValue<float>());
        controls2.Player.Fire.performed += x => { fire = x.ReadValue<float>(); };
        controls2.Player.Aim.performed += x => { aim = x.ReadValue<float>(); };
        controls2.Player.Reload.performed += x => { reload = x.ReadValue<float>(); };
        //controls2.Player.Interactions.performed += x => { interactions = x.ReadValue<float>(); };
        controls2.Player.Interactions.performed += x => Interactions();
        controls2.Player.Run.performed += x => { _run = x.ReadValue<float>(); };
        controls2.Player.MouseX.performed += x => { mouseX = x.ReadValue<float>(); };
        controls2.Player.MouseY.performed += x => { mouseY = x.ReadValue<float>(); };
    }
    //protected override void OnCreateManager()
    //{
    //}

    private void Update()
    {
        if (m.leftButton.isPressed)
            print("segurando");

        run = Convert.ToBoolean(_run);
    }

    void Teste(float a)
    {
        Debug.Log(a);
    }

    void Interactions()
    {
        if (GameManager.Instance.canEquip)
        {
            EquipmentManager.instance.Equip(GameManager.Instance.gunToEquip, gameObject);
            Destroy(GameManager.Instance.gunToDestroy);
            GameManager.Instance.pickUpText.text = "";
            GameManager.Instance.canEquip = false;
        }
    }

    //static void Crouch(ActionMap action)
    //{
    //    if (!entityAction.ContainsKey(action)) return;
    //    Entity e = entityAction[action];
    //    e.SetComponentData<_Input>(null, new KeyValuePair<string, object>("crouch", 1));
    //}
    //static void Aim(ActionMap action)
    //{
    //    if (!entityAction.ContainsKey(action)) return;
    //    Entity e = entityAction[action];
    //    e.SetComponentData<_Input>(null, new KeyValuePair<string, object>("aim", true));
    //}
    //static void Fire(ActionMap action)
    //{
    //    if (!entityAction.ContainsKey(action)) return;
    //    Entity e = entityAction[action];
    //}
    //static void Horizontal(ActionMap action)
    //{
    //    if (!entityAction.ContainsKey(action)) return;
    //    Entity e = entityAction[action];
    //}
    //static void Jump(ActionMap action)
    //{
    //    if (!entityAction.ContainsKey(action)) return;
    //    Entity e = entityAction[action];
    //}
    //static void Reload(ActionMap action)
    //{
    //    if (!entityAction.ContainsKey(action)) return;
    //    Entity e = entityAction[action];
    //}
    //static void Run(ActionMap action)
    //{
    //    if (!entityAction.ContainsKey(action)) return;
    //    Entity e = entityAction[action];
    //}
    //static void Vertical(ActionMap action)
    //{
    //    if (!entityAction.ContainsKey(action)) return;
    //    Entity e = entityAction[action];
    //}
    //static void Interactions(ActionMap action)
    //{
    //    if (!entityAction.ContainsKey(action)) return;
    //    Entity e = entityAction[action];
    //}

    private void OnEnable()
    {
        controls2.Enable();
    }

    private void OnDisable()
    {
        controls2.Disable();
    }
}
