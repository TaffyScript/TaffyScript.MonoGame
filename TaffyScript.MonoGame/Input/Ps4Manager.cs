using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Timers;
using TaffyScript.MonoGame.Input.Win32;

namespace TaffyScript.MonoGame.Input
{
    public class Ps4Manager
    {
        HashSet<IntPtr> _processed = new HashSet<IntPtr>();

        event Action<Ps4Controller> ControllerConnected;

        public List<Ps4Controller> Controllers { get; } = new List<Ps4Controller>();

        public Ps4Manager(bool poll = true)
        {
            if(poll)
                PollDevices();
        }

        public Ps4Manager(Action<Ps4Controller> onControllerConnected, bool poll = true)
        {
            ControllerConnected += onControllerConnected;
            if (poll)
                PollDevices();
        }

        public static Tuple<Ps4Manager, Timer> GetManagerWithTimedPoll(int ms)
            => GetManagerWithTimedPoll(ms, null);

        public static Tuple<Ps4Manager, Timer> GetManagerWithTimedPoll(int ms, Action<Ps4Controller> onControllerConnected)
        {
            var poll = new Timer();
            var manager = new Ps4Manager(onControllerConnected);
            poll.Elapsed += (s, e) => manager.PollDevices();
            poll.Interval = ms;
            poll.AutoReset = true;
            poll.Enabled = true;
            return Tuple.Create(manager, poll);
        }

        public void PollDevices()
        {
            var devices = NativeMethods.GetRawInputDeviceList();
            foreach(var device in devices)
            {
                if(device.DeviceType == InputDeviceType.HID && !_processed.Contains(device.DeviceHandle))
                {
                    if(NativeMethods.TryRegisterPS4Controller(device.DeviceHandle, out var controller, out var product))
                    {
                        _processed.Add(device.DeviceHandle);
                        Controllers.Add(controller);
                        controller.Disconnected += (s, e) => 
                        {
                            _processed.Remove(device.DeviceHandle);
                            Controllers.Remove(controller);
                        };
                        ControllerConnected?.Invoke(controller);
                    }
                    else if(product != null)
                        _processed.Add(device.DeviceHandle);
                }
            }
        }
    }
}
