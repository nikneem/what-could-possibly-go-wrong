/*
20 Years of software development - What could possibly go wrong?

Below is a complicated example that demonstrates excessive inheritance
across multiple levels, leading to rigid and hard-to-maintain code.
The developer initially thought that having a structured hierarchy
would help, but now, adding new features (like an XML export)
has become a nightmare.

Problem: Deep Inheritance Tree Gone Wrong
The system is designed to generate different types of reports with
multiple levels of base classes. However, as more features are added,
the base classes become bloated, and changing anything at the top
level affects all derived classes, making maintenance a nightmare.
*/

// Top-level base class
public abstract class Document
{
    public string Title { get; set; }
    public DateTime CreatedOn { get; set; }

    public virtual void Print()
    {
        Console.WriteLine($"Printing Document: {Title}");
    }

    public abstract void Generate();
}

// Level 2: Extending Document into a Report
public abstract class Report : Document
{
    public string Author { get; set; }

    // Assume all reports should have a CSV export (bad assumption)
    public virtual string ExportToCsv()
    {
        return $"Title: {Title}, CreatedOn: {CreatedOn:yyyy-MM-dd}, Author: {Author}";
    }
}

// Level 3: Specific Reports
public class SalesReport : Report
{
    public List<string> SalesData { get; set; } = new();

    public override void Generate()
    {
        SalesData.Add("Sale1: $100");
        SalesData.Add("Sale2: $200");
    }

    public override string ExportToCsv()
    {
        return base.ExportToCsv() + $", Sales: {string.Join(", ", SalesData)}";
    }
}

public class InventoryReport : Report
{
    public Dictionary<string, int> StockLevels { get; set; } = new();

    public override void Generate()
    {
        StockLevels["ItemA"] = 10;
        StockLevels["ItemB"] = 5;
    }

    public override string ExportToCsv()
    {
        return base.ExportToCsv() + $", Stock Levels: {string.Join(", ", StockLevels.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}";
    }
}
