namespace GlanceScreenshot;

/// <summary>
/// 截屏程序
/// </summary>
public partial class Screenshots : Form, IDisposable
{
    /// <summary>
    /// 屏幕图像缓存
    /// </summary>
    private readonly Dictionary<Rectangle, Image> _screenImages;

    /// <summary>
    /// 是否按下
    /// </summary>
    private bool _isDown;

    /// <summary>
    /// 首次按下的坐标
    /// </summary>
    private Point _downPoint = new(0, 0);

    /// <summary>
    /// 选择的矩形
    /// </summary>
    private Rectangle _selectRectangle;

    /// <summary>
    /// 选择的图片
    /// </summary>
    public Image? SelectImage => GetScreenshots();

    public Screenshots()
    {
        // 初始化缓存
        _screenImages = new Dictionary<Rectangle, Image>();

        InitializeComponent();

        // // 指定控件的样式和行为，可避免切换屏幕时闪屏
        // SetStyle(
        //     // UserPaint 如果为true，则控件将自己绘制，而不是操作系统绘制。如果为false，则不会引发绘制事件。此样式仅适用于从控件派生的类。
        //     ControlStyles.UserPaint |
        //     // 如果为true，则在调整控件大小时重新绘制控件。
        //     ControlStyles.ResizeRedraw |
        //     // 如果为真，则控制忽略窗口消息WM_ERASEBKGND以减少闪烁。仅当UserPaint位设置为true时，才应应用此样式。
        //     ControlStyles.AllPaintingInWmPaint |
        //     // 如果为真，则控件首先绘制到缓冲区，而不是直接绘制到屏幕，这可以减少闪烁。如果将此属性设置为true，则还应将AllPaintingInWmPaint设置为true。
        //     ControlStyles.OptimizedDoubleBuffer, true);
        // // 应用样式
        // UpdateStyles();

        // 顶层显示
        TopMost = true;
        // 从任务栏隐藏
        ShowInTaskbar = false;
        // 窗体的边框样式设为无边框
        FormBorderStyle = FormBorderStyle.None;
    }

    /// <summary>
    /// 第一次显示窗体前
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        // 默认鼠标样式
        Cursor = Cursors.Cross;
        // 开启辅助缓冲区减少重绘时的闪烁
        DoubleBuffered = true;
        // 设置鼠标所在屏幕的图片作为截图背景
        SetBackgroundImage();
        base.OnLoad(e);
    }

    /// <summary>
    /// 鼠标单击控件时
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseClick(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            Application.Exit();
        }

        base.OnMouseClick(e);
    }

    /// <summary>
    /// 鼠标指针位于控件上并按下鼠标键时
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _isDown = true;
            _downPoint = e.Location;
        }
        base.OnMouseDown(e);
    }

    /// <summary>
    /// 鼠标指针在控件上并释放鼠标键时
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.Button == MouseButtons.Left)
        {
            if (!_selectRectangle.IsEmpty)
            {
                DialogResult = DialogResult.OK;
            }
        }
    }

    /// <summary>
    /// 鼠标指针移到控件上时
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e.Button == MouseButtons.Left)
        {
            // 计算矩形起始坐标
            var initPoint = new Point(
                _downPoint.X < e.X ? _downPoint.X : e.X,
                _downPoint.Y < e.Y ? _downPoint.Y : e.Y);
            // 计算矩形长宽
            int width = Math.Abs(_downPoint.X - e.X);
            int height = Math.Abs(_downPoint.Y - e.Y);
            // 避免溢出
            if (width == 0) ++width;
            if (height == 0) ++height;
            var size = new Size(width, height);
            if (_selectRectangle.IsEmpty)
            {
                _selectRectangle = new Rectangle(initPoint, size);
            }
            else
            {
                _selectRectangle.Location = initPoint;
                _selectRectangle.Size = size;
            }
            Invalidate();
        }
    }

    /// <summary>
    /// 鼠标指针离开控件时
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseLeave(EventArgs e)
    {
        // 如果鼠标没有按下并且鼠标不在当前控件范围内，才重新设置背景
        if (!_isDown && !Bounds.Contains(Cursor.Position))
        {
            SetBackgroundImage();
        }
        base.OnMouseLeave(e);
    }

    /// <summary>
    /// 重绘控件时
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (_screenImages.Count>0 && !_selectRectangle.IsEmpty)
        {
            Graphics g = e.Graphics;

            var srcImage = _screenImages[Bounds];
            g.DrawImage(srcImage, _selectRectangle, _selectRectangle, GraphicsUnit.Pixel);
            // 绘制矩形
            using var linePen = new Pen(Color.FromArgb(255, 0, 174, 255), 1);
            // 定义线段样式，虚线
            // linePen.DashStyle = DashStyle.DashDot;
            // linePen.DashPattern = new float[] { 5, 2, 15, 4 };
            linePen.DashPattern = new float[] { 15, 5, 10, 5 };
            g.DrawRectangle(linePen, _selectRectangle);
        }
    }
    
    


    /// <summary>
    /// 设置背景
    /// </summary>
    /// <returns></returns>
    private void SetBackgroundImage()
    {
        // 清空背景
        BackgroundImage = null;
        Visible = false;

        Rectangle bounds = default;
        Image? backgroundImage = null;

        // 先从缓存中找屏幕数据
        if (_screenImages.Count != 0)
        {
            foreach (var keyValuePair in _screenImages)
            {
                // 如果鼠标所在的屏幕有缓存图片
                if (keyValuePair.Key.Contains(Cursor.Position))
                {
                    bounds = keyValuePair.Key;
                    backgroundImage = keyValuePair.Value;
                }
            }
        }

        // 如果没有找到缓存的数据
        if (backgroundImage == null)
        {
            // 获取鼠标所在的屏幕的边界信息
            bounds = Screen.FromPoint(Cursor.Position).Bounds;

            // 从当前控件创建图形
            using Graphics g0 = CreateGraphics();
            // 初始化一个屏幕大小的图像
            backgroundImage = new Bitmap(bounds.Width, bounds.Height, g0);
            // 从现有图像创建图形类的图像实例
            using Graphics g1 = Graphics.FromImage(backgroundImage);
            // 从屏幕复制图像
            g1.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
            // // 在图像上填充矩形边框
            // g1.DrawRectangle(new Pen(Color.Red, 5), 0, 0, Bounds.Width, Bounds.Height);
            // 加入缓存
            _screenImages.Add(bounds, backgroundImage);
        }

        Bounds = bounds;
        BackgroundImage = backgroundImage;
        // 设置截图区域大小和起始坐标
        Location = bounds.Location;
        Size = bounds.Size;

        // // 背景增加黑色半透明蒙版
        // using Graphics bgs = Graphics.FromImage(BackgroundImage);
        // using SolidBrush brush = new(Color.FromArgb(100, Color.Black));
        // bgs.FillRectangle(brush, bounds);

        Visible = true;
    }

    /// <summary>
    /// 获取最终截图
    /// </summary>
    /// <returns></returns>
    private Image? GetScreenshots()
    {
        if (!_selectRectangle.IsEmpty)
        {
            Image scr = new Bitmap(_selectRectangle.Width, _selectRectangle.Height);
            using Graphics gs = Graphics.FromImage(scr);
            gs.DrawImage(BackgroundImage, 0, 0, _selectRectangle, GraphicsUnit.Pixel);
            return scr;
        }

        return null;
    }
}