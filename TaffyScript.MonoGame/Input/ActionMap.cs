using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TaffyScript.MonoGame.Input
{
    public class ActionMap : ITsInstance
    {
        public int ControllerIndex  { get; set; }
        public List<Keys> Keys { get; } = new List<Keys>();
        public List<Buttons> Buttons { get; } = new List<Buttons>();
        public bool CurrentPressed { get; protected set; }
        public bool PreviousPressed { get; protected set; }

        public TsObject this[string memberName]
        {
            get => GetMember(memberName);
            set => SetMember(memberName, value);
        }

        public virtual string ObjectType => "TaffyScript.MonoGame.Input.ActionMap";

        public ActionMap(int controllerIndex)
        {
            ControllerIndex = controllerIndex;
        }

        public ActionMap(TsObject[] args)
        {
            ControllerIndex = (int)args[0];
        }

        public virtual TsObject Call(string scriptName, TsObject[] args)
        {
            switch(scriptName)
            {
                case "add_key":
                    return add_key(args);
                case "add_button":
                    return add_button(args);
                case "reset":
                    return reset(args);
                default:
                    throw new MissingMethodException(ObjectType, scriptName);
            }
        }

        public TsDelegate GetDelegate(string scriptName)
        {
            if (TryGetDelegate(scriptName, out var del))
                return del;
            throw new MissingMethodException(ObjectType, scriptName);
        }

        public virtual bool TryGetDelegate(string scriptName, out TsDelegate del)
        {
            switch(scriptName)
            {
                case "add_key":
                    del = new TsDelegate(add_key, "add_key");
                    return true;
                case "add_button":
                    del = new TsDelegate(add_button, "add_button");
                    return true;
                case "reset":
                    del = new TsDelegate(reset, "reset");
                    return true;
                default:
                    del = null;
                    return false;
            }
        }

        public virtual TsObject GetMember(string name)
        {
            switch(name)
            {
                case "controller":
                    return ControllerIndex;
                default:
                    if (TryGetDelegate(name, out var del))
                        return del;
                    throw new MissingFieldException(ObjectType, name);
            }
        }

        public virtual void SetMember(string name, TsObject value)
        {
            switch(name)
            {
                case "controller":
                    ControllerIndex = (int)value;
                    break;
                default:
                    throw new MissingFieldException(ObjectType, name);
            }
        }

        public virtual void Update(KeyboardState keyboard, IController controller)
        {
            PreviousPressed = CurrentPressed;
            foreach(var key in Keys)
            {
                if (keyboard.IsKeyDown(key))
                {
                    CurrentPressed = true;
                    return;
                }
            }
            if(controller != null && controller.IsConnected)
            {
                foreach(var button in Buttons)
                {
                    if(controller.CurrentButtonDown(button))
                    {
                        CurrentPressed = true;
                        return;
                    }
                }
            }
            CurrentPressed = false;
        }

        private TsObject add_key(TsObject[] args)
        {
            Keys.Add((Keys)(float)args[0]);
            return TsObject.Empty;
        }

        private TsObject add_button(TsObject[] args)
        {
            Buttons.Add((Buttons)(float)args[0]);
            return TsObject.Empty;
        }

        protected virtual TsObject reset(TsObject[] args)
        {
            Keys.Clear();
            Buttons.Clear();
            return TsObject.Empty;
        }

        public static implicit operator TsObject(ActionMap map)
        {
            return new TsInstanceWrapper(map);
        }

        public static explicit operator ActionMap(TsObject obj)
        {
            return (ActionMap)obj.WeakValue;
        }
    }

    public class MouseActionMap : ActionMap
    {
        public List<MouseButtons> MouseButtons { get; } = new List<MouseButtons>();
        public override string ObjectType => "TaffyScript.MonoGame.Input.MouseActionMap";

        public MouseActionMap(int controllerIndex)
            : base(controllerIndex)
        {
        }

        public override TsObject Call(string scriptName, TsObject[] args)
        {
            if (scriptName == "add_mouse_button")
                return add_mouse_button(args);
            return base.Call(scriptName, args);
        }

        public override bool TryGetDelegate(string scriptName, out TsDelegate del)
        {
            if (scriptName == "add_mouse_button")
            {
                del = new TsDelegate(add_mouse_button, "add_mouse_button");
                return true;
            }
            return base.TryGetDelegate(scriptName, out del);
        }

        public override void Update(KeyboardState keyboard, IController controller)
        {
            base.Update(keyboard, controller);
            if(!CurrentPressed)
            {
                foreach(var mb in MouseButtons)
                {
                    if(InputManager.MouseCheck(mb))
                    {
                        CurrentPressed = true;
                        return;
                    }
                }
            }
        }

        private TsObject add_mouse_button(TsObject[] args)
        {
            MouseButtons.Add((MouseButtons)(float)args[0]);
            return TsObject.Empty;
        }
    }
}
