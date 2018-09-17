using System;
using System.Threading.Tasks;
using DockerPowerTools.Docker.View;
using DockerPowerTools.Docker.ViewModel;

namespace DockerPowerTools.Docker
{
    public class DockerConnectionService
    {
        public Task<DockerConnection> GetDockerConnectionAsync()
        {
            var viewModel = new DockerConnectionDialogViewModel();

            var view = new DockerConnectionDialogView
            {
                DataContext = viewModel
            };

            if (view.ShowDialog() == true)
            {
                return Task.FromResult(viewModel.Connection);
            }

            return Task.FromResult<DockerConnection>(null);
        }
    }
}