using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

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

    private AppOptions _appOptions;

    private readonly IDisposable? _optionsReloadToken;

    private readonly ToolStripMenuItem _ocrMenu;
    private readonly ToolStripMenuItem _translateMenu;
    private readonly SynchronizationContext? _syncContext;

    private MainForm()
    {
        //获取UI线程同步上下文
        _syncContext = SynchronizationContext.Current;
        var options = App.GetService<IOptionsMonitor<AppOptions>>();
        _optionsReloadToken = options.OnChange(ReloadOptions);
        // _appOptions = App.GetOptionsMonitor<AppOptions>();
        _appOptions = options.CurrentValue;
        
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

        _ocrMenu = new ToolStripMenuItem("识别接口")
        {
            DropDownItems = 
            {
                new ToolStripMenuItem("有道"),
                new ToolStripMenuItem("百度"),
                new ToolStripMenuItem("本地1"),
                new ToolStripMenuItem("本地2")
            }
        };
        _ocrMenu.DropDownItemClicked += OnTypeDropDownItemClicked;
        txtOCR.ContextMenuStrip = new UIContextMenuStrip()
        {
            Items =
            {
                { "全选", null, (_, _) => txtOCR.SelectAll() },
                { "剪切", null, (_, _) => txtOCR.Cut() },
                { "复制", null, (_, _) => txtOCR.Copy() },
                { "粘贴", null, (_, _) => txtOCR.Paste() },
                "-",
                _ocrMenu
            }
        };
        txtOCR.TextChanged += TxtOCROnTextChanged;

        _translateMenu = new ToolStripMenuItem("翻译接口")
        {
            DropDownItems =
            {
                new ToolStripMenuItem("有道"),
                new ToolStripMenuItem("百度"),
                new ToolStripMenuItem("谷歌"),
                new ToolStripMenuItem("微软")
            }
        };
        _translateMenu.DropDownItemClicked += OnTypeDropDownItemClicked;
        txtTranslate.ContextMenuStrip = new UIContextMenuStrip()
        {
            Items =
            {
                { "全选", null, (_, _) => txtTranslate.SelectAll() },
                { "剪切", null, (_, _) => txtTranslate.Cut() },
                { "复制", null, (_, _) => txtTranslate.Copy() },
                { "粘贴", null, (_, _) => txtTranslate.Paste() },
                "-",
                _translateMenu
            }
        };
        Closing += OnClosing;
        Shown += OnShown;
        
        ReDropDownItemChecked();
    }

    /// <summary>
    /// 配置热重载
    /// </summary>
    /// <param name="options"></param>
    private void ReloadOptions(AppOptions options)
    {
        _appOptions = options;
        // 异步更新已选项
        _syncContext?.Post(_ => ReDropDownItemChecked(), null);
    }

    /// <summary>
    /// 刷新已选项
    /// </summary>
    private void ReDropDownItemChecked()
    {
        foreach (ToolStripMenuItem dropDownItem in _ocrMenu.DropDownItems)
        {
            dropDownItem.Checked = _appOptions.OCRType == dropDownItem.Text;
        }

        foreach (ToolStripMenuItem dropDownItem in _translateMenu.DropDownItems)
        {
            dropDownItem.Checked = _appOptions.TranslationType == dropDownItem.Text;
        }
    }

    /// <summary>
    /// 接口服务选择事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private async void OnTypeDropDownItemClicked(object? sender, ToolStripItemClickedEventArgs args)
    {
        var type = string.Empty;
        if (sender is ToolStripMenuItem stripMenuItem)
        {
            type = stripMenuItem.Text switch
            {
                "识别接口" => "OCRType",
                "翻译接口" => "TranslationType",
                _ => type
            };
        }

        if (args.ClickedItem is ToolStripMenuItem clickedItem && !type.IsNullOrEmpty())
        {
            string filePath = Path.Combine(App.HostEnvironment.ContentRootPath, "appsettings.json");
            string text = await File.ReadAllTextAsync(filePath);
            JsonNode? jsonNode = JsonNode.Parse(text);
            if (jsonNode?["App"] is { } app)
            {
                app[type] = clickedItem.Text;
                await File.WriteAllTextAsync(filePath,
                    jsonNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            }
        }
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
        var text = txtOCR.Text;
        if (!text.IsNullOrEmpty())
        {
            ITranslator trans = App.GetService<YouDaoLite>();
            var result = await trans.TranslateAsync(text, "Auto");
            text = result.Translation;
        }
        txtTranslate.Text = text;
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