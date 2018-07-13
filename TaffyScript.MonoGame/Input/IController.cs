using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TaffyScript;

namespace TaffyScript.MonoGame.Input
{
    public interface IController : ITsInstance
    {
        event EventHandler<int> Disconnected;

        int Index { get; }
        bool IsConnected { get; }
        float LX { get; }
        float LY { get; }
        float RX { get; }
        float RY { get; }
        float LeftTrigger { get; }
        float RightTrigger { get; }
        float LeftAxisDeadZone { get; set; }
        float RightAxisDeadZone { get; set; }

        bool CurrentButtonDown(Buttons button);
        bool PreviousButtonDown(Buttons button);

        void Update();
    }
}
