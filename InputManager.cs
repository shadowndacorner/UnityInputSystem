using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputSystem.Keybinds;
using System.Reflection;

namespace InputSystem
{
    public enum BindType
    {
        Button,
        Float,
        Vector
    }

    public enum InputType
    {
        KeyboardMouse,
        XboxGamepad,
        Count,
        Invalid
    }

    public static partial class Input
    {
        //[RuntimeInitializeOnLoadMethod]
        private static void InitializeBindings()
        {
            // Load keybindings
            foreach (var type in typeof(Input).GetTypeInfo().Assembly.ExportedTypes)
            {
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    foreach (var v in method.GetCustomAttributes(typeof(Attributes.BindingGenerationMethodAttribute), false))
                    {
                        method.Invoke(null, new object[0]);
                        break;
                    }
                }
            }

            // TODO: Load from file
        }

        private static List<Keybind> _b;
        private static List<Keybind> Bindings
        {
            get
            {
                if (_b == null)
                {
                    _b = new List<Keybind>();
                    InitializeBindings();
                }
                return _b;
            }
        }

        public static Keybind RegisterKeybind(Keybind bind)
        {
            var ind = Bindings.Count;
            bind.id = ind + 1;
            Bindings.Add(bind);
            return bind;
        }

        public static Keybind CreateButtonKeybind(string identifier, string nice_name = null, string description = null)
        {
            return RegisterKeybind(new Keybind(BindType.Button));
        }

        public static Keybind CreateFloatKeybind(string identifier, string nice_name = null, string description = null)
        {
            return RegisterKeybind(new Keybind(BindType.Float));
        }

        public static Keybind CreateVectorKeybind(string identifier, string nice_name = null, string description = null)
        {
            return RegisterKeybind(new Keybind(BindType.Vector));
        }

        public static Keybind GetBindingFromID(int id)
        {
            return (id > 0 && id <= Bindings.Count) ? Bindings[id - 1] : null;
        }
    }

    [System.Serializable]
    public struct ButtonKeybind
    {
        private int _id;

        public int ID
        {
            get
            {
                return _id;
            }
            internal set
            {
                _id = value;
            }
        }

        [SerializeField]
        private string _identifier;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _description;

        public ButtonKeybind(string identifier, string name = null, string description = null)
        {
            _identifier = identifier;
            _name = name;
            _description = description;
            _id = -1;
        }

        public Keybind Bind
        {
            get
            {
                Keybind _bind = Input.GetBindingFromID(ID);
                if (_bind == null)
                {
                    _bind = Input.CreateButtonKeybind(_identifier, _name, _description);
                    ID = _bind.id;
                }
                return _bind;
            }
        }

        public static implicit operator Keybind(ButtonKeybind bind)
        {
            return bind.Bind;
        }
    }


    [System.Serializable]
    public struct FloatKeybind
    {
        private int _id;

        public int ID
        {
            get
            {
                return _id;
            }
            internal set
            {
                _id = value;
            }
        }

        [SerializeField]
        private string _identifier;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _description;

        public FloatKeybind(string identifier, string name = null, string description = null)
        {
            _identifier = identifier;
            _name = name;
            _description = description;
            _id = -1;
        }

        public Keybind Bind
        {
            get
            {
                Keybind _bind = Input.GetBindingFromID(ID);
                if (_bind == null)
                {
                    _bind = Input.CreateFloatKeybind(_identifier, _name, _description);
                    ID = _bind.id;
                }
                return _bind;
            }
        }

        public static implicit operator Keybind(FloatKeybind bind)
        {
            return bind.Bind;
        }
    }

    [System.Serializable]
    public struct VectorKeybind
    {
        private int _id;

        public int ID
        {
            get
            {
                return _id;
            }
            internal set
            {
                _id = value;
            }
        }

        [SerializeField]
        private string _identifier;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _description;

        public VectorKeybind(string identifier, string name = null, string description = null)
        {
            _identifier = identifier;
            _name = name;
            _description = description;
            _id = -1;
        }

        public Keybind Bind
        {
            get
            {
                Keybind _bind = Input.GetBindingFromID(ID);
                if (_bind == null)
                {
                    _bind = Input.CreateVectorKeybind(_identifier, _name, _description);
                    ID = _bind.id;
                }
                return _bind;
            }
        }

        public static implicit operator Keybind(VectorKeybind bind)
        {
            return bind.Bind;
        }
    }
}
