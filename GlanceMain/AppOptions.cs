using Furion.ConfigurableOptions;

namespace GlanceMain;

/// <summary>
/// APP 配置项
/// </summary>
public class AppOptions : IConfigurableOptions
{
    /// <summary>
    /// 开机自启
    /// </summary>
    public bool AutoStart { get; set; }

    /// <summary>
    /// 自动隐藏
    /// </summary>
    public bool Hide { get; set; }

    /// <summary>
    /// 关闭到托盘
    /// </summary>
    public bool CloseToPallet { get; set; }

    /// <summary>
    /// 快捷翻译
    /// </summary>
    public bool QuickTranslation { get; set; }

    /// <summary>
    /// 自动复制
    /// </summary>
    public bool AutoCopy { get; set; }

    /// <summary>
    /// OCR 快捷键
    /// </summary>
    public string OCRShortcutKey { get; set; }

    /// <summary>
    /// 翻译快捷键
    /// </summary>
    public string TranslationShortcutKey { get; set; }
}