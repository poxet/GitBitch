namespace ClownCrew.GitBitch.Client.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CompositeRoot.Instance.TalkAgent.SayAsync("Hi! I'm git bitch alfa. You didn't think that my first words would be, Dada, or something stupid like that, did you?");
        }
    }
}
