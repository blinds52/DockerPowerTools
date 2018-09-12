using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Authentication;
using Docker.Registry.DotNet.Models;
using DockerPowerTools.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;

namespace DockerPowerTools.RegistryCopy.ViewModel
{
    public class RegistryViewModel : ViewModelBase
    {
        private bool _isAnonymous = true;

        private string _endpoint;
        private string _username;
        private string _password;
        private ObservableCollection<RepositoryViewModel> _repositories;
        private string _repositoryName;

        public RegistryViewModel OtherRegistry { get; set; }

        public ICommand LoadAllCommand { get; }
        public ICommand LoadSingleCommand { get; }
        public ICommand CopyCommand { get; }

        public RegistryViewModel()
        {
            LoadAllCommand = new RelayCommand(() => LoadAllAsync().IgnoreAsync(), CanPerform);
            LoadSingleCommand = new RelayCommand(LoadSingle, CanLoadSingle);
            CopyCommand = new RelayCommand(Copy, CanPerform);
        }

        private void LoadSingle()
        {
        }

        private bool CanLoadSingle()
        {
            if (!CanPerform())
                return false;

            if (string.IsNullOrWhiteSpace(RepositoryName))
                return false;

            return true;
        }

        private async Task LoadAllAsync()
        {
            try
            {
                using (var registryClient = await CreateRegistryClientAsync())
                {
                    var catalog = await registryClient.Catalog.GetCatalogAsync(new CatalogParameters());

                    var repositories = new List<RepositoryViewModel>();

                    foreach (var repository in catalog.Repositories.OrderBy(r => r))
                    {
                        //get the tags
                        var tags = await registryClient.Tags.ListImageTagsAsync(repository,
                            new ListImageTagsParameters());

                        var tagViewModels = tags.Tags
                            .Select(t => new TagViewModel(tags.Name, t))
                            .OrderBy(t => t.Tag)
                            .ToArray();

                        repositories.Add(new RepositoryViewModel(repository, tagViewModels));
                    }

                    var repositoriesCollection = new ObservableCollection<RepositoryViewModel>(repositories);

                    DispatcherHelper.CheckBeginInvokeOnUI(() => Repositories = repositoriesCollection);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Copy()
        {

        }

        internal async Task<IRegistryClient> CreateRegistryClientAsync()
        {
            var configuration = new RegistryClientConfiguration(new Uri(Endpoint));

            AuthenticationProvider authenticationProvider;

            if (IsAnonymous)
            {
                authenticationProvider = new AnonymousOAuthAuthenticationProvider();
            }
            else
            {
                authenticationProvider = new PasswordOAuthAuthenticationProvider(Username, Password);
            }

            var client = configuration.CreateClient(authenticationProvider);

            await client.System.PingAsync();

            return client;
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

        private bool CanPerform()
        {
            if (OtherRegistry == null)
                return false;

            if (!CanConnect())
                return false;

            if (!OtherRegistry.CanConnect())
                return false;

            return true;
        }

        public bool CanConnect()
        {
            if (string.IsNullOrWhiteSpace(Endpoint))
                return false;

            if (!IsAnonymous)
            {
                if (string.IsNullOrWhiteSpace(Username))
                    return false;

                if (string.IsNullOrWhiteSpace(Password))
                    return false;
            }

            return true;
        }

        public string RepositoryName
        {
            get => _repositoryName;
            set
            {
                _repositoryName = value; 
                RaisePropertyChanged();
            }
        }

        public bool IsAnonymous
        {
            get => _isAnonymous;
            set
            {
                _isAnonymous = value;
                RaisePropertyChanged();
            }
        }

        public string Endpoint
        {
            get => _endpoint;
            set
            {
                _endpoint = value;
                RaisePropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                RaisePropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged();
            }
        }
    }
}