using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Sample.OxyPlot.Android;

/// <summary>
/// Helper class to create various chart demonstrations
/// </summary>
public static class ChartDemos
{
    /// <summary>
    /// Creates a real-time data simulation chart
    /// </summary>
    public static PlotModel CreateRealTimeChart()
    {
        var model = new PlotModel 
        { 
            Title = "Real-time Data Simulation",
            Background = OxyColors.LightGray
        };

        model.Axes.Add(new LinearAxis 
        { 
            Position = AxisPosition.Bottom, 
            Title = "Time (seconds)",
            Minimum = 0,
            Maximum = 10
        });
        
        model.Axes.Add(new LinearAxis 
        { 
            Position = AxisPosition.Left, 
            Title = "Value",
            Minimum = -2,
            Maximum = 2
        });

        var series = new LineSeries
        {
            Title = "Sensor Data",
            Color = OxyColors.Green,
            StrokeThickness = 2,
            TrackerFormatString = "Time: {2:0.##}s\nValue: {4:0.###}"
        };

        // Simulate some real-time data
        var random = new Random();
        for (int i = 0; i <= 100; i++)
        {
            double time = i * 0.1;
            double value = Math.Sin(time * 2) + (random.NextDouble() - 0.5) * 0.3;
            series.Points.Add(new DataPoint(time, value));
        }

        model.Series.Add(series);
        return model;
    }

    /// <summary>
    /// Creates a simple line chart used by the sample app
    /// </summary>
    public static PlotModel CreateLineChart()
    {
        var model = new PlotModel
        {
            Title = "Line Chart Example",
            Background = OxyColors.LightGray
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

        return model;
    }

    /// <summary>
    /// Creates a bar chart used by the sample app
    /// </summary>
    public static PlotModel CreateBarChart()
    {
        var model = new PlotModel
        {
            Title = "Bar Chart Example",
            Background = OxyColors.LightGray
        };

        var categoryAxis = new CategoryAxis
        {
            Position = AxisPosition.Left,
            ItemsSource = new[] { "Category A", "Category B", "Category C", "Category D", "Category E" }
        };

        var valueAxis = new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Minimum = 0,
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
        return model;
    }

    /// <summary>
    /// Creates a scatter plot used by the sample app
    /// </summary>
    public static PlotModel CreateScatterPlot()
    {
        var model = new PlotModel
        {
            Title = "Scatter Plot Example",
            Background = OxyColors.LightGray
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
        return model;
    }

    /// <summary>
    /// Creates a pie chart used by the sample app
    /// </summary>
    public static PlotModel CreatePieChart()
    {
        var model = new PlotModel
        {
            Title = "Pie Chart Example",
            Background = OxyColors.LightGray
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
        return model;
    }

    /// <summary>
    /// Creates a heatmap demonstration
    /// </summary>
    public static PlotModel CreateHeatMapChart()
    {
        var model = new PlotModel
        {
            Title = "Heat Map Example",
            Background = OxyColors.LightGray
        };

        // Standard axes for coordinates
        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = "X",
            Minimum = 0,
            Maximum = 1
        });
        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "Y",
            Minimum = 0,
            Maximum = 1
        });

        // Color axis (legend)
        model.Axes.Add(new LinearColorAxis
        {
            Position = AxisPosition.Right,
            Palette = OxyPalettes.Jet(300),
            Title = "Value"
        });

        // Create a larger sample grid for a nicer demo
        const int nx = 60;
        const int ny = 40;
        var data = new double[nx, ny];

        var rnd = new Random(0);
        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                double x = (double)i / (nx - 1);
                double y = (double)j / (ny - 1);
                // Smooth pattern with some noise
                data[i, j] = Math.Sin(x * Math.PI * 4) * Math.Cos(y * Math.PI * 3) + (rnd.NextDouble() - 0.5) * 0.2;
            }
        }

        var heatMapSeries = new HeatMapSeries
        {
            X0 = 0,
            X1 = 1,
            Y0 = 0,
            Y1 = 1,
            Interpolate = true,
            RenderMethod = HeatMapRenderMethod.Bitmap,
            Data = data
        };

        model.Series.Add(heatMapSeries);
        return model;
    }

    /// <summary>
    /// Creates a financial/candlestick chart
    /// </summary>
    public static PlotModel CreateFinancialChart()
    {
        var model = new PlotModel 
        { 
            Title = "Candlestick Chart",
            Background = OxyColors.LightGray
        };

        model.Axes.Add(new LinearAxis 
        { 
            Position = AxisPosition.Bottom, 
            Title = "Days"
        });
        
        model.Axes.Add(new LinearAxis 
        { 
            Position = AxisPosition.Left, 
            Title = "Price ($)"
        });

        var candleSeries = new CandleStickSeries
        {
            Title = "Stock Price",
            IncreasingColor = OxyColors.Green,
            DecreasingColor = OxyColors.Red,
            TrackerFormatString = "Day: {1}\nOpen: ${2:0.##}\nHigh: ${3:0.##}\nLow: ${4:0.##}\nClose: ${5:0.##}"
        };

        // Generate sample financial data
        var random = new Random();
        double price = 100.0;
        
        for (int i = 0; i < 30; i++)
        {
            double open = price;
            double change = (random.NextDouble() - 0.5) * 10;
            double close = Math.Max(1, open + change);
            double high = Math.Max(open, close) + random.NextDouble() * 5;
            double low = Math.Min(open, close) - random.NextDouble() * 5;
            
            candleSeries.Items.Add(new HighLowItem(i, high, low, open, close));
            price = close;
        }

        model.Series.Add(candleSeries);
        return model;
    }

    /// <summary>
    /// Creates a multi-series chart with different types
    /// </summary>
    public static PlotModel CreateMultiSeriesChart()
    {
        var model = new PlotModel 
        { 
            Title = "Multi-Series Chart",
            Background = OxyColors.LightGray
        };

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

        // Add a line series
        var lineSeries = new LineSeries
        {
            Title = "Trend Line",
            Color = OxyColors.Blue,
            StrokeThickness = 2
        };

        // Add an area series
        var areaSeries = new AreaSeries
        {
            Title = "Area Data",
            Color = OxyColor.FromArgb(80, 255, 0, 0),
            Fill = OxyColor.FromArgb(80, 255, 0, 0)
        };

        // Add data to both series
        for (int i = 0; i <= 20; i++)
        {
            double x = i;
            double lineValue = Math.Sin(x * 0.3) * 5 + 10;
            double areaValue = Math.Cos(x * 0.2) * 3 + 5;
            
            lineSeries.Points.Add(new DataPoint(x, lineValue));
            areaSeries.Points.Add(new DataPoint(x, areaValue));
        }

        model.Series.Add(areaSeries);
        model.Series.Add(lineSeries);

        return model;
    }
}