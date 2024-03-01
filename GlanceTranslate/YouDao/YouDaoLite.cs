using System.Text;
using Furion.DependencyInjection;
using Furion.FriendlyException;
using Furion.RemoteRequest.Extensions;
using GlanceBase;
using Microsoft.Extensions.Options;

namespace GlanceTranslate.YouDao;

/// <summary>
/// 有道翻译轻量版
/// </summary>
public class YouDaoLite : ITranslator, ITransient
{
    /// <summary>
    /// 配置项
    /// </summary>
    private readonly YouDaoLiteOptions _options;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">配置项</param>
    public YouDaoLite(IOptionsMonitor<YouDaoLiteOptions> options)
    {
        _options = options.CurrentValue;
    }

    /// <inheritdoc/>
    public async Task<TranslationResult> TranslateAsync(string text, string to, string? from = null)
    {
        var response = await "trans"
            .SetClient(_options.Name)
            .SetBody(new { q = text, from = from ?? "Auto", to = to }, "application/x-www-form-urlencoded",
                Encoding.UTF8)
            .WithGZip().PostAsAsync<YouDaoResponseMessage>();
        if (response == null)
        {
            throw Oops.Bah("翻译失败，请求结果为空");
        }
        if (response.ErrorCode != "0")
        {
            throw Oops.Bah($"翻译失败，错误码：{response.ErrorCode}");
        }
        if (response.Translation == null)
        {
            throw Oops.Bah("翻译失败，翻译结果为 null");
        }
        
        return new TranslationResult(response.Translation[0], response.Query ?? "", response.L);
    }
}