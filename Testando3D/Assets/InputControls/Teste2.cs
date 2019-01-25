// GENERATED AUTOMATICALLY FROM 'Assets/InputControls/Teste2.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class Teste2 : InputActionAssetReference
{
    public Teste2()
    {
    }
    public Teste2(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Player
        m_Player = asset.GetActionMap("Player");
        m_Player_Horizontal = m_Player.GetAction("Horizontal");
        m_Player_Vertical = m_Player.GetAction("Vertical");
        m_Player_Fire = m_Player.GetAction("Fire");
        m_Player_Aim = m_Player.GetAction("Aim");
        m_Player_Interactions = m_Player.GetAction("Interactions");
        m_Player_Reload = m_Player.GetAction("Reload");
        m_Player_Crouch = m_Player.GetAction("Crouch");
        m_Player_Run = m_Player.GetAction("Run");
        m_Player_Jump = m_Player.GetAction("Jump");
        m_Player_MouseX = m_Player.GetAction("MouseX");
        m_Player_MouseY = m_Player.GetAction("MouseY");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Player = null;
        m_Player_Horizontal = null;
        m_Player_Vertical = null;
        m_Player_Fire = null;
        m_Player_Aim = null;
        m_Player_Interactions = null;
        m_Player_Reload = null;
        m_Player_Crouch = null;
        m_Player_Run = null;
        m_Player_Jump = null;
        m_Player_MouseX = null;
        m_Player_MouseY = null;
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
    private InputAction m_Player_Horizontal;
    private InputAction m_Player_Vertical;
    private InputAction m_Player_Fire;
    private InputAction m_Player_Aim;
    private InputAction m_Player_Interactions;
    private InputAction m_Player_Reload;
    private InputAction m_Player_Crouch;
    private InputAction m_Player_Run;
    private InputAction m_Player_Jump;
    private InputAction m_Player_MouseX;
    private InputAction m_Player_MouseY;
    public struct PlayerActions
    {
        private Teste2 m_Wrapper;
        public PlayerActions(Teste2 wrapper) { m_Wrapper = wrapper; }
        public InputAction @Horizontal { get { return m_Wrapper.m_Player_Horizontal; } }
        public InputAction @Vertical { get { return m_Wrapper.m_Player_Vertical; } }
        public InputAction @Fire { get { return m_Wrapper.m_Player_Fire; } }
        public InputAction @Aim { get { return m_Wrapper.m_Player_Aim; } }
        public InputAction @Interactions { get { return m_Wrapper.m_Player_Interactions; } }
        public InputAction @Reload { get { return m_Wrapper.m_Player_Reload; } }
        public InputAction @Crouch { get { return m_Wrapper.m_Player_Crouch; } }
        public InputAction @Run { get { return m_Wrapper.m_Player_Run; } }
        public InputAction @Jump { get { return m_Wrapper.m_Player_Jump; } }
        public InputAction @MouseX { get { return m_Wrapper.m_Player_MouseX; } }
        public InputAction @MouseY { get { return m_Wrapper.m_Player_MouseY; } }
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
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get

        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.GetControlSchemeIndex("KeyboardMouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadiSchemeIndex = -1;
    public InputControlScheme GamepadiScheme
    {
        get

        {
            if (m_GamepadiSchemeIndex == -1) m_GamepadiSchemeIndex = asset.GetControlSchemeIndex("Gamepadi");
            return asset.controlSchemes[m_GamepadiSchemeIndex];
        }
    }
}
