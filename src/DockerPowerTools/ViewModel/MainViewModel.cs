using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DockerPowerTools.Common;
using DockerPowerTools.DockerExplorer.ViewModel;
using DockerPowerTools.RegistryExplorer.ViewModel;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerPowerTools.ViewModel
{
    public class MainViewModel
    {
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

        private Task OpenRegistryExplorerAsync()
        {
            try
            {
                var registryExplorer = new RegistryExplorerViewModel();

                Tools.Add(registryExplorer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return Task.CompletedTask;
        }

        private bool CanOpenRegistryExplorer()
        {
            return true;
        }

        public ObservableCollection<ToolViewModelBase> Tools { get; } = new ObservableCollection<ToolViewModelBase>();
    }
}