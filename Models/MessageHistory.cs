namespace Q_speak.Models;

internal sealed class MessageHistory
{
    private const int Limit = 100;
    private readonly List<string> _messages;
    private int _position;
    private string _draft = "";

    public MessageHistory(List<string> messages)
    {
        _messages = messages;
        Trim();
        Reset();
    }

    public void Add(string text)
    {
        _messages.Add(text);
        Trim();
        Reset();
    }

    public string Previous(string currentText)
    {
        if (_messages.Count == 0) return currentText;
        if (_position == _messages.Count) _draft = currentText;
        if (_position > 0) _position--;
        return _messages[_position];
    }

    public string Next(string currentText)
    {
        if (_position == _messages.Count) return currentText;
        _position++;
        return _position == _messages.Count ? _draft : _messages[_position];
    }

    public void Reset()
    {
        _position = _messages.Count;
        _draft = "";
    }

    private void Trim()
    {
        if (_messages.Count > Limit)
            _messages.RemoveRange(0, _messages.Count - Limit);
    }
}
