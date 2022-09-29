using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Furion.FriendlyException;
using Furion.RemoteRequest.Extensions;

namespace GlanceOCR.YouDao;

/// <summary>
/// 有道 OCR 轻量版
/// </summary>
public class YouDaoOCRLite : IOCR
{
    /// <summary>
    /// 跟踪是否已调用 Dispose
    /// </summary>
    private bool _disposed = false;

    /// <inheritdoc/>
    public Task<bool> Check()
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc/>
    public async Task<string> DetectText(Image image)
    {
        using var m = new MemoryStream();
        image.Save(m, ImageFormat.Jpeg);
        var buffer = new byte[m.Length];
        //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
        m.Seek(0, SeekOrigin.Begin);
        m.Read(buffer, 0, buffer.Length);
        string str = Convert.ToBase64String(buffer);
        var response = await "ocrapi1"
            .SetClient("youdaolite")
            .SetBody(new { imgBase = $"data:image/jpeg;base64,{str}", lang = string.Empty, company = string.Empty }, "application/x-www-form-urlencoded", Encoding.UTF8)
            .WithGZip()
            .PostAsAsync<YouDaoOCRResponseMessage>();
        if (response.ErrorCode != "0")
        {
            throw Oops.Bah($"翻译失败，错误码：{response.ErrorCode}");
        }
        return response.ToString();
        //{"orientation":"UP","errorCode":"0","lines":[{"boundingBox":"279,1523,449,1523,449,1564,279,1564","words":"第四部分AS."},{"boundingBox":"2447,1453,2639,1453,2639,1496,2447,1496","words":"选择存储库"}]}
    }

    /// <summary>
    /// 实施 IDisposable。不要将此方法设为虚拟方法。派生类不应覆盖此方法
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose(bool disposing) 在两个不同的场景中执行。
    /// 如果 disposing 等于 true，则该方法已被用户代码直接或间接调用。
    /// 可以处置托管和非托管资源。
    /// 如果 disposing 等于 false，则该方法已由运行时从终结器内部调用，您不应引用其他对象。只能释放非托管资源。
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        // 检查是否已调用 Dispose
        if (!this._disposed)
        {
            // 如果 disposing 等于 true，则释放所有托管和非托管资源
            if (disposing)
            {
                // 处置托管资源
                // _engine!.Dispose();
            }
            // _engine = null;

            // 以下注释为清理非托管资源的示例代码
            // // 在此处调用适当的方法来清理非托管资源
            // // 如果 disposing 为 false，则只执行以下代码
            // CloseHandle(handle);
            // handle = IntPtr.Zero;

            // 标记处理已完成
            _disposed = true;
        }
    }
}