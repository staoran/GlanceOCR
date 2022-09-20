using Furion.ConfigurableOptions;

namespace GlanceTranslate.YouDao;

/// <summary>
/// 有道翻译配置项
/// </summary>
public class YouDaoOptions : TranslatorOptions, IConfigurableOptions
{
    /// <summary>
    /// 应用 ID
    /// </summary>
    public string? AppKey { get; set; }

    /// <summary>
    /// 应用密钥
    /// </summary>
    public string? AppSecret { get; set; }
}