using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Authentication;
using DockerPowerTools.Common;
using DockerPowerTools.Common.ViewModel;
using DockerPowerTools.Registry.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerPowerTools.Registry.ViewModel
{
    public class RegistryConnectionDialogViewModel : CloseableViewModel
    {
        private bool _isAnonymous = true;

        private string _registry;
        private string _username;
        private string _password;

        private readonly RegistryConnectionsModel _connections;
        private RegistryConnectionModel _selectedRegistry;
        private bool _rememberPassword;

        public RegistryConnectionDialogViewModel()
        {
            //Load up the saved connections
            _connections = RegistryConnectionsFactory.Load();

            SelectedRegistry =
                _connections.Connections.FirstOrDefault(r => r.Registry == _connections.SelectedRegistry);

            if (SelectedRegistry == null)
            {
                SelectedRegistry = _connections.Connections.FirstOrDefault();
            }

            ConnectCommand = new RelayCommand(() => ConnectAsync().IgnoreAsync(), CanConnect);
        }

        public ICommand ConnectCommand { get; }

        private async Task ConnectAsync()
        {
            try
            {
                string partialUrl = $"{Registry}/v2";

                var connectionType = await ConnectionTypeProbe.ProbeAsync(partialUrl);

                string uri;

                switch (connectionType)
                {
                    case ConnectionType.Https:
                        uri = $"https://{Registry}";
                        break;

                    case ConnectionType.Http:
                        uri = $"http://{Registry}";
                        break;

                    case ConnectionType.None:
                        throw new Exception($"No connection could be established with '{Registry}'");

                    default:
                        throw new Exception($"Unexpected value '{connectionType}'");
                }

                var configuration = new RegistryClientConfiguration(new Uri(uri));

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

                var connection = new RegistryConnection(client, Registry);

                Connection = connection;

                //Save the connections!
                Save();

                RequestClose(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to connect");
            }
        }

        private void Save()
        {
            var connections = _connections.Connections.ToList();

            if (SelectedRegistry == null)
            {
                //Need to add one!
                var connection = new RegistryConnectionModel
                {
                    Registry = Registry,
                    IsAnonymous = IsAnonymous,
                    Username = Username
                };

                if (RememberPassword)
                {
                    connection.Password = Password;
                }

                connections.Add(connection);
            }
            else
            {
                SelectedRegistry.IsAnonymous = IsAnonymous;
                SelectedRegistry.Username = Username;

                if (RememberPassword)
                {
                    SelectedRegistry.Password = Password;
                }
            }

            var toSave = new RegistryConnectionsModel()
            {
                Connections = connections.ToArray()
            };

            if (SelectedRegistry == null)
            {
                toSave.SelectedRegistry = Registry;
            }
            else
            {
                toSave.SelectedRegistry = SelectedRegistry.Registry;
            }

            RegistryConnectionsFactory.Save(toSave);
        }

        private bool CanConnect()
        {
            if (string.IsNullOrWhiteSpace(Registry))
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

        public bool RememberPassword
        {
            get => _rememberPassword;
            set
            {
                _rememberPassword = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<RegistryConnectionModel> Registries => _connections.Connections;

        public RegistryConnectionModel SelectedRegistry
        {
            get => _selectedRegistry;
            set
            {
                _selectedRegistry = value;

                if (value == null)
                {
                    Username = "";
                    Password = null;
                }
                else
                {
                    IsAnonymous = value.IsAnonymous;

                    if (value.IsAnonymous)
                    {
                        Username = "";
                        Password = "";
                        RememberPassword = false;
                    }
                    else
                    {
                        Username = value.Username;
                        Password = value.Password;
                        RememberPassword = !string.IsNullOrWhiteSpace(value.Password);
                    }
                }

                RaisePropertyChanged();
            }
        }

        public RegistryConnection Connection { get; private set; }

        public bool IsAnonymous
        {
            get => _isAnonymous;
            set
            {
                _isAnonymous = value;
                RaisePropertyChanged();
            }
        }

        public string Registry
        {
            get => _registry;
            set
            {
                _registry = value;
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