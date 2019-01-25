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
    [InitializeOnLoad]
    public static class KeyAction
    {
        public static Dictionary<string, KeyActionType> Actions;

        static KeyAction()
        {
            Actions = new Dictionary<string, KeyActionType>();

            Actions.Add("Fire", new KeyActionType(Key.KeyNull, GamePadKey.rightTrigger, MouseKey.leftButton));
        }

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
        #endregion
    }
}