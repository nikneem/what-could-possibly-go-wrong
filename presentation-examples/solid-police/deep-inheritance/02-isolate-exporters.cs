/*
Instead of forcing every report to inherit export logic, we extract
export functionality into strategies (interfaces). This makes the
system flexible and easier to extend.
*/


public interface IReportExporter
{
    string Export(Report report);
}

public class CsvExporter : IReportExporter
{
    public string Export(Report report)
    {
        return $"Title: {report.Title}, CreatedOn: {report.CreatedOn:yyyy-MM-dd}, Author: {report.Author}";
    }
}

public class JsonExporter : IReportExporter
{
    public string Export(Report report)
    {
        return System.Text.Json.JsonSerializer.Serialize(report);
    }
}

public class XmlExporter : IReportExporter
{
    public string Export(Report report)
    {
        return $"<Report><Title>{report.Title}</Title><CreatedOn>{report.CreatedOn:yyyy-MM-dd}</CreatedOn><Author>{report.Author}</Author></Report>";
    }
}
