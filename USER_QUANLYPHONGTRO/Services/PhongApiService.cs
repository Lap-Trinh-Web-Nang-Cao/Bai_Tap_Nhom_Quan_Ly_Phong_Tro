using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;

namespace USER_QUANLYPHONGTRO.Services
{
    /// <summary>
    /// Interface để gọi API phòng từ Backend
    /// </summary>
    public interface IPhongApiService
    {
        /// <summary>
        /// Lấy danh sách phòng công khai (có phân trang)
        /// </summary>
        Task<(List<PhongDto> rooms, int totalCount, int totalPages)> GetPublicRoomsAsync(
            int page = 1,
            int pageSize = 10,
            Guid? nhaTroId = null,
            long? minPrice = null,
            long? maxPrice = null);

        /// <summary>
        /// Lấy chi tiết một phòng
        /// </summary>
        Task<PhongDto> GetRoomDetailAsync(Guid phongId);
    }

    /// <summary>
    /// Thực hiện các cuộc gọi API phòng
    /// </summary>
    public class PhongApiService : IPhongApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public PhongApiService()
        {
            // ===== SSL/TLS Configuration =====
            // Cho phép TLS 1.2 (required cho .NET Framework 4.7.2)
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // Bỏ qua SSL certificate validation (CHỈ CHO DEVELOPMENT)
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) =>
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ SSL Certificate Check - Errors: {sslPolicyErrors}");
                    return true; // Accept all certificates for dev
                };

            // ===== HttpClient Configuration =====
            var handler = new HttpClientHandler
            {
                // Bỏ qua SSL validation ở handler level
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Handler SSL Check - Errors: {errors}");
                    return true;
                }
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            // Lấy URL API từ config
            _apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"]
                ?? "https://localhost:7039";

            System.Diagnostics.Debug.WriteLine($"✅ PhongApiService initialized");
            System.Diagnostics.Debug.WriteLine($"   ApiBaseUrl: {_apiBaseUrl}");
            System.Diagnostics.Debug.WriteLine($"   TLS Protocol: {ServicePointManager.SecurityProtocol}");
        }
        private static DateTimeOffset? ReadDateTimeOffset(JObject obj, string camelName, string pascalName)
        {
            var token = obj[camelName] ?? obj[pascalName];
            if (token == null || token.Type == JTokenType.Null) return null;

            // Newtonsoft can materialize ISO strings as DateTime.
            if (token.Type == JTokenType.Date)
            {
                var dt = token.Value<DateTime>();
                // Assume local kind when unspecified; keep consistent
                return new DateTimeOffset(dt);
            }

            var s = token.ToString();
            if (string.IsNullOrWhiteSpace(s)) return null;

            if (DateTimeOffset.TryParse(s, out var dto)) return dto;
            if (DateTime.TryParse(s, out var d)) return new DateTimeOffset(d);

            return null;
        }

        /// <summary>
        /// Lấy danh sách phòng công khai (phân trang)
        /// </summary>
        public async Task<(List<PhongDto> rooms, int totalCount, int totalPages)> GetPublicRoomsAsync(
            int page = 1,
            int pageSize = 10,
            Guid? nhaTroId = null,
            long? minPrice = null,
            long? maxPrice = null)
        {
            try
            {
                // Xây dựng URL với query parameters
                var url = $"{_apiBaseUrl}/api/phong?page={page}&pageSize={pageSize}";

                if (nhaTroId.HasValue)
                    url += $"&nhaTroId={nhaTroId}";

                if (minPrice.HasValue)
                    url += $"&minPrice={minPrice}";

                if (maxPrice.HasValue)
                    url += $"&maxPrice={maxPrice}";

                System.Diagnostics.Debug.WriteLine($"🔗 Calling API: {url}");

                // Gửi request
                var response = await _httpClient.GetAsync(url);

                System.Diagnostics.Debug.WriteLine($"📡 API Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"❌ API Error ({response.StatusCode}): {errorContent}");
                    return (new List<PhongDto>(), 0, 0);
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"✅ API Response Content Length: {jsonContent.Length}");
                System.Diagnostics.Debug.WriteLine($"📋 API Response JSON (first 500 chars): {jsonContent.Substring(0, Math.Min(500, jsonContent.Length))}");

                var root = JObject.Parse(jsonContent);

                // API shape: { success, data: { data: [...], totalCount, totalPages, ... }, message }
                var dataObj = (JObject)(root["data"] ?? root["Data"]);
                if (dataObj == null)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No data object in API response");
                    return (new List<PhongDto>(), 0, 0);
                }

                var items = (JArray)(dataObj["data"] ?? dataObj["Data"]);
                var rooms = new List<PhongDto>();

                if (items != null)
                {
                    System.Diagnostics.Debug.WriteLine($"📦 Found {items.Count} items in response");

                    foreach (var token in items)
                    {
                        if (!(token is JObject room)) continue;

                        try
                        {
                            var phongDto = new PhongDto
                            {
                                PhongId = Guid.Parse(room.Value<string>("phongId") ?? room.Value<string>("PhongId")),
                                NhaTroId = Guid.Parse(room.Value<string>("nhaTroId") ?? room.Value<string>("NhaTroId")),
                                TieuDe = room.Value<string>("tieuDe") ?? room.Value<string>("TieuDe") ?? "",
                                DienTich = room["dienTich"]?.Value<decimal?>() ?? room["DienTich"]?.Value<decimal?>(),
                                GiaTien = room["giaTien"]?.Value<long>() ?? room["GiaTien"]?.Value<long>() ?? 0,
                                TienCoc = room["tienCoc"]?.Value<long?>() ?? room["TienCoc"]?.Value<long?>(),
                                SoNguoiToiDa = room["soNguoiToiDa"]?.Value<int?>() ?? room["SoNguoiToiDa"]?.Value<int?>(),
                                TrangThai = room.Value<string>("trangThai") ?? room.Value<string>("TrangThai") ?? "",
                                DiemTrungBinh = room["diemTrungBinh"]?.Value<double?>() ?? room["DiemTrungBinh"]?.Value<double?>(),
                                SoLuongDanhGia = room["soLuongDanhGia"]?.Value<int?>() ?? room["SoLuongDanhGia"]?.Value<int?>(),
                                HinhAnhDaiDien = room.Value<string>("hinhAnhDaiDien") ?? room.Value<string>("HinhAnhDaiDien") ?? "/images/room-placeholder.jpg",
                                IsDuyet = room["isDuyet"]?.Value<bool?>() ?? room["IsDuyet"]?.Value<bool?>() ?? false,
                                IsBiKhoa = room["isBiKhoa"]?.Value<bool?>() ?? room["IsBiKhoa"]?.Value<bool?>() ?? false,
                                IsDeleted = room["isDeleted"]?.Value<bool?>() ?? room["IsDeleted"]?.Value<bool?>() ?? false,
                                CreatedAt = ReadDateTimeOffset(room, "createdAt", "CreatedAt"),
                                UpdatedAt = ReadDateTimeOffset(room, "updatedAt", "UpdatedAt")
                            };

                            var nhaTro = room["nhaTro"] as JObject ?? room["NhaTro"] as JObject;
                            if (nhaTro != null)
                            {
                                phongDto.NhaTro = new NhaTroDto
                                {
                                    NhaTroId = Guid.Parse(nhaTro.Value<string>("nhaTroId") ?? nhaTro.Value<string>("NhaTroId")),
                                    TieuDe = nhaTro.Value<string>("tieuDe") ?? nhaTro.Value<string>("TieuDe") ?? "",
                                    DiaChi = nhaTro.Value<string>("diaChi") ?? nhaTro.Value<string>("DiaChi")
                                };
                            }

                            rooms.Add(phongDto);
                        }
                        catch (Exception itemEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ Error mapping room item: {itemEx.Message}");
                            System.Diagnostics.Debug.WriteLine($"   Details: {itemEx.StackTrace}");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No data array in API response");
                }

                int totalCount = dataObj["totalCount"]?.Value<int>() ?? dataObj["TotalCount"]?.Value<int>() ?? 0;
                int totalPages = dataObj["totalPages"]?.Value<int>() ?? dataObj["TotalPages"]?.Value<int>() ?? 1;

                System.Diagnostics.Debug.WriteLine($"✅ GetPublicRoomsAsync Success: {rooms.Count} rooms, Total: {totalCount}, Pages: {totalPages}");
                return (rooms, totalCount, totalPages);
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HTTP Error in GetPublicRoomsAsync: {httpEx.Message}");
                System.Diagnostics.Debug.WriteLine($"   Inner Exception: {httpEx.InnerException?.Message}");

                // Log more details
                if (httpEx.InnerException is System.Net.Sockets.SocketException sockEx)
                {
                    System.Diagnostics.Debug.WriteLine($"   Socket Error Code: {sockEx.ErrorCode}");
                }

                return (new List<PhongDto>(), 0, 0);
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Request Timeout in GetPublicRoomsAsync: {ex.Message}");
                return (new List<PhongDto>(), 0, 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error calling Phong API: {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                return (new List<PhongDto>(), 0, 0);
            }
        }

        /// <summary>
        /// Lấy chi tiết một phòng
        /// </summary>
        public async Task<PhongDto> GetRoomDetailAsync(Guid phongId)
        {
            try
            {
                var url = $"{_apiBaseUrl}/api/phong/{phongId}";
                System.Diagnostics.Debug.WriteLine($"🔗 Calling API: {url}");
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ API Error ({response.StatusCode})");
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"📥 GetRoomDetail Response Length: {jsonContent.Length}");
                
                var root = JObject.Parse(jsonContent);

                // ✅ Backend có thể trả về cả 2 format:
                // 1. Wrapped: { success, data: {...} }
                // 2. Direct: { phongId, nhaTroId, ... }
                
                JObject room = null;
                
                // Check if response has "success" field (wrapped format)
                var hasSuccessField = root["success"] != null || root["Success"] != null;
                
                if (hasSuccessField)
                {
                    // Wrapped format
                    var dataToken = root["data"] ?? root["Data"];
                    if (dataToken != null && dataToken.Type != JTokenType.Null && dataToken is JObject)
                    {
                        room = (JObject)dataToken;
                        System.Diagnostics.Debug.WriteLine("📦 Using wrapped format (data property)");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("❌ GetRoomDetailAsync: missing Data in wrapped response");
                        return null;
                    }
                }
                else
                {
                    // Direct format - check if root has phongId/PhongId field
                    if (root["phongId"] != null || root["PhongId"] != null)
                    {
                        room = root;
                        System.Diagnostics.Debug.WriteLine("📦 Using direct format (root object)");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("❌ GetRoomDetailAsync: cannot find room data in response");
                        System.Diagnostics.Debug.WriteLine($"📋 Response preview: {jsonContent.Substring(0, Math.Min(300, jsonContent.Length))}");
                        return null;
                    }
                }

                var phongIdStr = room.Value<string>("phongId") ?? room.Value<string>("PhongId");
                var nhaTroIdStr = room.Value<string>("nhaTroId") ?? room.Value<string>("NhaTroId");
                if (string.IsNullOrWhiteSpace(phongIdStr) || string.IsNullOrWhiteSpace(nhaTroIdStr))
                {
                    System.Diagnostics.Debug.WriteLine("❌ GetRoomDetailAsync: invalid ids in response");
                    return null;
                }

                var phongDto = new PhongDto
                {
                    PhongId = Guid.Parse(phongIdStr),
                    NhaTroId = Guid.Parse(nhaTroIdStr),
                    TieuDe = room.Value<string>("tieuDe") ?? room.Value<string>("TieuDe") ?? "",
                    DienTich = room["dienTich"]?.Value<decimal?>() ?? room["DienTich"]?.Value<decimal?>(),
                    GiaTien = room["giaTien"]?.Value<long>() ?? room["GiaTien"]?.Value<long>() ?? 0,
                    TienCoc = room["tienCoc"]?.Value<long?>() ?? room["TienCoc"]?.Value<long?>(),
                    SoNguoiToiDa = room["soNguoiToiDa"]?.Value<int?>() ?? room["SoNguoiToiDa"]?.Value<int?>(),
                    TrangThai = room.Value<string>("trangThai") ?? room.Value<string>("TrangThai") ?? "",
                    DiemTrungBinh = room["diemTrungBinh"]?.Value<double?>() ?? room["DiemTrungBinh"]?.Value<double?>(),
                    SoLuongDanhGia = room["soLuongDanhGia"]?.Value<int?>() ?? room["SoLuongDanhGia"]?.Value<int?>(),
                    HinhAnhDaiDien = room.Value<string>("hinhAnhDaiDien") ?? room.Value<string>("HinhAnhDaiDien") ?? "/images/room-placeholder.jpg",
                    DanhSachHinhAnh = room["danhSachHinhAnh"]?.ToObject<List<string>>() ?? room["DanhSachHinhAnh"]?.ToObject<List<string>>() ?? new List<string>(),
                    MoTa = room.Value<string>("moTa") ?? room.Value<string>("MoTa") ?? "",
                    IsDuyet = room["isDuyet"]?.Value<bool?>() ?? room["IsDuyet"]?.Value<bool?>() ?? false,
                    IsBiKhoa = room["isBiKhoa"]?.Value<bool?>() ?? room["IsBiKhoa"]?.Value<bool?>() ?? false,
                    IsDeleted = room["isDeleted"]?.Value<bool?>() ?? room["IsDeleted"]?.Value<bool?>() ?? false,
                    CreatedAt = ReadDateTimeOffset(room, "createdAt", "CreatedAt"),
                    UpdatedAt = ReadDateTimeOffset(room, "updatedAt", "UpdatedAt")
                };

                // Lấy thông tin nhà trọ (nếu API có trả)
                var nhaTroObj = room["nhaTro"] as JObject ?? room["NhaTro"] as JObject;
                if (nhaTroObj != null)
                {
                    var nhaTroObjId = nhaTroObj.Value<string>("nhaTroId") ?? nhaTroObj.Value<string>("NhaTroId");
                    if (!string.IsNullOrWhiteSpace(nhaTroObjId))
                    {
                        phongDto.NhaTro = new NhaTroDto
                        {
                            NhaTroId = Guid.Parse(nhaTroObjId),
                            ChuTroId = Guid.Parse(nhaTroObj.Value<string>("chuTroId") ?? nhaTroObj.Value<string>("ChuTroId") ?? Guid.Empty.ToString()),
                            TieuDe = nhaTroObj.Value<string>("tieuDe") ?? nhaTroObj.Value<string>("TieuDe") ?? "",
                            DiaChi = nhaTroObj.Value<string>("diaChi") ?? nhaTroObj.Value<string>("DiaChi") ?? "",
                            SdtChuTro = nhaTroObj.Value<string>("sdtChuTro") ?? nhaTroObj.Value<string>("SdtChuTro") ?? ""
                        };
                    }
                }

                // Lấy danh sách tiện ích (nếu API detail có trả)
                var phongTienIchArr = room["phongTienIchs"] as JArray ?? room["PhongTienIchs"] as JArray;
                if (phongTienIchArr != null)
                {
                    foreach (var tiToken in phongTienIchArr)
                    {
                        var tiObj = tiToken as JObject;
                        if (tiObj == null) continue;

                        var tienIchObj = tiObj["tienIch"] as JObject ?? tiObj["TienIch"] as JObject;
                        phongDto.TienIchs.Add(new TienIchDto
                        {
                            TienIchId = tiObj["tienIchId"]?.Value<int>() ?? tiObj["TienIchId"]?.Value<int>() ?? 0,
                            Ten = tienIchObj?.Value<string>("ten") ?? tienIchObj?.Value<string>("Ten") ?? ""
                        });
                    }
                }

                System.Diagnostics.Debug.WriteLine($"✅ GetRoomDetailAsync Success: {phongDto.TieuDe}");
                return phongDto;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting room detail: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                return null;
            }
        }
    }
}