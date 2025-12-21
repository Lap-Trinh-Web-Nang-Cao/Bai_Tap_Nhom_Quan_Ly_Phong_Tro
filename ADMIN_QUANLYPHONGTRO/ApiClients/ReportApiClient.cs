using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
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
        private const string BASE_ENDPOINT = "baocaovipham";

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
                var request = new XuLyBaoCaoRequest
                {
                    TrangThai = "DA_XU_LY",
                    KetQua = ketQua,
                    NguoiXuLy = null // TODO: Lấy từ session admin
                };

                var result = await PutAsync<BaoCaoViPhamDto>(string.Format("{0}/{1}", BASE_ENDPOINT, id), request);
                return new ApiResponse<bool>
                {
                    Success = result != null,
                    Message = result != null ? "Xử lý thành công" : "Không thể xử lý báo cáo"
                };
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
                var request = new XuLyBaoCaoRequest
                {
                    TrangThai = "TU_CHOI",
                    KetQua = lyDo,
                    NguoiXuLy = null // TODO: Lấy từ session admin
                };

                var result = await PutAsync<BaoCaoViPhamDto>(string.Format("{0}/{1}", BASE_ENDPOINT, id), request);
                return new ApiResponse<bool>
                {
                    Success = result != null,
                    Message = result != null ? "Đã từ chối báo cáo" : "Không thể từ chối báo cáo"
                };
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
                return await DeleteAsync(string.Format("{0}/{1}", BASE_ENDPOINT, id));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ ReportApiClient.DeleteReport Error: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách loại vi phạm
        /// </summary>
        public async Task<List<ViPhamDto>> GetViolationTypes()
        {
            try
            {
                var result = await GetAsync<List<ViPhamDto>>("vipham");
                return result != null ? result : new List<ViPhamDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ ReportApiClient.GetViolationTypes Error: {0}", ex.Message));
                return new List<ViPhamDto>();
            }
        }
    }
}
