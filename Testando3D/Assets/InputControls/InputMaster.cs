// GENERATED AUTOMATICALLY FROM 'Assets/InputControls/InputMaster.inputactions'

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
        // Player1
        m_Player1 = asset.GetActionMap("Player1");
        m_Player1_Horizontal = m_Player1.GetAction("Horizontal");
        m_Player1_Vertical = m_Player1.GetAction("Vertical");
        m_Player1_Fire = m_Player1.GetAction("Fire");
        m_Player1_Aim = m_Player1.GetAction("Aim");
        m_Player1_PickupItemsAndEquipments = m_Player1.GetAction("PickupItemsAndEquipments");
        m_Player1_Reload = m_Player1.GetAction("Reload");
        m_Player1_Crouch = m_Player1.GetAction("Crouch");
        m_Player1_Run = m_Player1.GetAction("Run");
        m_Player1_Jump = m_Player1.GetAction("Jump");
        // Player2
        m_Player2 = asset.GetActionMap("Player2");
        m_Player2_Horizontal = m_Player2.GetAction("Horizontal");
        m_Player2_Vertical = m_Player2.GetAction("Vertical");
        m_Player2_Fire = m_Player2.GetAction("Fire");
        m_Player2_Aim = m_Player2.GetAction("Aim");
        m_Player2_PickupItemsAndEquipments = m_Player2.GetAction("PickupItemsAndEquipments");
        m_Player2_Reload = m_Player2.GetAction("Reload");
        m_Player2_Crouch = m_Player2.GetAction("Crouch");
        m_Player2_Run = m_Player2.GetAction("Run");
        m_Player2_Jump = m_Player2.GetAction("Jump");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Player1 = null;
        m_Player1_Horizontal = null;
        m_Player1_Vertical = null;
        m_Player1_Fire = null;
        m_Player1_Aim = null;
        m_Player1_PickupItemsAndEquipments = null;
        m_Player1_Reload = null;
        m_Player1_Crouch = null;
        m_Player1_Run = null;
        m_Player1_Jump = null;
        m_Player2 = null;
        m_Player2_Horizontal = null;
        m_Player2_Vertical = null;
        m_Player2_Fire = null;
        m_Player2_Aim = null;
        m_Player2_PickupItemsAndEquipments = null;
        m_Player2_Reload = null;
        m_Player2_Crouch = null;
        m_Player2_Run = null;
        m_Player2_Jump = null;
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
    // Player1
    private InputActionMap m_Player1;
    private InputAction m_Player1_Horizontal;
    private InputAction m_Player1_Vertical;
    private InputAction m_Player1_Fire;
    private InputAction m_Player1_Aim;
    private InputAction m_Player1_PickupItemsAndEquipments;
    private InputAction m_Player1_Reload;
    private InputAction m_Player1_Crouch;
    private InputAction m_Player1_Run;
    private InputAction m_Player1_Jump;
    public struct Player1Actions
    {
        private InputMaster m_Wrapper;
        public Player1Actions(InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Horizontal { get { return m_Wrapper.m_Player1_Horizontal; } }
        public InputAction @Vertical { get { return m_Wrapper.m_Player1_Vertical; } }
        public InputAction @Fire { get { return m_Wrapper.m_Player1_Fire; } }
        public InputAction @Aim { get { return m_Wrapper.m_Player1_Aim; } }
        public InputAction @PickupItemsAndEquipments { get { return m_Wrapper.m_Player1_PickupItemsAndEquipments; } }
        public InputAction @Reload { get { return m_Wrapper.m_Player1_Reload; } }
        public InputAction @Crouch { get { return m_Wrapper.m_Player1_Crouch; } }
        public InputAction @Run { get { return m_Wrapper.m_Player1_Run; } }
        public InputAction @Jump { get { return m_Wrapper.m_Player1_Jump; } }
        public InputActionMap Get() { return m_Wrapper.m_Player1; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(Player1Actions set) { return set.Get(); }
    }
    public Player1Actions @Player1
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new Player1Actions(this);
        }
    }
    // Player2
    private InputActionMap m_Player2;
    private InputAction m_Player2_Horizontal;
    private InputAction m_Player2_Vertical;
    private InputAction m_Player2_Fire;
    private InputAction m_Player2_Aim;
    private InputAction m_Player2_PickupItemsAndEquipments;
    private InputAction m_Player2_Reload;
    private InputAction m_Player2_Crouch;
    private InputAction m_Player2_Run;
    private InputAction m_Player2_Jump;
    public struct Player2Actions
    {
        private InputMaster m_Wrapper;
        public Player2Actions(InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Horizontal { get { return m_Wrapper.m_Player2_Horizontal; } }
        public InputAction @Vertical { get { return m_Wrapper.m_Player2_Vertical; } }
        public InputAction @Fire { get { return m_Wrapper.m_Player2_Fire; } }
        public InputAction @Aim { get { return m_Wrapper.m_Player2_Aim; } }
        public InputAction @PickupItemsAndEquipments { get { return m_Wrapper.m_Player2_PickupItemsAndEquipments; } }
        public InputAction @Reload { get { return m_Wrapper.m_Player2_Reload; } }
        public InputAction @Crouch { get { return m_Wrapper.m_Player2_Crouch; } }
        public InputAction @Run { get { return m_Wrapper.m_Player2_Run; } }
        public InputAction @Jump { get { return m_Wrapper.m_Player2_Jump; } }
        public InputActionMap Get() { return m_Wrapper.m_Player2; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(Player2Actions set) { return set.Get(); }
    }
    public Player2Actions @Player2
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new Player2Actions(this);
        }
    }
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get

        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.GetControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
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
    private int m_XboxOneGamepadSchemeIndex = -1;
    public InputControlScheme XboxOneGamepadScheme
    {
        get

        {
            if (m_XboxOneGamepadSchemeIndex == -1) m_XboxOneGamepadSchemeIndex = asset.GetControlSchemeIndex("XboxOneGamepad");
            return asset.controlSchemes[m_XboxOneGamepadSchemeIndex];
        }
    }
    private int m_XInputControllerWindowsSchemeIndex = -1;
    public InputControlScheme XInputControllerWindowsScheme
    {
        get

        {
            if (m_XInputControllerWindowsSchemeIndex == -1) m_XInputControllerWindowsSchemeIndex = asset.GetControlSchemeIndex("XInputControllerWindows");
            return asset.controlSchemes[m_XInputControllerWindowsSchemeIndex];
        }
    }
    private int m_PS4DualShockGamepadSchemeIndex = -1;
    public InputControlScheme PS4DualShockGamepadScheme
    {
        get

        {
            if (m_PS4DualShockGamepadSchemeIndex == -1) m_PS4DualShockGamepadSchemeIndex = asset.GetControlSchemeIndex("PS4DualShockGamepad");
            return asset.controlSchemes[m_PS4DualShockGamepadSchemeIndex];
        }
    }
}
