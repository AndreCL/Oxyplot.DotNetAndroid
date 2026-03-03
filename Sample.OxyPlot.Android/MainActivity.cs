using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.DotNetAndroid;
using OxyPlot.Series;

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
        
        // Add top padding to push content below the action bar
        _mainLayout.SetPadding(0, GetActionBarHeight(), 0, 0);

        // Create scrollable button layout
        _buttonScrollView = new ScrollView(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent,
                LinearLayout.LayoutParams.WrapContent)
        };

        _buttonLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            LayoutParameters = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.WrapContent,
                LinearLayout.LayoutParams.WrapContent)
        };

        // Create chart selection buttons
        CreateButtons();

        // Create the plot view
        _plotView = new PlotView(this)
        {
            LayoutParameters = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent,
                0, 1.0f) // weight = 1 to fill remaining space
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

    private void CreateButtons()
    {
        var buttonParams = new LinearLayout.LayoutParams(
            LinearLayout.LayoutParams.WrapContent, 
            LinearLayout.LayoutParams.WrapContent)
        {
            RightMargin = 10
        };

        _lineChartButton = new Button(this)
        {
            Text = "Line Chart",
            LayoutParameters = buttonParams
        };
        _lineChartButton.Click += (s, e) => ShowLineChart();

        _barChartButton = new Button(this)
        {
            Text = "Bar Chart",
            LayoutParameters = buttonParams
        };
        _barChartButton.Click += (s, e) => ShowBarChart();

        _scatterPlotButton = new Button(this)
        {
            Text = "Scatter Plot",
            LayoutParameters = buttonParams
        };
        _scatterPlotButton.Click += (s, e) => ShowScatterPlot();

        _pieChartButton = new Button(this)
        {
            Text = "Pie Chart",
            LayoutParameters = buttonParams
        };
        _pieChartButton.Click += (s, e) => ShowPieChart();

        _realTimeButton = new Button(this)
        {
            Text = "Real-time",
            LayoutParameters = buttonParams
        };
        _realTimeButton.Click += (s, e) => ShowRealTimeChart();

        _multiSeriesButton = new Button(this)
        {
            Text = "Multi-Series",
            LayoutParameters = buttonParams
        };
        _multiSeriesButton.Click += (s, e) => ShowMultiSeriesChart();

        _buttonLayout?.AddView(_lineChartButton);
        _buttonLayout?.AddView(_barChartButton);
        _buttonLayout?.AddView(_scatterPlotButton);
        _buttonLayout?.AddView(_pieChartButton);
        _buttonLayout?.AddView(_realTimeButton);
        _buttonLayout?.AddView(_multiSeriesButton);
    }

    private void ShowLineChart()
    {
        var model = new PlotModel 
        { 
            Title = "Line Chart Example",
            Background = OxyColors.White
        };

        // Add axes
        model.Axes.Add(new LinearAxis 
        { 
            Position = AxisPosition.Bottom, 
            Title = "X Axis" 
        });
        model.Axes.Add(new LinearAxis 
        { 
            Position = AxisPosition.Left, 
            Title = "Y Axis" 
        });

        // Create line series
        var lineSeries1 = new LineSeries
        {
            Title = "Sine Wave",
            Color = OxyColors.Blue,
            StrokeThickness = 2
        };

        var lineSeries2 = new LineSeries
        {
            Title = "Cosine Wave",
            Color = OxyColors.Red,
            StrokeThickness = 2
        };

        // Add data points
        for (int i = 0; i < 100; i++)
        {
            double x = i * 0.1;
            lineSeries1.Points.Add(new DataPoint(x, Math.Sin(x)));
            lineSeries2.Points.Add(new DataPoint(x, Math.Cos(x)));
        }

        model.Series.Add(lineSeries1);
        model.Series.Add(lineSeries2);

        _plotView!.Model = model;
        _plotView.Invalidate(); // Force redraw
    }

    private void ShowBarChart()
    {
        var model = new PlotModel 
        { 
            Title = "Bar Chart Example",
            Background = OxyColors.White
        };

        var categoryAxis = new CategoryAxis 
        { 
            Position = AxisPosition.Left,
            ItemsSource = new[] { "Category A", "Category B", "Category C", "Category D", "Category E" }
        };
        
        var valueAxis = new LinearAxis 
        { 
            Position = AxisPosition.Bottom, 
            MinimumPadding = 0.1, 
            MaximumPadding = 0.1 
        };

        model.Axes.Add(categoryAxis);
        model.Axes.Add(valueAxis);

        var barSeries = new BarSeries
        {
            Title = "Sample Data",
            FillColor = OxyColors.SkyBlue,
            StrokeThickness = 1,
            StrokeColor = OxyColors.Black
        };

        barSeries.Items.Add(new BarItem { Value = 25, Color = OxyColors.Red });
        barSeries.Items.Add(new BarItem { Value = 35, Color = OxyColors.Green });
        barSeries.Items.Add(new BarItem { Value = 45, Color = OxyColors.Blue });
        barSeries.Items.Add(new BarItem { Value = 20, Color = OxyColors.Orange });
        barSeries.Items.Add(new BarItem { Value = 30, Color = OxyColors.Purple });

        model.Series.Add(barSeries);
        _plotView!.Model = model;
        _plotView.Invalidate(); // Force redraw
    }

    private void ShowScatterPlot()
    {
        var model = new PlotModel 
        { 
            Title = "Scatter Plot Example",
            Background = OxyColors.White
        };

        model.Axes.Add(new LinearAxis 
        { 
            Position = AxisPosition.Bottom, 
            Title = "X Values" 
        });
        model.Axes.Add(new LinearAxis 
        { 
            Position = AxisPosition.Left, 
            Title = "Y Values" 
        });

        var scatterSeries = new ScatterSeries
        {
            Title = "Random Data",
            MarkerType = MarkerType.Circle,
            MarkerSize = 5,
            MarkerFill = OxyColors.Red,
            MarkerStroke = OxyColors.Black
        };

        var random = new Random();
        for (int i = 0; i < 50; i++)
        {
            scatterSeries.Points.Add(new ScatterPoint(
                random.NextDouble() * 10, 
                random.NextDouble() * 10));
        }

        model.Series.Add(scatterSeries);
        _plotView!.Model = model;
        _plotView.Invalidate(); // Force redraw
    }

    private void ShowPieChart()
    {
        var model = new PlotModel 
        { 
            Title = "Pie Chart Example",
            Background = OxyColors.White
        };

        var pieSeries = new PieSeries
        {
            StrokeThickness = 2.0,
            InsideLabelPosition = 0.8,
            AngleSpan = 360,
            StartAngle = 0
        };

        pieSeries.Slices.Add(new PieSlice("Category 1", 40) 
        { 
            IsExploded = false, 
            Fill = OxyColors.Red 
        });
        pieSeries.Slices.Add(new PieSlice("Category 2", 30) 
        { 
            IsExploded = true, 
            Fill = OxyColors.Green 
        });
        pieSeries.Slices.Add(new PieSlice("Category 3", 20) 
        { 
            IsExploded = false, 
            Fill = OxyColors.Blue 
        });
        pieSeries.Slices.Add(new PieSlice("Category 4", 10) 
        { 
            IsExploded = false, 
            Fill = OxyColors.Orange 
        });

        model.Series.Add(pieSeries);
        _plotView!.Model = model;
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