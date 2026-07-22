using AquaTrack.Application.DTOs;

namespace AquaTrack.Application.Services;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
}
