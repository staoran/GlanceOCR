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

        Serve.Run(GenericRunOptions.DefaultSilence);
        Application.SetHighDpiMode(HighDpiMode.PerMonitor);
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