using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace OxyPlot.DotNetAndroid;

/// <summary>
/// A view that displays tracker information
/// </summary>
internal class TrackerView : FrameLayout
{
    private readonly TextView _textView;
    private readonly View _backgroundView;
    private bool _isVisible;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackerView"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public TrackerView(Context context) : base(context)
    {
        // Create background
        _backgroundView = new View(context)
        {
            Background = CreateBackgroundDrawable()
        };
        AddView(_backgroundView);

        // Create text view
        _textView = new TextView(context)
        {
            TextSize = 12,
            Gravity = GravityFlags.Center
        };
        _textView.SetTextColor(Color.Black);
        _textView.SetPadding(
            (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 8, context.Resources!.DisplayMetrics),
            (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 4, context.Resources.DisplayMetrics),
            (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 8, context.Resources.DisplayMetrics),
            (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 4, context.Resources.DisplayMetrics)
        );
        AddView(_textView);

        Visibility = ViewStates.Gone;
    }

    /// <summary>
    /// Shows the tracker with the specified text at the given position.
    /// </summary>
    /// <param name="text">The text to show.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    public void ShowTracker(string text, float x, float y)
    {
        _textView.Text = text;
        
        // Measure the text
        _textView.Measure(
            MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified),
            MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified)
        );
        
        var width = _textView.MeasuredWidth;
        var height = _textView.MeasuredHeight;
        
        // Position the tracker, ensuring it stays on screen
        var parentView = (View?)Parent;
        if (parentView != null)
        {
            var parentWidth = parentView.Width;
            var parentHeight = parentView.Height;
            
            // Adjust position to keep tracker on screen
            if (x + width > parentWidth)
                x = parentWidth - width;
            if (x < 0)
                x = 0;
                
            if (y - height < 0)
                y = height;
            if (y > parentHeight)
                y = parentHeight;
        }
        
        // Set layout parameters
        var layoutParams = new FrameLayout.LayoutParams(width, height)
        {
            LeftMargin = (int)(x - width / 2),
            TopMargin = (int)(y - height)
        };
        LayoutParameters = layoutParams;
        
        Visibility = ViewStates.Visible;
        _isVisible = true;
    }

    /// <summary>
    /// Hides the tracker.
    /// </summary>
    public void HideTracker()
    {
        Visibility = ViewStates.Gone;
        _isVisible = false;
    }

    /// <summary>
    /// Gets a value indicating whether the tracker is visible.
    /// </summary>
    public bool IsVisible => _isVisible;

    /// <summary>
    /// Creates the background drawable.
    /// </summary>
    /// <returns>The background drawable.</returns>
    private Android.Graphics.Drawables.Drawable CreateBackgroundDrawable()
    {
        var drawable = new Android.Graphics.Drawables.GradientDrawable();
        drawable.SetShape(Android.Graphics.Drawables.ShapeType.Rectangle);
        drawable.SetColor(Color.White);
        drawable.SetStroke(2, Color.Gray);
        drawable.SetCornerRadius(8);
        return drawable;
    }
}