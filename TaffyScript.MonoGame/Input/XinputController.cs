using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace TaffyScript.MonoGame.Input
{
    public class XInputController : AbstractController
    {
        private GamePadState _currentState;
        private GamePadState _previousState;
        private bool _connected;

        public override int Index { get; }
        public override bool IsConnected => _connected;
        public override float LX => Math.Abs(_currentState.ThumbSticks.Left.X) >= LeftAxisDeadZone ? _currentState.ThumbSticks.Left.X : 0;
        public override float LY => Math.Abs(_currentState.ThumbSticks.Left.Y) >= LeftAxisDeadZone ? _currentState.ThumbSticks.Left.Y : 0;
        public override float RX => Math.Abs(_currentState.ThumbSticks.Right.X) >= RightAxisDeadZone ? _currentState.ThumbSticks.Right.X : 0;
        public override float RY => Math.Abs(_currentState.ThumbSticks.Right.Y) >= RightAxisDeadZone ? _currentState.ThumbSticks.Right.Y : 0;
        public override float LeftTrigger => _currentState.Triggers.Left;
        public override float RightTrigger => _currentState.Triggers.Right;
        public override string ObjectType => "xbox_controller";

        public override event EventHandler<int> Disconnected;
        public event EventHandler<int> Connected;

        public XInputController(int index)
        {
            Index = index;
            _currentState = GamePad.GetState(index);
        }

        public override void Update()
        {
            _previousState = _currentState;
            _currentState = GamePad.GetState(Index);
            if(!_connected && _currentState.IsConnected)
            {
                _connected = true;
                Connected?.Invoke(this, Index);
            }
            if (_connected && !_currentState.IsConnected)
            {
                _connected = false;
                Disconnected?.Invoke(this, Index);
            }
        }

        public override bool CurrentButtonDown(Buttons button)
        {
            switch(button)
            {
                case Buttons.LeftThumbstickUp:
                    return _currentState.ThumbSticks.Left.Y <= -LeftAxisDeadZone;
                case Buttons.LeftThumbstickLeft:
                    return _currentState.ThumbSticks.Left.X <= -LeftAxisDeadZone;
                case Buttons.LeftThumbstickDown:
                    return _currentState.ThumbSticks.Left.Y >= LeftAxisDeadZone;
                case Buttons.LeftThumbstickRight:
                    return _currentState.ThumbSticks.Left.X >= LeftAxisDeadZone;
                case Buttons.RightThumbstickUp:
                    return _currentState.ThumbSticks.Right.Y <= -RightAxisDeadZone;
                case Buttons.RightThumbstickLeft:
                    return _currentState.ThumbSticks.Right.X <= -RightAxisDeadZone;
                case Buttons.RightThumbstickDown:
                    return _currentState.ThumbSticks.Right.Y >= RightAxisDeadZone;
                case Buttons.RightThumbstickRight:
                    return _currentState.ThumbSticks.Right.X >= RightAxisDeadZone;
                default:
                    return _currentState.IsButtonDown(button);
            }
        }

        public override bool PreviousButtonDown(Buttons button)
        {
            switch (button)
            {
                case Buttons.LeftThumbstickUp:
                    return _previousState.ThumbSticks.Left.Y <= -LeftAxisDeadZone;
                case Buttons.LeftThumbstickLeft:
                    return _previousState.ThumbSticks.Left.X <= -LeftAxisDeadZone;
                case Buttons.LeftThumbstickDown:
                    return _previousState.ThumbSticks.Left.Y >= LeftAxisDeadZone;
                case Buttons.LeftThumbstickRight:
                    return _previousState.ThumbSticks.Left.X >= LeftAxisDeadZone;
                case Buttons.RightThumbstickUp:
                    return _previousState.ThumbSticks.Right.Y <= -RightAxisDeadZone;
                case Buttons.RightThumbstickLeft:
                    return _previousState.ThumbSticks.Right.X <= -RightAxisDeadZone;
                case Buttons.RightThumbstickDown:
                    return _previousState.ThumbSticks.Right.Y >= RightAxisDeadZone;
                case Buttons.RightThumbstickRight:
                    return _previousState.ThumbSticks.Right.X >= RightAxisDeadZone;
                default:
                    return _previousState.IsButtonDown(button);
            }
        }
    }
}