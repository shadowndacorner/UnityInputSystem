﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputSystem.Components;
using InputSystem;
using InputSystem.Keybinds;

public class AnyInputComponent : InputComponent
{
    public override bool AnyInput => base.AnyInput;
    public override InputType Type => base.Type;

    public InputComponent ActiveDriver;
    public List<InputComponent> Drivers = new List<InputComponent>();

    private void OnEnable()
    {
        foreach (var type in typeof(InputComponent).Assembly.ExportedTypes)
        {
            if (type != typeof(InputComponent) && type != typeof(AnyInputComponent) && typeof(InputComponent).IsAssignableFrom(type))
            {
                var go = new GameObject("AnyInput_" + type.Name);
                go.transform.parent = transform;
                go.hideFlags = HideFlags.DontSave;

                var inp = go.AddComponent(type) as InputComponent;
                inp.SetupAny();
                inp.gameObject.SetActive(inp.AnyRequiresEnabled);
                Drivers.Add(inp);
            }
        }

        Drivers.OrderBy((a) => a.AnyPriority);
    }

    private void OnDisable()
    {
        foreach (var v in Drivers)
        {
            Destroy(v.gameObject);
        }
    }

    public override bool GetButtonHeld(Keybind bind)
    {
        if (ActiveDriver)
            return ActiveDriver.GetButtonHeld(bind);

        return false;
    }

    public override bool GetButtonPressed(Keybind bind)
    {
        if (ActiveDriver)
            return ActiveDriver.GetButtonPressed(bind);

        return false;
    }

    public override bool GetButtonReleased(Keybind bind)
    {
        if (ActiveDriver)
            return ActiveDriver.GetButtonReleased(bind);

        return false;
    }

    public override float GetFloat(Keybind bind)
    {
        if (ActiveDriver)
            return ActiveDriver.GetFloat(bind);

        return 0;
    }

    public override Vector2 GetVector(Keybind bind)
    {
        if (ActiveDriver)
            return ActiveDriver.GetVector(bind);

        return Vector2.zero;
    }

    public override void Vibrate(float amount, float left, float right, float duration = 0)
    {
        base.Vibrate(amount, left, right, duration);
    }

    private void Update()
    {
        foreach(var driver in Drivers)
        {
            if (driver.AnyInput)
            {
                ActiveDriver = driver;
            }
        }
    }
}
