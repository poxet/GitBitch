using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Windows
{
    public class LockMachineCommand : GitBitchCommand
    {
        [DllImport("user32.dll")]
        private static extern bool LockWorkStation();

        public LockMachineCommand(ISettingAgent settingAgent)
            : base(settingAgent, "Lock", new[] { "lock machine" })
        {
        }

        public async override Task ExecuteAsync(string key, string phrase)
        {
            LockWorkStation();
        }
    }
}