using UnityEngine;
using System.Collections;
using Unity.Entities;
using Assets.Scripts.Components;
using Assets.Scripts.Systems;
using UnityEngine.Experimental.Input;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;

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
                CameraRect();
            }
            else if (change == InputDeviceChange.Removed && devices.Contains(device))
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players.playerComponent[i].gamepad == null) continue;
                    if (players.playerComponent[i].gamepad.id == device.id) {
                        Object.Destroy(players.playerComponent[i].gameObject);
                        GameManager.entityManager.DestroyEntity(players.Entity[i]);
                        CameraRect();
                        break;
                    }
                }
                devices.Remove(device);
            }

        };
    }

    protected override void OnUpdate()
    {
        var pc = players.playerComponent[0];
        if (pc.gamepad != null || pc.keyboard != null) return;

        pc.keyboard = Keyboard.current;
        pc.mouse = Mouse.current;
    }

    void CameraRect()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players.Length == 1)
            {
                players.playerComponent[i].firstPersonCamera.rect = PlayerUtils.oneCameraRects[i];
                players.playerComponent[i].gunCamera.rect = PlayerUtils.oneCameraRects[i];
            }
            else if (players.Length == 2)
            {
                players.playerComponent[i].firstPersonCamera.rect = PlayerUtils.twoCameraRects[i];
                players.playerComponent[i].gunCamera.rect = PlayerUtils.twoCameraRects[i];
            }
            else if (players.Length == 3)
            {
                players.playerComponent[i].firstPersonCamera.rect = PlayerUtils.threeCameraRects[i];
                players.playerComponent[i].gunCamera.rect = PlayerUtils.threeCameraRects[i];
            }
            else if (players.Length == 4)
            {
                players.playerComponent[i].firstPersonCamera.rect = PlayerUtils.fourCameraRects[i];
                players.playerComponent[i].gunCamera.rect = PlayerUtils.fourCameraRects[i];
            }
        }
    }
}
