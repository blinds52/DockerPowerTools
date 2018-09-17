using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DockerPowerTools.Common;
using DockerPowerTools.Docker;
using DockerPowerTools.Registry;
using DockerPowerTools.RegistryCopy.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerPowerTools.RegistryCopy.ViewModel
{
    public class RegistryCopyViewModel : ViewModelBase
    {
        private readonly RegistryConnection _sourceRegistryConnection;
        private readonly DockerConnectionService _dockerConnectionService = new DockerConnectionService();
        private readonly RegistryConnectionService _registryConnectionService = new RegistryConnectionService();
        private DockerConnection _dockerConnection;
        private RegistryConnection _targetRegistryConnection;

        public RegistryCopyViewModel(RegistryConnection sourceRegistryConnection, TagModel[] tags)
        {
            _sourceRegistryConnection = sourceRegistryConnection;
            Tags = new TagViewModel[]{};

            CopyCommand = new RelayCommand(Copy, CanCopy);

            ChooseDockerConnectionCommand = new RelayCommand(() => ChooseDockerConnectionAsync().IgnoreAsync());
            ChooseTargetRegistryConnectionCommand = new RelayCommand(() => ChooseTargetRegistryConnectionAsync().IgnoreAsync());

            Tags = tags
                .Select(t => new TagViewModel(t))
                .ToArray();
        }

        public ICommand ChooseDockerConnectionCommand { get; }
        public ICommand ChooseTargetRegistryConnectionCommand { get; }
        public ICommand CopyCommand { get; }

        private async Task ChooseDockerConnectionAsync()
        {
            var connection = await _dockerConnectionService.GetDockerConnectionAsync();

            if (connection != null)
            {
                DockerConnection = connection;

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private async Task ChooseTargetRegistryConnectionAsync()
        {
            var connection = await _registryConnectionService.GetRegistryConnectionAsync();

            if (connection != null)
            {
                TargetRegistryConnection = connection;

                CommandManager.InvalidateRequerySuggested();
            }
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
                    await tag.CopyAsync(_sourceRegistryConnection, _dockerConnection, _targetRegistryConnection, cancellationToken);
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