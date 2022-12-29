namespace GlanceMain;

public partial class MainForm : UIForm
{
    #region 单例化主窗体

    public static MainForm Current => LazyInitializer.Instance;

    private static class LazyInitializer
    {
        static LazyInitializer() { }
        public static readonly MainForm Instance = new();
    }

    #endregion

    private readonly AppOptions _appOptions;

    private MainForm()
    {
        // 初始化托盘图标
        NotifyIconManager.Current.Run("一目十行\n双击截图识别\n单击打开主界面", Resources.AppIcon, n =>
        {
            n.ContextMenuStrip = new UIContextMenuStrip()
            {
                Items =
                {
                    { "截图", null, btnScreenshots_Click },
                    "-",
                    { "设置", null, (_, _) => { } },
                    "-",
                    { "打开", null, (_, _) => { Show(); } },
                    { "退出", null, (_, _) => { Application.ExitThread(); } },
                }
            };
            n.MouseClick += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    Show();
                }
            };
            n.DoubleClick += btnScreenshots_Click;
        });

        InitializeComponent();
        Icon = Resources.AppIcon;
        
        txtOCR.TextChanged += TxtOCROnTextChanged;
        Closing += OnClosing;
        Shown += OnShown;
        _appOptions = App.GetOptionsMonitor<AppOptions>();
    }

    /// <summary>
    /// 窗体首次显示时
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnShown(object? sender, EventArgs e)
    {
        if (_appOptions.Hide)
        {
            Hide();
        }
    }

    /// <summary>
    /// 窗体关闭前
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (_appOptions.CloseToPallet)
        {
            e.Cancel = true;
            Hide();
        }
    }

    /// <summary>
    /// 第一次显示窗体前
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        // 按钮增加 Tip
        toolTip.SetToolTip(btnScreenshots, "截图并识别");
        toolTip.SetToolTip(btnTranslate, "翻译");
        base.OnLoad(e);

        // Keys.F4.ToString()
        HotkeyManager.Current.AddOrReplace(_appOptions.OCRShortcutKey, Enum.Parse<Keys>(_appOptions.OCRShortcutKey), btnScreenshots_Click);
        HotkeyManager.Current.AddOrReplace(_appOptions.TranslationShortcutKey, Enum.Parse<Keys>(_appOptions.TranslationShortcutKey), btnTranslate_Click);

        // 程序自启
        AutoStart.SetMeAutoStart(_appOptions.AutoStart);
    }

    /// <summary>
    /// OCR 文本改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TxtOCROnTextChanged(object? sender, EventArgs e)
    {
        // 快捷翻译
        if (_appOptions.QuickTranslation)
        {
            btnTranslate_Click(sender, e);
        }
    }

    /// <summary>
    /// 截图识别
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnScreenshots_Click(object? sender, EventArgs e)
    {
        Hide();
        using var screenshots = new Screenshots();
        if (screenshots.ShowDialog() == DialogResult.OK)
        {
            var image = screenshots.SelectImage;
            if (image != null)
            {
                ShowWaitForm("文字识别中...");
                try
                {
                    using IOCR ocr = new YouDaoOCRLite();
                    await ocr.Check();
                    txtOCR.Text = await ocr.DetectText(image);
                    if (_appOptions.AutoCopy)
                    {
                        Clipboard.SetDataObject(txtOCR.Text);
                    }
                }
                finally
                {
                    HideWaitForm();
                    Show();
                }
            }
        }
        Show();
    }

    /// <summary>
    /// 文本翻译
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnTranslate_Click(object? sender, EventArgs e)
    {
        splitContainer.Panel2Collapsed = false;
        ITranslator trans = App.GetService<YouDaoLite>();
        var result = await trans.TranslateAsync(txtOCR.Text, "Auto");
        txtTranslate.Text = result.Translation;
    }

    #region 内存回收

    [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);

    public void ClearMemory()
    {
        GC.Collect();

        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }
    }

    #endregion
}