using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputSystem.Keybinds;
using InputSystem.XboxGamepad;
using XInputDotNetPure;

#if UNITY_WINRT && !UNITY_EDITOR
using Windows.Gaming.Input;

namespace InputSystem.XboxGamepad
{
    public static class XInputWindowsGamepadAdapter
    {
        static XInputWindowsGamepadAdapter()
        {
            Debug.LogError("Initializing");
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
        }

        static HashSet<Gamepad> _pads = new HashSet<Gamepad>();
        private static void Gamepad_GamepadAdded(object sender, Gamepad pad)
        {
            if (_pads.Contains(pad))
            {
                Debug.LogError("PREEXISTING PAD");
            }
            else
            {
                _pads.Add(pad);
            }

            Debug.LogError("Gamepad added");
            pad.HeadsetConnected += Pad_HeadsetConnected;
            pad.HeadsetDisconnected += Pad_HeadsetDisconnected;
            pad.UserChanged += Pad_UserChanged;
        }

        private static void Gamepad_GamepadRemoved(object sender, Gamepad pad)
        {
            Debug.LogError("Gamepad removed");
            pad.HeadsetConnected -= Pad_HeadsetConnected;
            pad.HeadsetDisconnected -= Pad_HeadsetDisconnected;
            pad.UserChanged -= Pad_UserChanged;
        }

        private static void Pad_UserChanged(IGameController sender, Windows.System.UserChangedEventArgs args)
        {
            Debug.Log("user changed");
        }

        private static void Pad_HeadsetConnected(IGameController sender, Headset args)
        {
            Debug.Log("headset connected");
        }

        private static void Pad_HeadsetDisconnected(IGameController sender, Headset args)
        {
            Debug.Log("headset disconnected");
        }

        private static ButtonState ButtonPressed(this GamepadReading reading, GamepadButtons btn)
        {
            return (((int)reading.Buttons & (int)btn) != 0) ? ButtonState.Pressed : ButtonState.Released;
        }

        public static GamePadState ConvertWin10ToGamepadState(Gamepad pad)
        {
            var reading = pad.GetCurrentReading();
            var buttons = new GamePadButtons(
                reading.ButtonPressed(GamepadButtons.Menu),
                reading.ButtonPressed(GamepadButtons.View),
                reading.ButtonPressed(GamepadButtons.LeftThumbstick),
                reading.ButtonPressed(GamepadButtons.RightThumbstick),
                reading.ButtonPressed(GamepadButtons.LeftShoulder),
                reading.ButtonPressed(GamepadButtons.RightShoulder),
                ButtonState.Released,
                reading.ButtonPressed(GamepadButtons.A),
                reading.ButtonPressed(GamepadButtons.B),
                reading.ButtonPressed(GamepadButtons.X),
                reading.ButtonPressed(GamepadButtons.Y));

            var dpad = new GamePadDPad(
                reading.ButtonPressed(GamepadButtons.DPadUp),
                reading.ButtonPressed(GamepadButtons.DPadDown),
                reading.ButtonPressed(GamepadButtons.DPadLeft),
                reading.ButtonPressed(GamepadButtons.DPadRight));

            float leftx = (float)reading.LeftThumbstickX;
            float lefty = (float)reading.LeftThumbstickY;
            float rightx = (float)reading.RightThumbstickX;
            float righty = (float)reading.RightThumbstickY;

            const float deadzone = 0.1f;
            if (Mathf.Abs(leftx) < deadzone)
                leftx = 0;

            if (Mathf.Abs(lefty) < deadzone)
                lefty = 0;

            if (Mathf.Abs(rightx) < deadzone)
                rightx = 0;

            if (Mathf.Abs(righty) < deadzone)
                righty= 0;

            var sticks = new GamePadThumbSticks(
                new GamePadThumbSticks.StickValue(leftx, lefty),
                new GamePadThumbSticks.StickValue(rightx, righty)
            );

            var triggers = new GamePadTriggers(
                (float)reading.LeftTrigger,
                (float)reading.RightTrigger
            );

            return new GamePadState(true, 0, buttons, dpad, sticks, triggers);
        }
    }
}
#endif

namespace InputSystem.XboxGamepad
{
    public enum XboxButton
    {
        None,
        A,
        B,
        X,
        Y,
        LB,
        RB,
        Start,
        Back,
        DPadUp,
        DPadRight,
        DPadDown,
        DPadLeft,
        //Guide,
        LeftTrigger,
        RightTrigger,
        LeftStick,
        RightStick,

        // TODO: Stick directions
        Count
    }

    public enum XboxStick
    {
        None,
        LeftStick,
        RightStick,
        Count
    }

    public enum XboxTrigger
    {
        None,
        LeftTrigger,
        RightTrigger,
        Count
    }

    public static class XInputExtensions
    {
        public static void SetXboxStick(this Keybind _bind, XboxStick stick)
        {
            var bind = _bind.Xbox;
            bind.XboxStick = stick;
            _bind.Xbox = bind;
        }

        public static void SetXboxTrigger(this Keybind _bind, XboxTrigger trigger)
        {
            var bind = _bind.Xbox;
            bind.XboxTrigger = trigger;
            _bind.Xbox = bind;
        }

        public static void SetXboxButton(this Keybind _bind, XboxButton button)
        {
            var bind = _bind.Xbox;
            bind.XboxButton = button;
            _bind.Xbox = bind;
        }
    }

    public static class GamepadHelper
    {
        private static GamePadState _internalGetState(int index)
        {
#if UNITY_WINRT && !UNITY_EDITOR
            try
            {
                if (index < Gamepad.Gamepads.Count && index >= 0)
                {
                    var pad = Gamepad.Gamepads[index];
                    return XInputWindowsGamepadAdapter.ConvertWin10ToGamepadState(pad);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return new GamePadState(false, 0, new GamePadButtons(), new GamePadDPad(), new GamePadThumbSticks(), new GamePadTriggers());
#else
            return GamePad.GetState((PlayerIndex)index);
#endif
        }

        struct GamepadUpdateState
        {
            public GamePadState state;
            public int lastFrame;
        }

        private static List<GamepadUpdateState> _states;
        private static List<GamepadUpdateState> States
        {
            get
            {
                if (_states == null)
                {
                    _states = new List<GamepadUpdateState>();
                    for (int i = 0; i < 4; ++i)
                    {
                        _states.Add(default(GamepadUpdateState));
                    }
                }
                return _states;
            }
        }

        private static List<GamePadState> _prevStates;
        private static List<GamePadState> PrevStates
        {
            get
            {
                if (_prevStates == null)
                {
                    _prevStates = new List<GamePadState>();
                    for (int i = 0; i < 4; ++i)
                    {
                        _prevStates.Add(default(GamePadState));
                    }
                }
                return _prevStates;
            }
        }

        private const float triggerDeadZone = 0.01f;
        public static bool GetGamepadButtonHeld(GamePadState state, XboxButton btn)
        {
            switch (btn)
            {
                case XboxButton.A:
                    return state.Buttons.A == ButtonState.Pressed;
                case XboxButton.B:
                    return state.Buttons.B == ButtonState.Pressed;
                case XboxButton.Back:
                    return state.Buttons.Back == ButtonState.Pressed;
                case XboxButton.DPadDown:
                    return state.DPad.Down == ButtonState.Pressed;
                case XboxButton.DPadLeft:
                    return state.DPad.Left == ButtonState.Pressed;
                case XboxButton.DPadRight:
                    return state.DPad.Right == ButtonState.Pressed;
                case XboxButton.DPadUp:
                    return state.DPad.Up == ButtonState.Pressed;
                case XboxButton.LB:
                    return state.Buttons.LeftShoulder == ButtonState.Pressed;
                case XboxButton.LeftStick:
                    return state.Buttons.LeftStick == ButtonState.Pressed;
                case XboxButton.LeftTrigger:
                    return Mathf.Abs(state.Triggers.Left) > triggerDeadZone;
                case XboxButton.RB:
                    return state.Buttons.RightShoulder == ButtonState.Pressed;
                case XboxButton.RightStick:
                    return state.Buttons.RightStick == ButtonState.Pressed;
                case XboxButton.RightTrigger:
                    return Mathf.Abs(state.Triggers.Left) > triggerDeadZone;
                case XboxButton.Start:
                    return state.Buttons.Start == ButtonState.Pressed;
                case XboxButton.X:
                    return state.Buttons.X == ButtonState.Pressed;
                case XboxButton.Y:
                    return state.Buttons.Y == ButtonState.Pressed;
                case XboxButton.None:
                    return false;
            }
            throw new InvalidOperationException("Attempted to read unhandled gamepad button " + btn.ToString());
        }

        public static Vector2 GetGamepadStick(GamePadState state, XboxStick stick)
        {
            switch (stick)
            {
                case XboxStick.LeftStick:
                    return new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
                case XboxStick.RightStick:
                    return new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
                case XboxStick.None:
                    return Vector2.zero;
            }
            throw new InvalidOperationException("Attempted to read invalid stick " + stick);
        }

        public static float GetGamepadTrigger(GamePadState state, XboxTrigger trigger)
        {
            switch (trigger)
            {
                case XboxTrigger.LeftTrigger:
                    return state.Triggers.Left;
                case XboxTrigger.RightTrigger:
                    return state.Triggers.Right;
                case XboxTrigger.None:
                    return 0;
            }
            throw new InvalidOperationException("Attempted to read invalid trigger " + trigger);
        }

        private static void UpdateState(int index)
        {
            var st = States[index];
            if (st.lastFrame != Time.frameCount)
            {
                PrevStates[index] = st.state;
                st.state = _internalGetState(index);
                st.lastFrame = Time.frameCount;
                States[index] = st;
            }
        }

        private static GamePadState GetLastState(int index)
        {
            UpdateState(index);
            return PrevStates[index];
        }

        private static GamePadState GetState(int index)
        {
            UpdateState(index);
            return States[index].state;
        }

        public static bool GetGamepadButtonHeld(int index, XboxButton btn)
        {
            return GetGamepadButtonHeld(GetState(index), btn);
        }

        public static bool AnyInput(int index)
        {
            for (int i = 0; i < (int)XboxButton.Count; ++i)
            {
                if (GetGamepadButtonHeld(index, (XboxButton)i))
                {
                    return true;
                }
            }

            for (int i = 0; i < (int)XboxTrigger.Count; ++i)
            {
                if (GetGamepadTrigger(index, (XboxTrigger)i) > 0)
                {
                    return true;
                }
            }

            for (int i = 0; i < (int)XboxStick.Count; ++i)
            {
                if (GetGamepadStick(index, (XboxStick)i).sqrMagnitude > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GetGamepadButtonReleased(int index, XboxButton btn)
        {
            var prev = GetLastState(index);
            var cur = GetState(index);

            return GetGamepadButtonHeld(prev, btn) && !GetGamepadButtonHeld(cur, btn);
        }

        public static bool GetGamepadButtonPressed(int index, XboxButton btn)
        {
            var prev = GetLastState(index);
            var cur = GetState(index);
            return !GetGamepadButtonHeld(prev, btn) && GetGamepadButtonHeld(cur, btn);
        }

        public static Vector2 GetGamepadStick(int index, XboxStick stick)
        {
            var cur = GetState(index);
            return GetGamepadStick(cur, stick);
        }

        public static Vector2 GetGamepadStickDelta(int index, XboxStick stick)
        {
            var prev = GetLastState(index);
            var cur = GetState(index);
            return GetGamepadStick(cur, stick) - GetGamepadStick(prev, stick);
        }

        public static float GetGamepadTrigger(int index, XboxTrigger trigger)
        {
            var cur = GetState(index);
            return GetGamepadTrigger(cur, trigger);
        }

        public static float GetGamepadTriggerDelta(int index, XboxTrigger trigger)
        {
            var prev = GetLastState(index);
            var cur = GetState(index);
            return GetGamepadTrigger(cur, trigger) - GetGamepadTrigger(prev, trigger);
        }

        public static bool GamepadConnected(int index)
        {
            return index >= 4 || index < 0 || GetState(index).IsConnected;
        }

        public static void SetVibration(int index, float left, float right)
        {
            GamePad.SetVibration((PlayerIndex)index, left, right);
        }

        // This runs once per frame on the main thread
        // Exists so that input stays up to date in the
        // event that nobody is using a controller, that
        // way if someone connects one, they don't have
        // stale data
        public static void Update()
        {
            for (int i = 0; i < 4; ++i)
            {
                UpdateState(i);
            }
        }
    }
}

namespace InputSystem.Keybinds
{
    public partial class Keybind
    {
        public Binding Xbox
        {
            get
            {
                return InputTypes[(int)InputType.XboxGamepad];
            }
            set
            {
                InputTypes[(int)InputType.XboxGamepad] = value;
            }
        }
    }

    public partial struct Binding
    {
        public XboxButton XboxButton
        {
            get
            {
                return (XboxButton)_buttonBind;
            }
            set
            {
                _valid = true;
                _buttonBind = (int)value;
            }
        }

        public XboxStick XboxStick
        {
            get
            {
                return (XboxStick)_buttonBind;
            }
            set
            {
                _valid = true;
                _buttonBind = (int)value;
            }
        }

        public XboxTrigger XboxTrigger
        {
            get
            {
                return (XboxTrigger)_buttonBind;
            }
            set
            {
                _valid = true;
                _buttonBind = (int)value;
            }
        }
    }
}