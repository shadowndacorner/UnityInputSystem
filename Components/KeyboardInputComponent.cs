using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InputSystem.Keybinds;
using UInp = UnityEngine.Input;

namespace InputSystem.Components
{
    public class KeyboardInputComponent : InputComponent
    {
        public override bool IsConnected
        {
            get
            {
                return UInp.mousePresent;   // if a mouse isn't present, we can assume that kbm isn't supported
            }
        }

        public override int AnyPriority
        {
            get
            {
                return int.MaxValue;
            }
        }

        public override bool AnyRequiresEnabled => false;

        public override InputType Type
        {
            get
            {
                return InputType.KeyboardMouse;
            }
        }

        private Vector2 MouseMove
        {
            get
            {
                return new Vector2(UInp.GetAxis("Mouse X"), UInp.GetAxis("Mouse Y"));
            }
        }

        public override bool AnyInput
        {
            get
            {
                return UInp.anyKey || MouseMove.sqrMagnitude > 0;
            }
        }

        public override bool GetButtonHeld(Keybind bind)
        {
            if (bind.HasBinding(InputType.KeyboardMouse))
            {
                return UInp.GetKey(bind.GetBinding(InputType.KeyboardMouse).KeyCode);
            }
            return false;
        }

        public override bool GetButtonPressed(Keybind bind)
        {
            if (bind.HasBinding(InputType.KeyboardMouse))
            {
                return UInp.GetKeyDown(bind.GetBinding(InputType.KeyboardMouse).KeyCode);
            }
            return false;
        }

        public override bool GetButtonReleased(Keybind bind)
        {
            if (bind.HasBinding(InputType.KeyboardMouse))
            {
                return UInp.GetKeyUp(bind.GetBinding(InputType.KeyboardMouse).KeyCode);
            }
            return false;
        }

        public override Vector2 GetVector(Keybind _bind)
        {
            var bind = _bind.KBM;
            if (!bind.Valid)
                return Vector2.zero;

            if (bind.IsMouseLook)
            {
                return MouseMove;
            }
            else if (bind.IsScrollWheel)
            {
                return UInp.mouseScrollDelta;
            }

            var vec = Vector2.zero;
            bool used = false;
            if (UInp.GetKey(bind.KBMUpKey))
            {
                used = true;
                vec += Vector2.up;
            }

            if (UInp.GetKey(bind.KBMDownKey))
            {
                used = true;
                vec -= Vector2.up;
            }

            if (UInp.GetKey(bind.KBMRightKey))
            {
                used = true;
                vec += Vector2.right;
            }

            if (UInp.GetKey(bind.KBMLeftKey))
            {
                used = true;
                vec -= Vector2.right;
            }

            if (!used)
                return Vector2.zero;

            return vec.normalized;
        }

        public override float GetFloat(Keybind _bind)
        {
            var bind = _bind.KBM;
            if (!bind.Valid)
                return 0;

            if (bind.IsScrollWheel)
            {
                return UInp.mouseScrollDelta.y;
            }
            return GetButtonHeld(_bind) ? 1 : 0;
        }
    }
}