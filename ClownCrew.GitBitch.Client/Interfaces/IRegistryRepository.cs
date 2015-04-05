namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface IRegistryRepository
    {
        T GetSetting<T>(RegistryHKey registryHKey, string path, string keyName, T defaultValue);
        void SetSetting<T>(RegistryHKey registryHKey, string path, string keyName, T value);
        void RemoveSetting(RegistryHKey registryHKey, string path, string keyName);
        bool HasSetting(RegistryHKey registryHKey, string registryPath, string keyName);

        void SetAutoStart(RegistryHKey registryHKey, string keyName, string assemblyLocation);
        void RemoveAutoStart(RegistryHKey registryHKey, string keyName);
        bool IsAutoStartEnabled(RegistryHKey registryHKey, string keyName, string assemblyLocation);
    }
}