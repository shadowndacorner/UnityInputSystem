using UnityEngine;
using InputSystem.XboxGamepad;
using System.Threading.Tasks;

namespace InputSystem.XboxGamepad.Components
{
    public class XboxGamepadMonitor : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        static void InitXboxManager()
        {
            var go = new GameObject("XboxGamepadManager");
            go.AddComponent<XboxGamepadMonitor>();
            go.hideFlags = HideFlags.DontSave;
        }

        private void Update()
        {
            GamepadHelper.Update();
        }
    }
}
