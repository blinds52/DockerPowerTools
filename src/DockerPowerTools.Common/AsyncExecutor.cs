using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerPowerTools.Common
{
    public class AsyncExecutor : ViewModelBase
    {
        private bool _isBusy;
        private CancellationTokenSource _cts;

        public AsyncExecutor()
        {
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        public ICommand CancelCommand { get; }

        private void Cancel()
        {
            _cts?.Cancel();
        }

        private bool CanCancel()
        {
            var cts = _cts;

            return cts != null && !cts.IsCancellationRequested;
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }

        public bool CanExecute => _cts == null;

        public async Task<Exception> ExecuteAsync(Func<CancellationToken, Task> action)
        {
            if (!CanExecute)
                return null;

            try
            {
                _cts = new CancellationTokenSource();

                //Execute the action
                await action(_cts.Token);

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");

                return ex;
            }
            finally
            {
                _cts = null;

                CommandManager.InvalidateRequerySuggested();
            }           
        }
    }
}