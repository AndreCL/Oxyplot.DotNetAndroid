# OxyPlot Android Sample

This sample application demonstrates how to use the **OxyPlot.DotNetAndroid** library to create interactive charts in .NET Android applications.

## Features Demonstrated

### Chart Types
- **Line Charts** - Sine and cosine wave demonstrations with multiple series
- **Bar Charts** - Categorical data visualization with colored bars
- **Scatter Plots** - Random data point visualization
- **Pie Charts** - Percentage data with exploded slices
- **Real-time Charts** - Simulated sensor data with noise
- **Multi-Series Charts** - Combined line and area series

### Interactive Features
- **Pan** - Touch and drag to move around the chart
- **Pinch Zoom** - Pinch to zoom in/out on chart data
- **Tracker** - Touch data points to see detailed information
- **Multiple Chart Types** - Easy switching between different chart demonstrations

## Project Structure

```
Sample.OxyPlot.Android/
??? MainActivity.cs          # Main activity with chart switching
??? ChartDemos.cs           # Helper class with advanced chart examples
??? Resources/
?   ??? values/
?       ??? strings.xml     # App strings and configuration
??? Sample.OxyPlot.Android.csproj  # Project file with dependencies
```

## Dependencies

- **OxyPlot.DotNetAndroid** (project reference) - The main plotting library
- **Xamarin.AndroidX.AppCompat** - For modern Android UI components

## Usage

1. **Launch the app** - The application opens with a line chart by default
2. **Switch chart types** - Use the horizontal scrollable button bar at the top
3. **Interact with charts**:
   - **Pan**: Touch and drag to move around
   - **Zoom**: Pinch to zoom in/out  
   - **Track data**: Touch data points to see values

## Chart Examples

### Line Chart
Displays mathematical functions (sine/cosine waves) with multiple colored series and legends.

### Bar Chart
Shows categorical data with individual colored bars and value labels.

### Scatter Plot
Demonstrates random data points with circular markers and customizable appearance.

### Pie Chart
Displays percentage data with exploded slices and category labels.

### Real-time Chart
Simulates streaming sensor data with noise, demonstrating how to handle dynamic data.

### Multi-Series Chart
Combines different chart types (line and area) on the same plot with shared axes.

## Code Highlights

### Creating a Simple Chart
```csharp
// Create a plot model
var model = new PlotModel { Title = "My Chart" };

// Add axes
model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y" });

// Create and populate a line series
var series = new LineSeries { Title = "Data", Color = OxyColors.Blue };
for (int i = 0; i < 100; i++)
{
    series.Points.Add(new DataPoint(i, Math.Sin(i * 0.1)));
}
model.Series.Add(series);

// Assign to PlotView
plotView.Model = model;
```

### Touch Interaction
The PlotView automatically handles touch gestures:
- Pan gestures for moving around the chart
- Pinch gestures for zooming
- Touch tracking for data point information

## Building and Running

### Prerequisites
- .NET 10 or later
- Android SDK with API level 24+
- Android workload for .NET (`dotnet workload install android`)

### Build Commands
```bash
# Build the sample
dotnet build Sample.OxyPlot.Android

# Run on connected device/emulator
dotnet run --project Sample.OxyPlot.Android
```

## Customization

### Adding New Chart Types
1. Add a new method to `MainActivity` or `ChartDemos`
2. Create a `PlotModel` with desired series and configuration
3. Add a button to switch to the new chart type

### Styling Charts
- Modify colors using `OxyColors` or custom `OxyColor.FromRgb()`
- Adjust line thickness, marker sizes, and other visual properties
- Set custom titles, legends, and axis labels

## Performance Notes

- Charts are rendered using SkiaSharp for high performance
- Touch events are optimized for smooth interaction
- Memory usage is efficient with proper disposal of graphics resources

## License

This sample application is provided under the MIT license, same as the OxyPlot.DotNetAndroid library.