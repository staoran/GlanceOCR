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

        HotkeyManager.Current.AddOrReplace("F4", Keys.F4, btnScreenshots_Click);
        HotkeyManager.Current.AddOrReplace("F6", Keys.F6, btnTranslate_Click);
    }

    /// <summary>
    /// 截图识别
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnScreenshots_Click(object? sender, EventArgs e)
    {
        Hide();
        using Screenshots screenshots = new Screenshots();
        if (screenshots.ShowDialog() == DialogResult.OK)
        {
            var image = screenshots.SelectImage;
            if (image != null)
            {
                ShowWaitForm("文字识别中...");
                using IOCR ocr = new PaddleOCRService();
                await ocr.Check();
                txtOCR.Text = await ocr.DetectText(image);
                HideWaitForm();
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
}