using Docker.Registry.DotNet;

namespace DockerPowerTools.Registry
{
    public class RegistryConnection
    {
        public RegistryConnection(IRegistryClient client, string registry, bool isAnonymous, string username, string password)
        {
            Client = client;
            Registry = registry;
            IsAnonymous = isAnonymous;
            Username = username;
            Password = password;
        }

        public IRegistryClient Client { get; }

        public string Registry { get; }

        public bool IsAnonymous { get; }

        public string Username { get; }

        public string Password { get; }
    }
}