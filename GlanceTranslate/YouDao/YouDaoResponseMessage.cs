namespace GlanceTranslate.YouDao;

/// <summary>
/// 有道翻译响应结果
/// </summary>
/// <param name="ErrorCode">状态码</param>
/// <param name="Query">源语言</param>
/// <param name="Translation">翻译结果</param>
/// <param name="L">源语言和目标语言</param>
public record YouDaoResponseMessage(string ErrorCode, string? Query, string[]? Translation, string L);