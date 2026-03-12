using AndroidX.AppCompat.App;
using OxyPlot.DotNetAndroid;

namespace Sample.OxyPlot.Android;

[Activity(Label = "OxyPlot Android Sample", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    private PlotView? _plotView;
    private Button? _lineChartButton;
    private Button? _barChartButton;
    private Button? _scatterPlotButton;
    private Button? _pieChartButton;
    private Button? _realTimeButton;
    private Button? _multiSeriesButton;
    private Button? _heatMapButton;
    private Button? _financialButton;
    private LinearLayout? _mainLayout;
    private ScrollView? _buttonScrollView;
    private LinearLayout? _buttonLayout;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Create the main layout with top padding to account for action bar
        _mainLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent,
                LinearLayout.LayoutParams.MatchParent)
        };
        
        // Set black background to better see the plot area
        _mainLayout.SetBackgroundColor(global::Android.Graphics.Color.Black);
        
        // Add top padding to push content below the action bar and status bar
        _mainLayout.SetPadding(0, GetActionBarHeight() + GetStatusBarHeight(), 0, 0);

        // Create scrollable button layout
        _buttonScrollView = new ScrollView(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent,
                LinearLayout.LayoutParams.WrapContent)
            {
                BottomMargin = 16 // Add margin below buttons
            }
        };

        _buttonLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical, // Changed to vertical to stack rows
            LayoutParameters = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent, // Changed to match parent width
                LinearLayout.LayoutParams.WrapContent)
        };
        
        // Add padding to button layout
        _buttonLayout.SetPadding(16, 16, 16, 16);

        // Create chart selection buttons
        CreateButtons();

        // Create the plot view
        _plotView = new PlotView(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent,
                0, 1.0f) // weight = 1 to fill remaining space
            {
                TopMargin = 8 // Add small margin above plot
            }
        };

        // Add views to layouts
        _buttonScrollView.AddView(_buttonLayout);
        _mainLayout.AddView(_buttonScrollView);
        _mainLayout.AddView(_plotView);

        // Set the content view
        SetContentView(_mainLayout);

        // Show initial chart
        ShowLineChart();
    }

    private int GetActionBarHeight()
    {
        var tv = new global::Android.Util.TypedValue();
        if (Theme.ResolveAttribute(global::Android.Resource.Attribute.ActionBarSize, tv, true))
        {
            return global::Android.Util.TypedValue.ComplexToDimensionPixelSize(tv.Data, Resources!.DisplayMetrics);
        }
        return 0;
    }

    private int GetStatusBarHeight()
    {
        int result = 0;
        int resourceId = Resources!.GetIdentifier("status_bar_height", "dimen", "android");
        if (resourceId > 0)
        {
            result = Resources.GetDimensionPixelSize(resourceId);
        }
        return result;
    }

    private void CreateButtons()
    {
        _lineChartButton = new Button(this)
        {
            Text = "LINE CHART"
        };
        _lineChartButton.Click += (s, e) => ShowLineChart();
        // Prevent ScrollView from auto-scrolling when buttons receive focus
        _lineChartButton.Focusable = false;
        _lineChartButton.FocusableInTouchMode = false;

        _barChartButton = new Button(this)
        {
            Text = "BAR CHART"
        };
        _barChartButton.Click += (s, e) => ShowBarChart();
        _barChartButton.Focusable = false;
        _barChartButton.FocusableInTouchMode = false;

        _scatterPlotButton = new Button(this)
        {
            Text = "SCATTER PLOT"
        };
        _scatterPlotButton.Click += (s, e) => ShowScatterPlot();
        _scatterPlotButton.Focusable = false;
        _scatterPlotButton.FocusableInTouchMode = false;

        _pieChartButton = new Button(this)
        {
            Text = "PIE CHART"
        };
        _pieChartButton.Click += (s, e) => ShowPieChart();
        _pieChartButton.Focusable = false;
        _pieChartButton.FocusableInTouchMode = false;

        _heatMapButton = new Button(this)
        {
            Text = "HEAT MAP"
        };
        _heatMapButton.Click += (s, e) => ShowHeatMapChart();
        _heatMapButton.Focusable = false;
        _heatMapButton.FocusableInTouchMode = false;

        _financialButton = new Button(this)
        {
            Text = "FINANCIAL"
        };
        _financialButton.Click += (s, e) => ShowFinancialChart();
        _financialButton.Focusable = false;
        _financialButton.FocusableInTouchMode = false;

        _realTimeButton = new Button(this)
        {
            Text = "REAL-TIME"
        };
        _realTimeButton.Click += (s, e) => ShowRealTimeChart();
        _realTimeButton.Focusable = false;
        _realTimeButton.FocusableInTouchMode = false;

        _multiSeriesButton = new Button(this)
        {
            Text = "MULTI-SERIES"
        };
        _multiSeriesButton.Click += (s, e) => ShowMultiSeriesChart();
        _multiSeriesButton.Focusable = false;
        _multiSeriesButton.FocusableInTouchMode = false;

        // Create list of all buttons
        var buttons = new[] { _lineChartButton, _barChartButton, _scatterPlotButton,
                             _pieChartButton, _heatMapButton, _realTimeButton,
                             _financialButton, _multiSeriesButton };

        // Add buttons to rows with wrapping
        AddButtonsWithWrapping(buttons);

        // Ensure ScrollView is at the top after layout is created
        _buttonScrollView?.Post(() => _buttonScrollView.ScrollTo(0, 0));
    }

    private void AddButtonsWithWrapping(Button[] buttons)
    {
        // Get screen width
            var displayMetrics = Resources?.DisplayMetrics;
            // Convert to density-independent pixels (dp) for consistent breakpoints
            var screenWidthDp = displayMetrics != null ? displayMetrics.WidthPixels / displayMetrics.Density : 360f;

            // Choose sensible breakpoints in dp
            int maxButtonsPerRow = screenWidthDp < 480f ? 2 : (screenWidthDp < 800f ? 3 : 6);

            // Make sure the ScrollView will fill available viewport when appropriate
            _buttonScrollView!.FillViewport = true;

        LinearLayout? currentRow = null;
        int buttonsInCurrentRow = 0;

        foreach (var button in buttons)
        {
            // Create new row if needed
            if (currentRow == null || buttonsInCurrentRow >= maxButtonsPerRow)
            {
                currentRow = new LinearLayout(this)
                {
                    Orientation = Orientation.Horizontal,
                    LayoutParameters = new LinearLayout.LayoutParams(
                        LinearLayout.LayoutParams.MatchParent,
                        LinearLayout.LayoutParams.WrapContent)
                    {
                        BottomMargin = 8 // Small margin between rows
                    }
                };
                
                // Center the buttons in each row
                currentRow.SetGravity(global::Android.Views.GravityFlags.CenterHorizontal);
                
                _buttonLayout?.AddView(currentRow);
                buttonsInCurrentRow = 0;
            }

            // Distribute buttons evenly across the row. Use weight so they resize to fit.
            var buttonParams = new LinearLayout.LayoutParams(
                0,
                LinearLayout.LayoutParams.WrapContent,
                1.0f)
            {
                RightMargin = 8,
                LeftMargin = 8
            };

            button.LayoutParameters = buttonParams;
            currentRow.AddView(button);
            buttonsInCurrentRow++;
        }
    }

    private void ShowLineChart()
    {
        _plotView!.Model = ChartDemos.CreateLineChart();
        _plotView.Invalidate(); // Force redraw
    }

    private void ShowBarChart()
    {
        _plotView!.Model = ChartDemos.CreateBarChart();
        _plotView.Invalidate(); // Force redraw
    }

    private void ShowScatterPlot()
    {
        _plotView!.Model = ChartDemos.CreateScatterPlot();
        _plotView.Invalidate(); // Force redraw
    }

    private void ShowPieChart()
    {
        _plotView!.Model = ChartDemos.CreatePieChart();
        _plotView.Invalidate(); // Force redraw
    }

    private void ShowHeatMapChart()
    {
        _plotView!.Model = ChartDemos.CreateHeatMapChart();
        _plotView.Invalidate(); // Force redraw
    }

    private void ShowFinancialChart()
    {
        _plotView!.Model = ChartDemos.CreateFinancialChart();
        _plotView.Invalidate(); // Force redraw
    }

    private void ShowRealTimeChart()
    {
        _plotView!.Model = ChartDemos.CreateRealTimeChart();
        _plotView.Invalidate(); // Force redraw
    }

    private void ShowMultiSeriesChart()
    {
        _plotView!.Model = ChartDemos.CreateMultiSeriesChart();
        _plotView.Invalidate(); // Force redraw
    }
}