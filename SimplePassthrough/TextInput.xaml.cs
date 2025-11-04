using System.Windows;
using System.Windows.Controls;

namespace SimplePassthrough;

/// <summary>
/// Interaction logic for TextInput.xaml
/// </summary>
public partial class TextInput : UserControl
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextInput), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(TextInput), new PropertyMetadata(string.Empty));

    public TextInput()
    {
        InitializeComponent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty); 
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty); 
        set => SetValue(PlaceholderProperty, value);
    }
}
