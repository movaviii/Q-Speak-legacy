using System.Reflection;
using Q_speak.Models;
namespace Q_speak;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        using QForm form = new();
        using KeybindManager bind = new(form);
        using ContextMenuStrip menu = new();
        menu.Items.Add("Open (Ctrl + Shift + X)", null, async (s, e) => await form.ShowForm());
        menu.Items.Add("Exit", null, (s, e) => Application.Exit());

        using Stream? iconStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("Q_speak.Resources.Q-Speak.ico");
            
        using NotifyIcon icon = new()
        {
            Icon = iconStream != null ? new(iconStream) : SystemIcons.Application,
            Text = "Q-Speak",
            Visible = true,
            ContextMenuStrip = menu
        };
        Application.Run(form);
    }
}