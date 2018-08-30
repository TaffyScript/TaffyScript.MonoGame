using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myst.Collections;
using TaffyScript;
using TaffyScript.Collections;

namespace TaffyScript.MonoGame.Input
{
    [TaffyScriptBaseType]
    public static class InputManager
    {
#if Windows
        private static int _pollInterval;
#endif
        private static int _frames = 0;
        private static int _players;
        private static int _actionCount;
        private static Ps4Manager _ps4Manager;
        private static KeyboardState _keyboardCurrent;
        private static KeyboardState _keyboardPrevious;
        private static ClassBinder<IController> _controllers = new ClassBinder<IController>();
        private static MouseState _mouseCurrent;
        private static MouseState _mousePrevious;
        private static ActionMap[,] _actions;
        private static EventCache<bool> _controllerEvents = new EventCache<bool>();

        public static event EventHandler<IController> ControllerConnected;
        public static event EventHandler<IController> ControllerDisconnected;

        public static int Players => _players;
        public static int ActionCount => _actionCount;
        public static ActionMap[,] Actions => _actions;
        public static Vector2 MousePosition => _mouseCurrent.Position.ToVector2();

        public static bool KeyCheck(Keys key)
        {
            return _keyboardCurrent.IsKeyDown(key);
        }
        
        [TaffyScriptMethod]
        public static TsObject key_check(TsObject[] args)
        {
            return KeyCheck((Keys)(float)args[0]);
        }

        public static bool KeyCheckPressed(Keys key)
        {
            return _keyboardCurrent.IsKeyDown(key) && _keyboardPrevious.IsKeyUp(key);
        }

        [TaffyScriptMethod]
        public static TsObject key_check_pressed(TsObject[] args)
        {
            return KeyCheckPressed((Keys)(float)args[0]);
        }

        public static bool KeyCheckReleased(Keys key)
        {
            return _keyboardCurrent.IsKeyUp(key) && _keyboardPrevious.IsKeyDown(key);
        }

        [TaffyScriptMethod]
        public static TsObject key_check_released(TsObject[] args)
        {
            return KeyCheckReleased((Keys)(float)args[0]);
        }

        public static bool GamepadCheck(Buttons button, int index = 0)
        {
            return _controllers.Count > index && _controllers[index].CurrentButtonDown(button);
        }

        [TaffyScriptMethod]
        public static TsObject gamepad_check(TsObject[] args)
        {
            return GamepadCheck((Buttons)(float)args[0], args.Length > 1 ? (int)args[1] : 0);
        }

        public static bool GamepadCheckPressed(Buttons button, int index = 0)
        {
            return _controllers.Count > index && _controllers[index].CurrentButtonDown(button) && !_controllers[index].CurrentButtonDown(button);
        }

        [TaffyScriptMethod]
        public static TsObject gamepad_check_pressed(TsObject[] args)
        {
            return GamepadCheckPressed((Buttons)(float)args[0], args.Length > 1 ? (int)args[1] : 0);
        }

        public static bool GamepadCheckReleased(Buttons button, int index = 0)
        {
            return _controllers.Count > index && !_controllers[index].CurrentButtonDown(button) && _controllers[index].PreviousButtonDown(button);
        }

        [TaffyScriptMethod]
        public static TsObject gamepad_check_released(TsObject[] args)
        {
            return GamepadCheckReleased((Buttons)(float)args[0], args.Length > 1 ? (int)args[1] : 0);
        }

        private static bool MouseCheck(MouseState state, MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return state.LeftButton == ButtonState.Pressed;
                case MouseButtons.Middle:
                    return state.MiddleButton == ButtonState.Pressed;
                case MouseButtons.Right:
                    return state.RightButton == ButtonState.Pressed;
                case MouseButtons.X1:
                    return state.XButton1 == ButtonState.Pressed;
                case MouseButtons.X2:
                    return state.XButton2 == ButtonState.Pressed;
            }
            return false;
        }

        public static bool MouseCheck(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                case MouseButtons.Middle:
                case MouseButtons.Right:
                case MouseButtons.X1:
                case MouseButtons.X2:
                    return MouseCheck(_mouseCurrent, button);
                case MouseButtons.WheelDown:
                    return _mouseCurrent.ScrollWheelValue > _mousePrevious.ScrollWheelValue;
                case MouseButtons.WheelUp:
                    return _mouseCurrent.ScrollWheelValue < _mousePrevious.ScrollWheelValue;
            }
            return false;
        }

        [TaffyScriptMethod]
        public static TsObject mouse_check(TsObject[] args)
        {
            return MouseCheck((MouseButtons)(float)args[0]);
        }

        public static bool MouseCheckPressed(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                case MouseButtons.Middle:
                case MouseButtons.Right:
                case MouseButtons.X1:
                case MouseButtons.X2:
                    return MouseCheck(_mouseCurrent, button) && !MouseCheck(_mousePrevious, button);
                case MouseButtons.WheelDown:
                    return _mouseCurrent.ScrollWheelValue > _mousePrevious.ScrollWheelValue;
                case MouseButtons.WheelUp:
                    return _mouseCurrent.ScrollWheelValue < _mousePrevious.ScrollWheelValue;
            }
            return false;
        }

        [TaffyScriptMethod]
        public static TsObject mouse_check_pressed(TsObject[] args)
        {
            return MouseCheckPressed((MouseButtons)(float)args[0]);
        }

        public static bool MouseCheckReleased(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                case MouseButtons.Middle:
                case MouseButtons.Right:
                case MouseButtons.X1:
                case MouseButtons.X2:
                    return !MouseCheck(_mouseCurrent, button) && MouseCheck(_mousePrevious, button);
                default:
                    return false;
            }
        }

        [TaffyScriptMethod]
        public static TsObject mouse_check_released(TsObject[] args)
        {
            return MouseCheckReleased((MouseButtons)(float)args[0]);
        }

        [TaffyScriptMethod]
        public static TsObject mouse_has_moved(TsObject[] args)
        {
            return _mouseCurrent != _mousePrevious;
        }

        public static bool ActionCheck(int action, int player = 0)
        {
            return _actions[action, player].CurrentPressed;
        }

        [TaffyScriptMethod]
        public static TsObject action_check(TsObject[] args)
        {
            return ActionCheck((int)args[0], args.Length > 1 ? (int)args[1] : 0);
        }

        public static bool ActionCheckPressed(int action, int player = 0)
        {
            return _actions[action, player].CurrentPressed && !_actions[action, player].PreviousPressed;
        }

        [TaffyScriptMethod]
        public static TsObject action_check_pressed(TsObject[] args)
        {
            return ActionCheckPressed((int)args[0], args.Length > 1 ? (int)args[1] : 0);
        }

        public static bool ActionCheckReleased(int action, int player = 0)
        {
            return !_actions[action, player].CurrentPressed && _actions[action, player].PreviousPressed;
        }

        [TaffyScriptMethod]
        public static TsObject action_check_released(TsObject[] args)
        {
            return ActionCheckReleased((int)args[0], args.Length > 1 ? (int)args[1] : 0);
        }

        public static void Initialize(Game game, int ps4PollIntervalInFrames)
        {
            game.Exiting += (s, e) =>
            {
                foreach (var controller in _controllers)
                {
                    //Make sure any native handles are cleaned up.
                    if (controller is Ps4Controller ps4)
                        ps4.Close();
                }
            };

            for(var i = 0; i < GamePad.MaximumGamePadCount; i++)
            {
                var controller = new XInputController(i);
                _controllers.Add(controller);
                controller.Connected += (s, e) => ControllerConnected(null, (IController)s);
                controller.Disconnected += (s, e) => ControllerDisconnected(null, (IController)s);
            }

#if Windows

            _pollInterval = ps4PollIntervalInFrames;
            if(_pollInterval > 0)
            {
                _ps4Manager = new Ps4Manager((controller) =>
                {
                    var index = _controllers.Add(controller);
                    controller.SetIndex(index);
                    ControllerConnected?.Invoke(null, controller);
                    controller.Disconnected += (s, e) =>
                    {
                        var c = (IController)s;
                        ControllerDisconnected?.Invoke(null, c);
                        _controllers.FastRemove(c.Index);
                    };
                }, false);
                _frames = _pollInterval;
            }

#endif
        }

        public static void InitializeActions(int actions, int players)
        {
            if (actions <= 0)
                throw new ArgumentOutOfRangeException(nameof(actions));
            if (players <= 0)
                throw new ArgumentOutOfRangeException(nameof(players));

            _actionCount = actions;
            _players = players;

            _actions = new ActionMap[actions, players];
            for(var a = 0; a < actions; a++)
                for(var p = 0; p < players; p++)
                    _actions[a, p] = new ActionMap(0);
        }

        [TaffyScriptMethod]
        public static TsObject input_initialize(TsObject[] args)
        {
            InitializeActions((int)args[0], (int)args[1]);
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject input_get_action(TsObject[] args)
        {
            return _actions[(int)args[0], (int)args[1]];
        }

        [TaffyScriptMethod]
        public static TsObject input_set_action(TsObject[] args)
        {
            _actions[(int)args[0], (int)args[1]] = (ActionMap)args[2];
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject input_controller_connected_add(TsObject[] args)
        {
            var del = (TsDelegate)args[0];
            EventHandler<IController> handler = (s, e) => del.Invoke(new TsInstanceWrapper(e));
            ControllerConnected += handler;
            _controllerEvents.Cache(true, del, handler);
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject input_controller_connected_remove(TsObject[] args)
        {
            var del = (TsDelegate)args[0];
            if (_controllerEvents.TryRemove(true, del, out var handler))
            {
                ControllerConnected -= (EventHandler<IController>)handler;
                return true;
            }
            return false;
        }

        [TaffyScriptMethod]
        public static TsObject input_controller_disconnected_add(TsObject[] args)
        {
            var del = (TsDelegate)args[0];
            EventHandler<IController> handler = (s, e) => del.Invoke(new TsInstanceWrapper(e));
            ControllerDisconnected += handler;
            _controllerEvents.Cache(false, del, handler);
            return TsObject.Empty;
        }

        [TaffyScriptMethod]
        public static TsObject input_controller_disconnected_remove(TsObject[] args)
        {
            var del = (TsDelegate)args[0];
            if (_controllerEvents.TryRemove(false, del, out var handler))
            {
                ControllerDisconnected -= (EventHandler<IController>)handler;
                return true;
            }
            return false;
        }

        public static void Update()
        {
#if Windows
            if(_pollInterval > 0 && _frames++ >= _pollInterval)
            {
                _ps4Manager.PollDevices();
                _frames = 0;
            }
#endif
            _keyboardPrevious = _keyboardCurrent;
            _keyboardCurrent = Keyboard.GetState();
            _mousePrevious = _mouseCurrent;
            _mouseCurrent = Mouse.GetState();
            foreach (var controller in _controllers)
                controller.Update();

            if(_actions != null)
            {
                for(var a = 0; a < _actionCount; a++)
                {
                    for(var p = 0; p < _players; p++)
                    {
                        var act = _actions[a, p];
                        act.Update(_keyboardCurrent, _controllers.TryGetValue(act.ControllerIndex, out var controller) ? controller : null);
                    }
                }
            }
        }
    }
}
