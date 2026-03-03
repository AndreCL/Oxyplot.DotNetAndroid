using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using OxyPlot.SkiaSharp;
using OxyPlot.Series;
using SkiaSharp;
using System.Linq;

namespace OxyPlot.DotNetAndroid
{
    /// <summary>
    /// An Android View that renders OxyPlot charts using SkiaSharp
    /// </summary>
    public class PlotView : FrameLayout, IPlotView
    {
        private readonly SkiaCanvasView _canvasView;
        private readonly PlotCore _plotCore;
        private readonly TouchHandler _touchHandler;
        private readonly TrackerView _trackerView;
        private float _dpiScale = 1.0f;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PlotView"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PlotView(Context context) : base(context)
        {
            Initialize(context);
            _canvasView = new SkiaCanvasView(context);
            _plotCore = new PlotCore();
            _touchHandler = new TouchHandler(this);
            _trackerView = new TrackerView(context);
            SetupView();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlotView"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="attrs">The attributes.</param>
        public PlotView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context);
            _canvasView = new SkiaCanvasView(context, attrs);
            _plotCore = new PlotCore();
            _touchHandler = new TouchHandler(this);
            _trackerView = new TrackerView(context);
            SetupView();
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
            _canvasView = new SkiaCanvasView(context, attrs, defStyleAttr);
            _plotCore = new PlotCore();
            _touchHandler = new TouchHandler(this);
            _trackerView = new TrackerView(context);
            SetupView();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the plot model.
        /// </summary>
        public PlotModel? Model
        {
            get => _plotCore.Model;
            set => _plotCore.Model = value;
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
            _plotCore.InvalidatePlot(updateData);
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
            if (trackerHitResult?.Text != null)
            {
                _trackerView.ShowTracker(
                    trackerHitResult.Text,
                    (float)trackerHitResult.Position.X,
                    (float)trackerHitResult.Position.Y
                );
            }
        }

        /// <summary>
        /// Hides the tracker.
        /// </summary>
        public void HideTracker()
        {
            _trackerView.HideTracker();
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

        #region Private Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        /// <param name="context">The context.</param>
        private void Initialize(Context context)
        {
            _dpiScale = context.Resources?.DisplayMetrics?.Density ?? 1.0f;
        }

        /// <summary>
        /// Sets up the view hierarchy.
        /// </summary>
        private void SetupView()
        {
            // Add the canvas view
            AddView(_canvasView, new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent
            ));

            // Add the tracker view
            AddView(_trackerView, new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent
            ));

            // Wire up events
            _canvasView.PaintCanvas += OnPaintCanvas;
            _plotCore.PlotInvalidated += OnPlotInvalidated;

            // Enable touch events
            _canvasView.Touch += OnTouch;
        }

        /// <summary>
        /// Handles the paint canvas event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The paint canvas event args.</param>
        private void OnPaintCanvas(object? sender, PaintCanvasEventArgs e)
        {
            _plotCore.RenderToCanvas(e.Canvas, e.Width, e.Height, _dpiScale);
        }

        /// <summary>
        /// Handles the plot invalidated event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPlotInvalidated(object? sender, EventArgs e)
        {
            Post(() => _canvasView.Invalidate());
        }

        /// <summary>
        /// Handles touch events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The touch event arguments.</param>
        private void OnTouch(object? sender, View.TouchEventArgs e)
        {
            if (e.Event != null)
            {
                _touchHandler.HandleTouchEvent(e.Event);
                e.Handled = true;
            }
        }

        #endregion
    }

    /// <summary>
    /// Event arguments for canvas painting
    /// </summary>
    public class PaintCanvasEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the canvas.
        /// </summary>
        public SKCanvas Canvas { get; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaintCanvasEventArgs"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PaintCanvasEventArgs(SKCanvas canvas, int width, int height)
        {
            Canvas = canvas;
            Width = width;
            Height = height;
        }
    }

    /// <summary>
    /// A custom Android view that hosts a SkiaSharp canvas
    /// </summary>
    internal class SkiaCanvasView : View
    {
        /// <summary>
        /// Event raised when the canvas needs to be painted.
        /// </summary>
        public event EventHandler<PaintCanvasEventArgs>? PaintCanvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaCanvasView"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SkiaCanvasView(Context context) : base(context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaCanvasView"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="attrs">The attributes.</param>
        public SkiaCanvasView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaCanvasView"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="attrs">The attributes.</param>
        /// <param name="defStyleAttr">The default style attribute.</param>
        public SkiaCanvasView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        /// <summary>
        /// Called when the view should render its content.
        /// </summary>
        /// <param name="canvas">The canvas on which the background will be drawn.</param>
        protected override void OnDraw(Canvas? canvas)
        {
            base.OnDraw(canvas);

            if (canvas == null || Width == 0 || Height == 0)
                return;

            // Create bitmap for SkiaSharp
            using var bitmap = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var skCanvas = new SKCanvas(bitmap);

            // Raise the paint canvas event
            PaintCanvas?.Invoke(this, new PaintCanvasEventArgs(skCanvas, Width, Height));

            // Convert to Android bitmap and draw
            using var androidBitmap = bitmap.ToAndroidBitmap();
            canvas.DrawBitmap(androidBitmap, 0, 0, null);
        }
    }

    /// <summary>
    /// Extension methods for SkiaSharp integration with Android
    /// </summary>
    internal static class SkiaSharpExtensions
    {
        /// <summary>
        /// Converts a SkiaSharp bitmap to an Android bitmap.
        /// </summary>
        /// <param name="skBitmap">The SkiaSharp bitmap.</param>
        /// <returns>The Android bitmap.</returns>
        public static Bitmap ToAndroidBitmap(this SKBitmap skBitmap)
        {
            var androidBitmap = Bitmap.CreateBitmap(skBitmap.Width, skBitmap.Height, Bitmap.Config.Argb8888!)!;
            
            // Get pixel data from SKBitmap
            var pixels = skBitmap.Pixels;
            var colors = new int[pixels.Length];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                var color = pixels[i];
                colors[i] = Color.Argb(color.Alpha, color.Red, color.Green, color.Blue);
            }
            
            androidBitmap.SetPixels(colors, 0, skBitmap.Width, 0, 0, skBitmap.Width, skBitmap.Height);
            return androidBitmap;
        }

        /// <summary>
        /// Converts an OxyColor to a SkiaSharp SKColor.
        /// </summary>
        /// <param name="oxyColor">The OxyColor to convert.</param>
        /// <returns>The equivalent SKColor.</returns>
        public static SKColor ToSKColor(this OxyColor oxyColor)
        {
            return new SKColor(oxyColor.R, oxyColor.G, oxyColor.B, oxyColor.A);
        }
    }

    /// <summary>
    /// Core plotting functionality using SkiaSharp that can be used in .NET Android
    /// </summary>
    internal class PlotCore
    {
        private PlotModel? _model;
        private PlotController? _controller;
        private SkiaRenderContext? _renderContext;
        private bool _isModelInvalidated = true;

        /// <summary>
        /// Event raised when the plot needs to be invalidated
        /// </summary>
        public event EventHandler? PlotInvalidated;

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

                    InvalidatePlot();
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
        /// Invalidate the plot (not blocking)
        /// </summary>
        /// <param name="updateData">if set to true, all data collections will be updated.</param>
        public void InvalidatePlot(bool updateData = true)
        {
            if (updateData)
            {
                _isModelInvalidated = true;
            }
            
            PlotInvalidated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the cursor type (not supported on mobile platforms).
        /// </summary>
        /// <param name="cursorType">The cursor type.</param>
        public void SetCursorType(CursorType cursorType)
        {
            // Not supported on Android
        }

        /// <summary>
        /// Renders the plot to the specified SkiaSharp canvas.
        /// </summary>
        /// <param name="canvas">The SkiaSharp canvas to render to.</param>
        /// <param name="width">The width of the canvas.</param>
        /// <param name="height">The height of the canvas.</param>
        /// <param name="dpiScale">The DPI scale factor.</param>
        public void RenderToCanvas(SKCanvas canvas, int width, int height, float dpiScale = 1.0f)
        {
            if (canvas == null || Model == null || width == 0 || height == 0)
                return;

            try
            {
                canvas.Clear(SKColors.White);

                if (_isModelInvalidated)
                {
                    Model.InvalidatePlot(true);
                    _isModelInvalidated = false;
                }

                // Create a proper render context
                _renderContext ??= new SkiaRenderContext();
                _renderContext.SkCanvas = canvas;
                _renderContext.DpiScale = dpiScale;

                // Simple rendering - just draw the plot title for now to verify it's working
                var paint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 20 * dpiScale,
                    IsAntialias = true,
                    FakeBoldText = true
                };

                // Draw the plot title
                if (!string.IsNullOrEmpty(Model.Title))
                {
                    canvas.DrawText(Model.Title, width / 2f, 30 * dpiScale, paint);
                }

                // Draw series data as simple visualization
                var seriesPaint = new SKPaint
                {
                    Color = SKColors.Blue,
                    StrokeWidth = 2 * dpiScale,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                };

                // Draw a simple representation of the series
                foreach (var series in Model.Series)
                {
                    if (series is LineSeries lineSeries)
                    {
                        DrawLineSeries(canvas, lineSeries, width, height, dpiScale);
                    }
                    else if (series is BarSeries barSeries)
                    {
                        DrawBarSeries(canvas, barSeries, width, height, dpiScale);
                    }
                    else if (series is ScatterSeries scatterSeries)
                    {
                        DrawScatterSeries(canvas, scatterSeries, width, height, dpiScale);
                    }
                    else if (series is PieSeries pieSeries)
                    {
                        DrawPieSeries(canvas, pieSeries, width, height, dpiScale);
                    }
                }
            }
            catch (Exception ex)
            {
                // Draw error message
                var paint = new SKPaint
                {
                    Color = SKColors.Red,
                    TextSize = 16 * dpiScale,
                    IsAntialias = true
                };
                
                canvas.DrawText($"Rendering error: {ex.Message}", 10, 50, paint);
            }
        }

        private void DrawLineSeries(SKCanvas canvas, LineSeries series, int width, int height, float dpiScale)
        {
            if (series.Points.Count == 0) return;

            var paint = new SKPaint
            {
                Color = series.Color.ToSKColor(),
                StrokeWidth = (float)series.StrokeThickness * dpiScale,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            // Simple mapping from data coordinates to screen coordinates
            var points = series.Points.ToList();
            if (points.Count < 2) return;

            var minX = points.Min(p => p.X);
            var maxX = points.Max(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxY = points.Max(p => p.Y);

            var margin = 50 * dpiScale;
            var plotWidth = width - 2 * margin;
            var plotHeight = height - 2 * margin;

            using var path = new SKPath();
            bool first = true;

            foreach (var point in points)
            {
                var x = margin + (float)((point.X - minX) / (maxX - minX) * plotWidth);
                var y = margin + (float)((maxY - point.Y) / (maxY - minY) * plotHeight);

                if (first)
                {
                    path.MoveTo(x, y);
                    first = false;
                }
                else
                {
                    path.LineTo(x, y);
                }
            }

            canvas.DrawPath(path, paint);
        }

        private void DrawBarSeries(SKCanvas canvas, BarSeries series, int width, int height, float dpiScale)
        {
            if (series.Items.Count == 0) return;

            var margin = 50 * dpiScale;
            var plotWidth = width - 2 * margin;
            var plotHeight = height - 2 * margin;

            var maxValue = series.Items.Max(item => item.Value);
            var barWidth = plotWidth / series.Items.Count * 0.8f;
            var spacing = plotWidth / series.Items.Count * 0.2f;

            for (int i = 0; i < series.Items.Count; i++)
            {
                var item = series.Items[i];
                var paint = new SKPaint
                {
                    Color = item.Color.ToSKColor(),
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };

                var x = margin + i * (barWidth + spacing);
                var barHeight = (float)(item.Value / maxValue * plotHeight);
                var y = margin + plotHeight - barHeight;

                var rect = new SKRect(x, y, x + barWidth, margin + plotHeight);
                canvas.DrawRect(rect, paint);
            }
        }

        private void DrawScatterSeries(SKCanvas canvas, ScatterSeries scatterSeries, int width, int height, float dpiScale)
        {
            if (scatterSeries.Points.Count == 0) return;

            var paint = new SKPaint
            {
                Color = scatterSeries.MarkerFill.ToSKColor(),
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            var strokePaint = new SKPaint
            {
                Color = scatterSeries.MarkerStroke.ToSKColor(),
                StrokeWidth = 1 * dpiScale,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            // Simple mapping from data coordinates to screen coordinates
            var points = scatterSeries.Points.ToList();
            if (points.Count == 0) return;

            var minX = points.Min(p => p.X);
            var maxX = points.Max(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxY = points.Max(p => p.Y);

            var margin = 50 * dpiScale;
            var plotWidth = width - 2 * margin;
            var plotHeight = height - 2 * margin;

            foreach (var point in points)
            {
                var x = margin + (float)((point.X - minX) / (maxX - minX) * plotWidth);
                var y = margin + (float)((maxY - point.Y) / (maxY - minY) * plotHeight);
                var radius = (float)(scatterSeries.MarkerSize * dpiScale / 2);

                canvas.DrawCircle(x, y, radius, paint);
                canvas.DrawCircle(x, y, radius, strokePaint);
            }
        }

        private void DrawPieSeries(SKCanvas canvas, PieSeries pieSeries, int width, int height, float dpiScale)
        {
            if (pieSeries.Slices.Count == 0) return;

            var centerX = width / 2f;
            var centerY = height / 2f;
            var radius = Math.Min(width, height) * 0.3f;

            var total = pieSeries.Slices.Sum(slice => slice.Value);
            var startAngle = -90f; // Start at top

            foreach (var slice in pieSeries.Slices)
            {
                var paint = new SKPaint
                {
                    Color = slice.Fill.ToSKColor(),
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };

                var strokePaint = new SKPaint
                {
                    Color = SKColors.White,
                    StrokeWidth = 2 * dpiScale,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                };

                var sweepAngle = (float)(slice.Value / total * 360);
                
                var rect = new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);
                
                using var path = new SKPath();
                path.MoveTo(centerX, centerY);
                path.ArcTo(rect, startAngle, sweepAngle, false);
                path.Close();

                canvas.DrawPath(path, paint);
                canvas.DrawPath(path, strokePaint);

                startAngle += sweepAngle;
            }
        }

        /// <summary>
        /// Handles touch events for panning and zooming.
        /// </summary>
        /// <param name="view">The plot view.</param>
        /// <param name="touchEventArgs">The touch event arguments.</param>
        public void HandleTouchEvent(IPlotView view, OxyTouchEventArgs touchEventArgs)
        {
            var controller = ActualController;
            if (controller == null)
                return;

            // Handle different types of touch events
            if (touchEventArgs.DeltaScale.X != 1.0 || touchEventArgs.DeltaScale.Y != 1.0)
            {
                // Pinch gesture
                controller.HandleTouchDelta(view, touchEventArgs);
            }
            else if (touchEventArgs.DeltaTranslation.X != 0 || touchEventArgs.DeltaTranslation.Y != 0)
            {
                // Pan gesture
                controller.HandleTouchDelta(view, touchEventArgs);
            }
            else
            {
                // Tap or touch down/up
                controller.HandleTouchStarted(view, touchEventArgs);
            }
        }

        /// <summary>
        /// Called when the model is updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnModelUpdated(object? sender, EventArgs e)
        {
            InvalidatePlot();
        }

        /// <summary>
        /// Gets the client area rectangle for the given dimensions.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The client area rectangle.</returns>
        public OxyRect GetClientArea(int width, int height) => new(0, 0, width, height);
    }
}