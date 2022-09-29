using System.Drawing;
using PaddleOCRSharp;

namespace GlanceOCR;

public class PaddleOCRService : IOCR
{
    /// <summary>
    /// 跟踪是否已调用 Dispose
    /// </summary>
    private bool _disposed = false;

    /// <summary>
    /// 识别引擎
    /// </summary>
    private readonly PaddleOCREngine? _engine;

    /// <summary>
    /// 模型配置
    /// </summary>
    private readonly OCRModelConfig _modelConfig;

    /// <summary>
    /// 识别参数
    /// </summary>
    private readonly OCRParameter _parameter;

    public PaddleOCRService()
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        _modelConfig = new OCRModelConfig()
        {
            det_infer = Path.Combine(basePath, "inference", "ch_PP-OCRv3_det_infer"),
            cls_infer = Path.Combine(basePath, "inference", "ch_ppocr_mobile_v2.0_cls_infer"),
            rec_infer = Path.Combine(basePath, "inference", "ch_PP-OCRv3_rec_infer"),
            keys = Path.Combine(basePath, "inference", "ppocr_keys.txt")
        };

        _parameter = new OCRParameter()
        {
            numThread = 6, //预测并发线程数
            Enable_mkldnn = 1, //web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.
            use_angle_cls = 1, //是否开启方向检测，用于检测识别180旋转
            det_db_score_mode = 1, //是否使用多段线，即文字区域是用多段线还是用矩形，
            // rec_img_h = 32,
            UnClipRatio = 1.6f
        };

        _engine = new PaddleOCREngine(_modelConfig, _parameter);
    }

    /// <summary>
    /// 检查服务是否可用
    /// </summary>
    /// <returns></returns>
    public Task<bool> Check()
    {
        return Task.FromResult(
            Directory.Exists(_modelConfig.det_infer) &&
            Directory.Exists(_modelConfig.cls_infer) &&
            Directory.Exists(_modelConfig.rec_infer) &&
            File.Exists(_modelConfig.keys) &&
            _engine != null);
    }

    /// <summary>
    /// 文本识别
    /// </summary>
    /// <returns></returns>
    public async Task<string> DetectText(Image image)
    {
        var result = await Task.Run(() => _engine!.DetectText(image));
        return result.Text;
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
                _engine!.Dispose();
            }

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
    ~PaddleOCRService()
    {
        // 不要在此处重新创建 Dispose 清理代码。
        // 就可读性和可维护性而言，调用 Dispose(disposing: false) 是最佳的。
        Dispose(disposing: false);
    }
}