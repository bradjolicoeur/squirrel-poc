# Squirrel Proof-of-Concept (POC)

This repository contains proof-of-concept projects demonstrating the capabilities of the **Squirrel** library for data analysis in .NET. It showcases how to perform data wrangling, analysis, visualization, and reporting tasks on a sample housing dataset, much like you would with Pandas in Python.

The examples are provided as C# scripts (`.csx`), a Polyglot Notebook, and detailed observations in a [Markdown file](./notebook/squirrel-observations.md).

## Key Features Demonstrated
This repository provides a collection of examples that showcase how to use the Squirrel library for a variety of data analysis tasks. The following is a summary of the key features demonstrated in the examples:

- **Data Loading and Manipulation**: Loading data from CSV files, adding calculated columns, and filtering data.
- **Data Aggregation**: Grouping and aggregating data to calculate averages and other statistics.
- **Data Visualization**: Generating charts and graphs to visualize data.
- **SQL Generation**: Generating SQL scripts to create tables and insert data.
- **Report Generation**: Creating comprehensive HTML reports from data.

## Examples in this Repository

The `notebook/` directory contains all the example files.

### 1. House Price Analysis (`house-price-analysis.csx`)
This C# script performs an in-depth analysis of the housing data.
- **Calculates and displays**:
    - Average house price by various features (e.g., number of bedrooms, floor).
    - Average price for composite keys like floor/bedroom combinations.
- **Generates**:
    - A SQL script (`house-price-bedroom-analysis.sql`) to store analysis results.
    - A CSV file (`house-price-bedroom-analysis.csv`) of the analyzed data.

### 2. Interactive Notebook (`house-price-notebook.ipynb`)
A Polyglot Notebook that provides an interactive way to:
- Load `house.csv`.
- Calculate price per square meter.
- Generate a bar chart of average price per square meter by bedroom count.
- Saves the results to `house-price-notebook.csv` and the chart to `house-price-notebook-chart.html`.

### 3. Price Per Square Meter Analysis (`house-price-sqm-analysis.csx`)
A focused C# script that:
- Calculates the price per square meter.
- Generates a bar chart visualization (`house-price-per-sqm_chart.html`).
- Exports the processed data to `house-price-per-sqm.csv`.

### 4. SQL Data Loader (`house-price-sql-loader.csx`)
This script demonstrates ETL (Extract, Transform, Load) capabilities:
- Loads the processed `house-price-per-sqm.csv`.
- Cleans the data by rounding values.
- Generates a complete SQL script (`house-price-sql-loader.sql`) with `CREATE TABLE` and `INSERT` statements.

### 5. HTML Report Generation (`house-price-report.csx`)
A script that generates a comprehensive, styled HTML report (`house-price-report.html`) from the housing data, covering aspects like:
- Price premiums
- Age vs. Price analysis
- Impact of location and floor level

## How to Run the Examples
To run the C# scripts (`.csx`), you will need to have the `dotnet-script` tool installed. You can then execute a script from your terminal like this:
```bash
dotnet-script notebook/house-price-analysis.csx
```

The Jupyter Notebook can be run in a .NET Interactive Notebooks environment.

## Comparisons & Resources
To better understand how Squirrel fits into the .NET data ecosystem, check out these comparison reports:
- [Squirrel Observations](./notebook/squirrel-observations.md) - Key observations on using the library for data analysis.
- [Squirrel vs. Python Pandas](./notebook/squirrel-vs-pandas.md) - A guide for .NET engineers comparing Squirrel to the industry standard.
- [Squirrel vs. ML.NET DataFrame](./notebook/squirrel-vs-mlnet-dataframe.md) - A comparison with Microsoft's official DataFrame library.

## References
- [Official Squirrel Documentation](https://github.com/sudipto80/Squirrel/blob/master/Docs/index.md)
- [Squirrel GitHub Repository](https://github.com/sudipto80/Squirrel)
- [Squirrel Demo Video](https://www.youtube.com/watch?v=jv1znNEq5h4)