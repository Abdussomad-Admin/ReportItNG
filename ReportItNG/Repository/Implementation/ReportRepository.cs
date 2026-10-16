using Microsoft.EntityFrameworkCore;
using ReportItNG.Context;
using ReportItNG.Domain.Entites;
using ReportItNG.Repository.Interfacaes;

namespace ReportItNG.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ReportItDbContext _context;

    public ReportRepository(ReportItDbContext context)
    {
        _context = context;
    }

    public async Task<List<Report>> GetAllAsync()
    {
        return await _context.Reports
            .OrderByDescending(report => report.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<Report?> GetByReferenceAsync(string reference)
    {
        return await _context.Reports
            .FirstOrDefaultAsync(report => report.Reference == reference);
    }

    public async Task AddAsync(Report report)
    {
        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateStatusAsync(
        string reference,
        ReportStatus status)
    {
        var report = await GetByReferenceAsync(reference);

        if (report is null)
        {
            return false;
        }

        report.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }
}