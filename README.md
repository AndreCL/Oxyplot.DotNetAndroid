# OxyPlot.DotNetAndroid

High-performance charting library for .NET 10 Android applications using OxyPlot.

![Nuget](https://github.com/AndreCL/Oxyplot.DotNetAndroid/workflows/Nuget/badge.svg) 
![CodeQL](https://github.com/AndreCL/Oxyplot.DotNetAndroid/workflows/CodeQL/badge.svg) 
![Build](https://github.com/AndreCL/Oxyplot.DotNetAndroid/workflows/Build/badge.svg)  <a href="https://www.nuget.org/packages/Oxyplot.DotNetAndroid" alt="Nuget Package"><img src="https://img.shields.io/nuget/v/Oxyplot.DotNetAndroid.svg?logo=nuget" title="Go To Nuget Package" alt="Nuget Package"/></a> 
[![.NET](https://img.shields.io/badge/.NET-10-blue.svg)](https://dotnet.microsoft.com/download)
[![Android](https://img.shields.io/badge/Android-API%2021+-green.svg)](https://developer.android.com/about/versions/lollipop)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Features

- **Chart Types**: Line, Bar, Scatter, Pie, Real-time, Multi-series
- **Touch Gestures**: Pan, zoom, pinch with native Android support
- **High Performance**: Hardware-accelerated rendering
- **Customizable**: Themes, colors, fonts, and styling options

## Quick Start

```csharp
using OxyPlot.DotNetAndroid;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

// Create PlotView
var plotView = new PlotView(context);

// Create chart model
var model = new PlotModel { Title = "Sample Chart" };
model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

// Add data series
var series = new LineSeries { Title = "Data", Color = OxyColors.Blue };
for (int i = 0; i < 100; i++)
{
    series.Points.Add(new DataPoint(i * 0.1, Math.Sin(i * 0.1)));
}
model.Series.Add(series);

// Assign and render
plotView.Model = model;
plotView.Invalidate();
```

## Chart Examples

### Line Chart
```csharp
var model = new PlotModel { Title = "Line Chart" };
var series = new LineSeries { Color = OxyColors.Blue };
// Add data points...
model.Series.Add(series);
plotView.Model = model;
```

### Bar Chart
```csharp
var model = new PlotModel { Title = "Bar Chart" };
var barSeries = new BarSeries();
barSeries.Items.Add(new BarItem { Value = 25 });
// Add more items...
model.Series.Add(barSeries);
```

### Scatter Plot
```csharp
var model = new PlotModel { Title = "Scatter Plot" };
var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
// Add scatter points...
model.Series.Add(scatterSeries);
```

## Touch Interactions

- **Pan**: Drag to move around the chart
- **Zoom**: Pinch to zoom in/out
- **Reset**: Double-tap to reset view
- **Tracker**: Tap data points for details

## Requirements

- **.NET 10** Android target framework
- **Android API Level 21** (Android 5.0) or higher
- **OxyPlot.Core** and **OxyPlot.SkiaSharp** dependencies

## Sample Application

Clone and run the included sample:

```bash
git clone https://github.com/AndreCL/Oxyplot.DotNetAndroid.git
cd Oxyplot.DotNetAndroid
dotnet run --project Sample.OxyPlot.Android
```

## License

MIT License - see [LICENSE](LICENSE) for details.

## Support

- [Issues](https://github.com/AndreCL/Oxyplot.DotNetAndroid/issues)
- [Discussions](https://github.com/AndreCL/Oxyplot.DotNetAndroid/discussions)

Built on [OxyPlot](https://github.com/oxyplot/oxyplot) with SkiaSharp rendering.

