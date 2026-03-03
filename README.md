# OxyPlot.DotNetAndroid

A modern, high-performance charting library for .NET Android applications, providing native OxyPlot integration with SkiaSharp rendering.

[![.NET](https://img.shields.io/badge/.NET-10-blue.svg)](https://dotnet.microsoft.com/download)
[![Android](https://img.shields.io/badge/Android-API%2024+-green.svg)](https://developer.android.com/about/versions/nougat/android-7.0)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Features

 **Comprehensive Chart Types**
- Line charts with multiple series support
- Bar charts (horizontal and vertical)
- Scatter plots with customizable markers
- Pie charts with exploding slices
- Real-time data visualization
- Multi-series charts

**Performance**
- Hardware-accelerated rendering via SkiaSharp
- Smooth 60fps animations and interactions
- Optimized for mobile devices
- Memory efficient

**Mobile-First**
- Native Android touch gestures (pan, zoom, pinch)
- Responsive design for various screen sizes
- High-DPI support
- Touch-friendly interactive elements

**Customizable**
- Extensive styling options
- Theme support (Light/Dark)
- Custom colors, fonts, and layouts
- Configurable axes and legends

## Installation

### Package Manager Console
```powershell
Install-Package OxyPlot.DotNetAndroid
```

### .NET CLI
```bash
dotnet add package OxyPlot.DotNetAndroid
```

### PackageReference
```xml
<PackageReference Include="OxyPlot.DotNetAndroid" Version="1.0.0" />
```

## Quick Start

### 1. Basic Setup

Add the PlotView to your Android layout or create it programmatically:

```csharp
using OxyPlot.DotNetAndroid;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

// Create PlotView
var plotView = new PlotView(this)
{
    LayoutParameters = new LinearLayout.LayoutParams(
        LinearLayout.LayoutParams.MatchParent,
        LinearLayout.LayoutParams.MatchParent)
};

// Add to your layout
yourLayout.AddView(plotView);
```

### 2. Create Your First Chart

```csharp
// Create a plot model
var model = new PlotModel 
{ 
    Title = "My First Chart",
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

// Create a line series
var lineSeries = new LineSeries
{
    Title = "Sample Data",
    Color = OxyColors.Blue,
    StrokeThickness = 2
};

// Add data points
for (int i = 0; i < 100; i++)
{
    double x = i * 0.1;
    lineSeries.Points.Add(new DataPoint(x, Math.Sin(x)));
}

model.Series.Add(lineSeries);

// Set the model to the plot view
plotView.Model = model;
plotView.Invalidate(); // Refresh the view
```

## Chart Examples

### Line Chart
```csharp
private void CreateLineChart()
{
    var model = new PlotModel { Title = "Line Chart Example" };
    
    var series1 = new LineSeries
    {
        Title = "Sine Wave",
        Color = OxyColors.Blue
    };
    
    var series2 = new LineSeries
    {
        Title = "Cosine Wave", 
        Color = OxyColors.Red
    };
    
    for (int i = 0; i < 100; i++)
    {
        double x = i * 0.1;
        series1.Points.Add(new DataPoint(x, Math.Sin(x)));
        series2.Points.Add(new DataPoint(x, Math.Cos(x)));
    }
    
    model.Series.Add(series1);
    model.Series.Add(series2);
    
    plotView.Model = model;
    plotView.Invalidate();
}
```

### Bar Chart
```csharp
private void CreateBarChart()
{
    var model = new PlotModel { Title = "Bar Chart Example" };
    
    var categoryAxis = new CategoryAxis 
    { 
        Position = AxisPosition.Left,
        ItemsSource = new[] { "Category A", "Category B", "Category C" }
    };
    
    model.Axes.Add(categoryAxis);
    model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
    
    var barSeries = new BarSeries
    {
        Title = "Sample Data",
        FillColor = OxyColors.SkyBlue
    };
    
    barSeries.Items.Add(new BarItem { Value = 25, Color = OxyColors.Red });
    barSeries.Items.Add(new BarItem { Value = 35, Color = OxyColors.Green });
    barSeries.Items.Add(new BarItem { Value = 45, Color = OxyColors.Blue });
    
    model.Series.Add(barSeries);
    plotView.Model = model;
    plotView.Invalidate();
}
```

### Scatter Plot
```csharp
private void CreateScatterPlot()
{
    var model = new PlotModel { Title = "Scatter Plot Example" };
    
    var scatterSeries = new ScatterSeries
    {
        Title = "Random Data",
        MarkerType = MarkerType.Circle,
        MarkerSize = 5,
        MarkerFill = OxyColors.Red
    };
    
    var random = new Random();
    for (int i = 0; i < 50; i++)
    {
        scatterSeries.Points.Add(new ScatterPoint(
            random.NextDouble() * 10, 
            random.NextDouble() * 10));
    }
    
    model.Series.Add(scatterSeries);
    plotView.Model = model;
    plotView.Invalidate();
}
```

### Pie Chart
```csharp
private void CreatePieChart()
{
    var model = new PlotModel { Title = "Pie Chart Example" };
    
    var pieSeries = new PieSeries
    {
        StrokeThickness = 2.0,
        InsideLabelPosition = 0.8,
        AngleSpan = 360,
        StartAngle = 0
    };
    
    pieSeries.Slices.Add(new PieSlice("Category 1", 40) { Fill = OxyColors.Red });
    pieSeries.Slices.Add(new PieSlice("Category 2", 30) { Fill = OxyColors.Green });
    pieSeries.Slices.Add(new PieSlice("Category 3", 20) { Fill = OxyColors.Blue });
    pieSeries.Slices.Add(new PieSlice("Category 4", 10) { Fill = OxyColors.Orange });
    
    model.Series.Add(pieSeries);
    plotView.Model = model;
    plotView.Invalidate();
}
```

## Advanced Features

### Touch Interactions
The PlotView supports native Android touch gestures:

- **Pan**: Drag to move around the chart
- **Zoom**: Pinch to zoom in/out
- **Reset**: Double-tap to reset view

### Real-time Data Updates
```csharp
private void UpdateChartData()
{
    // Update your series data
    lineSeries.Points.Clear();
    
    // Add new data points
    for (int i = 0; i < newDataPoints.Length; i++)
    {
        lineSeries.Points.Add(new DataPoint(i, newDataPoints[i]));
    }
    
    // Refresh the chart
    plotView.InvalidatePlot(true);
    plotView.Invalidate();
}
```

### Custom Styling
```csharp
var model = new PlotModel 
{
    Title = "Custom Styled Chart",
    TitleColor = OxyColors.DarkBlue,
    Background = OxyColors.LightGray,
    PlotAreaBackground = OxyColors.White,
    PlotAreaBorderColor = OxyColors.Black,
    PlotAreaBorderThickness = new OxyThickness(1)
};
```

## Requirements

- **.NET 10** or later
- **Android API Level 24** (Android 7.0) or higher
- **SkiaSharp** (automatically included)
- **Xamarin.AndroidX.AppCompat** for proper theming

## Dependencies

```xml
<PackageReference Include="OxyPlot" Version="2.1.2" />
<PackageReference Include="OxyPlot.SkiaSharp" Version="2.1.2" />
<PackageReference Include="SkiaSharp" Version="2.88.7" />
<PackageReference Include="Xamarin.AndroidX.AppCompat" Version="1.6.1.2" />
```

## Sample Application

This repository includes a complete sample application demonstrating all chart types and features. To run the sample:

1. Clone the repository:
   ```bash
   git clone https://github.com/AndreCL/Oxyplot.DotNetAndroid.git
   ```

2. Open the solution in Visual Studio

3. Set `Sample.OxyPlot.Android` as the startup project

4. Build and run on your Android device or emulator

## Architecture

The library is built with the following components:

- **PlotView**: Main Android View that hosts the chart
- **SkiaCanvasView**: Custom view for SkiaSharp rendering
- **PlotCore**: Core plotting functionality and model management
- **TouchHandler**: Handles Android touch events and gestures
- **TrackerView**: Provides hover tooltips and data point information

## Performance Tips

1. **Use `Invalidate()`** after updating chart data to trigger redraws
2. **Batch data updates** instead of updating points individually
3. **Consider data sampling** for large datasets (>10,000 points)
4. **Use appropriate chart types** for your data (e.g., scatter plots for large datasets)
5. **Enable hardware acceleration** in your Android manifest

## Troubleshooting

### Common Issues

**Chart not displaying:**
- Ensure you call `plotView.Invalidate()` after setting the model
- Check that your Activity uses an AppCompat theme in AndroidManifest.xml

**Performance issues:**
- Reduce the number of data points for smoother rendering
- Use data decimation for large datasets

**Touch events not working:**
- Verify your PlotView has proper layout parameters
- Ensure no other views are intercepting touch events

## Contributing

We welcome contributions! Please read our [Contributing Guidelines](CONTRIBUTING.md) for details on:

- Code style and conventions
- Pull request process
- Issue reporting
- Development setup

## Roadmap

- [ ] Additional chart types (Candlestick, Heatmap, etc.)
- [ ] Enhanced animation support
- [ ] Improved accessibility features
- [ ] Performance optimizations for large datasets
- [ ] Additional gesture support

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built on top of the excellent [OxyPlot](https://github.com/oxyplot/oxyplot) library
- Powered by [SkiaSharp](https://github.com/mono/SkiaSharp) for cross-platform graphics
- Inspired by the need for high-performance charting in .NET Android applications

## Support

- [Documentation](https://github.com/AndreCL/Oxyplot.DotNetAndroid/wiki)
- [Report Issues](https://github.com/AndreCL/Oxyplot.DotNetAndroid/issues)
- [Discussions](https://github.com/AndreCL/Oxyplot.DotNetAndroid/discussions)
- Star this repository if you find it useful!

---

Made with ❤️ for the .NET Android community

