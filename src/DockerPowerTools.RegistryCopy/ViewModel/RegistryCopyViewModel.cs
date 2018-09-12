using System.Windows.Input;
using DockerPowerTools.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerPowerTools.RegistryCopy.ViewModel
{
    public class RegistryCopyViewModel : ViewModelBase
    {
        private string _dockerEndpoint;

        public RegistryCopyViewModel()
        {
            DockerEndpoint = DockerConstants.DefaultWindowsDockerEndpoint;

            TestConnectionCommand = new RelayCommand(TestConnection, CanTestConnection);
        }

        public ICommand TestConnectionCommand { get; }

        private void TestConnection()
        {

        }

        private bool CanTestConnection()
        {
            return !string.IsNullOrWhiteSpace(DockerEndpoint);
        }

        public string DockerEndpoint
        {
            get => _dockerEndpoint;
            set
            {
                _dockerEndpoint = value; 
                RaisePropertyChanged();
            }
        }
    }
}