namespace GlanceMain;

/// <summary>
/// 运行的实例检测和限制
/// </summary>
public static class RunningInstance
{
    /// <summary>
    /// 检测是否有正在运行的实例，并返回
    /// </summary>
    /// <returns></returns>
    private static Process? CheckProcess()
    {
        // 获取当前进程实例
        var currentProcess = Process.GetCurrentProcess();
        // 获取当前运行的同名进程实例
        var processes = Process.GetProcessesByName(currentProcess.ProcessName);
        // 遍历与当前进程名称相同的进程列表
        foreach (Process process in processes)
        {
            // 两个进程来自同一个文件
            if (process.Id != currentProcess.Id && process.MainModule?.FileName == currentProcess.MainModule?.FileName)
            {
                // 返回另一个进程实例
                return process;
            }
        }

        //找不到其他进程实例，返回null
        return null;
    }

    /// <summary>
    /// 处理正在运行的程序
    /// </summary>
    public static void Handle()
    {
        // 判断是否已经有实例在运行
        using Process? instance = CheckProcess();
        if (instance == null) return;

        // 正常显示窗口
        ShowWindowAsync(instance.MainWindowHandle, 1);
        // 将窗口放置在最前端
        SetForegroundWindow(instance.MainWindowHandle);

        // 退出当前程序
        Environment.Exit(1);
    }

    /// <summary>
    /// 该函数设置由不同线程产生的窗口的显示状态  
    /// </summary>  
    /// <param name="hWnd">窗口句柄</param>  
    /// <param name="cmdShow">指定窗口如何显示。查看允许值列表</param>  
    /// <returns>如果窗口原来可见，返回值为非零；如果窗口原来被隐藏，返回值为零</returns>                      
    [DllImport("User32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

    /// <summary>  
    ///  该函数将创建指定窗口的线程设置到前台，并且激活该窗口
    ///  系统给创建前台窗口的线程分配的权限稍高于其他线程。
    /// </summary>  
    /// <param name="hWnd">将被激活并被调入前台的窗口句柄</param>  
    /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>  
    [DllImport("User32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
}