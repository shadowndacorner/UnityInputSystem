using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputSystem.XboxGamepad;
using InputSystem.Keybinds;

namespace InputSystem.Components
{
    public class XboxControllerInput : InputComponent
    {
        public override bool IsConnected
        {
            get
            {
                return ValidController && GamepadHelper.GamepadConnected(PlayerIndex);
            }
        }

        public override InputType Type
        {
            get
            {
                return InputType.XboxGamepad;
            }
        }

        public override bool AnyInput
        {
            get
            {
                return ValidController && GamepadHelper.AnyInput(PlayerIndex);
            }
        }

        public override bool AnyRequiresEnabled => true;

        public override void SetupAny()
        {
            AnyController = true;
        }

        [SerializeField]
        private int _playerIndex;
        public int PlayerIndex { get { return _playerIndex; } set { _playerIndex = value; } }

        public bool AnyController;

        public bool ValidController
        {
            get
            {
                return PlayerIndex >= 0 && PlayerIndex < 4;
            }
        }

        public override bool GetButton(Keybind bind)
        {
            if (!ValidController || !bind.Xbox.Valid)
                return false;

            return GamepadHelper.GetGamepadButtonHeld(_playerIndex, bind.Xbox.XboxButton);
        }

        public override bool GetButtonPressed(Keybind bind)
        {
            if (!ValidController || !bind.Xbox.Valid)
                return false;

            return GamepadHelper.GetGamepadButtonPressed(_playerIndex, bind.Xbox.XboxButton);
        }

        public override bool GetButtonReleased(Keybind bind)
        {
            if (!ValidController || !bind.Xbox.Valid)
                return false;

            return GamepadHelper.GetGamepadButtonReleased(_playerIndex, bind.Xbox.XboxButton);
        }

        public override float GetFloat(Keybind bind)
        {
            if (!ValidController || !bind.Xbox.Valid)
                return 0;

            return GamepadHelper.GetGamepadTrigger(_playerIndex, bind.Xbox.XboxTrigger);
        }

        public override Vector2 GetVector(Keybind bind)
        {
            if (!ValidController || !bind.Xbox.Valid)
                return Vector2.zero;

            return GamepadHelper.GetGamepadStick(_playerIndex, bind.Xbox.XboxStick);
        }

        public override void Vibrate(float amount, float left, float right, float duration = 0)
        {
            if (!ValidController)
                return;

            GamepadHelper.SetVibration(_playerIndex, left, right);

            // TODO: Duration
        }

        private void OnDestroy()
        {
            GamepadHelper.SetVibration(PlayerIndex, 0, 0);
        }

        private void Update()
        {
            if (AnyController)
            {
                for (int i = 0; i < 4; ++i)
                {
                    if (!GamepadHelper.GamepadConnected(i))
                        continue;

                    if (GamepadHelper.AnyInput(i))
                    {
                        PlayerIndex = i;
                        break;
                    }
                }

                if (!GamepadHelper.GamepadConnected(PlayerIndex))
                    PlayerIndex = -1;
            }

        }
    }
}
