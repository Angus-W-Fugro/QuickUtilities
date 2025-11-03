using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimplePassthrough;

public abstract class Model : INotifyPropertyChanged
{
    /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RemovePropertyChangedHandlers()
    {
        PropertyChanged = null;
    }

    protected void NotifyAllPropertiesChanged()
    {
        NotifyPropertyChanged(null);
    }

    protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = "Needs to be compiled with VS2012 or higher")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class Command : ICommand
{
    private readonly Action _Action;
    private readonly Func<bool>? _CanExecute;

    public Command(Action action, Func<bool>? canExecute = null)
    {
        _Action = action;
        _CanExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
        return _CanExecute is null || _CanExecute.Invoke();
    }

    public void Execute(object? parameter)
    {
        _Action();
    }
}