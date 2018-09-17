using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.Registry.DotNet.Models;
using DockerPowerTools.Registry;
using GalaSoft.MvvmLight;

namespace DockerPowerTools.RegistryExplorer.ViewModel
{
    public class RepositoryViewModel : ViewModelBase
    {
        private readonly RegistryConnection _connection;
        private TagViewModel[] _tags;

        public RepositoryViewModel(RegistryConnection connection, string name)
        {
            _connection = connection;
            Name = name;
        }

        public async Task LoadAsync(CancellationToken cancellationToken)
        {
            //get the tags
            var tags = await _connection.Client.Tags.ListImageTagsAsync(Name, new ListImageTagsParameters(),
                cancellationToken);

            if (tags == null)
            {
                Tags = new TagViewModel[]{};
            }
            else
            {
                var tagViewModels = new List<TagViewModel>();

                if (tags?.Tags != null)
                {
                    foreach (var tag in tags.Tags)
                    {
                        if (tag != null)
                        {
                             tagViewModels.Add(new TagViewModel(_connection.Registry, tags.Name, tag));
                        }
                    }
                }

                Tags = tagViewModels.ToArray();
            }
        }

        public string Name { get; }

        public TagViewModel[] Tags
        {
            get => _tags;
            private set
            {
                _tags = value; 
                RaisePropertyChanged();
            }
        }
    }
}