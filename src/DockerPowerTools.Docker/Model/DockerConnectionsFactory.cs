using System;
using System.IO;
using DockerPowerTools.Common;
using Newtonsoft.Json;

namespace DockerPowerTools.Docker.Model
{
    public static class DockerConnectionsFactory
    {
        private static DockerConnectionsModel CreateDefaultConnections()
        {
            return new DockerConnectionsModel
            {
                Connections = new DockerConnectionModel[]
                {
                    new DockerConnectionModel
                    {
                        Endpoint = "npipe://./pipe/docker_engine",
                    },
                    new DockerConnectionModel
                    {
                        Endpoint = "unix:/var/run/docker.sock"
                    }
                }
            };
        }

        public static DockerConnectionsModel Load()
        {
            try
            {
                string json = File.ReadAllText(PathFactory.DockerConnectionsPath);

                var connections = JsonConvert.DeserializeObject<DockerConnectionsModel>(json);

                if (connections == null)
                    return CreateDefaultConnections();

                if (connections.Connections == null)
                    return CreateDefaultConnections();

                return connections;
            }
            catch (Exception)
            {
                return CreateDefaultConnections();
            }
        }

        public static void Save(DockerConnectionsModel model)
        {
            string path = PathFactory.DockerConnectionsPath;

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