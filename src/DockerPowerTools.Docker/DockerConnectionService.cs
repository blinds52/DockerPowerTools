using System;
using System.Threading.Tasks;
using DockerPowerTools.Docker.View;
using DockerPowerTools.Docker.ViewModel;

namespace DockerPowerTools.Docker
{
    public class DockerConnectionService
    {
        public async Task<DockerConnection> GetDockerConnectionAsync()
        {
            var viewModel = new DockerConnectionDialogViewModel();

            var view = new DockerConnectionDialogView
            {
                DataContext = viewModel
            };

            if (view.ShowDialog() == true)
            {
                return viewModel.Connection;
            }

            return null;
        }
    }
}