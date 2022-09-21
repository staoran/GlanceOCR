using System.Diagnostics;
using System.Runtime.InteropServices;
using Furion;
using GlanceTranslate.YouDao;
using Microsoft.Extensions.DependencyInjection;
using Sunny.UI;

namespace GlanceMain;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        #region 全局异常事件

        // 向事件中添加用于处理UI线程异常的事件处理程序
        Application.ThreadException += Application_ThreadException;

        // 设置未处理的异常模式以强制所有Windows窗体错误通过我们的处理程序
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        // 向事件中添加用于处理非UI线程异常的事件处理程序
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        #endregion

        // 多开处理
        HandleRunningInstance();

        // 启动服务主机
        Serve.Run(GenericRunOptions.DefaultSilence);

        // 处理高分问题
        Application.SetHighDpiMode(HighDpiMode.PerMonitor);
        // 应用程序初始化
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }

    #region 异常处理程序

    /// <summary>
    /// UI线程异常的事件处理程序
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        UIMessageTip.ShowError(e.Exception.Message);
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

    #endregion

    #region 检测处理多开

    /// <summary>
    /// 检测是否有正在运行的实例，并返回
    /// </summary>
    /// <returns></returns>
    private static Process? RunningInstance()
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
    private static void HandleRunningInstance()
    {
        // 判断是否已经有实例在运行
        Process? instance = RunningInstance();
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

    #endregion
}

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 强类型配置项
        services.AddConfigurableOptions<YouDaoOptions>();
        services.AddConfigurableOptions<YouDaoLiteOptions>();

        // 远程请求，配置请求客户端
        services.AddRemoteRequest(o =>
        {
            // 有道翻译
            var ydOptions = App.GetOptionsMonitor<YouDaoOptions>();
            o.AddHttpClient(ydOptions.Name, client =>
            {
                client.BaseAddress = new Uri(ydOptions.BaseURL);
                // client.DefaultRequestHeaders.Add("Accept", "");
                // client.DefaultRequestHeaders.Add("User-Agent", "");
            });

            // 有道翻译轻量版
            var ydLiteOptions = App.GetOptionsMonitor<YouDaoLiteOptions>();
            o.AddHttpClient(ydLiteOptions.Name, client =>
            {
                client.BaseAddress = new Uri(ydLiteOptions.BaseURL);
                client.DefaultRequestHeaders.Add("accept", "application/json, text/javascript, */*; q=0.01");
                client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
                client.DefaultRequestHeaders.Add("origin", "https://ai.youdao.com");
                client.DefaultRequestHeaders.Add("referer", "https://ai.youdao.com/");
                client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"105\", \" Not;A Brand\";v=\"99\", \"Chromium\";v=\"105\"");
                client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                client.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36 Edg/105.0.1343.42");
            });
        });
    }
}