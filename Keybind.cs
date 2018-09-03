using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

#if UNITY_WSA && !UNITY_EDITOR
using Windows.Gaming.Input;
#endif

namespace InputSystem.Keybinds
{
    [Serializable]
    public partial struct Binding
    {
        private bool _valid;

        public bool Valid
        {
            get
            {
                return _valid;
            }
        }

        private int _buttonBind;

        // TODO: Gamepad Buttons
    }

    [Serializable]
    public partial class Keybind
    {
        public Keybind(BindType type)
        {
            Type = type;
        }

        public int id;
        public BindType Type;

        // TODO: For builds, unsafe code w/ static length
        private List<Binding> _bind;
        public List<Binding> InputTypes
        {
            get
            {
                if (_bind == null)
                {
                    _bind = new List<Binding>();
                    for (int i = 0; i < (int)InputType.Count; ++i)
                    {
                        _bind.Add(default(Binding));
                    }
                }
                return _bind;
            }
        }

        public Binding GetBinding(InputType type)
        {
            return InputTypes[(int)type];
        }

        public bool HasBinding(InputType type)
        {
            return InputTypes[(int)type].Valid;
        }

        public bool AnyBindings
        {
            get
            {
                for(int i = 0; i < (int)InputType.Count; ++i)
                {
                    if (InputTypes[i].Valid)
                        return true;
                }
                return false;
            }
        }
    }
}
