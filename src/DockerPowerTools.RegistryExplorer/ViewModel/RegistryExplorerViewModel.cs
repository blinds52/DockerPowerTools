using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Docker.Registry.DotNet.Models;
using DockerPowerTools.Common;
using DockerPowerTools.Common.ViewModel;
using DockerPowerTools.Registry;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;

namespace DockerPowerTools.RegistryExplorer.ViewModel
{
    public class RegistryExplorerViewModel : ToolViewModelBase
    {
        private readonly RegistryConnection _connection;
        private ObservableCollection<RepositoryViewModel> _repositories = new ObservableCollection<RepositoryViewModel>();
        public override string Title => "Registry Explorer";

        public RegistryExplorerViewModel(RegistryConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

            DeleteCommand = new RelayCommand(() => DeleteAsync().IgnoreAsync(), CanDelete);
        }

        public ICommand DeleteCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PullCommand { get; }

        public AsyncExecutor AsyncExecutor { get; } = new AsyncExecutor();

        private Task DeleteAsync()
        {
            var selectedCount = Repositories.Sum(r => r.Tags.Length);

            var result = MessageBox.Show($"Delete {selectedCount} tag(s)?", "Delete", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Not Implemented");
            }
            
            return Task.CompletedTask;
        }

        private bool CanDelete()
        {
            return Repositories.Any(r => r.Tags.Any(t => t.IsSelected));
        }

        public async Task LoadAsync()
        {
            await AsyncExecutor.ExecuteAsync(LoadInnerAsync);
        }

        private async Task LoadInnerAsync(CancellationToken cancellationToken)
        {
            var repositories = await _connection.Client.Catalog.GetCatalogAsync(new CatalogParameters(), cancellationToken);

            var repositoryViewModels = new List<RepositoryViewModel>(repositories.Repositories.Length);

            foreach (var repository in repositories.Repositories)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var tags = await _connection.Client.Tags.ListImageTagsAsync(repository, new ListImageTagsParameters(), cancellationToken);

                    var tagViewModels = (tags.Tags ?? new string[0])
                        .Select(t => new TagViewModel(_connection.Registry, repository, t))
                        .OrderBy(t => t.Tag)
                        .ToArray();

                    var repositoryViewModel = new RepositoryViewModel(repository, tagViewModels);

                    repositoryViewModels.Add(repositoryViewModel);
                }
                catch (Exception)
                {
                    repositoryViewModels.Add(new RepositoryViewModel(repository, new TagViewModel[0]));
                }
            }

            //Do this on the UI thread
            DispatcherHelper.CheckBeginInvokeOnUI(() => Repositories = new ObservableCollection<RepositoryViewModel>(repositoryViewModels));
        }

        public ObservableCollection<RepositoryViewModel> Repositories
        {
            get => _repositories;
            private set
            {
                _repositories = value; 
                RaisePropertyChanged();
            }
        }
    }
}