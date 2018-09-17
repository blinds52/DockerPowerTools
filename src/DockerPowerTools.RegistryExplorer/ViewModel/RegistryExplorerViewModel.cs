using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Docker.Registry.DotNet.Models;
using DockerPowerTools.Common;
using DockerPowerTools.Common.ViewModel;
using DockerPowerTools.Registry;
using DockerPowerTools.RegistryCopy.Model;
using DockerPowerTools.RegistryCopy.View;
using DockerPowerTools.RegistryCopy.ViewModel;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;

namespace DockerPowerTools.RegistryExplorer.ViewModel
{
    public class RegistryExplorerViewModel : ToolViewModelBase
    {
        private readonly RegistryConnection _connection;
        private ObservableCollection<RepositoryViewModel> _repositories = new ObservableCollection<RepositoryViewModel>();
        public override string Title => "Registry Explorer";

        private bool _canLoadCatalog;
        private string _repository;

        public RegistryExplorerViewModel(RegistryConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

            DeleteCommand = new RelayCommand(Delete, CanDelete);
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            LoadRepositoryCommand = new RelayCommand(LoadRepository, CanLoadRepository);
            CopyCommand = new RelayCommand(Copy, CanCopy);
        }

        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand LoadRepositoryCommand { get; }

        public AsyncExecutor AsyncExecutor { get; } = new AsyncExecutor();

        private void LoadRepository()
        {
            AsyncExecutor.ExecuteAsync(LoadRepositoryAsync).IgnoreAsync();
        }

        private void Copy()
        {
            //Get the selected tags
            var selectedTags = Repositories
                .SelectMany(r => r.Tags.Where(t => t.IsSelected));

            var tagModels = selectedTags
                .Select(t => new TagModel
                {
                    Registry = t.Registry,
                    Repository = t.Repository,
                    Tag = t.Tag
                }).ToArray();

            var copyViewModel = new RegistryCopyViewModel(_connection, tagModels);

            var copyView = new RegistryCopyView
            {
                DataContext = copyViewModel
            };

            copyViewModel.LoadAsync().IgnoreAsync();

            copyView.Show();           
        }

        private bool CanCopy()
        {
            if (AsyncExecutor.IsBusy)
                return false;

            if (!AreAnyTagsSelected())
                return false;

            return true;
        }

        private void Refresh()
        {
            AsyncExecutor.ExecuteAsync(RefreshInnerAsync).IgnoreAsync();
        }

        private bool CanRefresh()
        {
            if (!AsyncExecutor.CanExecute)
                return false;

            return true;
        }

        private async Task RefreshInnerAsync(CancellationToken cancellationToken)
        {
            if (_canLoadCatalog)
            {
                await LoadInnerAsync(cancellationToken);
            }
            else
            {
                foreach (var repository in Repositories)
                {
                    await repository.LoadAsync(cancellationToken);
                }
            }
        }

        private bool CanLoadRepository()
        {
            if (!AsyncExecutor.CanExecute)
                return false;

            if (string.IsNullOrWhiteSpace(Repository))
                return false;

            return true;
        }

        private async Task LoadRepositoryAsync(CancellationToken cancellationToken)
        {
            try
            {
                var repositoryViewModel = new RepositoryViewModel(_connection, Repository);

                await repositoryViewModel.LoadAsync(cancellationToken);

                DispatcherHelper.CheckBeginInvokeOnUI(() => Repositories.Add(repositoryViewModel));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load repository");
            }
        }

        private void Delete()
        {
            var selectedTags = Repositories
                .SelectMany(r => r.Tags.Where(t => t.IsSelected))
                .ToArray();

            var result = MessageBox.Show($"Delete {selectedTags.Length} tag(s)?", "Delete", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                AsyncExecutor.ExecuteAsync(DeleteInnerAsync).IgnoreAsync();
            }
        }

        private async Task DeleteInnerAsync(CancellationToken cancellationToken)
        {
            try
            {
                foreach (var repository in Repositories)
                {
                    var tagsToDelete = repository.Tags
                        .Where(t => t.IsSelected)
                        .ToArray();

                    if (tagsToDelete.Length > 0)
                    {
                        foreach (var tag in tagsToDelete)
                        {
                            //Get the manifest
                            var manifest = await _connection.Client.Manifest.GetManifestAsync(tag.Repository, tag.Tag, cancellationToken);

                            string digest = manifest.DockerContentDigest;

                            if (!string.IsNullOrWhiteSpace(digest))
                            {
                                //Delete it!
                                await _connection.Client.Manifest.DeleteManifestAsync(tag.Repository, digest, cancellationToken);
                            }
                        }

                        //Refresh the tag
                        await repository.LoadAsync(cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Delete");
            }
        }

        private bool AreAnyTagsSelected()
        {
            return Repositories.Any(r => r.Tags.Any(t => t.IsSelected));
        }

        private bool CanDelete()
        {
            return AreAnyTagsSelected();
        }

        public Task LoadAsync()
        {
            AsyncExecutor.ExecuteAsync(LoadInnerAsync).IgnoreAsync();

            return Task.CompletedTask;
        }

        private async Task LoadInnerAsync(CancellationToken cancellationToken)
        {
            try
            {
                var repositories = await _connection.Client.Catalog.GetCatalogAsync(new CatalogParameters(), cancellationToken);

                _canLoadCatalog = true;

                var repositoryViewModels = new List<RepositoryViewModel>(repositories.Repositories.Length);

                foreach (var repository in repositories.Repositories)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var repositoryViewModel = new RepositoryViewModel(_connection, repository);

                        repositoryViewModels.Add(repositoryViewModel);

                        await repositoryViewModel.LoadAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }

                //Do this on the UI thread
                DispatcherHelper.CheckBeginInvokeOnUI(() => Repositories = new ObservableCollection<RepositoryViewModel>(repositoryViewModels));
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Unable to load catalog: {ex.Message}");
            }
        }

        public string Repository
        {
            get => _repository;
            set
            {
                _repository = value; 
                RaisePropertyChanged();
            }
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