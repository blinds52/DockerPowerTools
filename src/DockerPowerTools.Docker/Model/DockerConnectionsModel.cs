namespace DockerPowerTools.Docker.Model
{
    public class DockerConnectionsModel
    {
        public DockerConnectionModel[] Connections { get; set; }

        public string SelectedEndpoint { get; set; }
    }
}