using System.Collections.Generic;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ISettingAgent
    {
        bool HasSetting(string name);
        void SetSetting<T>(string name, T value);
        void SetSetting<T>(string subPath, string name, T value);
        T GetSetting<T>(string name, T defaultValue);
        T GetSetting<T>(string subPath, string name, T defaultValue);
        Dictionary<string, T> GetSettings<T>(string subPath);
        void UseAutoStart(bool value);
    }
}