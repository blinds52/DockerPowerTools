using System;
using System.IO;

namespace DockerPowerTools.Common
{
    public static class PathFactory
    {
        private const string SettingsFolder = "DockerPowerTools";

        public static string RegistryConnectionsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SettingsFolder, "registry-connections.json");

        public static string DockerConnectionsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SettingsFolder, "docker-connections.json");
    }
}
