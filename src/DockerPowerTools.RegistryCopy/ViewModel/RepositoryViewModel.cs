using GalaSoft.MvvmLight;

namespace DockerPowerTools.RegistryCopy.ViewModel
{
    public class RepositoryViewModel : ViewModelBase
    {
        public RepositoryViewModel(string name, TagViewModel[] tags)
        {
            Name = name;
            Tags = tags;
        }

        public string Name { get; }

        public TagViewModel[] Tags { get; }
    }
}