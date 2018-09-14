namespace DockerPowerTools.Registry.Model
{
    public class RegistryConnectionsModel
    {
        public RegistryConnectionModel[] Connections { get; set; }

        public string SelectedRegistry { get; set; }
    }
}