using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class SettingAgent : ISettingAgent
    {
        public T GetSetting<T>(string name, T defaultValue)
        {
            return defaultValue;
        }
    }
}