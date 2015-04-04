namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ISettingAgent
    {
        T GetSetting<T>(string prefix, T defaultValue);
    }
}