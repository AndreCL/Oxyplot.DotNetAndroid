using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace OxyPlot.DotNetAndroid;

/// <summary>
/// An Android View that renders OxyPlot charts.
/// </summary>
public class PlotView : View, IPlotView
{
    private PlotModel? _model;
    private IPlotController? _defaultController;
    private AndroidCanvasRenderContext? _rc;
    private readonly TouchHandler _touchHandler;
    internal float _dpiScale = 1.0f;
    private readonly object _renderingLock = new object();
    private readonly object _invalidateLock = new object();
    private bool _isModelInvalidated = true;
    private bool _updateDataFlag = true;

    public PlotView(Context context) : base(context)
    {
        _dpiScale = context.Resources?.DisplayMetrics?.Density ?? 1.0f;
        _touchHandler = new TouchHandler(this);
    }

    public PlotView(Context context, IAttributeSet attrs) : base(context, attrs)
    {
        _dpiScale = context.Resources?.DisplayMetrics?.Density ?? 1.0f;
        _touchHandler = new TouchHandler(this);
    }

    public PlotView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
    {
        _dpiScale = context.Resources?.DisplayMetrics?.Density ?? 1.0f;
        _touchHandler = new TouchHandler(this);
    }

    /// <summary>Gets or sets the plot model.</summary>
    public PlotModel? Model
    {
        get => _model;
        set
        {
            if (_model != value)
            {
                if (_model != null)
                    ((IPlotModel)_model).AttachPlotView(null);
                _model = value;
                if (_model != null)
                    ((IPlotModel)_model).AttachPlotView(this);
                InvalidatePlot();
            }
        }
    }

    /// <summary>Gets or sets the plot controller.</summary>
    public IPlotController? Controller { get; set; }

    /// <summary>Gets the actual model.</summary>
    public PlotModel? ActualModel => _model;

    /// <summary>Gets the actual plot controller.</summary>
    public IPlotController ActualController => Controller ?? (_defaultController ??= new PlotController());

    Model? IView.ActualModel => ActualModel;
    IController? IView.ActualController => ActualController;

    /// <summary>Gets the client area rectangle.</summary>
    public OxyRect ClientArea => new OxyRect(0, 0, Width, Height);

    /// <summary>Invalidates the plot (not blocking).</summary>
    public void InvalidatePlot(bool updateData = true)
    {
        lock (_invalidateLock)
        {
            _isModelInvalidated = true;
            _updateDataFlag = _updateDataFlag || updateData;
        }
        Invalidate();
    }

    public void SetCursorType(CursorType cursorType) { }
    public void SetClipboardText(string text) { }
    public void ShowTracker(TrackerHitResult trackerHitResult) { }
    public void HideTracker() { }
    public void ShowZoomRectangle(OxyRect rectangle) { }
    public void HideZoomRectangle() { }

    public override bool OnTouchEvent(MotionEvent? e)
    {
        if (e == null) return base.OnTouchEvent(e);
        var handled = base.OnTouchEvent(e);
        if (!handled)
        {
            _touchHandler.HandleTouchEvent(e);
            handled = true;
        }
        return handled;
    }

    public override bool OnKeyDown(Keycode keyCode, KeyEvent? e)
    {
        var handled = base.OnKeyDown(keyCode, e);
        if (!handled && e != null)
            handled = ActualController.HandleKeyDown(this, e.ToKeyEventArgs());
        return handled;
    }

    protected override void OnDetachedFromWindow()
    {
        Model = null;
        base.OnDetachedFromWindow();
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        var model = ActualModel;
        if (model == null || canvas == null) return;

        if (model.Background.IsVisible())
            canvas.DrawColor(model.Background.ToColor());

        lock (_invalidateLock)
        {
            if (_isModelInvalidated)
            {
                ((IPlotModel)model).Update(_updateDataFlag);
                _updateDataFlag = false;
                _isModelInvalidated = false;
            }
        }

        lock (_renderingLock)
        {
            _rc ??= new AndroidCanvasRenderContext(_dpiScale);
            _rc.SetTarget(canvas);
            ((IPlotModel)model).Render(_rc, new OxyRect(0, 0, Width, Height));
        }
    }
}

/// <summary>
/// Render context for Android Canvas.
/// </summary>
internal class AndroidCanvasRenderContext : IRenderContext
{
    private Canvas? _canvas;
    private readonly float _dpiScale;
    private readonly Paint _paint = new Paint { AntiAlias = true };
    // Cached objects reused each frame to avoid repeated JNI allocations
    private readonly Paint _measurePaint = new Paint { AntiAlias = true };
    private readonly Android.Graphics.Path _path = new Android.Graphics.Path();
    private readonly RectF _rectF = new RectF();

    public AndroidCanvasRenderContext(float dpiScale)
    {
        _dpiScale = dpiScale;
    }

    public void SetTarget(Canvas canvas) => _canvas = canvas;

    public bool RendersToScreen => true;
    public int ClipCount { get; private set; }

    public void CleanUp() { }

    public void DrawEllipse(OxyRect rect, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode = EdgeRenderingMode.Automatic)
    {
        if (_canvas == null) return;
        var rectF = ToRectF(rect);
        if (fill.IsVisible())
        {
            Configure(_paint, fill, Paint.Style.Fill, 0);
            _canvas.DrawOval(rectF, _paint);
        }
        if (stroke.IsVisible() && thickness > 0)
        {
            Configure(_paint, stroke, Paint.Style.Stroke, thickness);
            _canvas.DrawOval(rectF, _paint);
        }
    }

    public void DrawEllipses(IList<OxyRect> rectangles, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode = EdgeRenderingMode.Automatic)
    {
        foreach (var rect in rectangles)
            DrawEllipse(rect, fill, stroke, thickness, edgeRenderingMode);
    }

    public void DrawLine(IList<ScreenPoint> points, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[]? dashArray, LineJoin lineJoin)
    {
        if (_canvas == null || points.Count < 2) return;
        Configure(_paint, stroke, Paint.Style.Stroke, thickness);
        _path.Reset();
        _path.MoveTo((float)points[0].X, (float)points[0].Y);
        for (int i = 1; i < points.Count; i++)
            _path.LineTo((float)points[i].X, (float)points[i].Y);
        _canvas.DrawPath(_path, _paint);
    }

    public void DrawLineSegments(IList<ScreenPoint> points, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[]? dashArray, LineJoin lineJoin)
    {
        if (_canvas == null) return;
        Configure(_paint, stroke, Paint.Style.Stroke, thickness);
        for (int i = 0; i < points.Count - 1; i += 2)
            _canvas.DrawLine((float)points[i].X, (float)points[i].Y, (float)points[i + 1].X, (float)points[i + 1].Y, _paint);
    }

    public void DrawPolygon(IList<ScreenPoint> points, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[]? dashArray, LineJoin lineJoin)
    {
        if (_canvas == null || points.Count == 0) return;
        var path = BuildPath(points, close: true);
        if (fill.IsVisible())
        {
            Configure(_paint, fill, Paint.Style.Fill, 0);
            _canvas.DrawPath(path, _paint);
        }
        if (stroke.IsVisible() && thickness > 0)
        {
            Configure(_paint, stroke, Paint.Style.Stroke, thickness);
            _canvas.DrawPath(path, _paint);
        }
    }

    public void DrawPolygons(IList<IList<ScreenPoint>> polygons, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[]? dashArray, LineJoin lineJoin)
    {
        foreach (var polygon in polygons)
            DrawPolygon(polygon, fill, stroke, thickness, edgeRenderingMode, dashArray, lineJoin);
    }

    public void DrawRectangle(OxyRect rect, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode = EdgeRenderingMode.Automatic)
    {
        if (_canvas == null) return;
        var rectF = ToRectF(rect);
        if (fill.IsVisible())
        {
            Configure(_paint, fill, Paint.Style.Fill, 0);
            _canvas.DrawRect(rectF, _paint);
        }
        if (stroke.IsVisible() && thickness > 0)
        {
            Configure(_paint, stroke, Paint.Style.Stroke, thickness);
            _canvas.DrawRect(rectF, _paint);
        }
    }

    public void DrawRectangles(IList<OxyRect> rectangles, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode = EdgeRenderingMode.Automatic)
    {
        foreach (var rect in rectangles)
            DrawRectangle(rect, fill, stroke, thickness, edgeRenderingMode);
    }

    public void DrawText(ScreenPoint p, string text, OxyColor fill, string? fontFamily, double fontSize, double fontWeight, double rotate, HorizontalAlignment halign, VerticalAlignment valign, OxySize? maxSize)
    {
        if (_canvas == null || string.IsNullOrEmpty(text)) return;

        _paint.SetStyle(Paint.Style.Fill);
        _paint.Color = fill.ToColor();
        _paint.TextSize = (float)(fontSize * _dpiScale);
        _paint.FakeBoldText = fontWeight >= 700;

        var fm = _paint.GetFontMetrics();
        var textWidth = _paint.MeasureText(text);

        float dx = halign switch
        {
            HorizontalAlignment.Center => -textWidth / 2f,
            HorizontalAlignment.Right => -textWidth,
            _ => 0f
        };

        float dy = valign switch
        {
            VerticalAlignment.Top => -fm!.Ascent,
            VerticalAlignment.Middle => -(fm!.Ascent + fm.Descent) / 2f,
            VerticalAlignment.Bottom => -fm!.Descent,
            _ => 0f
        };

        _canvas.Save();
        _canvas.Translate((float)p.X, (float)p.Y);
        if (Math.Abs(rotate) > 0.001)
            _canvas.Rotate((float)rotate);
        _canvas.DrawText(text, dx, dy, _paint);
        _canvas.Restore();
    }

    public OxySize MeasureText(string text, string? fontFamily, double fontSize, double fontWeight)
    {
        if (string.IsNullOrEmpty(text)) return OxySize.Empty;
        _measurePaint.TextSize = (float)(fontSize * _dpiScale);
        _measurePaint.FakeBoldText = fontWeight >= 700;
        var fm = _measurePaint.GetFontMetrics();
        return new OxySize(_measurePaint.MeasureText(text), fm!.Bottom - fm.Top);
    }

    public void PushClip(OxyRect clippingRect)
    {
        _canvas?.Save();
        _canvas?.ClipRect((float)clippingRect.Left, (float)clippingRect.Top, (float)clippingRect.Right, (float)clippingRect.Bottom);
        ClipCount++;
    }

    public void PopClip()
    {
        _canvas?.Restore();
        ClipCount = Math.Max(0, ClipCount - 1);
    }

    public void SetClip(OxyRect rect) =>
        _canvas?.ClipRect((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);

    public void ResetClip() { }

    public void DrawImage(OxyImage source, double srcX, double srcY, double srcWidth, double srcHeight, double destX, double destY, double destWidth, double destHeight, double opacity, bool interpolate) { }

    public void SetToolTip(string text) { }

    private void Configure(Paint paint, OxyColor color, Paint.Style style, double thickness)
    {
        paint.Color = color.ToColor();
        paint.SetStyle(style);
        paint.StrokeWidth = (float)(thickness * _dpiScale);
    }

    private RectF ToRectF(OxyRect rect)
    {
        _rectF.Set((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
        return _rectF;
    }

    private Android.Graphics.Path BuildPath(IList<ScreenPoint> points, bool close)
    {
        _path.Reset();
        _path.MoveTo((float)points[0].X, (float)points[0].Y);
        for (int i = 1; i < points.Count; i++)
            _path.LineTo((float)points[i].X, (float)points[i].Y);
        if (close) _path.Close();
        return _path;
    }
}

/// <summary>
/// Extension methods for OxyPlot integration with Android.
/// </summary>
internal static class AndroidExtensions
{
    public static Color ToColor(this OxyColor oxyColor) =>
        new Color(oxyColor.R, oxyColor.G, oxyColor.B, oxyColor.A);

    public static OxyKeyEventArgs ToKeyEventArgs(this KeyEvent e) =>
        new OxyKeyEventArgs
        {
            Key = e.KeyCode.ToOxyKey(),
            ModifierKeys = e.MetaState.ToOxyModifierKeys()
        };

    public static OxyKey ToOxyKey(this Keycode keyCode) => keyCode switch
    {
        Keycode.A => OxyKey.A,
        Keycode.S => OxyKey.S,
        Keycode.D => OxyKey.D,
        Keycode.F => OxyKey.F,
        Keycode.Plus => OxyKey.Add,
        Keycode.Minus => OxyKey.Subtract,
        Keycode.DpadUp => OxyKey.Up,
        Keycode.DpadDown => OxyKey.Down,
        Keycode.DpadLeft => OxyKey.Left,
        Keycode.DpadRight => OxyKey.Right,
        _ => OxyKey.Unknown
    };

    public static OxyModifierKeys ToOxyModifierKeys(this MetaKeyStates metaState)
    {
        var modifiers = OxyModifierKeys.None;
        if (metaState.HasFlag(MetaKeyStates.ShiftOn)) modifiers |= OxyModifierKeys.Shift;
        if (metaState.HasFlag(MetaKeyStates.CtrlOn)) modifiers |= OxyModifierKeys.Control;
        if (metaState.HasFlag(MetaKeyStates.AltOn)) modifiers |= OxyModifierKeys.Alt;
        return modifiers;
    }
}
