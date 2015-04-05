using System;
using System.Collections.Generic;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class RegistryRepository : IRegistryRepository
    {
        private const string AutoStartLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";

        private static T ConvertValue<T>(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public Dictionary<T1, T2> GetSettings<T1, T2>(RegistryHKey registryHKey, string path)
        {
            var key = GetKey(registryHKey, path);
            if (key == null) throw new InvalidOperationException(string.Format("Cannot get key for registry path {0}.", path));

            var values = key.GetValueNames();

            var dict = new Dictionary<T1, T2>();

            foreach (var value in values)
            {
                var data = key.GetValue(value);

                dict.Add(ConvertValue<T1>(value), ConvertValue<T2>(data.ToString()));
            }

            return dict;
        }

        public T GetSetting<T>(RegistryHKey registryHKey, string path, string keyName, T defaultValue)
        {
            var key = GetKey(registryHKey, path);
            if (key == null) throw new InvalidOperationException(string.Format("Cannot get key for registry path {0}.", path));

            var value = key.GetValue(keyName);
            if (value == null)
            {
                if (defaultValue == null) throw new InvalidOperationException(string.Format("Cannot find setting for registry path {0} and key {1} and there is no default value provided.", path, keyName));
                return defaultValue;
            }
            return (T)value;
        }

        public void SetSetting<T>(RegistryHKey registryHKey, string path, string keyName, T value)
        {
            if (path == null) throw new ArgumentNullException("path", string.Format("Path cannot be null when saving to registry."));
            if (keyName == null) throw new ArgumentNullException("keyName", string.Format("KeyName cannot be null when saving to registry path '{0}'.", path));
            if (value == null) throw new ArgumentNullException("value", string.Format("Value cannot be null when saving to registry path '{0}' with key '{1}'.", path, keyName));

            var key = GetKey(registryHKey, path);
            if (key == null) throw new InvalidOperationException(string.Format("Cannot get key for registry path {0}.", path));

            key.SetValue(keyName, value);
        }

        public void RemoveSetting(RegistryHKey registryHKey, string path, string keyName)
        {
            var key = GetKey(registryHKey, path);
            if (key == null) throw new InvalidOperationException(string.Format("Cannot get key for registry path {0}.", path));

            key.DeleteValue(keyName);
        }

        public bool HasSetting(RegistryHKey registryHKey, string path, string keyName)
        {
            var key = GetKey(registryHKey, path);
            if (key == null)
                return false;

            var value = (string)key.GetValue(keyName);
            if (value == null)
                return false;

            return true;
        }

        public void SetAutoStart(RegistryHKey registryHKey, string keyName, string assemblyLocation)
        {
            if (!assemblyLocation.ToLower().EndsWith(".exe")) throw new InvalidOperationException(string.Format("The assembly {0} is not an executable (does not end with .exe), {1}.", keyName, assemblyLocation));
            if (!System.IO.File.Exists(assemblyLocation)) throw new InvalidOperationException(string.Format("The assembly {0} does not exist at location {1}.", keyName, assemblyLocation));

            SetSetting(registryHKey, AutoStartLocation, keyName, assemblyLocation);
        }

        public void RemoveAutoStart(RegistryHKey registryHKey, string keyName)
        {
            RemoveSetting(registryHKey, AutoStartLocation, keyName);
        }

        public bool IsAutoStartEnabled(RegistryHKey registryHKey, string keyName, string assemblyLocation)
        {
            if (!HasSetting(registryHKey, AutoStartLocation, keyName))
                return false;

            var result = GetSetting<string>(registryHKey, AutoStartLocation, keyName, null);
            return string.Compare(result, assemblyLocation, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        #region Private support methods


        private static Microsoft.Win32.RegistryKey GetKey(RegistryHKey environment, string path)
        {
            Microsoft.Win32.RegistryKey key;
            switch (environment)
            {
                case RegistryHKey.CurrentUser:
                    key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(path);
                    break;
                case RegistryHKey.LocalMachine:
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(path);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown environment {0}.", environment));
            }

            return key;
        }


        #endregion
    }
}