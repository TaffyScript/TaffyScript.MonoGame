using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework.Input;
using static Microsoft.Xna.Framework.MathHelper;
using Myst.Collections;
using TaffyScript.MonoGame.Input.Win32;
using TaffyScript;

namespace TaffyScript.MonoGame.Input
{
    public class Ps4Controller : AbstractController
    {
        #region Constants

        /// <summary>
        /// Used to normalize an axis value.
        /// </summary>
        private const float AXISNORMAl = .00787401574f;

        /// <summary>
        /// Used to normalize a trigger value.
        /// </summary>
        private const float TRIGGERNORMAL = .00392156862f;

        #endregion

        #region Events

        public override event EventHandler<int> Disconnected;

        #endregion

        #region Fields

        private byte[] _readData;
        private HidPCaps _caps;
        private FileStream _fsRead;
        private SafeFileHandle _handleRead;
        private Buttons _currentState;
        private Buttons _previousState;
        private Buttons _asyncState;
        private int _index;
        private bool _isConnected = true;
        private float _lx = 0;
        private float _ly = 0;
        private float _rx = 0;
        private float _ry = 0;
        private float _leftTrigger = 0;
        private float _rightTrigger = 0;

        #endregion

        #region Properties

        public override int Index => _index;
        public override bool IsConnected => _isConnected;
        public override float LX => _lx;
        public override float LY => _ly;
        public override float RX => _rx;
        public override float RY => _ry;
        public override float LeftTrigger => _leftTrigger;
        public override float RightTrigger => _rightTrigger;
        public override string ObjectType => "ps4_controller";

        #endregion

        #region Constructor

        internal Ps4Controller(SafeFileHandle read, HidPCaps caps)
        {
            _handleRead = read;
            _caps = caps;

            _fsRead = new FileStream(read, FileAccess.ReadWrite, _caps.InputReportByteLength, false);
            _readData = new byte[_caps.InputReportByteLength];

            ReadAsync();
        }

        #endregion

        #region Public Api

        public override bool CurrentButtonDown(Buttons button)
        {
            return _currentState.HasFlag(button);
        }

        public override bool PreviousButtonDown(Buttons button)
        {
            return _previousState.HasFlag(button);
        }

        public override void Update()
        {
            _previousState = _currentState;
            _currentState = _asyncState;
            _asyncState = new Buttons();
        }

        public override string ToString()
        {
            return $"PS4 Controller: {Index}";
        }

        public void SetIndex(int index)
        {
            _index = index;
        }

        #endregion

        #region Private Api

        public void Close()
        {
            if(IsConnected)
            {
                if (_fsRead != null)
                {
                    _fsRead.Close();
                    _fsRead = null;
                }
                if(_handleRead != null && !_handleRead.IsInvalid)
                {
                    _handleRead.Close();
                    _handleRead = null;
                }

                _isConnected = false;

                Disconnected?.Invoke(this, Index);
            }
        }

        private void ReadAsync()
        {
            if (_fsRead.CanRead)
                _fsRead.BeginRead(_readData, 0, _readData.Length, GetInputReportData, _readData);
            else
                Close();
        }

        private void GetInputReportData(IAsyncResult ar)
        {
            try
            {
                _fsRead.EndRead(ar);
                ReadAsync();
                OnDataReceived(_readData);
            }
            catch
            {
                Close();
            }
        }

        // Set values based on info read from the input buffer.
        // Information on what the buffer contains can be found here:
        // http://www.psdevwiki.com/ps4/DS4-USB#Report_Structure
        private void OnDataReceived(byte[] message)
        {
            var lx = Clamp((message[1] - 128) * AXISNORMAl, -1, 1);
            var ly = Clamp((message[2] - 128) * AXISNORMAl, -1, 1);
            var rx = Clamp((message[3] - 128) * AXISNORMAl, -1, 1);
            var ry = Clamp((message[4] - 128) * AXISNORMAl, -1, 1);

            _lx = Math.Abs(lx) >= LeftAxisDeadZone ? lx : 0;
            _ly = Math.Abs(ly) >= LeftAxisDeadZone ? ly : 0;
            _rx = Math.Abs(rx) >= RightAxisDeadZone ? rx : 0;
            _ry = Math.Abs(ry) >= RightAxisDeadZone ? ry : 0;

            _leftTrigger = Clamp(message[8] * TRIGGERNORMAL, 0, 1);
            _rightTrigger = Clamp(message[9] * TRIGGERNORMAL, 0, 1);

            byte dpad = (byte)(message[5] & 15);
            _asyncState = new Buttons();
            _asyncState |= ((dpad == 0 || dpad == 7 || dpad == 1) ? Buttons.DPadUp : 0);
            _asyncState |= ((dpad == 7 || dpad == 6 || dpad == 5) ? Buttons.DPadLeft : 0);
            _asyncState |= ((dpad == 5 || dpad == 4 || dpad == 3) ? Buttons.DPadDown : 0);
            _asyncState |= ((dpad == 3 || dpad == 2 || dpad == 1) ? Buttons.DPadRight : 0);

            _asyncState |= (((message[5] & 128) == 128) ? Buttons.Y : 0);
            _asyncState |= (((message[5] & 64) == 64) ? Buttons.B : 0);
            _asyncState |= (((message[5] & 32) == 32) ? Buttons.A : 0);
            _asyncState |= (((message[5] & 16) == 16) ? Buttons.X : 0);

            _asyncState |= (((message[6] & 1) == 1) ? Buttons.LeftShoulder : 0);
            _asyncState |= (((message[6] & 2) == 2) ? Buttons.RightShoulder : 0);
            _asyncState |= (((message[6] & 4) == 4) ? Buttons.LeftTrigger : 0);
            _asyncState |= (((message[6] & 8) == 8) ? Buttons.RightTrigger : 0);

            _asyncState |= (((message[6] & 16) == 16) ? Buttons.Back : 0);
            _asyncState |= (((message[6] & 32) == 32) ? Buttons.Start : 0);
            _asyncState |= (((message[6] & 64) == 64) ? Buttons.LeftStick : 0);
            _asyncState |= (((message[6] & 128) == 128) ? Buttons.RightStick : 0);

            _asyncState |= LY <= -LeftAxisDeadZone ? Buttons.LeftThumbstickUp : 0;
            _asyncState |= LX <= -LeftAxisDeadZone ? Buttons.LeftThumbstickLeft : 0;
            _asyncState |= LY >= LeftAxisDeadZone ? Buttons.LeftThumbstickDown : 0;
            _asyncState |= LX >= LeftAxisDeadZone ? Buttons.LeftThumbstickRight : 0;

            _asyncState |= RY <= -RightAxisDeadZone ? Buttons.RightThumbstickUp : 0;
            _asyncState |= RX <= -RightAxisDeadZone ? Buttons.RightThumbstickLeft : 0;
            _asyncState |= RY >= RightAxisDeadZone ? Buttons.RightThumbstickDown : 0;
            _asyncState |= RX >= RightAxisDeadZone ? Buttons.RightThumbstickRight : 0;

            _asyncState |= (((message[7] & 1) == 1) ? Buttons.BigButton : 0);

            // There is more data present, but none that is shared with the Monogame button set.

            _currentState |= _asyncState;
        }

        #endregion
    }
}
