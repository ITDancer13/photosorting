using System;
using System.Windows.Input;

namespace PhotoSorting.Model
{
    public class RelayCommand : ICommand
    {
        public Predicate<object> CanExecutePredicate { private get; set; }
        public Action<object> ExecuteAction { private get; set; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecutePredicate?.Invoke(parameter) ?? false;
        }

        public void Execute(object parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }
    }
}