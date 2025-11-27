#!/usr/bin/env dotnet-script
#r "nuget: TableAPI, 1.0.4.1"

// Adapted from example authored by Sudipta Mukherjee Nov 2025

// This script will perform data analysis on the house.csv dataset to generate insights such as average prices based on various features.
// It will also create a SQL script to store the analyzed data for further use.

using System.Globalization;
using Squirrel;
using System.IO;
using Squirrel.Cleansing;

void PrintAverageBy(Table data, string columnName)
{
    data.SplitOn(columnName)
        .Select(t => 
            new
            {
                Bedrooms = t.Key, 
                AveragePrice = Math.Round(t.Value["price"].Average(),2)
            })
        .ToTableFromAnonList()
        .SortBy("AveragePrice", how: SortDirection.Descending)
        .Top(10)
        .PrettyDump(header:$"Average Price by {columnName} (Top 10)", rowColor: ConsoleColor.Blue);
}

record BedRoomSize(int Bedrooms, int NetSqm);

// Load CSV data and add calculated column
var data = DataAcquisition.LoadCsv("house.csv");

// Print average price by all columns
data.ColumnHeaders.ToList().ForEach(f => PrintAverageBy(data,f));

//Composite Key 
//Average price per floor/bedroom combination 
//e.g: What's the average price of a three-bedroom apartment on the 14th floor?

var keys =  data["floor"]
                                .Zip(data["bedroom_count"], (f,b) => $"floor_{f}_{b}_bed");
data.AddColumn("FloorBed", keys.ToList());
data.SplitOn("FloorBed")
    .Select(t => new { FloorBed = t.Key,
        AveragePrice = Math.Round(t.Value["price"].Average(),2)})
    .ToTableFromAnonList()
    .SortBy("AveragePrice", how: SortDirection.Descending)
    .Top(10)
    .PrettyDump(header:"Average Price by Floor/Bedroom Combination (Top 10)", rowColor: ConsoleColor.Blue);

var avg14floor3bed = data.Filter("FloorBed", "floor_14_3_bed")["price"].Average();
Console.WriteLine($"Average price of 3 bed rooms on the 14th floor: {avg14floor3bed}");

// Distribution of net_sqm per bedroom
var bedroomNetSqm = data.SplitOn("bedroom_count")
    .Select(t => new { Bedrooms = t.Key, 
        NetSqm = t.Value["net_sqm"]
                                    //To the nearest int
                                   .Select(f => Math.Ceiling(Convert.ToDouble(f)))
                                   .Distinct()
                                   .OrderByDescending(c => c)
                                   .Select(c => c.ToString(CultureInfo.InvariantCulture))
                                   .Aggregate((a,b) => a + "|" + b)})
    .ToTableFromAnonList()
    .Explode("NetSqm", '|');
    
// Finding the bedroom sizes NetSqm for each 
var beds = "3";
var sizeOf3Beds = bedroomNetSqm.Filter("Bedrooms", beds)["NetSqm"];
// Setting a name of the table 
// 
bedroomNetSqm.Name = "BedroomNetSqm";
Console.WriteLine("Size of 3 bedrooms in decreasing order of sq meter (top 10):");
sizeOf3Beds.Take(10).ToList().ForEach(Console.WriteLine);

// Convert to strongly typed table
var bedRoomRecTable = RecordTable<BedRoomSize>.FromTable(bedroomNetSqm);

// Convert to SQL table (a table 
var bedRoomSqlTable = bedRoomRecTable.ToSqlTable();
// Create script to create the table 
var createSQL = bedRoomSqlTable.CreateTableScript;
var insertSQL = bedRoomSqlTable.RowInsertCommands;

Console.WriteLine("CREATE SQL Command");
// Output SQL to script file so we can hand it off to DBA
var fullSQL = createSQL + "\n\n" + string.Join("\n", insertSQL);
File.WriteAllText("house-price-bedroom-analysis.sql", fullSQL);
Console.WriteLine("SQL script written to house-price-bedroom-analysis.sql");
// Output to CSV for data review
File.WriteAllText("house-price-bedroom-analysis.csv", bedroomNetSqm.ToCsv());
Console.WriteLine("CSV data written to house-price-bedroom-analysis.csv");