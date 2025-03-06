public abstract class Report : Document
{
    public string Author { get; set; }

    // Instead of enforcing an export method, use a strategy pattern
    private IReportExporter _exporter;

    public void SetExporter(IReportExporter exporter)
    {
        _exporter = exporter;
    }

    public string Export()
    {
        if (_exporter == null)
            throw new InvalidOperationException("No export strategy set!");

        return _exporter.Export(this);
    }
}
