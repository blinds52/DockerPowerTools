using Docker.DotNet.Models;
using DockerPowerTools.Registry;

namespace DockerPowerTools.RegistryCopy.Extensions
{
    public static class RegistryConnectionExtensions
    {
        public static AuthConfig GetAuthConfig(this RegistryConnection registryConnection)
        {
            if (registryConnection.IsAnonymous)
                return new AuthConfig();

            return new AuthConfig()
            {
                Username = registryConnection.Username,
                Password = registryConnection.Password
            };
        }
    }
}