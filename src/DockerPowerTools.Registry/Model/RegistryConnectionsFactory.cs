using System;
using System.IO;
using System.Security.RightsManagement;
using DockerPowerTools.Common;
using Newtonsoft.Json;

namespace DockerPowerTools.Registry.Model
{
    public static class RegistryConnectionsFactory
    {
        private static RegistryConnectionsModel CreateDefaultConnections()
        {
            return new RegistryConnectionsModel()
            {
                Connections = new RegistryConnectionModel[]
                {
                    new RegistryConnectionModel()
                    {
                        Registry = "registry.hub.docker.com",
                        IsAnonymous = true
                    }
                }
            };
        }

        public static RegistryConnectionsModel Load()
        {
            try
            {
                string json = File.ReadAllText(PathFactory.RegistryConnectionsPath);

                var connections = JsonConvert.DeserializeObject<RegistryConnectionsModel>(json);

                if (connections == null)
                    return CreateDefaultConnections();

                if (connections.Connections == null)
                    return CreateDefaultConnections();

                return connections;
            }
            catch (Exception )
            {
                return CreateDefaultConnections();
            }
        }

        public static void Save(RegistryConnectionsModel model)
        {
            string path = PathFactory.RegistryConnectionsPath;

            string directoryName = Path.GetDirectoryName(path);

            if (directoryName != null)
            {
                Directory.CreateDirectory(directoryName);
            }

            string json = JsonConvert.SerializeObject(model, Formatting.Indented);

            File.WriteAllText(path, json);
        }
    }
}