using GalaSoft.MvvmLight;

namespace DockerPowerTools.RegistryCopy.ViewModel
{
    public class TagViewModel : ViewModelBase
    {
        private bool _isSelected;

        public TagViewModel(string repository, string tag)
        {
            Repository = repository;
            Tag = tag;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value; 
                RaisePropertyChanged();
            }
        }

        public string Repository { get; }

        public string Tag { get; }
    }
}