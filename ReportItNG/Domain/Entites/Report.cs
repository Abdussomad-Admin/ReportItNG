namespace ReportIt.Api.Models;

public enum ReportCategory
{
    Pothole,
    Waste,
    BrokenStreetlight,
    WaterLeak
}

public enum ReportStatus
{
    Submitted,
    InProgress,
    Resolved
}

public class Report
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportCategory Category { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? ImagePath { get; set; }

    public ReportStatus Status { get; set; } = ReportStatus.Submitted;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}