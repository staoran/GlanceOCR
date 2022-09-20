namespace GlanceTranslate;

/// <summary>
/// 翻译服务接口
/// </summary>
public interface ITranslator
{
    /// <summary>
    /// 文本翻译
    /// </summary>
    /// <param name="text">文本内容</param>
    /// <param name="to">目标语言</param>
    /// <param name="from">源语言，默认为自动</param>
    /// <returns>包含翻译结果的任务</returns>
    Task<TranslationResult> TranslateAsync(string text, string to, string? from = null);
}