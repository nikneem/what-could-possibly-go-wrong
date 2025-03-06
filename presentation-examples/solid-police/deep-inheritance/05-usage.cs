class Program
{
    static void Main()
    {
        var salesReport = new SalesReport { Title = "Q1 Sales", CreatedOn = DateTime.Now, Author = "John Doe" };
        salesReport.Generate();

        var inventoryReport = new InventoryReport { Title = "Stock Report", CreatedOn = DateTime.Now, Author = "Jane Smith" };
        inventoryReport.Generate();

        // Sales report exported to JSON
        salesReport.SetExporter(new JsonExporter());
        Console.WriteLine("Sales Report (JSON):");
        Console.WriteLine(salesReport.Export());

        // Inventory report exported to XML
        inventoryReport.SetExporter(new XmlExporter());
        Console.WriteLine("\nInventory Report (XML):");
        Console.WriteLine(inventoryReport.Export());

        // Sales report exported to CSV
        salesReport.SetExporter(new CsvExporter());
        Console.WriteLine("\nSales Report (CSV):");
        Console.WriteLine(salesReport.Export());
    }
}
