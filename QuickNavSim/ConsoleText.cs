namespace QuickNavSim;

/// <summary>
/// Pads the subsequent text with spaces to overwrite the previous text.
/// </summary>
public class ConsoleText
{
    private string _PreviousText = string.Empty;
    private string _LatestText = string.Empty;

    public string Text
    {
        get
        {
            return _LatestText;
        }
        set
        {
            value = value.PadRight(_PreviousText.Length, ' ');
            _PreviousText = _LatestText;
            _LatestText = value;
        }
    }

    public override string ToString()
    {
        return Text;
    }
}
