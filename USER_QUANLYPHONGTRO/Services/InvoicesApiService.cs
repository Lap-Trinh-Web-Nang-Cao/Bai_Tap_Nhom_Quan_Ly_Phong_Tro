using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;
using USER_QUANLYPHONGTRO.Services.Interfaces;

namespace USER_QUANLYPHONGTRO.Services
{
 public interface IInvoicesApiService
 {
 Task<ApiResponse<List<TenantInvoiceViewModel>>> GetInvoicesByTenantAsync(Guid tenantId, string bearerToken = null);
 Task<ApiResponse<object>> PayInvoiceAsync(Guid invoiceId, string bearerToken = null);
 }

 public class InvoicesApiService : IInvoicesApiService
 {
 private readonly IApiClient _apiClient;

 public InvoicesApiService() : this(new ApiClient()) { }

 public InvoicesApiService(IApiClient apiClient)
 {
 _apiClient = apiClient;
 }

 public Task<ApiResponse<List<TenantInvoiceViewModel>>> GetInvoicesByTenantAsync(Guid tenantId, string bearerToken = null)
 {
 var userIdStr = System.Web.HttpContext.Current?.Session?["UserId"]?.ToString();
 if (!Guid.TryParse(userIdStr, out var nguoiThueId))
 {
 return Task.FromResult(ApiResponse<List<TenantInvoiceViewModel>>.ErrorResult("Missing UserId (Guid) in session"));
 }
 return _apiClient.GetAsync<List<TenantInvoiceViewModel>>($"/api/hoadon/nguoithue/{nguoiThueId}", bearerToken);
 }

 public Task<ApiResponse<object>> PayInvoiceAsync(Guid invoiceId, string bearerToken = null)
 {
 // API backend hi?n ch? tr? message
 return _apiClient.PostAsync<object, object>($"/api/hoadon/{invoiceId}/thanhtoan", new { }, bearerToken);
 }
 }
}
