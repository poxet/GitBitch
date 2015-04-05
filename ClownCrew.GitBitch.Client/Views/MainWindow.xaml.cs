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
            //TODO: Move this to where it should be!

            //await CompositeRoot.Instance.TalkAgent.SayAsync("Hi! I'm git bitch alfa. You didn't think that my first words would be, Dada, or something stupid like that, did you?");

            //TODO: Do this first time only
            await SetBitchNameAsync();

            //TODO: Add application commands, like quit.

            //TODO: Create addons that can execut different types of commands, and that can trigger on different events and tell you what is happening
            //await CompositeRoot.Instance.CommandAgent.RegisterAsync(new GitBitchCommands());
            //await CompositeRoot.Instance.CommandAgent.RegisterAsync(new WindowsCommands());

            //TODO: Do this first time only (Or when manually triggered)
            await ScanDrive();
        }

        private static async Task ScanDrive()
        {
            var questionAnswerAlternatives = new List<QuestionAnswerAlternative<bool>>
            {
                new QuestionAnswerAlternative<bool>
                {
                    Phrases = new List<string> { "Yes" },
                    Response = true
                },
                new QuestionAnswerAlternative<bool>
                {
                    Phrases = new List<string> { "No" },
                    Response = false,
                    IsDefault = true
                }
            };

            var response = await CompositeRoot.Instance.TalkAgent.AskAsync("Do you want me to scan your drives for git repositories?", questionAnswerAlternatives);
            if (response.Response)
            {
                await CompositeRoot.Instance.TalkAgent.SayAsync("Starting to scan now.");
                await CompositeRoot.Instance.GitRepoAgent.SearchAsync();
            }
            else
            {
                await CompositeRoot.Instance.TalkAgent.SayAsync("Okey. Just use the command Scan if you want me to start scanning.");
            }
        }

        private static async Task SetBitchNameAsync()
        {
            //TODO: Check if a name has been assigned
            //TODO: Make it possible to manually enter names

            string[] names;
            if (System.IO.File.Exists("Names.txt")) 
                names = System.IO.File.ReadAllLines("Names.txt");
            else 
                names = new[] { Constants.DefaultBitchName, "Ivona", "Astra", "Zira" };

            var response = new Answer<bool>(false);
            var bitchName = new Answer<string>(names.First());
            while (!response.Response)
            {
                bitchName = await CompositeRoot.Instance.TalkAgent.AskAsync("What do you want my name to be?", names.Select(x => new QuestionAnswerAlternative<string> { Phrases = new List<string> { x }, Response = x }).ToList(), 5000);
                response = await CompositeRoot.Instance.TalkAgent.AskAsync(string.Format("So you want my name to be {0}?", bitchName.Response), new List<QuestionAnswerAlternative<bool>> { new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "Yes" }, Response = true }, new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "No" }, Response = false, IsDefault = true } });
            }

            await CompositeRoot.Instance.TalkAgent.SayAsync(string.Format("Allright, {0} it is.", bitchName.Response));

            //TODO: When the name changes, the phrases needs to be re-assigned so they work with the new name.
            //TODO: The text console output should reflect the new name.

            CompositeRoot.Instance.SettingAgent.SetSetting(Constants.BitchName, bitchName.Response);
        }
    }
}