using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using TaffyScript;

namespace TaffyScript.MonoGame.Input
{
    public abstract class AbstractController : IController
    {
        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public float LeftAxisDeadZone { get; set; } = .15f;
        public float RightAxisDeadZone { get; set; } = .15f;

        public abstract int Index { get; }
        public abstract bool IsConnected { get; }
        public abstract float LX { get; }
        public abstract float LY { get; }
        public abstract float RX { get; }
        public abstract float RY { get; }
        public abstract float LeftTrigger { get; }
        public abstract float RightTrigger { get; }
        public abstract string ObjectType { get; }

        public abstract event EventHandler<int> Disconnected;

        public abstract bool CurrentButtonDown(Buttons button);
        public abstract bool PreviousButtonDown(Buttons button);
        public abstract void Update();

        public TsObject Call(string scriptName, TsObject[] args)
        {
            switch (scriptName)
            {
                case "current_button_down":
                    return CurrentButtonDown((Buttons)(float)args[0]);
                case "previous_button_down":
                    return PreviousButtonDown((Buttons)(float)args[0]);
            }
            throw new MemberAccessException();
        }

        public TsObject GetMember(string name)
        {
            switch (name)
            {
                case "index":
                    return Index;
                case "is_connected":
                    return IsConnected;
                case "lx":
                    return LX;
                case "ly":
                    return LY;
                case "rx":
                    return RX;
                case "ry":
                    return RY;
                case "left_trigger":
                    return LeftTrigger;
                case "right_trigger":
                    return RightTrigger;
                case "left_axis_dead_zone":
                    return LeftAxisDeadZone;
                case "right_axis_dead_zone":
                    return RightAxisDeadZone;
                default:
                    if (TryGetDelegate(name, out var del))
                        return del;
                    throw new MemberAccessException();
            }
        }

        public void SetMember(string name, TsObject value)
        {
            switch (name)
            {
                case "left_axis_dead_zone":
                    LeftAxisDeadZone = (float)value;
                    break;
                case "right_axis_dead_zone":
                    RightAxisDeadZone = (float)value;
                    break;
                default:
                    throw new MemberAccessException();
            }
        }

        public bool TryGetDelegate(string delegateName, out TsDelegate del)
        {
            switch (delegateName)
            {
                case "current_button_down":
                    del = new TsDelegate((a) => CurrentButtonDown((Buttons)(float)a[0]), "current_button_down");
                    return true;
                case "previous_button_down":
                    del = new TsDelegate((a) => PreviousButtonDown((Buttons)(float)a[0]), "previous_button_down");
                    return true;
                default:
                    del = null;
                    return false;
            }
        }

        public TsDelegate GetDelegate(string delegateName)
        {
            if (TryGetDelegate(delegateName, out var del))
                return del;
            throw new MemberAccessException();
        }
    }
}
