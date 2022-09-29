using System.Drawing;
using OcrLiteLib;

namespace GlanceOCR;

public class OcrLiteService : IOCR
{
    /// <summary>
    /// 跟踪是否已调用 Dispose
    /// </summary>
    private bool _disposed = false;

    /// <summary>
    /// 识别引擎
    /// </summary>
    private OcrLite? _engine;

    public OcrLiteService()
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        // det 检测模型
        // rec 识别模型
        // cls 方向角度模型
        string modelsDir = basePath + "models";
        string detPath = modelsDir + "\\dbnet.onnx";
        string clsPath = modelsDir + "\\angle_net.onnx";
        string recPath = modelsDir + "\\crnn_lite_lstm.onnx";
        string keysPath = modelsDir + "\\keys.txt";
        _engine = new OcrLite();
        _engine.InitModels(detPath,clsPath,recPath,keysPath, 4);
    }

    /// <summary>
    /// 检查服务是否可用
    /// </summary>
    /// <returns></returns>
    public Task<bool> Check()
    {
        // return Task.FromResult(
        //     Directory.Exists(_modelConfig.det_infer) &&
        //     Directory.Exists(_modelConfig.cls_infer) &&
        //     Directory.Exists(_modelConfig.rec_infer) &&
        //     File.Exists(_modelConfig.keys) &&
        //     _engine != null);
        return Task.FromResult(true);
    }

    /// <summary>
    /// 文本识别
    /// </summary>
    /// <returns></returns>
    public async Task<string> DetectText(Image image)
    {
        var result = await Task.Run(() =>
        {
            int padding = 50;
            int imgResize = 1024;
            float boxScoreThresh = 0.618f;
            float boxThresh = 0.300f;
            float unClipRatio = 2.0f;
            bool doAngle = true;
            bool mostAngle = true;
            return _engine?.Detect(image, padding, imgResize, boxScoreThresh, boxThresh, unClipRatio, doAngle, mostAngle);
        });
        return result?.StrRes;
    }

    /// <summary>
    /// 实施 IDisposable。不要将此方法设为虚拟方法。派生类不应覆盖此方法
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        // 该对象将由 Dispose 方法清理。
        // 因此，您应该调用 GC.SuppressFinalize 将该对象从终结队列中移除，并防止该对象的终结代码再次执行。
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
            _engine = null;

            // 以下注释为清理非托管资源的示例代码
            // // 在此处调用适当的方法来清理非托管资源
            // // 如果 disposing 为 false，则只执行以下代码
            // CloseHandle(handle);
            // handle = IntPtr.Zero;

            // 标记处理已完成
            _disposed = true;
        }
    }

    /// <summary>
    /// 对终结代码使用 C# 终结器语法。
    /// 仅当未调用 Dispose 方法时，此终结器才会运行。
    /// 它为您的基类提供了最终确定的机会。不要在派生自此类的类型中提供终结器。
    /// </summary>
    ~OcrLiteService()
    {
        // 不要在此处重新创建 Dispose 清理代码。
        // 就可读性和可维护性而言，调用 Dispose(disposing: false) 是最佳的。
        Dispose(disposing: false);
    }
}