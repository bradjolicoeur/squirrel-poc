#!/usr/bin/env dotnet-script
#r "nuget: TableAPI, 1.0.4.1"

using Squirrel;
using System.IO;

// Load CSV data and add calculated column
var data = DataAcquisition.LoadCsv("house.csv");
data.AddColumn(columnName:"price_per_sqm", formula:"[price]/[net_sqm]", decimalDigits:2);

// Generate bar chart HTML
string html = data.Pick("bedroom_count", "price_per_sqm")
    .Aggregate("bedroom_count", AggregationMethod.Average)
    .RoundOffTo(2)
    .ToBarChartByGoogleDataVisualization("bedroom_count", "price_per_sqm", "price", GoogleDataVisualizationcs.BarChartType.Column);

// Write outputs
File.WriteAllText("house-price-per-sqm.csv", data.ToCsv());

StreamWriter writer = new StreamWriter("house-price-per-sqm_chart.html");
writer.Write(html);
writer.Close();

Console.WriteLine("Analysis complete!");
Console.WriteLine("Generated files:");
Console.WriteLine("  - house-price-per-sqm.csv: Processed data with price_per_sqm column");
Console.WriteLine("  - house-price-per-sqm_chart.html: Bar chart visualization");