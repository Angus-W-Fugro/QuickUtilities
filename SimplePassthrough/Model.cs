using System.ComponentModel;
using System.Runtime.CompilerServices;

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
