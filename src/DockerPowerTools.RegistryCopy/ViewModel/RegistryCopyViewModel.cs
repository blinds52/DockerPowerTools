using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DockerPowerTools.Common;
using DockerPowerTools.Docker;
using DockerPowerTools.Registry;
using GalaSoft.MvvmLight;

namespace DockerPowerTools.RegistryCopy.ViewModel
{
    public class RegistryCopyViewModel : ViewModelBase
    {
        private readonly DockerConnectionService _dockerConnectionService = new DockerConnectionService();
        private readonly RegistryConnectionService _registryConnectionService = new RegistryConnectionService();
        private DockerConnection _dockerConnection;
        private RegistryConnection _targetRegistryConnection;

        public RegistryCopyViewModel(RegistryConnection sourceRegistryConnection)
        {
            Tags = new TagViewModel[]{};
        }

        public ICommand ChooseDockerConnectionCommand { get; }
        public ICommand ChooseTargetRegistryConnectionCommand { get; }
        public ICommand CopyCommand { get; }

        private void ChooseDockerConnection()
        {

        }

        private void ChooseTargetRegistryConnection()
        {

        }

        private void Copy()
        {
            AsyncExecutor.ExecuteAsync(CopyInternalAsync).IgnoreAsync();
        }

        public Task LoadAsync()
        {
            return Task.CompletedTask;
        }

        private async Task CopyInternalAsync(CancellationToken cancellationToken)
        {
            try
            {
                foreach (var tag in Tags)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanCopy()
        {
            if (DockerConnection == null)
                return false;

            if (TargetRegistryConnection == null)
                return false;

            return true;
        }

        public DockerConnection DockerConnection
        {
            get => _dockerConnection;
            private set
            {
                _dockerConnection = value; 
                RaisePropertyChanged();
            }
        }

        public RegistryConnection TargetRegistryConnection
        {
            get => _targetRegistryConnection;
            set
            {
                _targetRegistryConnection = value;
                RaisePropertyChanged();
            }
        }

        public TagViewModel[] Tags { get; }

        public AsyncExecutor AsyncExecutor { get; } = new AsyncExecutor();        
    }
}