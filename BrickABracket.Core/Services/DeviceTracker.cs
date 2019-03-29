using System;
using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Core.Services
{
    public class DeviceTracker
    {
        private Func<string, IDevice> _deviceFactory {get;}
        private Dictionary<string, IDevice> _devices {get;} = new Dictionary<string, IDevice>();
        public DeviceTracker(Func<string, IDevice> deviceFactory)
        {
            _deviceFactory = deviceFactory;
            // TODO: Auto-connect to devices (or reconnect on restart)?
        }

        public bool AddDevice(string connectionString)
        {
            if (_devices.ContainsKey(connectionString))
                return false;
            var device = _deviceFactory(connectionString);
            if (!device.Connect())
                return false;
            _devices.Add(connectionString, device);
            return true;
        }

        public bool RemoveDevice(string connectionString)
        {
            if (!_devices.ContainsKey(connectionString))
                return false;
            try
            {
                var device = _devices[connectionString];
                _devices.Remove(connectionString);
                device.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}