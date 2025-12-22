using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Services.Interfaces;

namespace USER_QUANLYPHONGTRO.Services.Implementations
{
    /// <summary>
    /// Thực hiện các cuộc gọi API phòng
    /// </summary>
    public class PhongApiServiceImpl : IPhongApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public PhongApiServiceImpl()
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
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"]
                ?? "https://localhost:7039";

            System.Diagnostics.Debug.WriteLine($"✅ PhongApiServiceImpl initialized with ApiBaseUrl: {_apiBaseUrl}");
        }

        /// <summary>
        /// Helper: Map JObject room data to PhongDto
        /// </summary>
        private PhongDto MapRoomToDto(JObject room)
        {
            if (room == null) return null;

            var phongIdStr = room.Value<string>("phongId") ?? room.Value<string>("PhongId");
            var nhaTroIdStr = room.Value<string>("nhaTroId") ?? room.Value<string>("NhaTroId");
            
            if (string.IsNullOrWhiteSpace(phongIdStr) || string.IsNullOrWhiteSpace(nhaTroIdStr))
                return null;

            return new PhongDto
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
                UpdatedAt = ReadDateTimeOffset(room, "updatedAt", "UpdatedAt"),
                NhaTro = MapNhaTroToDto(room)
            };
        }

        /// <summary>
        /// Helper: Map NhaTro from JObject
        /// </summary>
        private NhaTroDto MapNhaTroToDto(JObject room)
        {
            var nhaTroObj = room["nhaTro"] as JObject ?? room["NhaTro"] as JObject;
            if (nhaTroObj == null) return null;

            var nhaTroObjId = nhaTroObj.Value<string>("nhaTroId") ?? nhaTroObj.Value<string>("NhaTroId");
            if (string.IsNullOrWhiteSpace(nhaTroObjId)) return null;

            return new NhaTroDto
            {
                NhaTroId = Guid.Parse(nhaTroObjId),
                ChuTroId = Guid.Parse(nhaTroObj.Value<string>("chuTroId") ?? nhaTroObj.Value<string>("ChuTroId") ?? Guid.Empty.ToString()),
                TieuDe = nhaTroObj.Value<string>("tieuDe") ?? nhaTroObj.Value<string>("TieuDe") ?? "",
                DiaChi = nhaTroObj.Value<string>("diaChi") ?? nhaTroObj.Value<string>("DiaChi") ?? "",
                SdtChuTro = nhaTroObj.Value<string>("sdtChuTro") ?? nhaTroObj.Value<string>("SdtChuTro") ?? ""
            };
        }

        private static DateTimeOffset? ReadDateTimeOffset(JObject obj, string camelName, string pascalName)
        {
            var token = obj[camelName] ?? obj[pascalName];
            if (token == null || token.Type == JTokenType.Null) return null;

            if (token.Type == JTokenType.Date)
            {
                var dt = token.Value<DateTime>();
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
                // Validate input
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Max limit

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
                            var phongDto = MapRoomToDto(room);
                            if (phongDto != null)
                                rooms.Add(phongDto);
                        }
                        catch (Exception itemEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ Error mapping room item: {itemEx.Message}");
                        }
                    }
                }

                int totalCount = dataObj["totalCount"]?.Value<int>() ?? dataObj["TotalCount"]?.Value<int>() ?? 0;
                int totalPages = dataObj["totalPages"]?.Value<int>() ?? dataObj["TotalPages"]?.Value<int>() ?? 1;

                System.Diagnostics.Debug.WriteLine($"✅ GetPublicRoomsAsync Success: {rooms.Count} rooms, Total: {totalCount}, Pages: {totalPages}");
                return (rooms, totalCount, totalPages);
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HTTP Error in GetPublicRoomsAsync: {httpEx.Message}");
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
                var root = JObject.Parse(jsonContent);

                // Backend trả dạng wrapper: { Success, Data, Message }
                var dataToken = root["data"] ?? root["Data"];
                if (dataToken == null || dataToken.Type == JTokenType.Null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ GetRoomDetailAsync: missing Data in response");
                    return null;
                }

                var room = (JObject)dataToken;
                var phongDto = MapRoomToDto(room);

                if (phongDto != null)
                {
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
                }

                return phongDto;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting room detail: {ex.Message}");
                return null;
            }
        }
    }
}
