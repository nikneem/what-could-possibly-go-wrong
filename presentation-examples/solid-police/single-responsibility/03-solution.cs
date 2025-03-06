/*
The Real Issue: SRP Is About "Cohesion," Not "Strict Responsibility"
Strict SRP doesn't work in practice because:
    A function/class inevitably does multiple things.
    Splitting too much leads to fragmentation and harder maintenance.
    Cohesion matters more than blindly avoiding multiple responsibilities.

Takeaway: Don't Overdo SRP—Think Cohesion Instead
Instead of splitting everything into tiny classes, focus on:
    Grouping responsibilities that naturally belong together.
    Keeping code easy to change without unnecessary complexity.
    Avoiding rigid SRP enforcement when it hurts readability.

SRP isn't a law—it’s a guideline. Be pragmatic, not dogmatic!
*/

public class InvoiceService
{
    private readonly InvoiceRepository _repository;

    public InvoiceService(InvoiceRepository repository)
    {
        _repository = repository;
    }

    public void ProcessInvoice(Invoice invoice)
    {
        _repository.SaveToDatabase(invoice);
        Console.WriteLine($"Invoice for {invoice.Customer}: ${invoice.Amount} printed.");
    }
}