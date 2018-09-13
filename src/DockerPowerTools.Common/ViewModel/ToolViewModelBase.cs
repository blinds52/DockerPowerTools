using GalaSoft.MvvmLight;

namespace DockerPowerTools.Common.ViewModel
{
    public abstract class ToolViewModelBase : ViewModelBase
    {
        public abstract string Title { get; }
    }
}