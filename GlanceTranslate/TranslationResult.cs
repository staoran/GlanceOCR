namespace GlanceTranslate;

/// <summary>
/// 翻译结果
/// </summary>
/// <param name="Translation">译文</param>
/// <param name="Form">源语言</param>
/// <param name="To">目标语言</param>
public record TranslationResult(string Translation, string Form, string To);