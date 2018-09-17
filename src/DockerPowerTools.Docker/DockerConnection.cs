using Docker.DotNet;

namespace DockerPowerTools.Docker
{
    public class DockerConnection
    {
        public DockerConnection(IDockerClient dockerClient, string endpoint)
        {
            DockerClient = dockerClient;
            Endpoint = endpoint;
        }

        public IDockerClient DockerClient { get; }

        public string Endpoint { get; }
    }
}