using System;
using System.Linq;
using System.Collections.Generic;
using Autofac.Features.Metadata;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Core.Services
{
    public class DeviceService
    {
        private IEnumerable<Meta<Func<string, IDevice>>> _deviceFactory {get;}
        private ScoreTracker _scoreTracker;
        private StatusTracker _statusTracker;
        private Dictionary<string, DeviceMetadata> _devices {get;} = new Dictionary<string, DeviceMetadata>();
        public IEnumerable<DeviceMetadata> Devices => _devices.Select(kv => kv.Value);
        public DeviceService(IEnumerable<Meta<Func<string, IDevice>>> deviceFactory,
            ScoreTracker scoreTracker, StatusTracker statusTracker)
        {
            _deviceFactory = deviceFactory;
            _scoreTracker = scoreTracker;
            _statusTracker = statusTracker;
            // TODO: Auto-connect to devices (or reconnect on restart)?
            // Should store last-known devices
        }

        public bool Add(string connectionString, string deviceType = "NXT", DeviceRole role = DeviceRole.None)
        {
            if (_devices.ContainsKey(connectionString))
                return false;
            var device = _deviceFactory
                .First(df => (string)df.Metadata["Type"] == deviceType)
                .Value(connectionString);
            if (!device.Connect())
                return false;
            _devices.Add(connectionString, new DeviceMetadata(device, DeviceRole.None, connectionString));
            AddRole(connectionString, role);
            return true;
        }
        public bool AddRole(string connectionString, DeviceRole role)
        {
            if (!_devices.ContainsKey(connectionString))
                return false;
            var device = _devices[connectionString];
            DeviceRole rolesToAdd = (role ^ device.Role) & role;
            if ((rolesToAdd & DeviceRole.ScoreProvider) == DeviceRole.ScoreProvider)
                _scoreTracker.Add(device.Device);
            if ((rolesToAdd & DeviceRole.StatusProvider) == DeviceRole.StatusProvider)
                _statusTracker.Add(device.Device);
            if ((rolesToAdd & DeviceRole.StatusFollower) == DeviceRole.StatusFollower)
                device.Device.FollowStatus(_statusTracker.Statuses);
            device.Role |= role;          
            return true;
        }

        public bool RemoveRole(string connectionString, DeviceRole role)
        {
            if (!_devices.ContainsKey(connectionString))
                return false;
                var device = _devices[connectionString];
                DeviceRole rolesToRemove = (role & device.Role);
            if ((rolesToRemove & DeviceRole.ScoreProvider) == DeviceRole.ScoreProvider)
                _scoreTracker.Remove(device.Device);
            if ((rolesToRemove & DeviceRole.StatusProvider) == DeviceRole.StatusProvider)
                _statusTracker.Remove(device.Device);
            if ((rolesToRemove & DeviceRole.StatusFollower) == DeviceRole.StatusFollower)
                device.Device.UnFollowStatus();
            device.Role &= (device.Role ^ role);
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