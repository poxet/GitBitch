namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ISettingAgent
    {
        void SetSetting<T>(string name, T value);
        T GetSetting<T>(string name, T defaultValue);
        T GetSetting<T>(string name);
    }
}