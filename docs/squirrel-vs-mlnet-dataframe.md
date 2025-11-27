# Squirrel (TableAPI) vs. Microsoft.Data.Analysis (ML.NET DataFrame)

This report compares **Squirrel (TableAPI)** with Microsoft's official **Microsoft.Data.Analysis (DataFrame)** library, often used in conjunction with ML.NET.

## Executive Summary

*   **Squirrel (TableAPI)** is a **high-level, business-centric** library designed for .NET engineers who need to perform ETL, data cleaning, and reporting with minimal boilerplate. It prioritizes developer ergonomics (fluent API, LINQ integration) over raw performance.
*   **Microsoft.Data.Analysis (DataFrame)** is a **low-level, performance-centric** library designed as a foundation for machine learning pipelines. It prioritizes memory efficiency and speed (using Apache Arrow memory layout) but has a steeper learning curve and a more verbose API for common tasks.

---

## Feature Comparison

| Feature | Squirrel (TableAPI) | Microsoft.Data.Analysis (DataFrame) |
| :--- | :--- | :--- |
| **Primary Use Case** | ETL, Data Cleaning, Reporting, Business Logic | Machine Learning Prep, High-Performance Compute |
| **API Style** | Fluent, LINQ-heavy, "Business Readable" | Vectorized, Column-based, Pandas-like (but verbose) |
| **Data Cleaning** | **Excellent** (Built-in `RemoveOutliers`, `Anonymize`, `Normalize`) | **Basic** (Manual filtering/imputation required) |
| **Type System** | Hybrid (Dynamic + Strong `RecordTable<T>`) | Strictly Columnar (`PrimitiveDataFrameColumn<T>`) |
| **SQL Integration** | Native `ToSqlTable()`, `LoadDataTable()` | Requires manual mapping or external libraries |
| **Visualization** | Built-in Google Charts & Console `.PrettyDump()` | None (requires external tools like XPlot) |
| **ML.NET Integration**| Can export to CSV/Collections for ML.NET | **Native** (Implements `IDataView` directly) |

---

## Detailed Analysis

### 1. Usability & Developer Experience
*   **Squirrel**: Designed to be "business readable." Methods like `.RemoveIfBetween("Age", 0, 18)` or `.Anonymize("Email")` are self-explanatory. It feels like writing English instructions for data manipulation.
*   **ML.NET DataFrame**: Feels like writing low-level array operations. Simple tasks often require understanding column types (`PrimitiveDataFrameColumn<float>`) and vectorized math.
    *   *Squirrel*: `data.AddColumn("Total", "[Price] * [Qty]")`
    *   *ML.NET*: `df["Total"] = df["Price"].Multiply(df["Qty"])` (Requires columns to be compatible numeric types)

### 2. Data Cleaning Capabilities
*   **Squirrel**: This is Squirrel's "killer feature." It includes a massive suite of built-in cleaning functions:
    *   `RemoveOutliers()` (IQR, Z-Score)
    *   `Normalize()` (Casing, trimming)
    *   `Anonymize()` (PII masking)
    *   `RemoveIncompleteRows()`
*   **ML.NET DataFrame**: Provides the *building blocks* for cleaning (filtering, filling nulls), but you have to build the logic yourself. There is no one-line "remove outliers" method; you would calculate the IQR manually and apply a filter mask.

### 3. Performance & Scalability
*   **Squirrel**: Operates primarily on `List<Dictionary<string, object>>` or `List<T>` under the hood (unless using `RecordTable`). It is fast enough for typical business datasets (thousands to hundreds of thousands of rows) but may incur GC overhead on millions of rows.
*   **ML.NET DataFrame**: Built on top of the Apache Arrow memory specification. It is extremely memory-efficient and cache-friendly. It is the better choice for datasets with millions of rows or when memory pressure is a concern.

### 4. Integration with ML.NET
*   **Squirrel**: To use with ML.NET, you typically export to CSV or an `IEnumerable<T>` and then load it into an `MLContext`.
*   **ML.NET DataFrame**: Implements `IDataView` natively. You can pass a `DataFrame` object directly into an ML.NET training pipeline (`mlContext.Model.Train(...)`), avoiding disk I/O or intermediate object allocation.

---

## Code Comparison: Filtering & Calculation

**Scenario**: Load data, filter rows where Price > 100, and add a Tax column (10%).

**Squirrel**
```csharp
var data = DataAcquisition.LoadCsv("data.csv");

var result = data
    .RemoveLessThanOrEqualTo("Price", 100)
    .AddColumn("Tax", "[Price] * 0.10");

result.PrettyDump();
```

**Microsoft.Data.Analysis**
```csharp
using Microsoft.Data.Analysis;

var df = DataFrame.LoadCsv("data.csv");

// Filter requires creating a boolean column mask
var priceCol = df.Columns["Price"];
var mask = priceCol.ElementwiseGreaterThan(100);
var filtered = df.Filter(mask);

// Calculation requires explicit column math
// Note: You often need to cast columns to specific types (e.g., float/decimal) first
filtered["Tax"] = filtered["Price"].Multiply(0.10f);

Console.WriteLine(filtered);
```

---

## Verdict

*   **Choose Squirrel (TableAPI) if**:
    *   You are building **ETL pipelines**, **importers**, or **business reports**.
    *   You need to **clean messy data** (deduplication, normalization, outlier removal).
    *   You value **readability** and **development speed** over raw execution speed.
    *   You are working with "small data" (Excel sheets, CSV exports, typical SQL query results).

*   **Choose Microsoft.Data.Analysis if**:
    *   You are building a **high-performance Machine Learning pipeline** with ML.NET.
    *   You are working with **large datasets** (millions of rows) where memory layout matters.
    *   You need to perform complex **vectorized mathematics** or statistical simulations.
    *   You are already deeply invested in the ML.NET ecosystem.
