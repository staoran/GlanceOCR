using System.Security.Cryptography;
using System.Text;
using Furion.DependencyInjection;
using Furion.FriendlyException;
using Furion.LinqBuilder;
using Furion.RemoteRequest.Extensions;
using Microsoft.Extensions.Options;

namespace GlanceTranslate.YouDao;

/// <summary>
/// 有道翻译
/// </summary>
public class YouDao : ITranslator, ITransient
{
    /// <summary>
    /// 配置项
    /// </summary>
    private readonly YouDaoOptions _options;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">配置项</param>
    public YouDao(IOptionsMonitor<YouDaoOptions> options)
    {
        _options = options.CurrentValue;
    }

    /// <inheritdoc/>
    public async Task<TranslationResult> TranslateAsync(string text, string to, string? from = null)
    {
        if (_options.AppKey.IsNullOrEmpty() || _options.AppSecret.IsNullOrEmpty())
        {
            throw Oops.Bah("使用有道翻译需要设置 AppKey 和 AppSecret");
        }
        Dictionary<string, string> dic = new();
        string appKey = _options.AppKey!;
        string appSecret = _options.AppSecret!;
        string salt = DateTime.Now.Millisecond.ToString();
        dic.Add("from", from ?? "auto");
        dic.Add("to", to);
        dic.Add("signType", "v3");
        TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        long millis = (long)ts.TotalMilliseconds;
        string curtime = Convert.ToString(millis / 1000);
        dic.Add("curtime", curtime);
        string signStr = appKey + Truncate(text) + salt + curtime + appSecret;
        string sign = ComputeHash(signStr, new SHA256CryptoServiceProvider());
        dic.Add("q", System.Web.HttpUtility.UrlEncode(text));
        dic.Add("appKey", appKey);
        dic.Add("salt", salt);
        dic.Add("sign", sign);
        var response = await "api".SetClient(_options.Name).SetBody(dic).PostAsAsync<YouDaoResponseMessage>();
        if (response.ErrorCode != "0")
        {
            throw Oops.Bah($"翻译失败，错误码：{response.ErrorCode}");
        }
        return new TranslationResult(response.Translation[0], response.Query, response.L);
    }

    /// <summary>
    /// 计算哈希值
    /// </summary>
    /// <param name="input"></param>
    /// <param name="algorithm"></param>
    /// <returns></returns>
    private static string ComputeHash(string input, HashAlgorithm algorithm)
    {
        Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
        return BitConverter.ToString(hashedBytes).Replace("-", "");
    }

    /// <summary>
    /// 截取特征字符串
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    private static string Truncate(string q)
    {
        if (q == null)
        {
            return null;
        }
        int len = q.Length;
        return len <= 20 ? q : (q.Substring(0, 10) + len + q.Substring(len - 10, 10));
    }
}