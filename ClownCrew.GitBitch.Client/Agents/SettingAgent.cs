using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class SettingAgent : ISettingAgent
    {
        private readonly IRegistryRepository _registryRepository;
        private string _registryPath = @"Software\Thargelion\GitBitch";

        public SettingAgent(IRegistryRepository registryRepository)
        {
            _registryRepository = registryRepository;
        }

        public bool HasSetting(string name)
        {
            return _registryRepository.HasSetting(RegistryHKey.CurrentUser, _registryPath, name);
        }

        public void SetSetting<T>(string name, T value)
        {
            _registryRepository.SetSetting(RegistryHKey.CurrentUser, _registryPath, name, value);
        }

        public T GetSetting<T>(string name, T defaultValue)
        {
            return _registryRepository.GetSetting(RegistryHKey.CurrentUser, _registryPath, name, defaultValue);
        }
    }
}