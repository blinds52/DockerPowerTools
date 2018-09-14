using System.Windows;

namespace DockerPowerTools.Registry.View
{
    /// <summary>
    /// Interaction logic for RegistryConnectionDialogView.xaml
    /// </summary>
    public partial class RegistryConnectionDialogView : Window
    {
        public RegistryConnectionDialogView()
        {
            InitializeComponent();
            RegistryTextBox.Focus();
        }

        private void RegistryConnectionDialogView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
