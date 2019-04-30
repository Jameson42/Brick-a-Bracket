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
        public bool Add(string connectionString, string program, string deviceType, string role)
        {
            if (!Enum.TryParse(role, true, out DeviceRole deviceRole))
                return false;
            return Add(connectionString, program, deviceType, deviceRole);
        }
        public bool Add(string connectionString, string program, string deviceType = "NXT", DeviceRole role = DeviceRole.None)
        {
            if (_devices.ContainsKey(connectionString))
                return false;
            var device = _deviceFactory
                .First(df => (string)df.Metadata["Type"] == deviceType)
                .Value(connectionString);
            if (!device.Connect())
                return false;
            _devices.Add(connectionString, new DeviceMetadata(device, DeviceRole.None, connectionString, program));
            SetRole(connectionString, role);
            SetProgram(connectionString, program);
            return true;
        }
        public bool SetProgram(string connectionString, string program)
        {
            if (!_devices.ContainsKey(connectionString))
                return false;
            var device = _devices[connectionString];
            device.Program = program;
            device.Device.Program = program;
            return true;
        }
        public bool SetRole(string connectionString, string role)
        {
            if (!Enum.TryParse(role, true, out DeviceRole deviceRole))
                return false;
            return SetRole(connectionString, deviceRole);
        }
        public bool SetRole(string connectionString, DeviceRole role)
        {
            if (!_devices.ContainsKey(connectionString))
                return false;
            var device = _devices[connectionString];
            DeviceRole rolesToAdd = (role ^ device.Role) & role;
            DeviceRole rolesToRemove = (role & device.Role);
            AddRoles(device, rolesToAdd);
            RemoveRoles(device, rolesToRemove);
            return true;
        }
        private void AddRoles(DeviceMetadata device, DeviceRole roles)
        {
            if ((roles & DeviceRole.ScoreProvider) == DeviceRole.ScoreProvider)
                _scoreTracker.Add(device.Device);
            if ((roles & DeviceRole.StatusProvider) == DeviceRole.StatusProvider)
                _statusTracker.Add(device.Device);
            if ((roles & DeviceRole.StatusFollower) == DeviceRole.StatusFollower)
                device.Device.FollowStatus(_statusTracker.Statuses);
            device.Role |= roles;
        }
        private void RemoveRoles(DeviceMetadata device, DeviceRole roles)
        {
            if ((roles & DeviceRole.ScoreProvider) == DeviceRole.ScoreProvider)
                _scoreTracker.Remove(device.Device);
            if ((roles & DeviceRole.StatusProvider) == DeviceRole.StatusProvider)
                _statusTracker.Remove(device.Device);
            if ((roles & DeviceRole.StatusFollower) == DeviceRole.StatusFollower)
                device.Device.UnFollowStatus();
            device.Role &= (device.Role ^ roles);
        }

        public bool Remove(string connectionString)
        {
            if (!_devices.ContainsKey(connectionString))
                return false;
            try
            {
                var device = _devices[connectionString];
                _devices.Remove(connectionString);
                RemoveRoles(device, DeviceRole.All);
                device.Device.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public class DeviceMetadata
        {
            public DeviceMetadata(IDevice device, DeviceRole role, string connectionString, string program)
            {
                Device = device;
                Role = role;
                ConnectionString = connectionString;
                Program = program;
            }
            public IDevice Device {get;}
            public DeviceRole Role {get;set;}
            public string ConnectionString {get;}
            public string Program {get;set;}
        }
    }
}