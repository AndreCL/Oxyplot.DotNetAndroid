using Android.Views;

namespace OxyPlot.DotNetAndroid;

/// <summary>
/// Handles touch events for the plot view
/// </summary>
internal class TouchHandler
{
    private readonly PlotView _plotView;
    private readonly Dictionary<int, ScreenPoint> _previousTouches = new();
    private bool _isPinching;
    private float _initialDistance;
    private ScreenPoint _initialCenter;

    /// <summary>
    /// Initializes a new instance of the <see cref="TouchHandler"/> class.
    /// </summary>
    /// <param name="plotView">The plot view.</param>
    public TouchHandler(PlotView plotView)
    {
        _plotView = plotView;
    }

    /// <summary>
    /// Handles the touch event.
    /// </summary>
    /// <param name="e">The motion event.</param>
    public void HandleTouchEvent(MotionEvent e)
    {
        var controller = _plotView.ActualController;
        var model = _plotView.ActualModel;
        
        if (controller == null || model == null)
            return;

        switch (e.ActionMasked)
        {
            case MotionEventActions.Down:
                HandleTouchDown(e, controller);
                break;
                
            case MotionEventActions.PointerDown:
                HandlePointerDown(e, controller);
                break;
                
            case MotionEventActions.Move:
                HandleTouchMove(e, controller);
                break;
                
            case MotionEventActions.Up:
            case MotionEventActions.PointerUp:
                HandleTouchUp(e, controller);
                break;
                
            case MotionEventActions.Cancel:
                HandleTouchCancel(controller);
                break;
        }
    }

    /// <summary>
    /// Handles the touch down event.
    /// </summary>
    /// <param name="e">The motion event.</param>
    /// <param name="controller">The plot controller.</param>
    private void HandleTouchDown(MotionEvent e, IPlotController controller)
    {
        var pointerId = e.GetPointerId(0);
        var screenPoint = new ScreenPoint(e.GetX(0), e.GetY(0));
        
        _previousTouches[pointerId] = screenPoint;
        _isPinching = false;

        // Handle touch down
        var args = new OxyTouchEventArgs
        {
            Position = screenPoint,
            DeltaTranslation = new ScreenVector(0, 0),
            DeltaScale = new ScreenVector(1, 1)
        };

        controller.HandleTouchStarted(_plotView, args);
    }

    /// <summary>
    /// Handles the pointer down event (multi-touch).
    /// </summary>
    /// <param name="e">The motion event.</param>
    /// <param name="controller">The plot controller.</param>
    private void HandlePointerDown(MotionEvent e, IPlotController controller)
    {
        if (e.PointerCount >= 2)
        {
            // Start pinch gesture
            _isPinching = true;
            
            var pointer1 = new ScreenPoint(e.GetX(0), e.GetY(0));
            var pointer2 = new ScreenPoint(e.GetX(1), e.GetY(1));
            
            _initialDistance = GetDistance(pointer1, pointer2);
            _initialCenter = GetCenter(pointer1, pointer2);
            
            // Store all current touches
            _previousTouches.Clear();
            for (int i = 0; i < e.PointerCount; i++)
            {
                var pointerId = e.GetPointerId(i);
                var screenPoint = new ScreenPoint(e.GetX(i), e.GetY(i));
                _previousTouches[pointerId] = screenPoint;
            }
        }
    }

    /// <summary>
    /// Handles the touch move event.
    /// </summary>
    /// <param name="e">The motion event.</param>
    /// <param name="controller">The plot controller.</param>
    private void HandleTouchMove(MotionEvent e, IPlotController controller)
    {
        if (_isPinching && e.PointerCount >= 2)
        {
            HandlePinchGesture(e, controller);
        }
        else if (e.PointerCount == 1)
        {
            HandlePanGesture(e, controller);
        }
    }

    /// <summary>
    /// Handles the pinch gesture.
    /// </summary>
    /// <param name="e">The motion event.</param>
    /// <param name="controller">The plot controller.</param>
    private void HandlePinchGesture(MotionEvent e, IPlotController controller)
    {
        var pointer1 = new ScreenPoint(e.GetX(0), e.GetY(0));
        var pointer2 = new ScreenPoint(e.GetX(1), e.GetY(1));
        
        var currentDistance = GetDistance(pointer1, pointer2);
        var currentCenter = GetCenter(pointer1, pointer2);
        
        var scale = currentDistance / _initialDistance;
        var deltaTranslation = currentCenter - _initialCenter;

        var args = new OxyTouchEventArgs
        {
            Position = _initialCenter,
            DeltaTranslation = deltaTranslation,
            DeltaScale = new ScreenVector(scale, scale)
        };

        controller.HandleTouchDelta(_plotView, args);
    }

    /// <summary>
    /// Handles the pan gesture.
    /// </summary>
    /// <param name="e">The motion event.</param>
    /// <param name="controller">The plot controller.</param>
    private void HandlePanGesture(MotionEvent e, IPlotController controller)
    {
        var pointerId = e.GetPointerId(0);
        var currentPoint = new ScreenPoint(e.GetX(0), e.GetY(0));
        
        if (_previousTouches.TryGetValue(pointerId, out var previousPoint))
        {
            var deltaTranslation = currentPoint - previousPoint;
            
            var args = new OxyTouchEventArgs
            {
                Position = currentPoint,
                DeltaTranslation = deltaTranslation,
                DeltaScale = new ScreenVector(1, 1)
            };

            controller.HandleTouchDelta(_plotView, args);
            _previousTouches[pointerId] = currentPoint;
        }
    }

    /// <summary>
    /// Handles the touch up event.
    /// </summary>
    /// <param name="e">The motion event.</param>
    /// <param name="controller">The plot controller.</param>
    private void HandleTouchUp(MotionEvent e, IPlotController controller)
    {
        var actionIndex = e.ActionIndex;
        var pointerId = e.GetPointerId(actionIndex);
        
        // Remove the released pointer
        _previousTouches.Remove(pointerId);
        
        if (e.PointerCount <= 2)
        {
            _isPinching = false;
        }

        // If this was the last finger, end the touch
        if (e.PointerCount == 1)
        {
            var screenPoint = new ScreenPoint(e.GetX(actionIndex), e.GetY(actionIndex));
            
            var args = new OxyTouchEventArgs
            {
                Position = screenPoint,
                DeltaTranslation = new ScreenVector(0, 0),
                DeltaScale = new ScreenVector(1, 1)
            };

            controller.HandleTouchCompleted(_plotView, args);
        }
    }

    /// <summary>
    /// Handles the touch cancel event.
    /// </summary>
    /// <param name="controller">The plot controller.</param>
    private void HandleTouchCancel(IPlotController controller)
    {
        _previousTouches.Clear();
        _isPinching = false;
        
        // Cancel the current operation
        controller.HandleTouchCompleted(_plotView, new OxyTouchEventArgs());
    }

    /// <summary>
    /// Gets the distance between two points.
    /// </summary>
    /// <param name="point1">The first point.</param>
    /// <param name="point2">The second point.</param>
    /// <returns>The distance.</returns>
    private static float GetDistance(ScreenPoint point1, ScreenPoint point2)
    {
        var dx = point2.X - point1.X;
        var dy = point2.Y - point1.Y;
        return (float)Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Gets the center point between two points.
    /// </summary>
    /// <param name="point1">The first point.</param>
    /// <param name="point2">The second point.</param>
    /// <returns>The center point.</returns>
    private static ScreenPoint GetCenter(ScreenPoint point1, ScreenPoint point2)
    {
        return new ScreenPoint((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
    }
}