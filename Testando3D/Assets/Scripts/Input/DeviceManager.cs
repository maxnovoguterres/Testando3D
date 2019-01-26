using UnityEngine;
using System.Collections;
using Unity.Entities;
using Assets.Scripts.Components;
using Assets.Scripts.Systems;
using UnityEngine.Experimental.Input;
using System.Collections.Generic;
using System.Linq;

public class DeviceManager : ComponentSystem
{
    struct Player
    {
        public ComponentArray<PlayerComponent> playerComponent;
        public readonly int Length;
        public EntityArray Entity;
    }

    [Inject] Player players;
    List<InputDevice> devices = new List<InputDevice>();

    protected override void OnCreateManager()
    {
        UnityEngine.Experimental.Input.InputSystem.onDeviceChange += delegate (InputDevice device, InputDeviceChange change)
        {
            if (device.description.interfaceName != "HID") return;
            if (change == InputDeviceChange.Added)
            {
                var player = Object.Instantiate(GameManager.Instance.Player, new Vector3(), new Quaternion());
                var pc = player.GetComponent<PlayerComponent>();
                pc.gamepad = Gamepad.all.Single(x => x.id == device.id);
                pc.PlayerLocalID = players.Length;
                devices.Add(device);
            }
            else if (change == InputDeviceChange.Removed && devices.Contains(device))
                devices.Remove(device);
        };
    }

    protected override void OnUpdate()
    {
        var pc = players.playerComponent[0];
        if (pc.gamepad != null || pc.keyboard != null) return;

        pc.keyboard = Keyboard.current;
        pc.mouse = Mouse.current;
    }
}
