using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class SettingAgent : ISettingAgent
    {
        private const string RegistryPath = @"Software\Thargelion\GitBitch";
        private readonly IRegistryRepository _registryRepository;

        public SettingAgent(IRegistryRepository registryRepository)
        {
            _registryRepository = registryRepository;
        }

        public bool HasSetting(string name)
        {
            return _registryRepository.HasSetting(RegistryHKey.CurrentUser, RegistryPath, name);
        }

        public void SetSetting<T>(string name, T value)
        {
            _registryRepository.SetSetting(RegistryHKey.CurrentUser, RegistryPath, name, value);
        }

        public void SetSetting<T>(string subPath, string name, T value)
        {
            _registryRepository.SetSetting(RegistryHKey.CurrentUser, RegistryPath + "\\" + subPath, name, value);
        }

        public T GetSetting<T>(string name, T defaultValue)
        {
            return _registryRepository.GetSetting(RegistryHKey.CurrentUser, RegistryPath, name, defaultValue);
        }
    }
}