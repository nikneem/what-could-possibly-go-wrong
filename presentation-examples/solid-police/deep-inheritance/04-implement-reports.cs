public class SalesReport : Report
{
    public List<string> SalesData { get; set; } = new();

    public override void Generate()
    {
        SalesData.Add("Sale1: $100");
        SalesData.Add("Sale2: $200");
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
}
