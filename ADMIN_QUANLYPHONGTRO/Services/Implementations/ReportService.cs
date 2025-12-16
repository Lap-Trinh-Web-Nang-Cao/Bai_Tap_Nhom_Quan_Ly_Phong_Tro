using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;

namespace ADMIN_QUANLYPHONGTRO.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ReportApiClient _apiClient;

        public ReportService()
        {
            _apiClient = new ReportApiClient();
        }

        public Task<PagedResult<BaoCaoViPhamDto>> GetReportsAsync(int pageIndex, int pageSize)
        {
            return _apiClient.GetReports(pageIndex, pageSize);
        }

        public Task<ApiResponse<bool>> ResolveReportAsync(string baoCaoId)
        {
            return _apiClient.ResolveReport(baoCaoId);
        }
    }
}
