using System.Windows;

namespace ClownCrew.GitBitch.Client.Views
{
    public partial class EnterStringWindow : Window
    {
        public EnterStringWindow()
        {
            InitializeComponent();
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
