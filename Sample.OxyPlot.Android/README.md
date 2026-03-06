# OxyPlot Android Sample

A .NET 10 Android sample demonstrating the **OxyPlot.DotNetAndroid** library for creating interactive charts.

## Features

- **Chart Types**: Line, Bar, Scatter, Pie, Real-time, and Multi-series charts
- **Touch Interaction**: Pan, pinch-zoom, and data point tracking
- **Responsive UI**: Vertical button layout with scrollable chart switching

## Quick Start

```bash
# Build and run
dotnet build Sample.OxyPlot.Android
dotnet run --project Sample.OxyPlot.Android
```

**Prerequisites**: .NET 10, Android SDK (API 24+), `dotnet workload install android`

## Project Structure

- `MainActivity.cs` - Main activity with chart switching UI
- `ChartDemos.cs` - Chart creation helper methods
- `Resources/` - Android resources (layouts, strings, icons)

## Usage

1. Launch the app (opens with line chart)
2. Use buttons to switch between chart types
3. Touch gestures: pan to move, pinch to zoom, tap for data values

## Basic Chart Creation

```csharp
var model = new PlotModel { Title = "Sample Chart" };
model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

var series = new LineSeries { Title = "Data", Color = OxyColors.Blue };
series.Points.Add(new DataPoint(0, 1));
// Add more points...

model.Series.Add(series);
plotView.Model = model;
```

## Dependencies

- **OxyPlot.DotNetAndroid** (project reference)
- **.NET 10 Android** target framework