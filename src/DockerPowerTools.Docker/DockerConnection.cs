using Docker.DotNet;

namespace DockerPowerTools.Docker
{
    public class DockerConnection
    {
        public DockerConnection(IDockerClient dockerClient)
        {
            DockerClient = dockerClient;
        }

        public IDockerClient DockerClient { get; }
    }
}