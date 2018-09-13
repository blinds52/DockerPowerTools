using GalaSoft.MvvmLight;

namespace DockerPowerTools.ViewModel
{
    public abstract class ToolViewModelBase : ViewModelBase
    {
        public abstract string Title { get; }
    }
}