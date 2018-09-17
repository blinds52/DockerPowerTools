using System.Threading.Tasks;
using DockerPowerTools.Common.ViewModel;
using DockerPowerTools.Docker;

namespace DockerPowerTools.DockerExplorer.ViewModel
{
    public class DockerExplorerViewModel : ToolViewModelBase
    {
        private readonly DockerConnection _connection;

        public DockerExplorerViewModel(DockerConnection connection)
        {
            _connection = connection;
        }

        public Task LoadAsync()
        {
            return Task.CompletedTask;
        }

        public override string Title => "Docker Explorer";
    }
}