using System;
using System.Collections.Generic;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Core.Services
{
    public class DeviceService
    {
        private Func<string, IDevice> _deviceFactory {get;}
        private ScoreTracker _scoreTracker;
        private StatusTracker _statusTracker;
        private Dictionary<string, DeviceMetadata> _devices {get;} = new Dictionary<string, DeviceMetadata>();
        public DeviceService(Func<string, IDevice> deviceFactory,
            ScoreTracker scoreTracker, StatusTracker statusTracker)
        {
            _deviceFactory = deviceFactory;
            _scoreTracker = scoreTracker;
            _statusTracker = statusTracker;
            // TODO: Auto-connect to devices (or reconnect on restart)?
        }

        public bool Add(string connectionString, DeviceRole role)
        {
            if (_devices.ContainsKey(connectionString))
                return false;
            var device = _deviceFactory(connectionString);
            if (!device.Connect())
                return false;
            _devices.Add(connectionString, new DeviceMetadata(device, role, connectionString));
            return true;
        }

        private bool AddRole(string connectionString, DeviceRole role)
        {
            // TODO: check if role already exists on device, otherwise add
            return true;
        }

        public bool Remove(string connectionString)
        {
            if (!_devices.ContainsKey(connectionString))
                return false;
            try
            {
                var device = _devices[connectionString];
                _devices.Remove(connectionString);
                device.Device.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public struct DeviceMetadata
        {
            public DeviceMetadata(IDevice device, DeviceRole role, string connectionString)
            {
                Device = device;
                Role = role;
                ConnectionString = connectionString;
            }
            public IDevice Device;
            public DeviceRole Role;
            public string ConnectionString;
        }
    }
}