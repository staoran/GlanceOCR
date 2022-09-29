using System.Text;

namespace GlanceOCR.YouDao;

/// <summary>
/// 有道 OCR 识别响应数据
/// </summary>
public class YouDaoOCRResponseMessage
{
    public YouDaoOCRResponseMessage(string errorCode, string orientation, List<YouDaoOCRLine>? lines)
    {
        ErrorCode = errorCode;
        Orientation = orientation;
        Lines = lines;
    }

    /// <summary>
    /// 错误码
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// 图片方向
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// 文本块
    /// </summary>
    public List<YouDaoOCRLine>? Lines { get; }

    public void Deconstruct(out string errorCode, out string orientation, out List<YouDaoOCRLine>? lines)
    {
        errorCode = ErrorCode;
        orientation = Orientation;
        lines = Lines;
    }

    public override string ToString()
    {
        if (ErrorCode != "0" || Lines == null || Lines.Count == 0)
            return string.Empty;

        var str = new StringBuilder();
        Lines.ForEach(x => str.AppendLine(x.Words));

        return str.ToString();
    }
}

/// <summary>
/// 文本块数据
/// </summary>
/// <param name="BoundingBox">文本位置</param>
/// <param name="Words">文本</param>
public record YouDaoOCRLine(string BoundingBox, string Words);