using ReportIt.Api.Models;

namespace ReportItNG.Repository.Interfacaes
{
    public interface IReportRepository
    {
        Task<List<Report>> GetAllAsync();
        Task<Report?> GetByReferenceAsync(string reference);
        Task AddAsync(Report report);
        Task<bool> UpdateStatusAsync(string reference, ReportStatus status);
    }
}