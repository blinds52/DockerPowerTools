using System;
using System.Threading.Tasks;
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Authentication;
using DockerPowerTools.Registry.View;
using DockerPowerTools.Registry.ViewModel;

namespace DockerPowerTools.Registry
{
    public class RegistryConnectionService
    {
        public Task<RegistryConnection> GetRegistryConnectionAsync()
        {
            RegistryConnection connection = null;

            var viewModel = new RegistryConnectionDialogViewModel();

            var view = new RegistryConnectionDialogView
            {
                DataContext = viewModel
            };

            if (view.ShowDialog() == true)
            {
                connection = viewModel.Connection;
            }
            
            return Task.FromResult(connection);
        }
    }
}