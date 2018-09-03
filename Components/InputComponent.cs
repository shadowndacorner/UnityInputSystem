using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputSystem.Keybinds;

namespace InputSystem.Components
{
    public class InputComponent : MonoBehaviour
    {
        public virtual bool IsConnected
        {
            get
            {
                return true;
            }
        }

        public virtual InputType Type
        {
            get
            {
                return InputType.Invalid;
            }
        }

        public virtual bool AnyInput
        {
            get
            {
                return false;
            }
        }

        public virtual int AnyPriority
        {
            get
            {
                return 0;
            }
        }

        public virtual bool AnyRequiresEnabled
        {
            get
            {
                return false;
            }
        }

        public virtual void SetupAny() { }

        public virtual Vector2 GetVector(Keybind bind)
        {
            return Vector2.zero;
        }

        public virtual float GetFloat(Keybind bind)
        {
            return 0;
        }

        public virtual bool GetButton(Keybind bind)
        {
            return false;
        }

        public virtual bool GetButtonPressed(Keybind bind)
        {
            return GetButton(bind);
        }

        public virtual bool GetButtonReleased(Keybind bind)
        {
            return false;
        }

        public virtual void Vibrate(float amount, float left, float right, float duration = 0)
        {

        }
    }
}