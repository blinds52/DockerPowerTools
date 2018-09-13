using System;
using System.Threading.Tasks;
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Authentication;

namespace DockerPowerTools.Registry
{
    public class RegistryConnectionService
    {
        public Task<RegistryConnection> GetRegistryConnectionAsync()
        {
            const string endpoint = "https://stagingregistry.captiveaire.com:5000";

            var configuration =
                new RegistryClientConfiguration(new Uri(endpoint));

            var client =
                configuration.CreateClient(new PasswordOAuthAuthenticationProvider("<username>", "<password>"));

            var connection  = new RegistryConnection(client, endpoint);

            return Task.FromResult(connection);
        }
    }
}