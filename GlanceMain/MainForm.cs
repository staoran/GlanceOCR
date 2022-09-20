using Furion;
using GlanceOCR;
using GlanceScreenshot;
using GlanceTranslate;
using GlanceTranslate.YouDao;
using Sunny.UI;

namespace GlanceMain;

public partial class MainForm : UIForm
{
    public MainForm()
    {
        InitializeComponent();
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
    }

    /// <summary>
    /// 截图识别
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnScreenshots_Click(object sender, EventArgs e)
    {
        Hide();
        using Screenshots screenshots = new Screenshots();
        Show();
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