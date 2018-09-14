namespace DockerPowerTools.Registry.Model
{
    public class RegistryConnectionModel
    {
        public string Registry { get; set; }

        public bool IsAnonymous { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}