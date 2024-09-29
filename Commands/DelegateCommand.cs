using System;
using System.Windows.Input;

namespace Flixplorer.Commands
{
    /// <summary>
    /// A class that simplifies the implementation of the ICommand interface.
    /// </summary>
    class DelegateCommand : ICommand
    {
        /// <summary>
        /// A delegate that defines a method that determines whether the command's action can execute in its current state.
        /// </summary>
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// A delegate that defines a method that represents the command's execution logic.
        /// </summary>
        private readonly Action<object> _execute;

        /// <summary>
        /// A delegate that defines a method that represents the command's execution logic (without parameter).
        /// </summary>
        private readonly Action? _executeWithoutParams;

        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute, Action executeWithoutParams)
        {
            this._execute = execute;
            this._canExecute = canExecute;
            this._executeWithoutParams = executeWithoutParams;
        }

        public DelegateCommand(Action<object> execute) : this(execute, null, null) { }

        public DelegateCommand(Action executeWithoutParams) : this(null, null, executeWithoutParams) { }

        /// <summary>
        /// Determines whether the command's action can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>True if the command's action can be executed, otherwise false</returns>
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// Executes the command's action.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute(parameter);
            }
            else
            {
                _executeWithoutParams?.Invoke();
            }
        }

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}