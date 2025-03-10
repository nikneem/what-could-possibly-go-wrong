class Program
{
    static void Main()
    {
        var salesReport = new SalesReport
        {
            Title = "Q1 Sales",
            CreatedOn = DateTime.Now,
            Author = "John Doe"
        };
        salesReport.Generate();

        var inventoryReport = new InventoryReport
        {
            Title = "Stock Report",
            CreatedOn = DateTime.Now,
            Author = "Jane Smith"
        };
        inventoryReport.Generate();

        // Sales report exported to JSON
        Console.WriteLine("Sales Report (JSON):");
        Console.WriteLine(salesReport.Export(new JsonExporter()));

        // Inventory report exported to XML
        Console.WriteLine("\nInventory Report (XML):");
        Console.WriteLine(inventoryReport.Export(new XmlExporter()));

        // Sales report exported to CSV
        Console.WriteLine("\nSales Report (CSV):");
        Console.WriteLine(salesReport.Export(new CsvExporter()));
    }
}
