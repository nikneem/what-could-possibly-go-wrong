public abstract class Report : Document
{
    public string Author { get; set; }

    // Instead of enforcing an export method, use a strategy pattern

    public string Export(IExporter exporter)
    {
        if (exporter == null)
            throw new InvalidOperationException("No export strategy set!");

        return exporter.Export(this);
    }
}
