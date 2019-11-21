using System;
using System.Linq;
using System.Collections.Generic;
using Autofac.Features.Metadata;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Core.Services
{
    public class DeviceService : IDeviceRemover
    {
        private IEnumerable<Meta<Func<string, IDevice>>> DeviceFactory { get; }
        private readonly ScoreTracker _scoreTracker;
        private readonly StatusTracker _statusTracker;
        private Dictionary<string, DeviceMetadata> DeviceDictionary { get; } = new Dictionary<string, DeviceMetadata>();
        public IEnumerable<DeviceMetadata> Devices => DeviceDictionary.Select(kv => kv.Value);
        public DeviceService(IEnumerable<Meta<Func<string, IDevice>>> deviceFactory,
            ScoreTracker scoreTracker, StatusTracker statusTracker)
        {
            DeviceFactory = deviceFactory;
            _scoreTracker = scoreTracker;
            _statusTracker = statusTracker;
            // TODO: Auto-connect to devices (or reconnect on restart)?
            // Should store last-known devices
        }
        public DeviceMetadata Add(string deviceType, string connectionString, int role, string program)
        {
            return Add(deviceType, connectionString, (DeviceRole)role, program);
        }
        public DeviceMetadata Add(string deviceType, string connectionString, DeviceRole role = DeviceRole.None, string program = "")
        {
            if (DeviceDictionary.ContainsKey(connectionString))
                return DeviceDictionary[connectionString];
            var device = DeviceFactory
                .First(df => (string)df.Metadata["Type"] == deviceType)
                .Value(connectionString);
            if (!device.Connect())
                return null;
            var metaData =  new DeviceMetadata(device, DeviceRole.None, connectionString, program, deviceType);
            DeviceDictionary.Add(connectionString, metaData);
            SetRole(connectionString, role);
            SetProgram(connectionString, program);
            return metaData;
        }

        public IEnumerable<DeviceMetadata> TryAddAll()
        {
            foreach (var option in GetDeviceOptions())
            {
                foreach (var port in option.Ports)
                {
                    yield return Add(option.DeviceType, port);
                }
            }
        }

        public IEnumerable<DeviceOptions> GetDeviceOptions() => DeviceFactory
            .Select(df => new DeviceOptions((string)df.Metadata["Type"], (string[])df.Metadata["Ports"]));
        public bool SetProgram(string connectionString, string program)
        {
            if (!DeviceDictionary.ContainsKey(connectionString))
                return false;
            var device = DeviceDictionary[connectionString];
            device.Program = program;
            device.Device.Program = program;
            return true;
        }
        public bool SetRole(string connectionString, int role)
        {
            return SetRole(connectionString, (DeviceRole)role);
        }
        public bool SetRole(string connectionString, string role)
        {
            if (!Enum.TryParse(role, true, out DeviceRole deviceRole))
                return false;
            return SetRole(connectionString, deviceRole);
        }
        public bool SetRole(string connectionString, DeviceRole role)
        {
            if (!DeviceDictionary.ContainsKey(connectionString))
                return false;
            var device = DeviceDictionary[connectionString];
            DeviceRole rolesToAdd = (role ^ device.Role) & role;
            DeviceRole rolesToRemove = (role ^ device.Role) & (~role);
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
            if (!DeviceDictionary.ContainsKey(connectionString))
                return false;
            try
            {
                var device = DeviceDictionary[connectionString];
                DeviceDictionary.Remove(connectionString);
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
            public DeviceMetadata(IDevice device, DeviceRole role, string connectionString, string program, string deviceType)
            {
                Device = device;
                Role = role;
                ConnectionString = connectionString;
                Program = program;
                DeviceType = deviceType;
                Programs = device.GetPrograms();
            }
            public IDevice Device { get; }
            public DeviceRole Role { get; set; }
            public string ConnectionString { get; }
            public string Program { get; set; }
            public IEnumerable<string> Programs { get; }
            public string DeviceType { get; set; }
        }

        public class DeviceOptions
        {
            public DeviceOptions(string deviceType, IEnumerable<string> ports)
            {
                DeviceType = deviceType;
                Ports = ports;
            }
            public string DeviceType { get; }
            public IEnumerable<string> Ports { get; }
        }
    }
}