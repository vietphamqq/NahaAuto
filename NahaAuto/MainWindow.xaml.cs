using NahaAuto.Code;
using System.Windows;

namespace NahaAuto
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RandomCreation_Click(object sender, RoutedEventArgs e)
        {
            var window = new RandomAccountWindow();
            window.ShowDialog();
        }
    }
}