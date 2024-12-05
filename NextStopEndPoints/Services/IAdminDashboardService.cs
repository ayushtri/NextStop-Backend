using NextStopEndPoints.DTOs;

namespace NextStopEndPoints.Services
{
    public interface IAdminDashboardService
    {
        Task<bool> AssignRole(AssignRoleDTO assignRoleDto);
        Task<ReportDTO> GenerateReports(GenerateReportsDTO generateReportsDto);
    }
}
