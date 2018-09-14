using System.Windows;
using DockerPowerTools.ViewModel;

namespace DockerPowerTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
        }

        private void ExitMenu_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
