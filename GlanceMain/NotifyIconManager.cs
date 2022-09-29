namespace GlanceMain;

/// <summary>
/// 托盘图标管理
/// </summary>
public class NotifyIconManager : IDisposable
{
    #region 单例化

    public static NotifyIconManager Current => LazyInitializer.Instance;

    private static class LazyInitializer
    {
        static LazyInitializer() { }
        public static readonly NotifyIconManager Instance = new();
    }

    #endregion

    /// <summary>
    /// 跟踪是否已调用 Dispose
    /// </summary>
    private bool _disposed = false;

    private NotifyIcon Instance { get; }

    private NotifyIconManager()
    {
        Instance = new NotifyIcon()
        {
            Visible = true
            // ContextMenuStrip = new UIContextMenuStrip()
            // {
            //     // ShowCheckMargin = true,
            //     // ShowImageMargin = true,
            //     Items =
            //     {
            //         { "截图", null, (sender, args) => { } },
            //         "-",
            //         { "设置", null, (sender, args) => { } },
            //         "-",
            //         { "打开", null, (sender, args) => { } },
            //         { "退出", null, (sender, args) => { Application.ExitThread(); } },
            //     }
            // }
        };
    }

    /// <summary>
    /// 配置托盘
    /// </summary>
    /// <param name="dic">托盘文本</param>
    /// <param name="icon">托盘图标</param>
    /// <param name="notifyIconOptions">自定义托盘</param>
    public void Run(string dic, Icon icon, Action<NotifyIcon> notifyIconOptions)
    {
        Instance.Icon = icon;
        Instance.Text = dic;
        notifyIconOptions(Instance);
        // Instance.MouseClick += (sender, args) =>
        // {
        //     if (args.Button == MouseButtons.Left)
        //     {
        //         Scoped.Create(((_, scope) =>
        //         {
        //             scope.ServiceProvider.GetRequiredService<MainForm>().ShowDialog();
        //         }));
        //     }
        // };
        //Instance.DoubleClick += (sender, args) => App.GetRequiredService<MainForm>().Show();
    }

    /// <summary>
    /// 增加托盘菜单
    /// </summary>
    /// <param name="name">菜单名称</param>
    /// <param name="image">菜单图标</param>
    /// <param name="eventHandler">菜单单击事件</param>
    public void AddMenuItem(string name, Image? image = null, EventHandler? eventHandler = null)
    {
        Instance.ContextMenuStrip ??= new UIContextMenuStrip() {ShowCheckMargin = true};
        Instance.ContextMenuStrip.Items.Add(name, image, eventHandler);
    }

    /// <summary>
    /// 实施 IDisposable。不要将此方法设为虚拟方法。派生类不应覆盖此方法
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        // 该对象将由 Dispose 方法清理。
        // 因此，您应该调用 GC.SuppressFinalize 将该对象从终结队列中移除，并防止该对象的终结代码再次执行。
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose(bool disposing) 在两个不同的场景中执行。
    /// 如果 disposing 等于 true，则该方法已被用户代码直接或间接调用。
    /// 可以处置托管和非托管资源。
    /// 如果 disposing 等于 false，则该方法已由运行时从终结器内部调用，您不应引用其他对象。只能释放非托管资源。
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        // 检查是否已调用 Dispose
        if (!_disposed)
        {
            // 如果 disposing 等于 true，则释放所有托管和非托管资源
            if (disposing)
            {
                // 处置托管资源
                Instance!.Dispose();
            }

            // 以下注释为清理非托管资源的示例代码
            // // 在此处调用适当的方法来清理非托管资源
            // // 如果 disposing 为 false，则只执行以下代码
            // CloseHandle(handle);
            // handle = IntPtr.Zero;

            // 标记处理已完成
            _disposed = true;
        }
    }

    /// <summary>
    /// 对终结代码使用 C# 终结器语法。
    /// 仅当未调用 Dispose 方法时，此终结器才会运行。
    /// 它为您的基类提供了最终确定的机会。不要在派生自此类的类型中提供终结器。
    /// </summary>
    ~NotifyIconManager()
    {
        // 不要在此处重新创建 Dispose 清理代码。
        // 就可读性和可维护性而言，调用 Dispose(disposing: false) 是最佳的。
        Dispose(disposing: false);
    }
}