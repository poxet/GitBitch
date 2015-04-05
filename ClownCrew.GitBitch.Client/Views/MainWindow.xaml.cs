using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Agents;
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
            //await SetBitchNameAsync();

            await CompositeRoot.Instance.CommandAgent.RegisterAsync(new GitBitchCommands());
            await CompositeRoot.Instance.CommandAgent.RegisterAsync(new WindowsCommands());
            
            await CompositeRoot.Instance.TalkAgent.SayAsync("Hi! I'm git bitch alfa. You didn't think that my first words would be, Dada, or something stupid like that, did you?");

            //TODO: Check if the application knows of any repo.
            //var response = await CompositeRoot.Instance.TalkAgent.AskAsync("Do you want me to scan your drives for git repositories?", new List<QuestionAnswerAlternative<bool>> { new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "Yes" }, Response = true }, new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "No" }, Response = false, IsDefault = true } });
            //if (response.Response)
            //{
                await CompositeRoot.Instance.GitRepoAgent.SearchAsync();
            //}
        }

        private static async Task SetBitchNameAsync()
        {
            //TODO: Check if a name has been assigned
            //TODO: Have many many names to choose between
            //TODO: Make it possible to manually enter names
            var names = new List<string> { Constants.DefaultBitchName, "Ivona", "Astra", "Zira" };
            var response = new Answer<bool>(false);
            var bitchName = new Answer<string>(names.First());
            while (!response.Response)
            {
                bitchName = await CompositeRoot.Instance.TalkAgent.AskAsync("What do you want my name to be?", names.Select(x => new QuestionAnswerAlternative<string> { Phrases = new List<string> { x }, Response = x }).ToList(), 5000);
                response = await CompositeRoot.Instance.TalkAgent.AskAsync(string.Format("So you want my name to be {0}?", bitchName.Response), new List<QuestionAnswerAlternative<bool>> { new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "Yes" }, Response = true }, new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "No" }, Response = false, IsDefault = true } });
            }

            CompositeRoot.Instance.SettingAgent.SetSetting("BitchName", new Tuple<string, bool>(bitchName.Response, true));
        }
    }
}