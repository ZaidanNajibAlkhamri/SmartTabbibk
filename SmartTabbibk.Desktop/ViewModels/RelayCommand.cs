using System.Windows.Input;

namespace SmartTabbibk.Desktop.ViewModels
{
    /// <summary>
    /// تطبيق يدوي بسيط لواجهة ICommand — هو اللي بيخلي زرار في XAML
    /// يقدر "يستدعي" Method في الـ ViewModel مباشرة عن طريق Binding،
    /// من غير أي كود Code-Behind في الـ View (نفس فلسفة MVVM بالضبط)
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _executeAsync;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);

        public async void Execute(object? parameter)
        {
            _isExecuting = true;
            CommandManager.InvalidateRequerySuggested();
            try
            {
                await _executeAsync();
            }
            finally
            {
                _isExecuting = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
