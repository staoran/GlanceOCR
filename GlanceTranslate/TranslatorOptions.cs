namespace GlanceTranslate;

/// <summary>
/// 翻译服务基础配置
/// </summary>
public class TranslatorOptions
{
    /// <summary>
    /// 服务名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 服务地址
    /// </summary>
    public string BaseURL { get; set; }

    /// <summary>
    /// 支持的源语言
    /// {"中": "zh-CHS","英": "en"}
    /// </summary>
    public IReadOnlyDictionary<string, string> SourceLanguages { get; set; }

    /// <summary>
    /// 支持的目标语言，空则表示源语言支持互相转换
    /// {"中": ["俄","英"], "英": ["俄","中"]}
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? TargetLanguages { get; set; }
}