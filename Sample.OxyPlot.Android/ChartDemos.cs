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
            Background = OxyColors.White
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
    /// Creates a heatmap demonstration
    /// </summary>
    public static PlotModel CreateHeatMapChart()
    {
        var model = new PlotModel 
        { 
            Title = "Heat Map Example",
            Background = OxyColors.White
        };

        model.Axes.Add(new LinearColorAxis 
        { 
            Position = AxisPosition.Right,
            Palette = OxyPalettes.Jet(500)
        });

        var heatMapSeries = new HeatMapSeries
        {
            X0 = 0,
            X1 = 3,
            Y0 = 0,
            Y1 = 3,
            Interpolate = true,
            RenderMethod = HeatMapRenderMethod.Bitmap
        };

        // Create sample data
        var data = new double[4, 4];
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                data[x, y] = Math.Sin(x * 0.5) * Math.Cos(y * 0.5) + 
                             (new Random().NextDouble() - 0.5) * 0.2;
            }
        }

        heatMapSeries.Data = data;
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
            Background = OxyColors.White
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
            Background = OxyColors.White
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