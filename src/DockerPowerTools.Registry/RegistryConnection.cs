using Docker.Registry.DotNet;

namespace DockerPowerTools.Registry
{
    public class RegistryConnection
    {
        public RegistryConnection(IRegistryClient client, string endpoint)
        {
            Client = client;
            Endpoint = endpoint;
        }

        public IRegistryClient Client { get; }

        public string Endpoint { get; }
    }
}