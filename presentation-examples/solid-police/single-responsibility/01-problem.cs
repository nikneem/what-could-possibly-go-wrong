/*
The Single Responsibility Principle (SRP) in SOLID is often interpreted
too strictly, and in reality, true single responsibility rarely exists.
Almost every function or class implicitly has multiple responsibilities,
making SRP more of a guideline rather than an absolute rule.

Let's break it down with an example where enforcing "true" SRP makes
things worse and where practicality beats purity.

A common example of SRP states that a class should have only one reason
to change. But what does a "reason to change" mean? Consider a simple
Invoice class:

According to strict SRP
Printing is a UI concern → Should be in a different class.
Saving is a persistence concern → Should be in a different class.
*/

public class Invoice
{
    public string Customer { get; set; }
    public decimal Amount { get; set; }

    public void Print()
    {
        Console.WriteLine($"Invoice for {Customer}: ${Amount}");
    }

    public void SaveToDatabase()
    {
        Console.WriteLine("Saving invoice to database...");
    }
}


