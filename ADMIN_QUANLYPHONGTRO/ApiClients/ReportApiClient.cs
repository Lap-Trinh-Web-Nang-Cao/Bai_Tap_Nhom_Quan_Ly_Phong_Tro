using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    /// <summary>
    /// API Client cho quản lý báo cáo vi phạm
    /// </summary>
    public class ReportApiClient : BaseApiClient
    {
        private const string BASE_ENDPOINT = "api/baocaovipham";

        /// <summary>
        /// Lấy danh sách tất cả báo cáo vi phạm
        /// - Hỗ trợ nhiều hình thức response:
        ///   1) Mảng JSON trực tiếp [...]
        ///   2) Wrapped object with items / Items / data / Data
        ///   3) Paged object { items: [...], totalRecords: ... }
        /// </summary>
        public async Task<List<BaoCaoViPhamDto>> GetAllReports()
        {
            try
            {
                var token = await GetAsync<JToken>(BASE_ENDPOINT);
                if (token == null)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ ReportApiClient.GetAllReports: response token is null");
                    return new List<BaoCaoViPhamDto>();
                }

                System.Diagnostics.Debug.WriteLine($"🔍 ReportApiClient.GetAllReports - received token type: {token.Type}");

                // Case 1: API returned JArray directly
                if (token.Type == JTokenType.Array)
                {
                    var list = token.ToObject<List<BaoCaoViPhamDto>>() ?? new List<BaoCaoViPhamDto>();
                    System.Diagnostics.Debug.WriteLine($"✅ Parsed array response: {list.Count} reports");
                    return list;
                }

                // Case 2: API returned object — try common keys (case-insensitive)
                if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;

                    // Try common property names
                    var candidates = new[] { "items", "Items", "data", "Data" };
                    foreach (var key in candidates)
                    {
                        if (obj.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out JToken value) && value != null)
                        {
                            if (value.Type == JTokenType.Array)
                            {
                                var list = value.ToObject<List<BaoCaoViPhamDto>>() ?? new List<BaoCaoViPhamDto>();
                                System.Diagnostics.Debug.WriteLine($"✅ Parsed '{key}' array: {list.Count} reports");
                                return list;
                            }
                            // Sometimes data is wrapped deeper: { data: { items: [...] } }
                            if (value.Type == JTokenType.Object)
                            {
                                var inner = value["items"] ?? value["Items"] ?? value["data"] ?? value["Data"];
                                if (inner != null && inner.Type == JTokenType.Array)
                                {
                                    var list = inner.ToObject<List<BaoCaoViPhamDto>>() ?? new List<BaoCaoViPhamDto>();
                                    System.Diagnostics.Debug.WriteLine($"✅ Parsed nested '{key}.items': {list.Count} reports");
                                    return list;
                                }
                            }
                        }
                    }

                    // Fallback: try to find first array property
                    var firstArray = obj.Properties().FirstOrDefault(p => p.Value != null && p.Value.Type == JTokenType.Array);
                    if (firstArray != null)
                    {
                        var list = firstArray.Value.ToObject<List<BaoCaoViPhamDto>>() ?? new List<BaoCaoViPhamDto>();
                        System.Diagnostics.Debug.WriteLine($"✅ Parsed first array property '{firstArray.Name}': {list.Count} reports");
                        return list;
                    }

                    // Finally, maybe the object itself maps to single DTO -> wrap it
                    try
                    {
                        var single = obj.ToObject<BaoCaoViPhamDto>();
                        if (single != null)
                        {
                            System.Diagnostics.Debug.WriteLine("✅ Parsed single object response into one report (wrapped into list)");
                            return new List<BaoCaoViPhamDto> { single };
                        }
                    }
                    catch { /* ignore */ }
                }

                System.Diagnostics.Debug.WriteLine("⚠️ ReportApiClient.GetAllReports: Unable to parse response -> returning empty list");
                return new List<BaoCaoViPhamDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportApiClient.GetAllReports Error: {ex.Message}");
                return new List<BaoCaoViPhamDto>();
            }
        }

        /// <summary>
        /// Lấy danh sách báo cáo với phân trang
        /// - Hỗ trợ cả response có trường Items/items/data và trực tiếp trả về mảng
        /// </summary>
        public async Task<PagedResult<BaoCaoViPhamDto>> GetReports(int pageIndex, int pageSize, string keyword = "", string status = "")
        {
            try
            {
                var url = string.Format("{0}?pageIndex={1}&pageSize={2}", BASE_ENDPOINT, pageIndex, pageSize);
                if (!string.IsNullOrEmpty(keyword))
                    url += "&keyword=" + Uri.EscapeDataString(keyword);
                if (!string.IsNullOrEmpty(status))
                    url += "&trangThai=" + Uri.EscapeDataString(status);

                var token = await GetAsync<JToken>(url);
                if (token == null)
                    return new PagedResult<BaoCaoViPhamDto> { Items = new List<BaoCaoViPhamDto>(), TotalRecords = 0, PageIndex = pageIndex, PageSize = pageSize };

                // If response is an array
                if (token.Type == JTokenType.Array)
                {
                    var arrayItems = token.ToObject<List<BaoCaoViPhamDto>>() ?? new List<BaoCaoViPhamDto>();
                    return new PagedResult<BaoCaoViPhamDto>
                    {
                        Items = arrayItems,
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TotalRecords = arrayItems.Count
                    };
                }

                // If response is object
                var obj = token as JObject;
                var itemsToken = obj?["Items"] ?? obj?["items"] ?? obj?["data"] ?? obj?["Data"];
                var items = itemsToken != null && itemsToken.Type == JTokenType.Array
                    ? itemsToken.ToObject<List<BaoCaoViPhamDto>>()
                    : new List<BaoCaoViPhamDto>();

                // Try get totalRecords / TotalRecords / totalCount
                var totalToken = obj?["TotalRecords"] ?? obj?["totalRecords"] ?? obj?["totalCount"] ?? obj?["TotalCount"];
                int total = 0;
                if (totalToken != null && int.TryParse(totalToken.ToString(), out var t))
                    total = t;
                else
                    total = items?.Count ?? 0;

                return new PagedResult<BaoCaoViPhamDto>
                {
                    Items = items ?? new List<BaoCaoViPhamDto>(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = total
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportApiClient.GetReports Error: {ex.Message}");
                return new PagedResult<BaoCaoViPhamDto>
                {
                    Items = new List<BaoCaoViPhamDto>(),
                    TotalRecords = 0,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
            }
        }

        /// <summary>
        /// Lấy chi tiết một báo cáo
        /// - Sử dụng fallback: nếu API trả wrapper { success:true, data: {...} } thì lấy data
        /// </summary>
        public async Task<BaoCaoViPhamDto> GetReportById(string id)
        {
            try
            {
                // Try direct deserialization first
                try
                {
                    var direct = await GetAsync<BaoCaoViPhamDto>(string.Format("{0}/{1}", BASE_ENDPOINT, id));
                    if (direct != null) return direct;
                }
                catch
                {
                    // ignore, fallback to token parsing below
                }

                var token = await GetAsync<JToken>(string.Format("{0}/{1}", BASE_ENDPOINT, id));
                if (token == null) return null;

                if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;
                    var data = obj["data"] ?? obj["Data"] ?? obj["payload"];
                    if (data != null && data.Type == JTokenType.Object)
                        return data.ToObject<BaoCaoViPhamDto>();
                    // If object maps directly to DTO
                    return obj.ToObject<BaoCaoViPhamDto>();
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportApiClient.GetReportById Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Xử lý báo cáo (đánh dấu đã xử lý)
        /// </summary>
        public async Task<ApiResponse<bool>> ResolveReport(string id, string ketQua = "Đã xử lý vi phạm")
        {
            try
            {
                var request = new { id = id, ketQua = ketQua };
                var response = await PostAsync<dynamic>("api/baocaovipham/resolve", request);
                
                if (response != null && response.success == true)
                {
                    return new ApiResponse<bool> { Success = true, Message = "Xử lý thành công" };
                }
                
                return new ApiResponse<bool> { Success = false, Message = response?.message ?? "Không thể xử lý báo cáo" };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ ReportApiClient.ResolveReport Error: {0}", ex.Message));
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Từ chối báo cáo
        /// </summary>
        public async Task<ApiResponse<bool>> RejectReport(string id, string lyDo)
        {
            try
            {
                var request = new { id = id, lyDo = lyDo };
                var response = await PostAsync<dynamic>("api/baocaovipham/reject", request);
                
                if (response != null && response.success == true)
                {
                    return new ApiResponse<bool> { Success = true, Message = "Đã từ chối báo cáo" };
                }
                
                return new ApiResponse<bool> { Success = false, Message = response?.message ?? "Không thể từ chối báo cáo" };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ ReportApiClient.RejectReport Error: {0}", ex.Message));
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Xóa báo cáo
        /// </summary>
        public async Task<bool> DeleteReport(string id)
        {
            try
            {
                var request = new { id = id };
                var response = await PostAsync<dynamic>("api/baocaovipham/delete", request);
                return response != null && response.success == true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ ReportApiClient.DeleteReport Error: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Cập nhật trạng thái báo cáo
        /// </summary>
        public async Task<ApiResponse<bool>> UpdateStatus(string id, string trangThai, string chiTiet = "")
        {
            try
            {
                // Use PostAsync for start action (status = DANG_XU_LY)
                if (trangThai == "DANG_XU_LY")
                {
                    var request = new { id = id };
                    var response = await PostAsync<dynamic>("api/baocaovipham/start", request);
                    
                    if (response != null && response.success == true)
                    {
                        return new ApiResponse<bool> { Success = true, Message = $"Đã cập nhật trạng thái" };
                    }
                    
                    return new ApiResponse<bool> { Success = false, Message = response?.message ?? "Không thể cập nhật trạng thái" };
                }

                // For other status changes, use resolve/reject
                return new ApiResponse<bool> { Success = false, Message = "Không hỗ trợ cập nhật trạng thái này" };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ ReportApiClient.UpdateStatus Error: {0}", ex.Message));
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Lấy danh sách loại vi phạm
        /// </summary>
        public async Task<List<ViPhamDto>> GetViolationTypes()
        {
            try
            {
                var result = await GetAsync<List<ViPhamDto>>("api/vipham");
                return result != null ? result : new List<ViPhamDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ ReportApiClient.GetViolationTypes Error: {0}", ex.Message));
                return new List<ViPhamDto>();
            }
        }

        /// <summary>
        /// Lấy thống kê báo cáo từ Backend API
        /// GET: /api/baocaovipham/statistics
        /// </summary>
        public async Task<ReportStatistics> GetStatistics()
        {
            try
            {
                // Call Backend API: GET /api/baocaovipham/statistics
                var token = await GetAsync<JToken>("api/baocaovipham/statistics");
                if (token == null)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ ReportApiClient.GetStatistics: response token is null");
                    return new ReportStatistics { TotalReports = 0, PendingReports = 0, ProcessingReports = 0, ResolvedReports = 0, RejectedReports = 0 };
                }

                System.Diagnostics.Debug.WriteLine($"🔍 ReportApiClient.GetStatistics - received token type: {token.Type}");

                if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;
                    
                    // Extract data from response wrapper
                    var dataToken = obj["data"] ?? obj["Data"];
                    if (dataToken != null && dataToken.Type == JTokenType.Object)
                    {
                        var data = (JObject)dataToken;
                        
                        System.Diagnostics.Debug.WriteLine($"✅ Parsing statistics data: {dataToken}");

                        var stats = new ReportStatistics
                        {
                            TotalReports = data["tongSoBaoCao"]?.Value<int>() ?? 
                                          data["TotalReports"]?.Value<int>() ?? 0,
                            PendingReports = data["choXuLy"]?.Value<int>() ?? 
                                            data["PendingReports"]?.Value<int>() ?? 0,
                            ProcessingReports = data["dangXuLy"]?.Value<int>() ?? 
                                               data["ProcessingReports"]?.Value<int>() ?? 0,
                            ResolvedReports = data["daXuLy"]?.Value<int>() ?? 
                                             data["ResolvedReports"]?.Value<int>() ?? 0,
                            RejectedReports = data["tuChoi"]?.Value<int>() ?? 
                                             data["RejectedReports"]?.Value<int>() ?? 0
                        };

                        System.Diagnostics.Debug.WriteLine($"✅ Statistics parsed: Total={stats.TotalReports}, Pending={stats.PendingReports}, Processing={stats.ProcessingReports}, Resolved={stats.ResolvedReports}, Rejected={stats.RejectedReports}");

                        return stats;
                    }
                }

                System.Diagnostics.Debug.WriteLine("⚠️ ReportApiClient.GetStatistics: Unable to parse response -> returning empty stats");
                return new ReportStatistics { TotalReports = 0, PendingReports = 0, ProcessingReports = 0, ResolvedReports = 0, RejectedReports = 0 };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportApiClient.GetStatistics Error: {ex.Message}");
                return new ReportStatistics { TotalReports = 0, PendingReports = 0, ProcessingReports = 0, ResolvedReports = 0, RejectedReports = 0 };
            }
        }
    }
}
