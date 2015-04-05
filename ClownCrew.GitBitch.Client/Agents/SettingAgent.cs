using System.Collections.Generic;
using System.Configuration;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class SettingAgent : ISettingAgent
    {
        private readonly Dictionary<string, object> _settings = new Dictionary<string, object>();

        public void SetSetting<T>(string name, T value)
        {
            _settings.Add(name, value);
        }

        public T GetSetting<T>(string name, T defaultValue)
        {
            if (_settings.ContainsKey(name))
                return (T)_settings[name];
            return defaultValue;
        }

        public T GetSetting<T>(string name)
        {
            if (_settings.ContainsKey(name))
                return (T)_settings[name];
            throw new SettingsPropertyNotFoundException("Cannot find setting with name " + name + ".");
        }
    }
}