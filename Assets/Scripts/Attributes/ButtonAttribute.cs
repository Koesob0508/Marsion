using System;

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : Attribute
{
    public string Label { get; }

    public ButtonAttribute() { }

    public ButtonAttribute(string label) => Label = label;
}
