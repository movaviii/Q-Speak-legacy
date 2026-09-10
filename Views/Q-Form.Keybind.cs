using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Q_speak;
partial class QForm
{
    // P/Invoke
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("user32.dll", SetLastError = true)] private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    /// <summary>
    /// Public async show routine you can call from any thread.
    /// It will marshal to UI using BeginInvoke if necessary.
    /// </summary>
    public async Task ShowForm()
    {
        await Task.Run(new Action(() =>
        {
            AttachThreadInput(GetCurrentThreadId(), GetWindowThreadProcessId(GetForegroundWindow(), out _), true);
            _input.Clear();
            Show();
            Activate();
            BringToFront();
            _input.Focus();
        }));
    }
}

