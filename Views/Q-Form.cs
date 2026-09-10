namespace Q_speak;

using SpVoice;
public partial class QForm : Form
{
    private string _currentVoice = _config.PreferredVoice;
    private string _lastText = "";
    public QForm()
    {
        InitializeComponent();

        KeyDown += QKeyDown;
        Deactivate += (s, e) =>
        {
            if (_settingsMenu.Visible && _settingsMenu.Bounds.Contains(Cursor.Position))
                return;
            _voicesMenu.Hide();
            _outputsMenu.Hide();
            _settingsMenu.Hide();
            Hide();
        };

        RotateToMakeFirst(_currentVoice);

        _qSpeak.Voice = _qSpeak.GetVoices().Cast<SpObjectToken>().FirstOrDefault(o => o.GetDescription() == _currentVoice);
        _qSpeak.AudioOutput = _qSpeak.GetAudioOutputs().Cast<SpObjectToken>().FirstOrDefault(o => o.GetDescription() == _config.PreferredOutput);
    }

    private void QKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Enter:
                if (_input.Focused)
                {
                    Speak();
                    break;
                }
                return;

            case Keys.Up:
                (_lastText, _input.Text) = (_input.Text, _lastText);
                _input.SelectionStart = _input.Text.Length;
                break;

            case Keys.Escape:
                Hide();
                break;

            default:
                return; // if none, return
        }

        // if switch passed, handle event
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    private void Speak()
    {
        var text = _input.Text.Trim();
        if (string.IsNullOrEmpty(text)) return;
        _lastText = text;
        Hide();
        _qSpeak.Speak($"<pitch absmiddle='{Math.Clamp(_config.Pitch, -10, 10)}'>{text}</pitch>", SpeechVoiceSpeakFlags.SVSFIsXML | SpeechVoiceSpeakFlags.SVSFlagsAsync);
    }

    private void StopSpeak()
    {
        _qSpeak.Speak("", SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }
        
    private void ToggleVoice()
    {
        var voices = _config.SelectedVoices;
        if (voices is null || voices.Count <= 1) return; // nothing to change
        _currentVoice = voices[0];
        voices.RemoveAt(0);
        voices.Add(_currentVoice);
        _toggleVoiceButton.Text = "Voice: " + CleanVoiceName(_currentVoice);
        _qSpeak.Voice = _qSpeak.GetVoices().Cast<SpObjectToken>().FirstOrDefault(o => o.GetDescription() == _currentVoice);
    }

    private static string CleanVoiceName(string voice)
    {
        var split = voice.Split(' ');
        var clean = split.Length > 1 ? split[1] : split[0];
        return _config.PreferredVoice == voice ? clean + " !" : clean;
    }

    // Rotate the list until given voice is at index 0 (no persistent index used)
    private static void RotateToMakeFirst(string voice)
    {
        var voices = _config.SelectedVoices;
        if (voices is null || voices.Count == 0 || !voices.Contains(voice)) return;

        int rotate = (voices.IndexOf(voice) + 1) % voices.Count; // number of elements to move from front to back
        if (rotate == 0) return; // voice is either last or not found

        List<string> rotated = [.. voices.Skip(rotate).Concat(voices.Take(rotate))];
        voices.Clear();
        voices.AddRange(rotated);
    }
}
