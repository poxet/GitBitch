using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Commands.Application;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await SetBitchNameAsync();
            await App.RegisterCommandsAsync();
            await Greeting();
            await CompositeRoot.Instance.TalkAgent.SayAsync("What can I help you with?");
        }

        private static async Task Greeting()
        {
            if (CompositeRoot.Instance.SettingAgent.HasSetting("YourName"))
            {
                var yourname = CompositeRoot.Instance.SettingAgent.GetSetting("YourName", Constants.BitchName);
                await CompositeRoot.Instance.TalkAgent.SayAsync("Wellcome back " + yourname + ".");
                return;
            }

            var bitchName = CompositeRoot.Instance.SettingAgent.GetSetting(Constants.BitchName, Constants.DefaultBitchName);
            await CompositeRoot.Instance.TalkAgent.SayAsync("Hi! this is git bitch alfa. My name is " + bitchName + ".");

            await AskForOperstorsName();
        }

        private static async Task AskForOperstorsName()
        {
            var names = new List<string> { Constants.DefaultBitchName, "Ivona", "Astra", "Zira", "Leeloominai", "Leeloo", "Master", "Commander", "Mister", "Miss", "Mistress" };
            if (System.IO.File.Exists("Names.txt")) names.AddRange(System.IO.File.ReadAllLines("Names.txt"));
            var alternatives = names.Select(x => new QuestionAnswerAlternative<string> { Phrases = new List<string> { x }, Response = x }).ToList();
            var response = await CompositeRoot.Instance.QuestionAgent.AskAsync("What is your name?", alternatives, 12000);
            if (response.Response == Constants.DefaultBitchName) await CompositeRoot.Instance.TalkAgent.SayAsync("Have it your way, I will just call you " + response.Response + " then.");
            else await CompositeRoot.Instance.TalkAgent.SayAsync("O my god, you can't be serious. How am I supposed to work with someone named " + response.Response + ".");

            CompositeRoot.Instance.SettingAgent.SetSetting("YourName", response.Response);
        }

        private static async Task SetBitchNameAsync()
        {
            if (CompositeRoot.Instance.SettingAgent.HasSetting(Constants.BitchName)) return;

            await new ChangeNameCommand(CompositeRoot.Instance.SettingAgent).ExecuteAsync(string.Empty);
        }
    }
}