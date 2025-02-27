using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

//------------Model------------//
namespace Livrable1.Model
{
    //------------Class BaseViewModel------------//
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Event triggered when a property value changes
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to raise the PropertyChanged event
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    //------------Class BaseViewModel------------//


    //------------Class RelayCommand------------//
    public class RelayCommand : ICommand
    {
        private readonly Action _execute; // The action to execute when the command is triggered
        private readonly Func<bool> _canExecute; // Function to determine if the command can execute

        // Constructor initializing execute action and optional canExecute function
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? (() => true);
        }

        // Determines whether the command can execute
        public bool CanExecute(object? parameter) => _canExecute();

        // Executes the command
        public void Execute(object? parameter) => _execute();

        // Event triggered when the ability to execute changes (linked to CommandManager)
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
    //------------Class RelayCommand------------//
}
//------------Model------------//