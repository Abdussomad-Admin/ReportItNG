using Microsoft.AspNetCore.Mvc;
using ReportIt.Api.Models;
using ReportItNG.Repository.Interfacaes;

namespace ReportItNG.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportRepository _repository;

    public ReportsController(IReportRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Report>>> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{reference}")]
    public async Task<ActionResult<Report>> GetByReference(string reference)
    {
        var report = await _repository.GetByReferenceAsync(reference);

        if (report is null)
        {
            return NotFound();
        }

        return Ok(report);
    }

    [HttpPost]
    public async Task<ActionResult<Report>> Create(Report report)
    {
        report.Id = 0;
        report.Reference = $"RIT-{Guid.NewGuid():N}"[..12].ToUpperInvariant();
        report.Status = ReportStatus.Submitted;
        report.CreatedAtUtc = DateTime.UtcNow;

        await _repository.AddAsync(report);

        return CreatedAtAction(
            nameof(GetByReference),
            new { reference = report.Reference },
            report);
    }

    [HttpPatch("{reference}/status")]
    public async Task<IActionResult> UpdateStatus(
        string reference,
        [FromBody] ReportStatus status)
    {
        var updated = await _repository.UpdateStatusAsync(reference, status);

        return updated ? NoContent() : NotFound();
    }
}