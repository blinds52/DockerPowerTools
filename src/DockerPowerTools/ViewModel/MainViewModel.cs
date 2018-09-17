using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DockerPowerTools.Common;
using DockerPowerTools.Common.ViewModel;
using DockerPowerTools.Docker;
using DockerPowerTools.DockerExplorer.ViewModel;
using DockerPowerTools.Registry;
using DockerPowerTools.RegistryExplorer.ViewModel;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerPowerTools.ViewModel
{
    public class MainViewModel
    {
        private readonly RegistryConnectionService _registryConnectionService = new RegistryConnectionService();
        private readonly DockerConnectionService _dockerConnectionService = new DockerConnectionService();

        public MainViewModel()
        {
            OpenDockerExplorerCommand = new RelayCommand(() => OpenDockerExplorerAsync().IgnoreAsync(), CanOpenDockerExplorer);
            OpenRegistryExplorerCommand = new RelayCommand(() => OpenRegistryExplorerAsync().IgnoreAsync(), CanOpenRegistryExplorer);
        }

        public ICommand OpenDockerExplorerCommand { get; }
        public ICommand OpenRegistryExplorerCommand { get; }

        private async Task OpenDockerExplorerAsync()
        {
            try
            {
                var connection = await _dockerConnectionService.GetDockerConnectionAsync();

                if (connection != null)
                {
                    var dockerExplorer = new DockerExplorerViewModel(connection);

                    Tools.Add(dockerExplorer);

                    await dockerExplorer.LoadAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanOpenDockerExplorer()
        {
            return true;
        }

        private async Task OpenRegistryExplorerAsync()
        {
            try
            {
                var connection = await _registryConnectionService.GetRegistryConnectionAsync();

                if (connection != null)
                {
                    var registryExplorer = new RegistryExplorerViewModel(connection);

                    Tools.Add(registryExplorer);

                    await registryExplorer.LoadAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanOpenRegistryExplorer()
        {
            return true;
        }

        public ObservableCollection<ToolViewModelBase> Tools { get; } = new ObservableCollection<ToolViewModelBase>();
    }
}