using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace OxyPlot.DotNetAndroid;

/// <summary>
/// An Android View that renders OxyPlot charts using SkiaSharp
/// </summary>
public class PlotView : View, IPlotView
{
    private readonly PlotCore _plotCore;
    private readonly TouchHandler _touchHandler;
    private readonly TrackerView? _trackerView;
    internal float _dpiScale = 1.0f;
    private readonly object _renderingLock = new object();
    private readonly object _invalidateLock = new object();
    private bool _isModelInvalidated = true;
    private bool _updateDataFlag = true;

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotView"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public PlotView(Context context) : base(context)
    {
        Initialize(context);
        _plotCore = new PlotCore();
        _touchHandler = new TouchHandler(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotView"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="attrs">The attributes.</param>
    public PlotView(Context context, IAttributeSet attrs) : base(context, attrs)
    {
        Initialize(context);
        _plotCore = new PlotCore();
        _touchHandler = new TouchHandler(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotView"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="attrs">The attributes.</param>
    /// <param name="defStyleAttr">The default style attribute.</param>
    public PlotView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
    {
        Initialize(context);
        _plotCore = new PlotCore();
        _touchHandler = new TouchHandler(this);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the plot model.
    /// </summary>
    public PlotModel? Model
    {
        get => _plotCore.Model;
        set
        {
            if (_plotCore.Model != value)
            {
                if (_plotCore.Model != null)
                {
                    ((IPlotModel)_plotCore.Model).AttachPlotView(null);
                }

                _plotCore.Model = value;

                if (value != null)
                {
                    ((IPlotModel)value).AttachPlotView(this);
                }

                InvalidatePlot();
            }
        }
    }

    /// <summary>
    /// Gets or sets the plot controller.
    /// </summary>
    public PlotController? Controller
    {
        get => _plotCore.Controller;
        set => _plotCore.Controller = value;
    }

    /// <summary>
    /// Gets the actual model.
    /// </summary>
    public PlotModel? ActualModel => _plotCore.ActualModel;

    /// <summary>
    /// Gets the actual plot controller.
    /// </summary>
    public PlotController? ActualController => _plotCore.ActualController;

    #endregion

    #region IPlotView Implementation

    /// <summary>
    /// Gets the actual model.
    /// </summary>
    Model? IView.ActualModel => ActualModel;

    /// <summary>
    /// Gets the actual controller.
    /// </summary>
    IController? IView.ActualController => ActualController;

    /// <summary>
    /// Gets the client area rectangle.
    /// </summary>
    /// <returns>The client area rectangle.</returns>
    public OxyRect ClientArea => _plotCore.GetClientArea(Width, Height);

    /// <summary>
    /// Invalidates the plot (not blocking).
    /// </summary>
    /// <param name="updateData">if set to true, all data collections will be updated.</param>
    public void InvalidatePlot(bool updateData = true)
    {
        lock (_invalidateLock)
        {
            _isModelInvalidated = true;
            _updateDataFlag = _updateDataFlag || updateData;
        }

        Invalidate();
    }

    /// <summary>
    /// Sets the cursor type (not supported on mobile platforms).
    /// </summary>
    /// <param name="cursorType">The cursor type.</param>
    public void SetCursorType(CursorType cursorType)
    {
        _plotCore.SetCursorType(cursorType);
    }

    /// <summary>
    /// Sets the clipboard text (not supported on Android).
    /// </summary>
    /// <param name="text">The text.</param>
    public void SetClipboardText(string text)
    {
        // Not implemented for Android
    }

    /// <summary>
    /// Shows the tracker.
    /// </summary>
    /// <param name="trackerHitResult">The tracker data.</param>
    public void ShowTracker(TrackerHitResult trackerHitResult)
    {
        // Tracker functionality can be implemented later if needed
    }

    /// <summary>
    /// Hides the tracker.
    /// </summary>
    public void HideTracker()
    {
        // Tracker functionality can be implemented later if needed
    }

    /// <summary>
    /// Shows the zoom rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    public void ShowZoomRectangle(OxyRect rectangle)
    {
        // Not implemented for touch devices
    }

    /// <summary>
    /// Hides the zoom rectangle.
    /// </summary>
    public void HideZoomRectangle()
    {
        // Not implemented for touch devices
    }

    #endregion

    #region Private Methods and Overrides

    /// <summary>
    /// Initializes the view.
    /// </summary>
    /// <param name="context">The context.</param>
    private void Initialize(Context context)
    {
        _dpiScale = context.Resources?.DisplayMetrics?.Density ?? 1.0f;
    }

    /// <summary>
    /// Handles touch screen motion events.
    /// </summary>
    /// <param name="e">The motion event arguments.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
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

    /// <summary>
    /// Handles key down events.
    /// </summary>
    /// <param name="keyCode">The key code.</param>
    /// <param name="e">The event arguments.</param>
    /// <returns><c>true</c> if the event was handled.</returns>
    public override bool OnKeyDown(Keycode keyCode, KeyEvent? e)
    {
        var handled = base.OnKeyDown(keyCode, e);
        if (!handled && e != null)
        {
            handled = ActualController?.HandleKeyDown(this, e.ToKeyEventArgs()) ?? false;
        }
        return handled;
    }

    /// <summary>
    /// Draws the content of the control.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    protected override void OnDraw(Canvas? canvas)
    {
        base.OnDraw(canvas);

        var actualModel = ActualModel;
        if (actualModel == null || canvas == null)
            return;

        if (actualModel.Background.IsVisible())
        {
            canvas.DrawColor(actualModel.Background.ToColor());
        }

        lock (_invalidateLock)
        {
            if (_isModelInvalidated)
            {
                ((IPlotModel)actualModel).Update(_updateDataFlag);
                _updateDataFlag = false;
                _isModelInvalidated = false;
            }
        }

        lock (_renderingLock)
        {
            _plotCore.RenderToCanvas(canvas, Width, Height, _dpiScale);
        }
    }

    #endregion
}

/// <summary>
/// Core plotting functionality for .NET Android
/// </summary>
internal class PlotCore
{
    private PlotModel? _model;
    private PlotController? _controller;

    /// <summary>
    /// Gets or sets the plot model.
    /// </summary>
    public PlotModel? Model
    {
        get => _model;
        set
        {
            if (_model != value)
            {
                if (_model != null)
                {
                    _model.Updated -= OnModelUpdated;
                }

                _model = value;

                if (_model != null)
                {
                    _model.Updated += OnModelUpdated;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the plot controller.
    /// </summary>
    public PlotController? Controller
    {
        get => _controller ??= new PlotController();
        set => _controller = value;
    }

    /// <summary>
    /// Gets the actual model.
    /// </summary>
    public PlotModel? ActualModel => Model;

    /// <summary>
    /// Gets the actual plot controller.
    /// </summary>
    public PlotController? ActualController => Controller;

    /// <summary>
    /// Sets the cursor type (not supported on mobile platforms).
    /// </summary>
    /// <param name="cursorType">The cursor type.</param>
    public void SetCursorType(CursorType cursorType)
    {
        // Not supported on Android
    }

    /// <summary>
    /// Renders the plot to the specified Android canvas.
    /// </summary>
    /// <param name="canvas">The Android canvas to render to.</param>
    /// <param name="width">The width of the canvas.</param>
    /// <param name="height">The height of the canvas.</param>
    /// <param name="dpiScale">The DPI scale factor.</param>
    public void RenderToCanvas(Canvas canvas, int width, int height, float dpiScale)
    {
        if (canvas == null || Model == null || width == 0 || height == 0)
            return;

        try
        {
            // Create render context for Android Canvas
            var renderContext = new AndroidCanvasRenderContext(canvas, dpiScale);
            
            // Use the actual canvas dimensions for OxyPlot rendering
            var rect = new OxyRect(0, 0, width, height);
            
            // Set margins with much larger bottom space to prevent X-axis title overlap
            Model.PlotMargins = new OxyThickness(180, 30, 15, 200); // left, top, right, bottom
            
            // Use OxyPlot's official rendering
            ((IPlotModel)Model).Render(renderContext, rect);
        }
        catch (Exception ex)
        {
            // Draw error message using Android Paint
            var paint = new Paint
            {
                Color = Color.Red,
                TextSize = 16 * dpiScale,
                AntiAlias = true
            };
            canvas.DrawText($"Rendering error: {ex.Message}", 10, 50, paint);
        }
    }

    /// <summary>
    /// Called when the model is updated.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnModelUpdated(object? sender, EventArgs e)
    {
        // Model update handled by PlotView
    }

    /// <summary>
    /// Gets the client area rectangle for the given dimensions.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>The client area rectangle.</returns>
    public OxyRect GetClientArea(int width, int height) => new(0, 0, width, height);
}

/// <summary>
/// Render context for Android Canvas
/// </summary>
internal class AndroidCanvasRenderContext : IRenderContext
{
    private readonly Canvas _canvas;
    private readonly float _dpiScale;
    private readonly Dictionary<OxyColor, Paint> _paintCache = new();

    public AndroidCanvasRenderContext(Canvas canvas, float dpiScale)
    {
        _canvas = canvas;
        _dpiScale = dpiScale;
    }

    public bool RendersToScreen => true;
    public int ClipCount { get; private set; }

    public void CleanUp()
    {
        // Cleanup paint cache
        foreach (var paint in _paintCache.Values)
        {
            paint.Dispose();
        }
        _paintCache.Clear();
    }

    public void DrawEllipse(OxyRect rect, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode = EdgeRenderingMode.Automatic)
    {
        var paint = GetPaint(fill, stroke, thickness);
        var rectF = new RectF((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
        
        if (fill.IsVisible())
        {
            paint.SetStyle(Paint.Style.Fill);
            _canvas.DrawOval(rectF, paint);
        }
        
        if (stroke.IsVisible() && thickness > 0)
        {
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = (float)(thickness * _dpiScale);
            _canvas.DrawOval(rectF, paint);
        }
    }

    public void DrawEllipses(IList<OxyRect> rectangles, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode = EdgeRenderingMode.Automatic)
    {
        foreach (var rect in rectangles)
        {
            DrawEllipse(rect, fill, stroke, thickness, edgeRenderingMode);
        }
    }

    public void DrawLine(IList<ScreenPoint> points, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[] dashArray, LineJoin lineJoin)
    {
        if (points.Count < 2) return;
        
        var paint = GetPaint(OxyColors.Transparent, stroke, thickness);
        paint.SetStyle(Paint.Style.Stroke);
        paint.StrokeWidth = (float)(thickness * _dpiScale);

        var path = new Android.Graphics.Path();
        path.MoveTo((float)points[0].X, (float)points[0].Y);
        
        for (int i = 1; i < points.Count; i++)
        {
            path.LineTo((float)points[i].X, (float)points[i].Y);
        }
        
        _canvas.DrawPath(path, paint);
    }

    public void DrawLineSegments(IList<ScreenPoint> points, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[] dashArray, LineJoin lineJoin)
    {
        var paint = GetPaint(OxyColors.Transparent, stroke, thickness);
        paint.SetStyle(Paint.Style.Stroke);
        paint.StrokeWidth = (float)(thickness * _dpiScale);

        for (int i = 0; i < points.Count - 1; i += 2)
        {
            _canvas.DrawLine(
                (float)points[i].X, (float)points[i].Y,
                (float)points[i + 1].X, (float)points[i + 1].Y,
                paint);
        }
    }

    public void DrawPolygon(IList<ScreenPoint> points, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[] dashArray, LineJoin lineJoin)
    {
        var paint = GetPaint(fill, stroke, thickness);
        var path = new Android.Graphics.Path();
        
        if (points.Count > 0)
        {
            path.MoveTo((float)points[0].X, (float)points[0].Y);
            for (int i = 1; i < points.Count; i++)
            {
                path.LineTo((float)points[i].X, (float)points[i].Y);
            }
            path.Close();
        }

        if (fill.IsVisible())
        {
            paint.SetStyle(Paint.Style.Fill);
            _canvas.DrawPath(path, paint);
        }
        
        if (stroke.IsVisible() && thickness > 0)
        {
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = (float)(thickness * _dpiScale);
            _canvas.DrawPath(path, paint);
        }
    }

    public void DrawPolygons(IList<IList<ScreenPoint>> polygons, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode, double[] dashArray, LineJoin lineJoin)
    {
        foreach (var polygon in polygons)
        {
            DrawPolygon(polygon, fill, stroke, thickness, edgeRenderingMode, dashArray, lineJoin);
        }
    }

    public void DrawRectangle(OxyRect rect, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode = EdgeRenderingMode.Automatic)
    {
        var paint = GetPaint(fill, stroke, thickness);
        var rectF = new RectF((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
        
        if (fill.IsVisible())
        {
            paint.SetStyle(Paint.Style.Fill);
            _canvas.DrawRect(rectF, paint);
        }
        
        if (stroke.IsVisible() && thickness > 0)
        {
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = (float)(thickness * _dpiScale);
            _canvas.DrawRect(rectF, paint);
        }
    }

    public void DrawRectangles(IList<OxyRect> rectangles, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode = EdgeRenderingMode.Automatic)
    {
        foreach (var rect in rectangles)
        {
            DrawRectangle(rect, fill, stroke, thickness, edgeRenderingMode);
        }
    }

    public void DrawText(ScreenPoint p, string text, OxyColor fill, string fontFamily, double fontSize, double fontWeight, double rotate, HorizontalAlignment halign, VerticalAlignment valign, OxySize? maxSize)
    {
        // Handle null or empty text
        if (string.IsNullOrEmpty(text))
            return;
            
        var paint = GetPaint(fill, OxyColors.Transparent, 0);
        paint.SetStyle(Paint.Style.Fill);
        paint.TextSize = (float)(fontSize * _dpiScale);
        
        var bounds = new Rect();
        paint.GetTextBounds(text, 0, text.Length, bounds);
        
        // Get font metrics for proper baseline positioning
        var fontMetrics = paint.GetFontMetrics();
        
        var x = (float)p.X;
        var y = (float)p.Y;
        
        // Detect Y-axis titles (rotated text on the left side) and force left alignment
        // Use broader detection criteria to catch Y-axis titles reliably
        bool isYAxisTitle = Math.Abs(rotate - (-90)) < 1 && p.X < 200; // Rotated -90 degrees and in left margin area
        
        // Detect X-axis titles (horizontal text in the bottom area, but not tick labels)
        // Axis titles are typically longer than tick labels and positioned differently
        bool isXAxisTitle = Math.Abs(rotate) < 1 && 
                           p.Y > (_canvas.Height - 300) && 
                           text.Length > 2 && // Axis titles are longer than "0", "1", "2", etc.
                           !double.TryParse(text, out _); // Not a number (tick labels are usually numbers)
        
        // Force left alignment for Y-axis titles and adjust position for better spacing
        var effectiveHalign = isYAxisTitle ? HorizontalAlignment.Left : halign;
        
        // For Y-axis titles, move them further left within the margin for better left alignment
        if (isYAxisTitle)
        {
            x -= 40 * _dpiScale; // Move Y-axis titles further left for true left alignment
        }
        
        // For X-axis titles, position them at the bottom of the PlotView
        if (isXAxisTitle)
        {
            y = _canvas.Height - 50 * _dpiScale; // Position 50 pixels from bottom
        }
        
        // Adjust for vertical alignment - Android draws from baseline, we need to adjust
        switch (valign)
        {
            case VerticalAlignment.Top:
                y -= fontMetrics.Ascent; // Move up by ascent to align top
                break;
            case VerticalAlignment.Middle:
                y -= (fontMetrics.Ascent + fontMetrics.Descent) / 2f; // Center vertically
                break;
            case VerticalAlignment.Bottom:
                y -= fontMetrics.Descent; // Move up by descent to align bottom
                break;
        }
        
        // Adjust for horizontal alignment
        switch (effectiveHalign)
        {
            case HorizontalAlignment.Center:
                x -= bounds.Width() / 2f;
                break;
            case HorizontalAlignment.Right:
                x -= bounds.Width();
                break;
        }
        
        _canvas.DrawText(text, x, y, paint);
    }

    public OxySize MeasureText(string text, string fontFamily, double fontSize, double fontWeight)
    {
        // Handle null or empty text
        if (string.IsNullOrEmpty(text))
            return OxySize.Empty;
            
        var paint = new Paint();
        paint.TextSize = (float)(fontSize * _dpiScale);
        
        var bounds = new Rect();
        paint.GetTextBounds(text, 0, text.Length, bounds);
        
        // Get font metrics to match DrawText positioning
        var fontMetrics = paint.GetFontMetrics();
        
        // Return size that includes proper height calculation with minimal padding
        var width = bounds.Width() / _dpiScale;
        var height = (fontMetrics.Bottom - fontMetrics.Top) / _dpiScale;
        
        // Add minimal padding - let explicit margins handle spacing
        width += 4;  // Minimal padding for width
        height += 4; // Minimal padding for height
        
        return new OxySize(width, height);
    }

    public void SetClip(OxyRect rect)
    {
        _canvas.ClipRect((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
    }

    public void ResetClip()
    {
        // Android canvas doesn't have a direct reset clip, would need to save/restore
    }

    public void DrawImage(OxyImage source, double srcX, double srcY, double srcWidth, double srcHeight, double destX, double destY, double destWidth, double destHeight, double opacity, bool interpolate)
    {
        // Image rendering can be implemented if needed
    }

    public void SetToolTip(string text)
    {
        // Tooltips handled by TrackerView
    }

    private Paint GetPaint(OxyColor fill, OxyColor stroke, double thickness)
    {
        var key = stroke.IsVisible() ? stroke : fill;
        if (!_paintCache.TryGetValue(key, out var paint))
        {
            paint = new Paint();
            paint.AntiAlias = true;
            _paintCache[key] = paint;
        }
        
        if (fill.IsVisible())
            paint.Color = fill.ToColor();
        else if (stroke.IsVisible())
            paint.Color = stroke.ToColor();
            
        return paint;
    }

    public void PushClip(OxyRect clippingRect)
    {
        _canvas.Save();
        SetClip(clippingRect);
        ClipCount++;
    }

    public void PopClip()
    {
        _canvas.Restore();
        ClipCount = Math.Max(0, ClipCount - 1);
    }
}

/// <summary>
/// Extension methods for OxyPlot integration with Android
/// </summary>
internal static class AndroidExtensions
{
    public static Color ToColor(this OxyColor oxyColor)
    {
        return new Color(oxyColor.R, oxyColor.G, oxyColor.B, oxyColor.A);
    }

    public static OxyKeyEventArgs ToKeyEventArgs(this KeyEvent e)
    {
        return new OxyKeyEventArgs
        {
            Key = e.KeyCode.ToOxyKey(),
            ModifierKeys = e.MetaState.ToOxyModifierKeys()
        };
    }

    public static OxyKey ToOxyKey(this Keycode keyCode)
    {
        return keyCode switch
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
    }

    public static OxyModifierKeys ToOxyModifierKeys(this MetaKeyStates metaState)
    {
        var modifiers = OxyModifierKeys.None;
        
        if (metaState.HasFlag(MetaKeyStates.ShiftOn))
            modifiers |= OxyModifierKeys.Shift;
        if (metaState.HasFlag(MetaKeyStates.CtrlOn))
            modifiers |= OxyModifierKeys.Control;
        if (metaState.HasFlag(MetaKeyStates.AltOn))
            modifiers |= OxyModifierKeys.Alt;
            
        return modifiers;
    }
}