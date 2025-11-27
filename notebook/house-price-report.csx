#!/usr/bin/env dotnet-script

// Adapted from example authored by Sudipta Mukherjee Nov 2025

// This script will generate a comprehensive HTML report analyzing housing market data based on the house.csv file.
// It covers multiple aspects such as price premiums, age vs price, location proximity, floor level impact, size efficiency, and bedroom count value.
// The report is styled for readability and includes key insights, tables, and placeholders for charts.

#r "nuget: TableAPI, 1.0.4.1"

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Squirrel;

string GetHtmlHeader()
{
    return @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Housing Market Analysis Report</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            background: #f5f5f5;
            padding: 20px;
        }
        
        .container {
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            box-shadow: 0 0 20px rgba(0,0,0,0.1);
        }
        
        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 60px 40px;
            text-align: center;
        }
        
        .header h1 {
            font-size: 3em;
            margin-bottom: 10px;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.2);
        }
        
        .header .subtitle {
            font-size: 1.3em;
            opacity: 0.9;
        }
        
        .header .date {
            margin-top: 20px;
            font-size: 0.9em;
            opacity: 0.8;
        }
        
        .content {
            padding: 40px;
        }
        
        .section {
            margin-bottom: 50px;
            page-break-inside: avoid;
        }
        
        .section-header {
            background: linear-gradient(to right, #667eea, #764ba2);
            color: white;
            padding: 20px;
            margin-bottom: 30px;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }
        
        .section-header h2 {
            font-size: 2em;
            margin-bottom: 5px;
        }
        
        .section-header .section-subtitle {
            opacity: 0.9;
            font-size: 1.1em;
        }
        
        .insight-box {
            background: #f8f9fa;
            border-left: 4px solid #667eea;
            padding: 20px;
            margin: 20px 0;
            border-radius: 4px;
        }
        
        .insight-box.warning {
            border-left-color: #e74c3c;
            background: #fee;
        }
        
        .insight-box.success {
            border-left-color: #27ae60;
            background: #efe;
        }
        
        .insight-box.info {
            border-left-color: #3498db;
            background: #eff8ff;
        }
        
        .insight-box h3 {
            margin-bottom: 10px;
            color: #2c3e50;
        }
        
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin: 30px 0;
        }
        
        .stat-card {
            background: white;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            padding: 25px;
            text-align: center;
            transition: transform 0.3s;
        }
        
        .stat-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
        }
        
        .stat-card .stat-value {
            font-size: 2.5em;
            font-weight: bold;
            color: #667eea;
            margin: 10px 0;
        }
        
        .stat-card .stat-label {
            color: #666;
            font-size: 1.1em;
        }
        
        table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        
        table thead {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }
        
        table th {
            padding: 15px;
            text-align: left;
            font-weight: 600;
            text-transform: uppercase;
            font-size: 0.9em;
            letter-spacing: 0.5px;
        }
        
        table td {
            padding: 12px 15px;
            border-bottom: 1px solid #e0e0e0;
        }
        
        table tbody tr:hover {
            background: #f8f9fa;
        }
        
        table tbody tr:nth-child(even) {
            background: #fafafa;
        }
        
        .highlight {
            background: #fff3cd;
            padding: 2px 6px;
            border-radius: 3px;
            font-weight: bold;
        }
        
        .positive {
            color: #27ae60;
            font-weight: bold;
        }
        
        .negative {
            color: #e74c3c;
            font-weight: bold;
        }
        
        .neutral {
            color: #3498db;
            font-weight: bold;
        }
        
        .footer {
            background: #2c3e50;
            color: white;
            padding: 30px 40px;
            text-align: center;
        }
        
        .footer p {
            margin: 10px 0;
        }
        
        .methodology {
            background: #ecf0f1;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
        }
        
        .methodology h4 {
            margin-bottom: 10px;
            color: #2c3e50;
        }
        
        .chart-placeholder {
            background: #f8f9fa;
            border: 2px dashed #ccc;
            padding: 40px;
            text-align: center;
            border-radius: 8px;
            margin: 20px 0;
            color: #999;
        }
        
        .executive-summary {
            background: linear-gradient(135deg, #667eea15 0%, #764ba215 100%);
            padding: 30px;
            border-radius: 8px;
            margin-bottom: 40px;
        }
        
        .executive-summary h2 {
            color: #667eea;
            margin-bottom: 20px;
        }
        
        .key-findings {
            list-style: none;
            padding: 0;
        }
        
        .key-findings li {
            padding: 10px 0 10px 30px;
            position: relative;
            border-bottom: 1px solid #e0e0e0;
        }
        
        .key-findings li:before {
            content: '▶';
            position: absolute;
            left: 0;
            color: #667eea;
            font-size: 0.8em;
        }
        
        .key-findings li:last-child {
            border-bottom: none;
        }
        
        @media print {
            body {
                background: white;
                padding: 0;
            }
            
            .container {
                box-shadow: none;
            }
            
            .section {
                page-break-inside: avoid;
            }
            
            .stat-card:hover {
                transform: none;
            }
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🏠 Housing Market Analysis Report</h1>
            <div class='subtitle'>Comprehensive Data Journalism Investigation</div>
            <div class='date'>Generated: " + DateTime.Now.ToString("MMMM dd, yyyy") + @"</div>
        </div>
        <div class='content'>
";
}

string GetHtmlFooter()
{
    return @"
        </div>
        <div class='footer'>
            <p><strong>Housing Market Analysis Report</strong></p>
            <p>Generated using Squirrel Data Analysis Framework</p>
            <p>© " + DateTime.Now.Year + @" | Data Journalism Project</p>
            <p style='margin-top: 20px; font-size: 0.9em; opacity: 0.8;'>
                This report analyzes " + DateTime.Now.ToString("MMMM yyyy") + @" housing market data.
                All findings are based on statistical analysis and should be validated with domain experts.
            </p>
        </div>
    </div>
</body>
</html>";
}

string GenerateExecutiveSummary(Table houses)
{
    var avgPrice = houses["price"].Select(decimal.Parse).Average();
    var medianPrice = houses["price"].Select(decimal.Parse).OrderBy(x => x).ElementAt(houses.RowCount / 2);
    var avgSize = houses["net_sqm"].Select(decimal.Parse).Average();
    var avgAge = houses["age"].Select(int.Parse).Average();
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='executive-summary'>
                <h2>📊 Executive Summary</h2>
                <p style='font-size: 1.1em; margin-bottom: 20px;'>
                    This comprehensive analysis examines <strong>" + houses.RowCount.ToString("N0") + @" properties</strong> 
                    to uncover market dynamics, pricing patterns, and investment opportunities.
                </p>
                
                <div class='stats-grid'>
                    <div class='stat-card'>
                        <div class='stat-label'>Average Price</div>
                        <div class='stat-value'>" + avgPrice.ToString("N0") + @"</div>
                    </div>
                    <div class='stat-card'>
                        <div class='stat-label'>Median Price</div>
                        <div class='stat-value'>" + medianPrice.ToString("N0") + @"</div>
                    </div>
                    <div class='stat-card'>
                        <div class='stat-label'>Average Size</div>
                        <div class='stat-value'>" + avgSize.ToString("F1") + @" m²</div>
                    </div>
                    <div class='stat-card'>
                        <div class='stat-label'>Average Age</div>
                        <div class='stat-value'>" + avgAge.ToString("F1") + @" yrs</div>
                    </div>
                </div>
                
                <h3 style='margin-top: 30px; margin-bottom: 15px; color: #2c3e50;'>Key Findings</h3>
                <ul class='key-findings'>
                    <li>Market shows narrow price clustering despite significant variation in property characteristics</li>
                    <li>Location proximity to city center demonstrates measurable impact on property values</li>
                    <li>Age-related depreciation patterns vary significantly by location zone</li>
                    <li>Distinct market segments emerge: Luxury, Value, Budget, and Family-oriented properties</li>
                    <li>Investment opportunities exist in undervalued properties with strong fundamentals</li>
                </ul>
            </div>
");
    return sb.ToString();
}

string GeneratePricePremiumAnalysis(Table houses)
{
    houses.AddColumn("price_per_sqm", "[price] / [net_sqm]", 2);
    var topProperties = houses.SortBy("price_per_sqm", how: SortDirection.Descending).Top(10);
    
    var avgPricePerSqm = houses["price_per_sqm"].Select(decimal.Parse).Average();
    var minPrice = houses["price"].Select(decimal.Parse).Min();
    var maxPrice = houses["price"].Select(decimal.Parse).Max();
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>💰 1. Price Premium Analysis</h2>
                    <div class='section-subtitle'>What drives the price premium in this market?</div>
                </div>
                
                <div class='insight-box info'>
                    <h3>Key Insight</h3>
                    <p>The average price per square meter is <span class='highlight'>" + avgPricePerSqm.ToString("N2") + @"</span>. 
                    Property prices range from <strong>" + minPrice.ToString("N0") + @"</strong> to 
                    <strong>" + maxPrice.ToString("N0") + @"</strong>, showing a relatively narrow clustering 
                    despite huge variations in size and features.</p>
                </div>
                
                <h3>Top 10 Properties by Price per Square Meter</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Bedrooms</th>
                            <th>Size (m²)</th>
                            <th>Price</th>
                            <th>Price/m²</th>
                            <th>Center Dist</th>
                            <th>Metro Dist</th>
                            <th>Age</th>
                        </tr>
                    </thead>
                    <tbody>
");
    
    for (int i = 0; i < topProperties.RowCount; i++)
    {
        var row = topProperties[i];
        sb.Append($@"
                        <tr>
                            <td>{row["bedroom_count"]}</td>
                            <td>{decimal.Parse(row["net_sqm"]):F2}</td>
                            <td>{decimal.Parse(row["price"]):N0}</td>
                            <td><strong>{decimal.Parse(row["price_per_sqm"]):N2}</strong></td>
                            <td>{decimal.Parse(row["center_distance"]):F0}m</td>
                            <td>{decimal.Parse(row["metro_distance"]):F0}m</td>
                            <td>{row["age"]} yrs</td>
                        </tr>
");
    }
    
    sb.Append(@"
                    </tbody>
                </table>
            </div>
");
    return sb.ToString();
}

string GenerateAgeAnalysis(Table houses)
{
    var newHomes = houses.GetRowsWhere(row => int.Parse(row["age"]) <= 5);
    var midAge = houses.GetRowsWhere(row => int.Parse(row["age"]) > 5 && int.Parse(row["age"]) <= 30);
    var oldHomes = houses.GetRowsWhere(row => int.Parse(row["age"]) > 30);
    
var avgNewPrice = newHomes["price"].Select(decimal.Parse).Average();
var avgMidPrice = midAge["price"].Select(decimal.Parse).Average();
var avgOldPrice = oldHomes["price"].Select(decimal.Parse).Average();    var priceDiff = avgNewPrice - avgOldPrice;
    var percentDiff = (priceDiff / avgOldPrice) * 100;
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>📅 2. Age vs Price Analysis</h2>
                    <div class='section-subtitle'>Do newer properties command higher prices?</div>
                </div>
                
                <div class='insight-box " + (priceDiff > 0 ? "success" : "warning") + @"'>
                    <h3>Key Finding</h3>
                    <p>New homes (0-5 years) command a <span class='" + (priceDiff > 0 ? "positive" : "negative") + @"'>" 
                        + Math.Abs(percentDiff).ToString("F2") + @"%</span> " 
                        + (priceDiff > 0 ? "premium" : "discount") + @" compared to older properties (30+ years).</p>
                </div>
                
                <h3>Price Comparison by Age Group</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Age Group</th>
                            <th>Count</th>
                            <th>Average Price</th>
                            <th>% of Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><strong>New (0-5 years)</strong></td>
                            <td>" + newHomes.RowCount.ToString("N0") + @"</td>
                            <td class='positive'>" + avgNewPrice.ToString("N2") + @"</td>
                            <td>" + ((decimal)newHomes.RowCount / houses.RowCount * 100).ToString("F1") + @"%</td>
                        </tr>
                        <tr>
                            <td><strong>Mid-age (6-30 years)</strong></td>
                            <td>" + midAge.RowCount.ToString("N0") + @"</td>
                            <td class='neutral'>" + avgMidPrice.ToString("N2") + @"</td>
                            <td>" + ((decimal)midAge.RowCount / houses.RowCount * 100).ToString("F1") + @"%</td>
                        </tr>
                        <tr>
                            <td><strong>Old (30+ years)</strong></td>
                            <td>" + oldHomes.RowCount.ToString("N0") + @"</td>
                            <td class='negative'>" + avgOldPrice.ToString("N2") + @"</td>
                            <td>" + ((decimal)oldHomes.RowCount / houses.RowCount * 100).ToString("F1") + @"%</td>
                        </tr>
                    </tbody>
                </table>
            </div>
");
    return sb.ToString();
}

string GenerateLocationAnalysis(Table houses)
{
    houses.AddColumn("distance_bucket", "Round([center_distance] / 100,2) * 100", 0);
    
    var distanceGroups = houses.SplitOn("distance_bucket");
    var sortedKeys = distanceGroups.Keys.OrderBy(k => decimal.Parse(k)).Take(10).ToList();
    
    var closest = houses.GetRowsWhere(row => decimal.Parse(row["center_distance"]) < 500);
    var farthest = houses.GetRowsWhere(row => decimal.Parse(row["center_distance"]) > 1500);
    
    var closestAvg = closest["price"].Select(decimal.Parse).Average();
    var farthestAvg = farthest["price"].Select(decimal.Parse).Average();
    var premium = closestAvg - farthestAvg;
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>📍 3. Location Proximity Value</h2>
                    <div class='section-subtitle'>How much is proximity to city center worth?</div>
                </div>
                
                <div class='insight-box success'>
                    <h3>Key Finding</h3>
                    <p>Properties within 500m of city center command a premium of 
                    <span class='highlight positive'>" + premium.ToString("N0") + @"</span> compared to 
                    properties beyond 1,500m. This represents approximately 
                    <strong>" + (premium / farthestAvg * 100).ToString("F1") + @"%</strong> price difference.</p>
                </div>
                
                <h3>Average Price by Distance from City Center</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Distance (m)</th>
                            <th>Average Price</th>
                            <th>Property Count</th>
                        </tr>
                    </thead>
                    <tbody>
");
    
    foreach (var distance in sortedKeys)
    {
        var group = distanceGroups[distance];
        var avgPrice = group["price"].Select(decimal.Parse).Average();
        sb.Append($@"
                        <tr>
                            <td>{distance}m</td>
                            <td>{avgPrice:N2}</td>
                            <td>{group.RowCount:N0}</td>
                        </tr>
");
    }
    
    sb.Append(@"
                    </tbody>
                </table>
            </div>
");
    return sb.ToString();
}

string GenerateFloorAnalysis(Table houses)
{
    var lowFloors = houses.GetRowsWhere(row => int.Parse(row["floor"]) <= 5);
    var midFloors = houses.GetRowsWhere(row => int.Parse(row["floor"]) > 5 && int.Parse(row["floor"]) <= 15);
    var highFloors = houses.GetRowsWhere(row => int.Parse(row["floor"]) > 15);
    
    var avgLow = lowFloors["price"].Select(decimal.Parse).Average();
    var avgMid = midFloors["price"].Select(decimal.Parse).Average();
    var avgHigh = highFloors["price"].Select(decimal.Parse).Average();
    
    var maxFloorAvg = Math.Max(avgLow, Math.Max(avgMid, avgHigh));
    var floorPremium = ((maxFloorAvg - avgLow) / avgLow) * 100;
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>🏢 4. Vertical Price Premium Analysis</h2>
                    <div class='section-subtitle'>Is there a price premium for higher floors?</div>
                </div>
                
                <div class='insight-box info'>
                    <h3>Key Finding</h3>
                    <p>Floor level shows a <strong>" + Math.Abs(floorPremium).ToString("F2") + @"%</strong> 
                    " + (avgHigh > avgLow ? "premium for high floors" : "discount for high floors") + @" 
                    compared to low-level properties.</p>
                </div>
                
                <h3>Price Analysis by Floor Level</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Floor Level</th>
                            <th>Count</th>
                            <th>Average Price</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><strong>Low Floors (1-5)</strong></td>
                            <td>" + lowFloors.RowCount.ToString("N0") + @"</td>
                            <td>" + avgLow.ToString("N2") + @"</td>
                        </tr>
                        <tr>
                            <td><strong>Mid Floors (6-15)</strong></td>
                            <td>" + midFloors.RowCount.ToString("N0") + @"</td>
                            <td>" + avgMid.ToString("N2") + @"</td>
                        </tr>
                        <tr>
                            <td><strong>High Floors (16+)</strong></td>
                            <td>" + highFloors.RowCount.ToString("N0") + @"</td>
                            <td>" + avgHigh.ToString("N2") + @"</td>
                        </tr>
                    </tbody>
                </table>
            </div>
");
    return sb.ToString();
}

string GenerateSizeEfficiencyAnalysis(Table houses)
{
    if (!houses.ColumnHeaders.Contains("price_per_sqm"))
    {
        houses.AddColumn("price_per_sqm", "[price] / [net_sqm]", 2);
    }
    
    var small = houses.GetRowsWhere(row => decimal.Parse(row["net_sqm"]) < 30);
    var medium = houses.GetRowsWhere(row => decimal.Parse(row["net_sqm"]) >= 30 && decimal.Parse(row["net_sqm"]) < 80);
    var large = houses.GetRowsWhere(row => decimal.Parse(row["net_sqm"]) >= 80 && decimal.Parse(row["net_sqm"]) < 150);
    var veryLarge = houses.GetRowsWhere(row => decimal.Parse(row["net_sqm"]) >= 150);
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>📏 5. Size Efficiency Analysis</h2>
                    <div class='section-subtitle'>What's the sweet spot for property size?</div>
                </div>
                
                <h3>Price Efficiency by Size Category</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Size Category</th>
                            <th>Count</th>
                            <th>Avg Price/m²</th>
                            <th>Avg Total Price</th>
                        </tr>
                    </thead>
                    <tbody>
");
    
    var categories = new[] {
        ("Small (<30 m²)", small),
        ("Medium (30-80 m²)", medium),
        ("Large (80-150 m²)", large),
        ("Very Large (150+ m²)", veryLarge)
    };
    
    foreach (var (label, segment) in categories)
    {
        if (segment.RowCount > 0)
        {
            var avgPricePerSqm = segment["price_per_sqm"].Select(decimal.Parse).Average();
            var avgTotalPrice = segment["price"].Select(decimal.Parse).Average();
            sb.Append($@"
                        <tr>
                            <td><strong>{label}</strong></td>
                            <td>{segment.RowCount:N0}</td>
                            <td>{avgPricePerSqm:N2}</td>
                            <td>{avgTotalPrice:N2}</td>
                        </tr>
");
        }
    }
    
    sb.Append(@"
                    </tbody>
                </table>
            </div>
");
    return sb.ToString();
}

string GenerateBedroomAnalysis(Table houses)
{
    var bedroomGroups = houses.SplitOn("bedroom_count");
    var sortedBedrooms = bedroomGroups.Keys.OrderBy(k => int.Parse(k)).ToList();
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>🛏️ 6. Bedroom Count Value Analysis</h2>
                    <div class='section-subtitle'>Which bedroom counts offer the best value?</div>
                </div>
                
                <h3>Average Price by Bedroom Count</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Bedrooms</th>
                            <th>Average Price</th>
                            <th>Property Count</th>
                            <th>% of Market</th>
                        </tr>
                    </thead>
                    <tbody>
");
    
    foreach (var bedrooms in sortedBedrooms)
    {
        var group = bedroomGroups[bedrooms];
        var avgPrice = group["price"].Select(decimal.Parse).Average();
        var marketShare = (decimal)group.RowCount / houses.RowCount * 100;
        
        sb.Append($@"
                        <tr>
                            <td><strong>{bedrooms}</strong></td>
                            <td>{avgPrice:N2}</td>
                            <td>{group.RowCount:N0}</td>
                            <td>{marketShare:F1}%</td>
                        </tr>
");
    }
    
    sb.Append(@"
                    </tbody>
                </table>
            </div>
");
    return sb.ToString();
}

string GenerateAccessibilityAnalysis(Table houses)
{
    var metroConvenient = houses.GetRowsWhere(row => 
        decimal.Parse(row["metro_distance"]) < 100 && 
        decimal.Parse(row["center_distance"]) > 1000);
    
    var centerConvenient = houses.GetRowsWhere(row => 
        decimal.Parse(row["center_distance"]) < 500 && 
        decimal.Parse(row["metro_distance"]) > 150);
    
    var dualConvenient = houses.GetRowsWhere(row => 
        decimal.Parse(row["metro_distance"]) < 100 && 
        decimal.Parse(row["center_distance"]) < 500);
    
    var metroAvg = metroConvenient.RowCount > 0 ? metroConvenient["price"].Select(decimal.Parse).Average() : 0;
    var centerAvg = centerConvenient.RowCount > 0 ? centerConvenient["price"].Select(decimal.Parse).Average() : 0;
    var dualAvg = dualConvenient.RowCount > 0 ? dualConvenient["price"].Select(decimal.Parse).Average() : 0;
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>🚇 7. Accessibility Factor Comparison</h2>
                    <div class='section-subtitle'>Metro access vs City center proximity</div>
                </div>
                
                <div class='insight-box success'>
                    <h3>Key Finding</h3>
                    <p>Properties with dual convenience (close to both metro and city center) command 
                    the highest prices at <span class='highlight positive'>" + dualAvg.ToString("N0") + @"</span>, 
                    representing a premium over single-access properties.</p>
                </div>
                
                <h3>Accessibility Comparison</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Category</th>
                            <th>Definition</th>
                            <th>Count</th>
                            <th>Avg Price</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><strong>Metro-Convenient</strong></td>
                            <td>Metro < 100m, Center > 1000m</td>
                            <td>" + metroConvenient.RowCount.ToString("N0") + @"</td>
                            <td>" + metroAvg.ToString("N2") + @"</td>
                        </tr>
                        <tr>
                            <td><strong>Center-Convenient</strong></td>
                            <td>Center < 500m, Metro > 150m</td>
                            <td>" + centerConvenient.RowCount.ToString("N0") + @"</td>
                            <td>" + centerAvg.ToString("N2") + @"</td>
                        </tr>
                        <tr>
                            <td><strong>Dual-Convenient</strong></td>
                            <td>Metro < 100m, Center < 500m</td>
                            <td>" + dualConvenient.RowCount.ToString("N0") + @"</td>
                            <td class='positive'>" + dualAvg.ToString("N2") + @"</td>
                        </tr>
                    </tbody>
                </table>
            </div>
");
    return sb.ToString();
}

string GenerateMarketSegmentation(Table houses)
{
    if (!houses.ColumnHeaders.Contains("price_per_sqm"))
    {
        houses.AddColumn("price_per_sqm", "[price] / [net_sqm]", 2);
    }
    
    var luxury = houses.GetRowsWhere(row =>
        decimal.Parse(row["price"]) > 95000 &&
        decimal.Parse(row["center_distance"]) < 500 &&
        int.Parse(row["age"]) < 10);
    
    var value = houses.GetRowsWhere(row =>
        decimal.Parse(row["net_sqm"]) > 40 &&
        decimal.Parse(row["price"]) < 93000 &&
        decimal.Parse(row["metro_distance"]) < 150);
    
    var budget = houses.GetRowsWhere(row =>
        decimal.Parse(row["price"]) < 91000 &&
        decimal.Parse(row["net_sqm"]) < 40);
    
    var family = houses.GetRowsWhere(row =>
        int.Parse(row["bedroom_count"]) >= 3 &&
        decimal.Parse(row["net_sqm"]) > 80 &&
        decimal.Parse(row["metro_distance"]) < 200);
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>🎯 8. Market Segmentation Analysis</h2>
                    <div class='section-subtitle'>Identifying distinct buyer personas and market niches</div>
                </div>
                
                <div class='stats-grid'>
");
    
    var segments = new[] {
        ("Luxury", luxury, "High price, central, new"),
        ("Value", value, "Good size, affordable, accessible"),
        ("Budget", budget, "Small, affordable"),
        ("Family", family, "3+ bedrooms, spacious, accessible")
    };
    
    foreach (var (name, segment, description) in segments)
    {
        if (segment.RowCount > 0)
        {
            var avgPrice = segment["price"].Select(decimal.Parse).Average();
            sb.Append($@"
                    <div class='stat-card'>
                        <div class='stat-label'>{name} Segment</div>
                        <div class='stat-value'>{segment.RowCount}</div>
                        <p style='color: #666; font-size: 0.9em; margin-top: 10px;'>{description}</p>
                        <p style='color: #667eea; font-weight: bold; margin-top: 5px;'>Avg: {avgPrice:N0}</p>
                    </div>
");
        }
    }
    
    sb.Append(@"
                </div>
            </div>
");
    return sb.ToString();
}

string GenerateAffordabilityAnalysis(Table houses)
{
    decimal medianIncome = 30000;
    decimal affordablePrice = medianIncome * 3;
    
    var affordable = houses.GetRowsWhere(row => 
        decimal.Parse(row["price"]) <= affordablePrice);
    
    var marginally = houses.GetRowsWhere(row => 
        decimal.Parse(row["price"]) > affordablePrice && 
        decimal.Parse(row["price"]) <= affordablePrice * 1.2m);
    
    var unaffordable = houses.GetRowsWhere(row => 
        decimal.Parse(row["price"]) > affordablePrice * 1.2m);
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>💵 9. Affordability Analysis</h2>
                    <div class='section-subtitle'>Assessing market accessibility based on income thresholds</div>
                </div>
                
                <div class='insight-box warning'>
                    <h3>Affordability Context</h3>
                    <p>Based on median household income of <strong>" + medianIncome.ToString("N0") + @"</strong>, 
                    the affordable price threshold (3x income) is <span class='highlight'>" + affordablePrice.ToString("N0") + @"</span>. 
                    Only <strong>" + ((decimal)affordable.RowCount / houses.RowCount * 100).ToString("F1") + @"%</strong> 
                    of properties fall within this range.</p>
                </div>
                
                <div class='stats-grid'>
                    <div class='stat-card'>
                        <div class='stat-label'>Affordable</div>
                        <div class='stat-value' style='color: #27ae60;'>" + affordable.RowCount + @"</div>
                        <p style='margin-top: 10px; color: #666;'>" + ((decimal)affordable.RowCount / houses.RowCount * 100).ToString("F1") + @"% of market</p>
                    </div>
                    <div class='stat-card'>
                        <div class='stat-label'>Marginally Affordable</div>
                        <div class='stat-value' style='color: #f39c12;'>" + marginally.RowCount + @"</div>
                        <p style='margin-top: 10px; color: #666;'>" + ((decimal)marginally.RowCount / houses.RowCount * 100).ToString("F1") + @"% of market</p>
                    </div>
                    <div class='stat-card'>
                        <div class='stat-label'>Unaffordable</div>
                        <div class='stat-value' style='color: #e74c3c;'>" + unaffordable.RowCount + @"</div>
                        <p style='margin-top: 10px; color: #666;'>" + ((decimal)unaffordable.RowCount / houses.RowCount * 100).ToString("F1") + @"% of market</p>
                    </div>
                </div>
            </div>
");
    return sb.ToString();
}

string GenerateInvestmentOpportunities(Table houses)
{
    if (!houses.ColumnHeaders.Contains("price_per_sqm"))
    {
        houses.AddColumn("price_per_sqm", "[price] / [net_sqm]", 2);
    }
    
    var pricePerSqmValues = houses["price_per_sqm"].Select(decimal.Parse).ToList();
    var medianPricePerSqm = pricePerSqmValues.OrderBy(x => x).ElementAt(pricePerSqmValues.Count / 2);
    
    var undervalued = houses.GetRowsWhere(row =>
        decimal.Parse(row["price_per_sqm"]) < medianPricePerSqm * 0.95m &&
        decimal.Parse(row["center_distance"]) < 1200 &&
        decimal.Parse(row["metro_distance"]) < 200);
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>💎 10. Investment Opportunities</h2>
                    <div class='section-subtitle'>Identifying undervalued properties with strong fundamentals</div>
                </div>
                
                <div class='insight-box success'>
                    <h3>Investment Strategy</h3>
                    <p>Found <strong>" + undervalued.RowCount + @"</strong> undervalued properties priced below 
                    95% of median price per sqm (<span class='highlight'>" + (medianPricePerSqm * 0.95m).ToString("N2") + @"</span>), 
                    yet maintaining good location fundamentals (< 1200m from center, < 200m from metro).</p>
                </div>
");
    
    if (undervalued.RowCount > 0)
    {
        var top = undervalued.SortBy("price_per_sqm").Top(10);
        sb.Append(@"
                <h3>Top 10 Investment Opportunities</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Bedrooms</th>
                            <th>Size (m²)</th>
                            <th>Price</th>
                            <th>Price/m²</th>
                            <th>Age</th>
                            <th>Center Dist</th>
                        </tr>
                    </thead>
                    <tbody>
");
        
        for (int i = 0; i < top.RowCount; i++)
        {
            var row = top[i];
            sb.Append($@"
                        <tr>
                            <td>{row["bedroom_count"]}</td>
                            <td>{decimal.Parse(row["net_sqm"]):F2}</td>
                            <td>{decimal.Parse(row["price"]):N0}</td>
                            <td><strong>{decimal.Parse(row["price_per_sqm"]):N2}</strong></td>
                            <td>{row["age"]} yrs</td>
                            <td>{decimal.Parse(row["center_distance"]):F0}m</td>
                        </tr>
");
        }
        
        sb.Append(@"
                    </tbody>
                </table>
");
    }
    
    sb.Append("</div>");
    return sb.ToString();
}

string GenerateAgeDepreciationByLocation(Table houses)
{
    houses.AddColumn("distance_zone", "Round([center_distance] / 500,2) * 500", 0);
    
    var zones = houses.SplitOn("distance_zone");
    var sortedZones = zones.Keys.OrderBy(k => decimal.Parse(k)).Take(5).ToList();
    
    var sb = new StringBuilder();
    sb.Append(@"
            <div class='section'>
                <div class='section-header'>
                    <h2>📉 11. Age Depreciation by Location</h2>
                    <div class='section-subtitle'>Zone-specific depreciation patterns reveal location resilience</div>
                </div>
                
                <h3>Depreciation Analysis by Distance Zone</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Zone (m)</th>
                            <th>New (0-10y) Avg</th>
                            <th>Old (30+y) Avg</th>
                            <th>Depreciation %</th>
                            <th>Properties</th>
                        </tr>
                    </thead>
                    <tbody>
");
    
    foreach (var zone in sortedZones)
    {
        var zoneTable = zones[zone];
        var newProps = zoneTable.GetRowsWhere(row => int.Parse(row["age"]) <= 10);
        var oldProps = zoneTable.GetRowsWhere(row => int.Parse(row["age"]) > 30);
        
        if (newProps.RowCount > 0 && oldProps.RowCount > 0)
        {
            var newAvg = newProps["price"].Select(decimal.Parse).Average();
            var oldAvg = oldProps["price"].Select(decimal.Parse).Average();
            var depreciation = newAvg - oldAvg;
            var depreciationPct = (depreciation / newAvg) * 100;
            
            var colorClass = depreciationPct > 5 ? "negative" : depreciationPct < -5 ? "positive" : "neutral";
            
            sb.Append($@"
                        <tr>
                            <td>{zone}m</td>
                            <td>{newAvg:N2}</td>
                            <td>{oldAvg:N2}</td>
                            <td class='{colorClass}'>{depreciationPct:F2}%</td>
                            <td>{zoneTable.RowCount:N0}</td>
                        </tr>
");
        }
    }
    
    sb.Append(@"
                    </tbody>
                </table>
            </div>
");
    return sb.ToString();
}

// Main execution
var houses = DataAcquisition.LoadCsv("house.csv");

Console.WriteLine("=== GENERATING COMPREHENSIVE HTML REPORT ===\n");

var htmlReport = new StringBuilder();

// Add HTML header and CSS
htmlReport.Append(GetHtmlHeader());

// Add Executive Summary
htmlReport.Append(GenerateExecutiveSummary(houses));

// Add all analyses
htmlReport.Append(GeneratePricePremiumAnalysis(houses));
htmlReport.Append(GenerateAgeAnalysis(houses));
htmlReport.Append(GenerateLocationAnalysis(houses));
htmlReport.Append(GenerateFloorAnalysis(houses));
htmlReport.Append(GenerateSizeEfficiencyAnalysis(houses));
htmlReport.Append(GenerateBedroomAnalysis(houses));
htmlReport.Append(GenerateAccessibilityAnalysis(houses));
htmlReport.Append(GenerateMarketSegmentation(houses));
htmlReport.Append(GenerateAffordabilityAnalysis(houses));
htmlReport.Append(GenerateInvestmentOpportunities(houses));
htmlReport.Append(GenerateAgeDepreciationByLocation(houses));

// Add footer
htmlReport.Append(GetHtmlFooter());

// Save to file
File.WriteAllText("house-price-report.html", htmlReport.ToString());

Console.WriteLine("✓ HTML report generated: house-price-report.html");
Console.WriteLine("✓ Open this file in a browser, then use Print to PDF");