using ClownCrew.GitBitch.Client.Agents;

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
    }
}