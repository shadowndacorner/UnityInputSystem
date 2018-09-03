# Unity Input System

This is a custom input system for Unity with runtime rebindable keybindings and a very slick API.  It currently has support for keyboard/mouse (using Unity's builtin input) and XInput (via XInputDotNet).

## Usage in Unity
Currently, you can use KeyboardInputComponent, XboxControllerInputComponent, or AnyInputComponent.  AnyComponent will automatically swap between any supported input method.  This is very useful for singleplayer games.

XboxControllerInputComponent can also allow input from either a specific Xbox controller OR from any.  This simply requires setting the `PlayerIndex` or `AnyController` fields respectively.

Simply attach the desired input component to a game object, set up your keybindings to be accessible (as shown in the static Binds class below), and read input from your classes.

## Example
```c#
// Binds.cs
using UnityEngine;
using InputSystem.KeyboardMouse;
using InputSystem.Components;
using InputSystem.XboxGamepad;

public static class Binds
{
    public static InputSystem.VectorKeybind MoveBind = new InputSystem.VectorKeybind("move", "This Is A Pretty Name", "Here you can describe what the key will do");
    public static InputSystem.VectorKeybind AimBind = new InputSystem.VectorKeybind("aim"); // above params aren't necessary
    public static InputSystem.ButtonKeybind JumpButton = new InputSystem.ButtonKeybind("jump");
    public static InputSystem.ButtonKeybind SprintButton;   // in fact, you don't have to assign a string ID if you don't want to

    [InputSystem.Attributes.BindingGenerationMethod]
    static void GenerateBindings()
    {
        // Set up defaults if we haven't loaded in
        if (!MoveBind.Bind.AnyBindings)
        {
            MoveBind.Bind
                // Assign the keyboard keys WASD to map to a vector
                .SetKeyboardVector(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D)
                // And assign the left stick to map to a vector
                .SetXboxStick(XboxStick.LeftStick);
        }

        if (!AimBind.Bind.AnyBindings)
        {
            AimBind.Bind
                // Use mouse look
                .SetUseMouse()
                // Use the right stick
                .SetXboxStick(XboxStick.RightStick);
        }

        if (!JumpButton.Bind.AnyBindings)
        {
            JumpButton.Bind
                .SetKeyboardMouseButton(KeyCode.Space)
                .SetXboxButton(XboxButton.A);
        }

        if (!SprintButton.Bind.AnyBindings)
        {
            SprintButton.Bind
                .SetKeyboardMouseButton(KeyCode.LeftShift)
                .SetXboxButton(XboxButton.LeftStick);
        }
    }
}

...
// MoveSprite.cs
using UnityEngine;
using InputSystem.KeyboardMouse;
using InputSystem.Components;
using InputSystem.XboxGamepad;
public class MoveSprite : MonoBehaviour
{
    // Lazily loading the input controller
    private InputComponent _inp;
    public InputComponent Input
    {
        get
        {
            if (!_inp)
            {
                _inp = GetComponent<InputComponent>();
            }
            return _inp;
        }
    }

    void Update()
    {
        // If we have a valid input controller...
        if (Input)
        {
            float mult = 1;
            
            // If we are holding down the sprint button (shift or left stick by default)
            if (Input.GetButtonHeld(Binds.SprintButton))
            {
                // up our speed
                mult *= 2;
            }
            
            // Move with wasd + mouse OR left/right stick
            transform.position += (Vector3)((Input.GetVector(Binds.MoveBind) + Input.GetVector(Binds.AimBind)) * Time.deltaTime) * mult;
        }

        // If we press the jump key (space or A)...
        if ((Input && Input.GetButtonPressed(Binds.JumpButton)))
        {
            // reset our position (because that's what a jump does, ofc)
            transform.position = Vector3.zero;
        }
    }
}
```