using System;

namespace TaffyScript.MonoGame.Input
{
    [Flags]
    public enum MouseButtons
    {
        None = 0,
        Left = 1,
        Middle = 2,
        Right = 4,
        WheelUp = 8,
        WheelDown = 16,
        X1 = 32,
        X2 = 64
    }
}
