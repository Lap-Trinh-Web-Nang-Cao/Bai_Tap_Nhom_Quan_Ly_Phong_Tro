using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiClient _apiClient;

        public HomeController()
        {
            _apiClient = new ApiClient();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔵 Home.Index - Starting");
                
                // Check if user just logged in
                if (TempData["LoginSuccess"] != null)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Login success detected for: {TempData["UserName"]}");
                    ViewBag.LoginSuccess = true;
                }
                
                // Lấy 6 phòng nổi bật
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=6");

                System.Diagnostics.Debug.WriteLine($"📡 Home Response Success: {response?.Success}");
                
                if (response != null && response.Success && response.Data != null)
                {
                    // Handle both JArray and JObject responses
                    Newtonsoft.Json.Linq.JArray dataArray = null;
                    
                    dataArray = response.Data as Newtonsoft.Json.Linq.JArray;
                    
                    if (dataArray == null)
                    {
                        var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                        if (jData != null)
                        {
                            dataArray = jData["data"] as Newtonsoft.Json.Linq.JArray 
                                     ?? jData["Data"] as Newtonsoft.Json.Linq.JArray;
                        }
                    }

                    if (dataArray != null)
                    {
                        var roomsList = new List<PhongDto>();
                        int imageIndex = 0;
                        
                        foreach (var item in dataArray)
                        {
                            roomsList.Add(MapToPhongDto(item, imageIndex));
                            imageIndex++;
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"✅ Home.Index - Mapped {roomsList.Count} rooms");
                        return View(roomsList);
                    }
                }

                System.Diagnostics.Debug.WriteLine("⚠️ Home.Index - No data, returning empty list");
                return View(new List<PhongDto>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Home.Index Error: {ex.Message}\n{ex.StackTrace}");
                return View(new List<PhongDto>());
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        #region Helper Methods

        private T GetValue<T>(Newtonsoft.Json.Linq.JToken token, T defaultValue = default)
        {
            if (token == null || token.Type == Newtonsoft.Json.Linq.JTokenType.Null)
                return defaultValue;
            try
            {
                if (token.Type == Newtonsoft.Json.Linq.JTokenType.String && string.IsNullOrWhiteSpace(token.ToString()))
                {
                    return defaultValue;
                }
                return token.ToObject<T>();
            }
            catch
            {
                return defaultValue;
            }
        }

        private PhongDto MapToPhongDto(Newtonsoft.Json.Linq.JToken item, int imageIndex)
        {
            const string defaultImage = "/images/banner-login.png";
            string apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7039";
            var hinhAnhToken = item["HinhAnhDaiDien"] ?? item["hinhAnhDaiDien"];
            string hinhAnhFromApi = hinhAnhToken?.ToString();
            string finalImagePath;

            if (string.IsNullOrEmpty(hinhAnhFromApi) || hinhAnhFromApi == "string")
            {
                finalImagePath = defaultImage;
            }
            else if (hinhAnhFromApi.StartsWith("http") || hinhAnhFromApi.StartsWith("~"))
            {
                finalImagePath = hinhAnhFromApi;
            }
            else if (hinhAnhFromApi.StartsWith("/"))
            {
                finalImagePath = apiBaseUrl.TrimEnd('/') + hinhAnhFromApi;
            }
            else
            {
                finalImagePath = apiBaseUrl.TrimEnd('/') + "/uploads/" + hinhAnhFromApi;
            }

            var phong = new PhongDto
            {
                PhongId = GetValue<Guid>(item["PhongId"] ?? item["phongId"], Guid.Empty),
                NhaTroId = GetValue<Guid>(item["NhaTroId"] ?? item["nhaTroId"], Guid.Empty),
                TieuDe = GetValue<string>(item["TieuDe"] ?? item["tieuDe"], "Không có tiêu đề"),
                DienTich = GetValue<decimal?>(item["DienTich"] ?? item["dienTich"], null),
                GiaTien = GetValue<long>(item["GiaTien"] ?? item["giaTien"], 0),
                TienCoc = GetValue<long?>(item["TienCoc"] ?? item["tienCoc"], null),
                SoNguoiToiDa = GetValue<int>(item["SoNguoiToiDa"] ?? item["soNguoiToiDa"], 1),
                TrangThai = GetValue<string>(item["TrangThai"] ?? item["trangThai"], ""),
                DiemTrungBinh = GetValue<double?>(item["DiemTrungBinh"] ?? item["diemTrungBinh"], null),
                SoLuongDanhGia = GetValue<int>(item["SoLuongDanhGia"] ?? item["soLuongDanhGia"], 0),
                IsDuyet = GetValue<bool>(item["IsDuyet"] ?? item["isDuyet"], false),
                IsBiKhoa = GetValue<bool>(item["IsBiKhoa"] ?? item["isBiKhoa"], false),
                HinhAnhDaiDien = finalImagePath,
                MoTa = GetValue<string>(item["MoTa"] ?? item["moTa"], "")
            };

            var nhaTroToken = item["NhaTro"] ?? item["nhaTro"];
            if (nhaTroToken != null && nhaTroToken.Type != Newtonsoft.Json.Linq.JTokenType.Null)
            {
                phong.NhaTro = new NhaTroDto
                {
                    NhaTroId = GetValue<Guid>(nhaTroToken["NhaTroId"] ?? nhaTroToken["nhaTroId"], Guid.Empty),
                    TieuDe = GetValue<string>(nhaTroToken["TieuDe"] ?? nhaTroToken["tieuDe"], ""),
                    DiaChi = GetValue<string>(nhaTroToken["DiaChi"] ?? nhaTroToken["diaChi"], "")
                };
            }

            return phong;
        }

        #endregion
    }
}
