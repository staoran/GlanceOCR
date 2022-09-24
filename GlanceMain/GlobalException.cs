namespace GlanceMain;

/// <summary>
/// 全局异常处理程序
/// </summary>
public static class GlobalException
{
    /// <summary>
    /// 处理全局异常
    /// </summary>
    public static void Handle()
    {
        // 向事件中添加用于处理UI线程异常的事件处理程序
        Application.ThreadException += Application_ThreadException;

        // 设置未处理的异常模式以强制所有Windows窗体错误通过我们的处理程序
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        // 向事件中添加用于处理非UI线程异常的事件处理程序
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    /// <summary>
    /// UI线程异常的事件处理程序
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        if(e.Exception is HotkeyAlreadyRegisteredException ex)
            UIMessageTip.ShowError(MainForm.Current, $"快捷键 {ex.Name} 已经被其他程序注册");
        else
        {
            UIMessageTip.ShowError(e.Exception.Message);
        }
    }

    /// <summary>
    /// 非UI线程异常的事件处理程序
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            UIMessageTip.ShowError(ex.Message);
        }
    }
}