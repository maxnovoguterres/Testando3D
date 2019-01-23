// GENERATED AUTOMATICALLY FROM 'Assets/NewInputSystem/InputMaster.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class InputMaster : InputActionAssetReference
{
    public InputMaster()
    {
    }
    public InputMaster(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Player
        m_Player = asset.GetActionMap("Player");
        m_Player_Fire = m_Player.GetAction("Fire");
        m_Player_Aim = m_Player.GetAction("Aim");
        m_Player_Crouch = m_Player.GetAction("Crouch");
        m_Player_Jump = m_Player.GetAction("Jump");
        m_Player_PickItemsAndEquipments = m_Player.GetAction("PickItemsAndEquipments");
        m_Player_Reload = m_Player.GetAction("Reload");
        m_Player_Horizontal = m_Player.GetAction("Horizontal");
        m_Player_Vertical = m_Player.GetAction("Vertical");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Player = null;
        m_Player_Fire = null;
        m_Player_Aim = null;
        m_Player_Crouch = null;
        m_Player_Jump = null;
        m_Player_PickItemsAndEquipments = null;
        m_Player_Reload = null;
        m_Player_Horizontal = null;
        m_Player_Vertical = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Player
    private InputActionMap m_Player;
    private InputAction m_Player_Fire;
    private InputAction m_Player_Aim;
    private InputAction m_Player_Crouch;
    private InputAction m_Player_Jump;
    private InputAction m_Player_PickItemsAndEquipments;
    private InputAction m_Player_Reload;
    private InputAction m_Player_Horizontal;
    private InputAction m_Player_Vertical;
    public struct PlayerActions
    {
        private InputMaster m_Wrapper;
        public PlayerActions(InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Fire { get { return m_Wrapper.m_Player_Fire; } }
        public InputAction @Aim { get { return m_Wrapper.m_Player_Aim; } }
        public InputAction @Crouch { get { return m_Wrapper.m_Player_Crouch; } }
        public InputAction @Jump { get { return m_Wrapper.m_Player_Jump; } }
        public InputAction @PickItemsAndEquipments { get { return m_Wrapper.m_Player_PickItemsAndEquipments; } }
        public InputAction @Reload { get { return m_Wrapper.m_Player_Reload; } }
        public InputAction @Horizontal { get { return m_Wrapper.m_Player_Horizontal; } }
        public InputAction @Vertical { get { return m_Wrapper.m_Player_Vertical; } }
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
    }
    public PlayerActions @Player
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new PlayerActions(this);
        }
    }
    private int m_KeyboardandmouseSchemeIndex = -1;
    public InputControlScheme KeyboardandmouseScheme
    {
        get

        {
            if (m_KeyboardandmouseSchemeIndex == -1) m_KeyboardandmouseSchemeIndex = asset.GetControlSchemeIndex("Keyboard and mouse");
            return asset.controlSchemes[m_KeyboardandmouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get

        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.GetControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
}
