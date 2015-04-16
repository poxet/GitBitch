using System.ComponentModel;
using System.Windows;

namespace ClownCrew.GitBitch.Client.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}