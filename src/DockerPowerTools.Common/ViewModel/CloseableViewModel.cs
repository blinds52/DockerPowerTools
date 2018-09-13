using System;
using Cas.Common.WPF.Behaviors;
using GalaSoft.MvvmLight;

namespace DockerPowerTools.Common.ViewModel
{
    public abstract class CloseableViewModel : ViewModelBase, ICloseableViewModel
    {
        public event EventHandler<CloseEventArgs> Close;

        /// <inheritdoc />
        public virtual bool CanClose()
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void Closed()
        {
        }

        /// <summary>
        /// Call this to raise the Close event.
        /// </summary>
        /// <param name="dialogResult"></param>
        protected void RequestClose(bool? dialogResult)
        {
            Close?.Invoke(this, new CloseEventArgs(dialogResult));
        }
    }
}