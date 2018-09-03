using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputSystem;
using InputSystem.Keybinds;

namespace InputSystem.KeyboardMouse
{
    public static class KeyboardMouseExtensions
    {
        public static Keybind SetKeyboardVector(this Keybind _bind, KeyCode up, KeyCode left, KeyCode down, KeyCode right)
        {
            if (_bind.Type != BindType.Vector)
            {
                throw new InvalidOperationException("Attempted to set keyboard vector for non vector input");
            }

            var bind = _bind.KBM;
            bind.KBMUpKey = up;
            bind.KBMDownKey = down;
            bind.KBMRightKey = right;
            bind.KBMLeftKey = left;
            _bind.KBM = bind;
            return _bind;
        }

        public static Keybind SetUseMouse(this Keybind _bind)
        {
            if (_bind.Type != BindType.Vector)
            {
                throw new InvalidOperationException("Attempted to set mouse for non vector input");
            }

            var bind = _bind.KBM;
            bind.IsMouseLook = true;
            _bind.KBM = bind;
            return _bind;
        }

        public static Keybind SetUseScrollWheel(this Keybind _bind)
        {
            if (_bind.Type != BindType.Vector && _bind.Type != BindType.Float)
            {
                throw new InvalidOperationException("Attempted to set mouse wheel for non vector, non float input");
            }

            var bind = _bind.KBM;
            bind.IsScrollWheel = true;
            _bind.KBM = bind;
            return _bind;
        }

        public static Keybind SetKeyboardMouseButton(this Keybind _bind, KeyCode code)
        {
            if (_bind.Type != BindType.Button)
            {
                throw new InvalidOperationException("Attempted to set keyboard vector for non button input");
            }

            var bind = _bind.KBM;
            bind.KeyCode = code;
            _bind.KBM = bind;
            return _bind;
        }
    }
}

namespace InputSystem.Keybinds
{
    public partial class Keybind
    {
        public Binding KBM
        {
            get
            {
                return InputTypes[(int)InputType.KeyboardMouse];
            }
            set
            {
                InputTypes[(int)InputType.KeyboardMouse] = value;
            }
        }
    }

    public partial struct Binding
    {
        // Mouse
        enum MouseMode
        {
            None,
            Axis,
            Scroll
        }

        private MouseMode _mouse_mode;

        private KeyCode _kbLeft;
        private KeyCode _kbRight;
        private KeyCode _kbDown;

        // Keyboard
        public KeyCode KeyCode
        {
            get
            {
                return (KeyCode)_buttonBind;
            }
            set
            {
                _valid = true;
                _buttonBind = (int)value;
            }
        }

        public KeyCode KBMUpKey
        {
            get
            {
                return KeyCode;
            }
            set
            {
                KeyCode = value;
            }
        }

        public KeyCode KBMRightKey
        {
            get
            {
                return _kbRight;
            }
            set
            {
                _valid = true;
                _kbRight = value;
            }
        }

        public KeyCode KBMLeftKey
        {
            get
            {
                return _kbLeft;
            }
            set
            {
                _valid = true;
                _kbLeft = value;
            }
        }

        public KeyCode KBMDownKey
        {
            get
            {
                return _kbDown;
            }
            set
            {
                _valid = true;
                _kbDown = value;
            }
        }

        public bool IsMouseLook
        {
            get
            {
                return _mouse_mode == MouseMode.Axis;
            }
            set
            {
                _valid = true;
                _mouse_mode = MouseMode.Axis;
            }
        }

        public bool IsScrollWheel
        {
            get
            {
                return _mouse_mode == MouseMode.Scroll;
            }
            set
            {
                _valid = true;
                _mouse_mode = MouseMode.Scroll;
            }
        }
    }
}