using System;
using System.Windows.Input;

namespace Gol.Core.Prism
{
    /// <summary>
    ///     DelegateCommand implementation.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object>? _canExecute;
        private readonly Action<object> _execute;

        /// <summary>
        ///     Constructor for <see cref="DelegateCommand" />.
        /// </summary>
        public DelegateCommand(Action<object> execute, Predicate<object>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc />
        bool ICommand.CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        /// <inheritdoc />
        void ICommand.Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        ///     Raise can execute command.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}