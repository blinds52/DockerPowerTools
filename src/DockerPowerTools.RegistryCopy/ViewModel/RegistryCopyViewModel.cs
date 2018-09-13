using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Docker.DotNet;
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

            TestConnectionCommand = new RelayCommand(() => TestConnectionAsync().IgnoreAsync(), CanTestConnection);
        }

        public ICommand TestConnectionCommand { get; }

        public AsyncExecutor AsyncExecutor { get; } = new AsyncExecutor();

        private async Task TestConnectionAsync()
        {
            try
            {
                await AsyncExecutor.ExecuteAsync(CreateDockerClientAsync);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Connection Failed");
            }   
        }

        private bool CanTestConnection()
        {
            if (!AsyncExecutor.CanExecute)
                return false;

            if (string.IsNullOrWhiteSpace(DockerEndpoint))
                return false;

            return true;
        }

        public async Task<IDockerClient> CreateDockerClientAsync(CancellationToken cancellationToken)
        {
            var dockerClientConfiguration = new DockerClientConfiguration(new Uri(DockerEndpoint));

            var dockerClient = dockerClientConfiguration.CreateClient();

            await dockerClient.System.PingAsync(cancellationToken);

            return dockerClient;
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