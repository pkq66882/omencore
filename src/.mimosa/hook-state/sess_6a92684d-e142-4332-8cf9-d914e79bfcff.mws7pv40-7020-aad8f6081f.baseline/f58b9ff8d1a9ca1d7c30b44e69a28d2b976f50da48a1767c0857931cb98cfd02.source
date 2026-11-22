using OmenCore.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace OmenCore.Utils
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object?, Task> _executeAsync;
        private readonly Func<object?, bool>? _canExecute;
        private bool _isExecuting;
        private int _raiseCanExecuteQueued;

        public AsyncRelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _executeAsync(parameter);
            }
            catch (Exception ex)
            {
                App.Logging.Error($"Command execution failed: {ex.Message}", ex);

                System.Windows.Application.Current?.Dispatcher?.BeginInvoke(() =>
                {
                    System.Windows.MessageBox.Show(
                        string.Format(Strings.AsyncRelayCommand_OperationFailedFormat, ex.Message),
                        Strings.AsyncRelayCommand_OperationFailedTitle,
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                });
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                Interlocked.Exchange(ref _raiseCanExecuteQueued, 0);
            }
            else
            {
                if (Interlocked.CompareExchange(ref _raiseCanExecuteQueued, 1, 0) != 0)
                {
                    return;
                }

                dispatcher.BeginInvoke(() =>
                {
                    Interlocked.Exchange(ref _raiseCanExecuteQueued, 0);
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
            }
        }
    }
}
