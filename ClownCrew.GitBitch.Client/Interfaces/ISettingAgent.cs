﻿namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ISettingAgent
    {
        bool HasSetting(string name);
        void SetSetting<T>(string name, T value);
        T GetSetting<T>(string name, T defaultValue);
    }
}