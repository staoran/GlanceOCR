using System.Text.Json;
using System.Text.Json.Nodes;
using Furion.DependencyInjection;
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

    /// <summary>
    /// OCR 接口选择菜单
    /// </summary>
    private readonly ToolStripMenuItem _ocrMenu;

    /// <summary>
    /// 翻译接口选择菜单
    /// </summary>
    private readonly ToolStripMenuItem _translateMenu;

    /// <summary>
    /// UI线程同步上下文
    /// </summary>
    private readonly SynchronizationContext? _syncContext;

    /// <summary>
    /// OCR 提供器
    /// </summary>
    private readonly INamedServiceProvider<IOCR> _ocrProvider;

    /// <summary>
    /// 翻译提供器
    /// </summary>
    private readonly INamedServiceProvider<ITranslator> _translatorProvider;

    private MainForm()
    {
        #region 初始化

        // 获取UI线程同步上下文
        _syncContext = SynchronizationContext.Current;
        // 获取 APP 配置项
        var options = App.GetService<IOptionsMonitor<AppOptions>>();
        // 注册配置源更改事件
        _optionsReloadToken = options.OnChange(ReloadOptions);
        // _appOptions = App.GetOptionsMonitor<AppOptions>();
        _appOptions = options.CurrentValue;
        // 获取 OCR 提供程序
        _ocrProvider = App.GetService<INamedServiceProvider<IOCR>>();
        // 获取翻译提供程序
        _translatorProvider = App.GetService<INamedServiceProvider<ITranslator>>();

        #endregion

        #region 初始化托盘图标

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

        #endregion

        InitializeComponent();
        Icon = Resources.AppIcon;

        #region OCR 文本框

        #region 图片识别接口配置菜单

        _ocrMenu = new ToolStripMenuItem("识别接口")
        {
            DropDownItems = 
            {
                CreateMenu("有道轻量", nameof(YouDaoOCRLite)),
                CreateMenu("百度", ""),
                new ToolStripMenuItem("本地1"),
                new ToolStripMenuItem("本地2")
            }
        };
        // 选择后保存配置
        _ocrMenu.DropDownItemClicked += OnTypeDropDownItemClicked;

        #endregion

        #region OCR 文本框右键菜单

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

        #endregion

        txtOCR.TextChanged += TxtOCROnTextChanged;

        #endregion

        #region 翻译文本框

        #region 翻译接口配置菜单

        _translateMenu = new ToolStripMenuItem("翻译接口")
        {
            DropDownItems =
            {
                CreateMenu("有道轻量", nameof(YouDaoLite)),
                CreateMenu("有道", nameof(YouDao)),
                new ToolStripMenuItem("谷歌"),
                new ToolStripMenuItem("微软")
            }
        };
        // 选择后保存配置
        _translateMenu.DropDownItemClicked += OnTypeDropDownItemClicked;

        #endregion

        #region 翻译文本框右键菜单

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

        #endregion

        #endregion

        Closing += OnClosing;
        Shown += OnShown;

        // 根据配置文件初始化配置项菜单
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
            dropDownItem.Checked = _appOptions.OCRType == dropDownItem.Name;
        }

        foreach (ToolStripMenuItem dropDownItem in _translateMenu.DropDownItems)
        {
            dropDownItem.Checked = _appOptions.TranslationType == dropDownItem.Name;
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
                app[type] = clickedItem.Name;
                await File.WriteAllTextAsync(filePath,
                    jsonNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            }
        }
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    /// <param name="text"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private ToolStripMenuItem CreateMenu(string text, string name)
    {
        return new ToolStripMenuItem(text)
        {
            Name = name
        };
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
                    using IOCR ocr = _ocrProvider.GetService<ITransient>(_appOptions.OCRType);
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
            ITranslator trans = _translatorProvider.GetService<ITransient>(_appOptions.TranslationType);
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