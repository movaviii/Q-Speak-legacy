using Gma.System.MouseKeyHook;

namespace Q_speak.Models
{
    public class KeybindManager : IDisposable
    {
        private readonly IKeyboardMouseEvents _globalHook;
        private readonly QForm _form;

        public KeybindManager(QForm form)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;
        }

        private void GlobalHook_KeyDown(object? sender, KeyEventArgs e)
        {
            // Control + Shift + X
            if (e.Control && e.Shift && e.KeyCode == Keys.X)
            {
                // Marshal to UI thread and call ShowFormAsync (non-blocking)
                // Use BeginInvoke so we don't block hook thread
                _form.BeginInvoke(new(async () =>
                {
                    await _form.ShowForm().ConfigureAwait(true);
                }));

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        public void Dispose()
        {
            try
            {
                _globalHook.KeyDown -= GlobalHook_KeyDown;
                _globalHook.Dispose();
            }
            catch { }
        }
    }
}