/*
Result?
We've split responsibilities.
BUT we now have three classes for one simple operation.
*/



// Pure data model (now Invoice does "only one thing")
public class Invoice
{
    public string Customer { get; set; }
    public decimal Amount { get; set; }
}

// Separate responsibility for printing
public class InvoicePrinter
{
    public void Print(Invoice invoice)
    {
        Console.WriteLine($"Invoice for {invoice.Customer}: ${invoice.Amount}");
    }
}

// Separate responsibility for persistence
public class InvoiceRepository
{
    public void SaveToDatabase(Invoice invoice)
    {
        Console.WriteLine("Saving invoice to database...");
    }
}