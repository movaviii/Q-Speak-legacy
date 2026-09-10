using System.ComponentModel;
using SpVoice;

namespace Q_speak;

partial class QForm
{
    private IContainer _components = null;
    private TextBox _input;
    private Button _speakButton;
    private Button _pauseButton;
    private Button _toggleVoiceButton;
    private Button _settingsButton;
    private ContextMenuStrip _settingsMenu;
    private ContextMenuStrip _voicesMenu;
    private ContextMenuStrip _outputsMenu;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (_components != null))
        {
            _components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _components = new System.ComponentModel.Container();
        Text = "Q-Speak";
        // TopMost = true;
        ClientSize = new Size(380, 100);
        ShowInTaskbar = false;
        ControlBox = false;
        KeyPreview = true;
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        StartPosition = FormStartPosition.CenterScreen;

        // Label
        Controls.Add(new Label()
        {
            Text = "Type text to speak (Enter to Speak, Esc to Close):",
            Left = 10,
            AutoSize = true
        });

        // Input
        Controls.Add(_input = new TextBox()
        {
            Top = 25,
            Left = 10,
            Width = 360,
            Height = 4
        });

        // Speak button
        _speakButton = new Button()
        {
            Text = "▷",
            Top = 60,
            Left = 10,
            Height = 30,
            Width = 30
        };
        _speakButton.Click += (s, e) => Speak();
        Controls.Add(_speakButton);

        // Pause button
        _pauseButton = new Button()
        {
            Text = "❘❘",
            Top = 60,
            Left = 45,
            Height = 30,
            Width = 30, 
        };
        _pauseButton.Click += (s, e) => StopSpeak();
        Controls.Add(_pauseButton);

        // Toggle voice button
        _toggleVoiceButton = new Button()
        {
            Text = "Voice: " + CleanVoiceName(_currentVoice),
            Top = 60,
            Left = 80,
            Height = 30,
            Width = 255
        };
        _toggleVoiceButton.Click += (s, e) => ToggleVoice();
        Controls.Add(_toggleVoiceButton);

        // Settings 3-point button
        _settingsButton = new Button
        {
            Text = "⋮",
            Top = 60,
            Left = 340,
            Height = 30,
            Width = 30,
        };
        _settingsButton.Click += (s, e) =>
        {
            _settingsMenu.Show(_settingsButton, new Point(0, _settingsButton.Height));
        };
        Controls.Add(_settingsButton);


        var Pitch = CreateTrackItem("Pitch", pitch =>
        {
            _config.Pitch = pitch;
            PostConfig(_config);
        }, Math.Clamp(_config.Pitch, -10, 10));

        var Rate = CreateTrackItem("Rate", rate =>
        {
            _config.Rate = rate;
            PostConfig(_config);
            _qSpeak.Rate = Math.Clamp(rate, -10, 10);
        }, Math.Clamp(_config.Rate, -10, 10));

        var Volume = CreateTrackItem("Vol", volume =>
        {
            _config.Volume = volume;
            PostConfig(_config);
            _qSpeak.Volume = Math.Clamp(volume, 0, 100);
        }, Math.Clamp(_config.Volume, 0, 100), 0, 100);

        // Settings menu, opens up on settignsMenu click
        _settingsMenu = new ContextMenuStrip();
        _settingsMenu.Items.Add("Prefer Current Voice", null, (s, e) =>
        {
            _config.PreferredVoice = _currentVoice;
            _toggleVoiceButton.Text = "Voice: " + CleanVoiceName(_currentVoice);
            PostConfig(_config);
        });
        _settingsMenu.Items.Add("Select voices", null, (s, e) => _voicesMenu.Show(_settingsButton, new Point(0, _settingsButton.Height)));
        _settingsMenu.Items.Add("Select output device", null, (s, e) => _outputsMenu.Show(_settingsButton, new Point(0, _settingsButton.Height)));
        _settingsMenu.Items.Add(Pitch);
        _settingsMenu.Items.Add(Rate);
        _settingsMenu.Items.Add(Volume);
        _settingsMenu.Items.Add("Reset Config", null, (s, e) => {
            PostConfig(_config = DefaultConfig());
            Application.Restart();
        });
        _settingsMenu.Items.Add(new ToolStripSeparator());
        _settingsMenu.Items.Add("Exit", null, (s, e) => Application.Exit());

        // Voices select menu, opens on Settings Menu "Select voices" button click
        _voicesMenu = new ContextMenuStrip();
        foreach (var voice in _allVoices)
        {
            var item = new ToolStripMenuItem(voice)
            {
                Text = CleanVoiceName(voice),
                Checked = _config.SelectedVoices.Contains(voice),
                CheckOnClick = true
            };

            item.CheckedChanged += (s, e) =>
            {
                if (item.Checked)
                    _config.SelectedVoices = [.. _allVoices.Where(v => _config.SelectedVoices.Contains(v) || v == voice)];
                else
                    _config.SelectedVoices.Remove(voice);
                PostConfig(_config);
            };

            _voicesMenu.Items.Add(item);
        }

        _outputsMenu = new ContextMenuStrip();
        foreach (var output in _allOutputs)
        {
            var item = new ToolStripMenuItem(output)
            {
                Text = output,
                Checked = output == _config.PreferredOutput,
                CheckOnClick = true,
            };

            item.Click += (s, e) =>
            {
                foreach (ToolStripMenuItem other in _outputsMenu.Items)
                {
                    if (other != item) other.Checked = false;
                }
                item.Checked = true;
                _config.PreferredOutput = output;
                PostConfig(_config);
                _qSpeak.AudioOutput = _qSpeak.GetAudioOutputs().Cast<SpObjectToken>().FirstOrDefault(o => o.GetDescription() == output);
            };

            _outputsMenu.Items.Add(item);
        }
    }
    
    private ToolStripControlHost CreateTrackItem(string label, Action<int> onSave, int defaultValue = 0, int min = -10, int max = 10)
    {
        var panel = new Panel()
        {
            Height = 20
        };

        var lbl = new Label
        {
            Width = 70,
        };

        var track = new TrackBar
        {
            AutoSize = false,
            Minimum = min,
            Maximum = max,
            Value = defaultValue,
            Height = 20,
            Width = panel.Width - lbl.Width,
            Left = lbl.Right,
        };

        track.ValueChanged += (s, e) => lbl.Text = $"{label}: {track.Value}";

        track.MouseUp += (s, e) => onSave.Invoke(track.Value);

        lbl.Text = $"{label}: {track.Value}";

        panel.Controls.Add(lbl);
        panel.Controls.Add(track);

        var host = new ToolStripControlHost(panel);
        host.Tag = track;

        return host;
    }
}