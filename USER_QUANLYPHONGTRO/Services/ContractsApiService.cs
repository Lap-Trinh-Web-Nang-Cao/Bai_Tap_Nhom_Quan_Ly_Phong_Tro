using System;
using System.Threading.Tasks;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;
using USER_QUANLYPHONGTRO.Services.Interfaces;

namespace USER_QUANLYPHONGTRO.Services
{
 public interface IContractsApiService
 {
 Task<ApiResponse<TenantContractViewModel>> GetActiveContractByTenantAsync(Guid tenantId, string bearerToken = null);
 }

 public class ContractsApiService : IContractsApiService
 {
 private readonly IApiClient _apiClient;

 public ContractsApiService() : this(new ApiClient()) { }

 public ContractsApiService(IApiClient apiClient)
 {
 _apiClient = apiClient;
 }

 public Task<ApiResponse<TenantContractViewModel>> GetActiveContractByTenantAsync(Guid tenantId, string bearerToken = null)
 {
 var userIdStr = System.Web.HttpContext.Current?.Session?["UserId"]?.ToString();
 if (!Guid.TryParse(userIdStr, out var nguoiThueId))
 {
 return Task.FromResult(ApiResponse<TenantContractViewModel>.ErrorResult("Missing UserId (Guid) in session"));
 }
 return _apiClient.GetAsync<TenantContractViewModel>($"/api/hopdong/nguoithue/{nguoiThueId}/hieuluc", bearerToken);
 }
 }
}
