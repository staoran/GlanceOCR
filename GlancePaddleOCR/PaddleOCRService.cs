﻿using System.Drawing;
using Furion.DependencyInjection;
using PaddleOCRSharp;

namespace GlanceOCR;

public class PaddleOCRService : IOCR, ITransient
{
    /// <summary>
    /// 跟踪是否已调用 Dispose
    /// </summary>
    private bool _disposed = false;

    /// <summary>
    /// 识别引擎
    /// </summary>
    private PaddleOCREngine? _engine;

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
            det_infer = Path.Combine(basePath, "inference", "ch_PP-OCRv4_det_infer"),
            cls_infer = Path.Combine(basePath, "inference", "ch_ppocr_mobile_v2.0_cls_infer"),
            rec_infer = Path.Combine(basePath, "inference", "ch_PP-OCRv4_rec_infer"),
            keys = Path.Combine(basePath, "inference", "ppocr_keys.txt")
        };
        
        //OCR参数
        _parameter = new OCRParameter()
        {
            cpu_math_library_num_threads = 10, //预测并发线程数
            enable_mkldnn = true,
            cls = false, //是否执行文字方向分类；默认false
            det = true, //是否开启文本框检测，用于检测文本块
            use_angle_cls = true, //是否开启方向检测，用于检测识别180旋转
            det_db_score_mode = true, //是否使用多段线，即文字区域是用多段线还是用矩形，
            det_db_unclip_ratio = 1.6f,
            max_side_len = 960,
            rec_img_h = 48,
            rec_img_w = 320,
            det_db_thresh = 0.3f,
            det_db_box_thresh = 0.618f,
        };

        //初始化OCR引擎
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
    public async Task<string> DetectText(Bitmap image)
    {
        using var ms = new MemoryStream();
        image.Save(ms, image.RawFormat);
        var result = await Task.Run(() => _engine!.DetectText(ms.ToArray()));
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
    ~PaddleOCRService()
    {
        // 不要在此处重新创建 Dispose 清理代码。
        // 就可读性和可维护性而言，调用 Dispose(disposing: false) 是最佳的。
        Dispose(disposing: false);
    }
}