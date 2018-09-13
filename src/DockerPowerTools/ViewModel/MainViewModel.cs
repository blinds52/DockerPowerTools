using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DockerPowerTools.Common;
using DockerPowerTools.Common.ViewModel;
using DockerPowerTools.DockerExplorer.ViewModel;
using DockerPowerTools.Registry;
using DockerPowerTools.RegistryExplorer.ViewModel;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerPowerTools.ViewModel
{
    public class MainViewModel
    {
        private readonly RegistryConnectionService _registryConnectionService = new RegistryConnectionService();

        public MainViewModel()
        {
            OpenDockerExplorerCommand = new RelayCommand(() => OpenDockerExplorerAsync().IgnoreAsync(), CanOpenDockerExplorer);
            OpenRegistryExplorerCommand = new RelayCommand(() => OpenRegistryExplorerAsync().IgnoreAsync(), CanOpenRegistryExplorer);
        }

        public ICommand OpenDockerExplorerCommand { get; }
        public ICommand OpenRegistryExplorerCommand { get; }

        private Task OpenDockerExplorerAsync()
        {
            try
            {
                var dockerExplorer = new DockerExplorerViewModel();

                Tools.Add(dockerExplorer);            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return Task.CompletedTask;
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

                    await registryExplorer.LoadAsync();

                    Tools.Add(registryExplorer);
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