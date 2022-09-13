namespace GlanceMain;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitor);
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}