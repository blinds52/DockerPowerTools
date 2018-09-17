using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Docker.DotNet;
using DockerPowerTools.Common;
using DockerPowerTools.Common.ViewModel;
using DockerPowerTools.Docker.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerPowerTools.Docker.ViewModel
{
    public class DockerConnectionDialogViewModel : CloseableViewModel
    {
        private readonly DockerConnectionsModel _connections;
        private DockerConnectionModel _selectedConnection;
        private string _endpoint;

        public DockerConnectionDialogViewModel()
        {
            _connections = DockerConnectionsFactory.Load();

            SelectedConnection =_connections.Connections
                .FirstOrDefault(r => r.Endpoint == _connections.SelectedEndpoint);

            if (SelectedConnection == null)
            {
                SelectedConnection = _connections.Connections.FirstOrDefault();
            }

            ConnectCommand = new RelayCommand(() => ConnectAsync().IgnoreAsync(), CanConnect);
        }

        private bool CanConnect()
        {
            if (SelectedConnection == null && string.IsNullOrWhiteSpace(Endpoint))
                return false;

            return true;
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

        public DockerConnectionModel SelectedConnection
        {
            get => _selectedConnection;
            set
            {
                _selectedConnection = value; 
                RaisePropertyChanged();
            }
        }

        public ICommand ConnectCommand { get; }

        public IEnumerable<DockerConnectionModel> Connections => _connections.Connections;

        private async Task ConnectAsync()
        {
            try
            {
                string endpoint;

                if (SelectedConnection == null)
                {
                    endpoint = Endpoint;
                }
                else
                {
                    endpoint = SelectedConnection.Endpoint;
                }

                var configuration = new DockerClientConfiguration(new Uri(endpoint));

                var client = configuration.CreateClient();

                await client.System.PingAsync();

                Connection = new DockerConnection(client, endpoint);

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

            if (SelectedConnection == null)
            {
                var connection = new DockerConnectionModel
                {
                    Endpoint = Endpoint,
                };

                connections.Add(connection);
            }

            var toSave = new DockerConnectionsModel
            {
                Connections = connections.ToArray()
            };

            if (SelectedConnection == null)
            {
                toSave.SelectedEndpoint = Endpoint;
            }
            else
            {
                toSave.SelectedEndpoint = SelectedConnection.Endpoint;
            }

            DockerConnectionsFactory.Save(toSave);
        }

        public DockerConnection Connection { get; private set; }
    }
}