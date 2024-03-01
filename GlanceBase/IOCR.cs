using System.Drawing;

namespace GlanceOCR;

public interface IOCR : IDisposable
{
    /// <summary>
    /// 检查服务是否可用
    /// </summary>
    /// <returns></returns>
    Task<bool> Check();

    /// <summary>
    /// 文本识别
    /// </summary>
    /// <returns></returns>
    Task<string> DetectText(Bitmap image);
}