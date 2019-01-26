using Assets.Scripts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Controls;
using Assets.Scripts.Input.Shared;
using Assets.Scripts.Input.Helper;

namespace Assets.Scripts.Input.Shared
{
    //[InitializeOnLoad]
    public static class KeyAction
    {
        #region [Actions]
        public static Dictionary<string, KeyActionType> Actions = new Dictionary<string, KeyActionType>
        {
            {"Horizontal", new KeyActionType(Key.KeyNull, GamePadKey.leftStickX, Key.KeyNull) },
            {"Vertical", new KeyActionType(Key.KeyNull, GamePadKey.leftStickY, Key.KeyNull)},
            {"Fire", new KeyActionType(Key.KeyNull, GamePadKey.rightTrigger, MouseKey.leftButton)},
            {"Aim", new KeyActionType(Key.KeyNull, GamePadKey.leftTrigger, MouseKey.rightButton)},
            {"Interactions", new KeyActionType(KeyBoardKey.tKey, GamePadKey.yButton, Key.KeyNull)},
            {"Reload", new KeyActionType(KeyBoardKey.rKey, GamePadKey.xButton, Key.KeyNull)},
            {"Crouch", new KeyActionType(KeyBoardKey.leftCtrlKey, GamePadKey.bButton, Key.KeyNull)},
            {"Run", new KeyActionType(KeyBoardKey.leftShiftKey, GamePadKey.leftStickButton, Key.KeyNull)},
            {"Jump", new KeyActionType(KeyBoardKey.spaceKey, GamePadKey.buttonSouth, Key.KeyNull)},
            {"MouseX", new KeyActionType(Key.KeyNull, GamePadKey.rightStickX, MouseKey.deltaX)},
            {"MouseY", new KeyActionType(Key.KeyNull, GamePadKey.rightStickY, MouseKey.deltaY)},
            {"CursorOn", new KeyActionType(KeyBoardKey.escapeKey, GamePadKey.startButton, Key.KeyNull)},
            {"CursorOff", new KeyActionType(Key.KeyNull, GamePadKey.selectButton, MouseKey.leftButton)}
        };
        #endregion

        #region [GetButtonMethods]
        public static bool GetButton(this PlayerComponent p, string action)
        {
            if (!Actions.ContainsKey(action))
            {
                Debug.Log($"Action {action} não existente");
                return false;
            }

            var keyActionType = Actions[action];

            if (p.gamepad != null)
            {
                var key = keyActionType.Gamepad;
                if (key.Type == typeof(DpadControl))
                    return ((DpadControl)typeof(Gamepad).GetProperty(key.Name).GetValue(p.gamepad)).GetButtonControl(key.Bit).isPressed;
                else
                    return ((ButtonControl)typeof(Gamepad).GetProperty(key.Name).GetValue(p.gamepad)).isPressed;
            }
            else
            {
                return !string.IsNullOrWhiteSpace(keyActionType.Keyboard.Name) ? ((KeyControl)typeof(Keyboard).GetProperty(keyActionType.Keyboard.Name).GetValue(p.keyboard)).isPressed : false || !string.IsNullOrWhiteSpace(keyActionType.Mouse.Name) ? ((ButtonControl)typeof(Mouse).GetProperty(keyActionType.Mouse.Name).GetValue(p.mouse)).isPressed : false;
            }
        }

        public static bool GetButtonUp(this PlayerComponent p, string action)
        {
            if (!Actions.ContainsKey(action))
            {
                Debug.Log($"Action {action} não existente");
                return false;
            }

            var keyActionType = Actions[action];

            if (p.gamepad != null)
            {
                var key = keyActionType.Gamepad;
                if (key.Type == typeof(DpadControl))
                    return ((DpadControl)typeof(Gamepad).GetProperty(key.Name).GetValue(p.gamepad)).GetButtonControl(key.Bit).wasReleasedThisFrame;
                else
                    return ((ButtonControl)typeof(Gamepad).GetProperty(key.Name).GetValue(p.gamepad)).wasReleasedThisFrame;
            }
            else
            {
                return !string.IsNullOrWhiteSpace(keyActionType.Keyboard.Name) ? ((KeyControl)typeof(Keyboard).GetProperty(keyActionType.Keyboard.Name).GetValue(p.keyboard)).wasReleasedThisFrame : false || !string.IsNullOrWhiteSpace(keyActionType.Mouse.Name) ? ((ButtonControl)typeof(Mouse).GetProperty(keyActionType.Mouse.Name).GetValue(p.mouse)).wasReleasedThisFrame : false;
            }
        }

        public static bool GetButtonDown(this PlayerComponent p, string action)
        {
            if (!Actions.ContainsKey(action))
            {
                Debug.Log($"Action {action} não existente");
                return false;
            }

            var keyActionType = Actions[action];

            if (p.gamepad != null)
            {
                var key = keyActionType.Gamepad;
                if (key.Type == typeof(DpadControl))
                    return ((DpadControl)typeof(Gamepad).GetProperty(key.Name).GetValue(p.gamepad)).GetButtonControl(key.Bit).wasPressedThisFrame;
                else
                    return ((ButtonControl)typeof(Gamepad).GetProperty(key.Name).GetValue(p.gamepad)).wasPressedThisFrame;
            }
            else
            {
                return !string.IsNullOrWhiteSpace(keyActionType.Keyboard.Name) ? ((KeyControl)typeof(Keyboard).GetProperty(keyActionType.Keyboard.Name).GetValue(p.keyboard)).wasPressedThisFrame : false || !string.IsNullOrWhiteSpace(keyActionType.Mouse.Name) ? ((ButtonControl)typeof(Mouse).GetProperty(keyActionType.Mouse.Name).GetValue(p.mouse)).wasPressedThisFrame : false;
            }
        }

        public static float GetAxis(this PlayerComponent p, string action)
        {
            if (!Actions.ContainsKey(action))
            {
                Debug.Log($"Action {action} não existente");
                return 0;
            }

            var keyActionType = Actions[action];

            if (p.gamepad != null)
            {
                var key = keyActionType.Gamepad;
                var r = ((StickControl)typeof(Gamepad).GetProperty(key.Name).GetValue(p.gamepad)).GetAxis(key.Bit).ReadValue();
                return (r > 0.15f && r >= 0) || (r < -0.15f && r < 0) ? r : 0;
            }
            else
            {
                //!string.IsNullOrWhiteSpace(keyActionType.Keyboard.Name) ? ((KeyControl)typeof(Keyboard).GetProperty(keyActionType.Keyboard.Name).GetValue(p.keyboard)).wasPressedThisFrame : false ||
                var mouseKey = keyActionType.Mouse;
                var r = !string.IsNullOrWhiteSpace(mouseKey.Name) ? ((Vector2Control)typeof(Mouse).GetProperty(mouseKey.Name).GetValue(p.mouse)).GetAxis(mouseKey.Bit).ReadValue() / 100 : 0;
                return (r > 0.15f && r >= 0) || (r < -0.15f && r < 0) ? r : 0;
            }
        }
        #endregion

        #region [Helper]
        private static ButtonControl GetButtonControl(this DpadControl dpad, int bit)
        {
            switch (bit)
            {
                default:
                case 0:
                    return dpad.up;
                case 1:
                    return dpad.down;
                case 2:
                    return dpad.left;
                case 3:
                    return dpad.right;
            }
        }
        private static AxisControl GetAxis(this Vector2Control v, int bit)
        {
            switch (bit)
            {
                default:
                case 0:
                    return v.x;
                case 1:
                    return v.y;
            }
        }
        #endregion
    }
}