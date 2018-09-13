using Docker.Registry.DotNet;

namespace DockerPowerTools.Registry
{
    public class RegistryConnection
    {
        public RegistryConnection(IRegistryClient client, string registry)
        {
            Client = client;
            Registry = registry;
        }

        public IRegistryClient Client { get; }

        public string Registry { get; }
    }
}