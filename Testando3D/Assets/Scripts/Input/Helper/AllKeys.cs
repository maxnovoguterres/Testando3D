using Assets.Scripts.Input.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.Input.Controls;

namespace Assets.Scripts.Input.Helper
{
    #region KeyBoard
    public static class KeyBoardKey
    {
        public static Key homeKey { get { return new Key { Name = "homeKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key endKey { get { return new Key { Name = "endKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key insertKey { get { return new Key { Name = "insertKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key deleteKey { get { return new Key { Name = "deleteKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key capsLockKey { get { return new Key { Name = "capsLockKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key scrollLockKey { get { return new Key { Name = "scrollLockKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key printScreenKey { get { return new Key { Name = "printScreenKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key pageUpKey { get { return new Key { Name = "pageUpKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key pauseKey { get { return new Key { Name = "pauseKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpadEnterKey { get { return new Key { Name = "numpadEnterKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpadDivideKey { get { return new Key { Name = "numpadDivideKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpadMultiplyKey { get { return new Key { Name = "numpadMultiplyK", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numLockKey { get { return new Key { Name = "numLockKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key pageDownKey { get { return new Key { Name = "pageDownKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key downArrowKey { get { return new Key { Name = "downArrowKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpadMinusKey { get { return new Key { Name = "numpadMinusKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key upArrowKey { get { return new Key { Name = "upArrowKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key rightArrowKey { get { return new Key { Name = "rightArrowKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key leftArrowKey { get { return new Key { Name = "leftArrowKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key escapeKey { get { return new Key { Name = "escapeKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key contextMenuKey { get { return new Key { Name = "contextMenuKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key rightCommandKey { get { return new Key { Name = "rightCommandKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key leftCommandKey { get { return new Key { Name = "leftCommandKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key rightAppleKey { get { return new Key { Name = "rightAppleKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key leftAppleKey { get { return new Key { Name = "leftAppleKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key rightWindowsKey { get { return new Key { Name = "rightWindowsKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key leftWindowsKey { get { return new Key { Name = "leftWindowsKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key rightMetaKey { get { return new Key { Name = "rightMetaKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key backspaceKey { get { return new Key { Name = "backspaceKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpadPlusKey { get { return new Key { Name = "numpadPlusKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpadEqualsKey { get { return new Key { Name = "numpadEqualsKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key leftMetaKey { get { return new Key { Name = "leftMetaKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key oem5Key { get { return new Key { Name = "oem5Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key oem4Key { get { return new Key { Name = "oem4Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key oem3Key { get { return new Key { Name = "oem3Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key oem2Key { get { return new Key { Name = "oem2Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key oem1Key { get { return new Key { Name = "oem1Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f12Key { get { return new Key { Name = "f12Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f11Key { get { return new Key { Name = "f11Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f10Key { get { return new Key { Name = "f10Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f9Key { get { return new Key { Name = "f9Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f8Key { get { return new Key { Name = "f8Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f7Key { get { return new Key { Name = "f7Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f6Key { get { return new Key { Name = "f6Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpadPeriodKey { get { return new Key { Name = "numpadPeriodKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f5Key { get { return new Key { Name = "f5Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f3Key { get { return new Key { Name = "f3Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f2Key { get { return new Key { Name = "f2Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f1Key { get { return new Key { Name = "f1Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpad9Key { get { return new Key { Name = "numpad9Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpad8Key { get { return new Key { Name = "numpad8Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpad7Key { get { return new Key { Name = "numpad7Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpad6Key { get { return new Key { Name = "numpad6Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpad5Key { get { return new Key { Name = "numpad5Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpad4Key { get { return new Key { Name = "numpad4Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpad3Key { get { return new Key { Name = "numpad3Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpad2Key { get { return new Key { Name = "numpad2Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpad1Key { get { return new Key { Name = "numpad1Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key numpad0Key { get { return new Key { Name = "numpad0Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key f4Key { get { return new Key { Name = "f4Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key rightCtrlKey { get { return new Key { Name = "rightCtrlKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key leftCtrlKey { get { return new Key { Name = "leftCtrlKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key rightAltKey { get { return new Key { Name = "rightAltKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key iKey { get { return new Key { Name = "iKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key gKey { get { return new Key { Name = "gKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key fKey { get { return new Key { Name = "fKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key eKey { get { return new Key { Name = "eKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key dKey { get { return new Key { Name = "dKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key cKey { get { return new Key { Name = "cKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key bKey { get { return new Key { Name = "bKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key aKey { get { return new Key { Name = "aKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key equalsKey { get { return new Key { Name = "equalsKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key minusKey { get { return new Key { Name = "minusKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key rightBracketKey { get { return new Key { Name = "rightBracketKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key leftBracketKey { get { return new Key { Name = "leftBracketKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key jKey { get { return new Key { Name = "jKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key backslashKey { get { return new Key { Name = "backslashKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key periodKey { get { return new Key { Name = "periodKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key commaKey { get { return new Key { Name = "commaKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key semicolonKey { get { return new Key { Name = "semicolonKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key quoteKey { get { return new Key { Name = "quoteKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key backquoteKey { get { return new Key { Name = "backquoteKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key tabKey { get { return new Key { Name = "tabKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key enterKey { get { return new Key { Name = "enterKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key spaceKey { get { return new Key { Name = "spaceKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key slashKey { get { return new Key { Name = "slashKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key kKey { get { return new Key { Name = "kKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key hKey { get { return new Key { Name = "hKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key mKey { get { return new Key { Name = "mKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key leftAltKey { get { return new Key { Name = "leftAltKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key rightShiftKey { get { return new Key { Name = "rightShiftKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key leftShiftKey { get { return new Key { Name = "leftShiftKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key digit0Key { get { return new Key { Name = "digit0Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key digit9Key { get { return new Key { Name = "digit9Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key digit8Key { get { return new Key { Name = "digit8Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key digit7Key { get { return new Key { Name = "digit7Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key digit6Key { get { return new Key { Name = "digit6Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key digit5Key { get { return new Key { Name = "digit5Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key digit4Key { get { return new Key { Name = "digit4Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key lKey { get { return new Key { Name = "lKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key digit2Key { get { return new Key { Name = "digit2Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key digit1Key { get { return new Key { Name = "digit1Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key digit3Key { get { return new Key { Name = "digit3Key", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key yKey { get { return new Key { Name = "yKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key nKey { get { return new Key { Name = "nKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key zKey { get { return new Key { Name = "zKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key pKey { get { return new Key { Name = "pKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key qKey { get { return new Key { Name = "qKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key rKey { get { return new Key { Name = "rKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key oKey { get { return new Key { Name = "oKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key tKey { get { return new Key { Name = "tKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key uKey { get { return new Key { Name = "uKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key vKey { get { return new Key { Name = "vKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key wKey { get { return new Key { Name = "wKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key xKey { get { return new Key { Name = "xKey", Type = typeof(KeyControl), Bit = 1 }; } }
        public static Key sKey { get { return new Key { Name = "sKey", Type = typeof(KeyControl), Bit = 1 }; } }
    }
    #endregion

    #region [Mouse]
    public static class MouseKey
    {
        public static Key leftButton { get { return new Key { Name = "leftButton", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key middleButton { get { return new Key { Name = "middleButton", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key rightButton { get { return new Key { Name = "rightButton", Type = typeof(ButtonControl), Bit = 1 }; } }
    }
    #endregion

    #region [GamePad]
    public static class GamePadKey
    {
        public static Key yButton { get { return new Key { Name = "yButton", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key xButton { get { return new Key { Name = "xButton", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key bButton { get { return new Key { Name = "bButton", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key aButton { get { return new Key { Name = "aButton", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key rightTrigger { get { return new Key { Name = "rightTrigger", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key leftTrigger { get { return new Key { Name = "leftTrigger", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key rightStick { get { return new Key { Name = "rightStick", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key leftStick { get { return new Key { Name = "leftStick", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key rightShoulder { get { return new Key { Name = "rightShoulder", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key leftShoulder { get { return new Key { Name = "leftShoulder", Type = typeof(ButtonControl), Bit = 1 }; } }
        //public static Key dpad { get { return new Key { Name = "dpad", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key dpadUp { get { return new Key { Name = "dpad", Type = typeof(DpadControl), Bit = 0 }; } }
        public static Key dpadDown { get { return new Key { Name = "dpad", Type = typeof(DpadControl), Bit = 1 }; } }
        public static Key dpadLeft { get { return new Key { Name = "dpad", Type = typeof(DpadControl), Bit = 2 }; } }
        public static Key dpadRight { get { return new Key { Name = "dpad", Type = typeof(DpadControl), Bit = 3 }; } }
        public static Key selectButton { get { return new Key { Name = "selectButton", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key startButton { get { return new Key { Name = "startButton", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key leftStickButton { get { return new Key { Name = "leftStickButton", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key buttonEast { get { return new Key { Name = "buttonEast", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key buttonSouth { get { return new Key { Name = "buttonSouth", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key buttonNorth { get { return new Key { Name = "buttonNorth", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key buttonWest { get { return new Key { Name = "buttonWest", Type = typeof(ButtonControl), Bit = 1 }; } }
        public static Key rightStickButton { get { return new Key { Name = "rightStickButton", Type = typeof(ButtonControl), Bit = 1 }; } }
    }
    #endregion
}
