using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Authentication;
using DockerPowerTools.Common;
using DockerPowerTools.Common.ViewModel;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerPowerTools.Registry.ViewModel
{
    public class RegistryConnectionDialogViewModel : CloseableViewModel
    {
        private const string DefaultEndpoint = "registry.hub.docker.com";

        private bool _isAnonymous = true;

        private string _registry = DefaultEndpoint;
        private string _username;
        private string _password;

        public RegistryConnectionDialogViewModel()
        {
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

                RequestClose(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to connect");
            }
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