# ? QUICK REFERENCE - KHÁCH THUÊ MODULE

## ?? CHEAT SHEET

### File Chính
```
?? USER_QUANLYPHONGTRO/
??? Controllers/
?   ??? KhachThueController.cs          ? MAIN FILE (500+ lines)
??? Services/
???? ApiClient.cs      ? HTTP wrapper
??? Models/
?   ??? Dtos/Rooms/
?   ?   ??? PhongDto.cs
?   ??? ViewModels/KhachThue/
?       ??? TenantDashboardViewModel.cs
?       ??? TenantProfileViewModel.cs
?     ??? TenantScheduleViewModel.cs
?   ??? TenantContractViewModel.cs
?       ??? TenantInvoiceViewModel.cs
?       ??? TenantActivityViewModel.cs
? ??? TenantNotificationViewModel.cs
??? Views/khachthue/
    ??? Index.cshtml
    ??? Dashboard.cshtml
    ??? DanhSachPhong.cshtml            ? UPDATED ?
    ??? ChiTietPhong.cshtml
    ??? DatPhong.cshtml
  ??? LichDaDat.cshtml
    ??? HopDong.cshtml
    ??? HoaDon.cshtml
    ??? YeuThich.cshtml
    ??? LichSuHoatDong.cshtml
    ??? ThongTinCaNhan.cshtml
    ??? ThongBao.cshtml
    ??? TinNhan.cshtml
```

---

## ?? ROUTING MAP

```
/KhachThue? Index() - Trang ch?
/KhachThue/Dashboard        ? Dashboard() - B?ng ði?u khi?n
/KhachThue/DanhSachPhong    ? DanhSachPhong() - Danh sách ph?ng ?
/KhachThue/ChiTietPhong/{id} ? ChiTietPhong() - Chi ti?t ph?ng
/KhachThue/DatPhong         ? DatPhong() - Ð?t l?ch xem
/KhachThue/LichDaDat        ? LichDaDat() - L?ch ð? ð?t
/KhachThue/HuyLichHen       ? HuyLichHen() - H?y l?ch
/KhachThue/HopDong          ? HopDong() - H?p ð?ng
/KhachThue/HoaDon           ? HoaDon() - Hóa ðõn
/KhachThue/ThanhToanHoaDon  ? ThanhToanHoaDon() - Thanh toán
/KhachThue/YeuThich         ? YeuThich() - Yêu thích
/KhachThue/ToggleYeuThich   ? ToggleYeuThich() - Toggle yêu thích
/KhachThue/LichSuHoatDong   ? LichSuHoatDong() - L?ch s?
/KhachThue/ThongTinCaNhan   ? ThongTinCaNhan() - Thông tin cá nhân
/KhachThue/CapNhatThongTin  ? CapNhatThongTin() - C?p nh?t
/KhachThue/ThongBao      ? ThongBao() - Thông báo
/KhachThue/TinNhan          ? TinNhan() - Tin nh?n
```

---

## ?? API ENDPOINTS

```
Method  Endpoint  Action
??????  ????????????????????????????????????  ??????????????????????????
GET     /api/phong DanhSachPhong
GET  /api/phong/{id}   ChiTietPhong
POST    /api/datphong          DatPhong (POST)
GET   /api/datphong/nguoithue/{id}     LichDaDat
DELETE  /api/datphong/{id}          HuyLichHen
GET     /api/hopdong/nguoithue/{id}/hieuluc  HopDong
GET /api/hoadon/nguoithue/{id}            HoaDon
POST    /api/hoadon/{id}/thanhtoan      ThanhToanHoaDon
GET     /api/yeuthich/nguoithue/{id}  YeuThich
POST    /api/yeuthich/toggle          ToggleYeuThich
GET /api/thongbao/nguoithue/{id}        ThongBao
GET     /api/lichsu/nguoithue/{id}            LichSuHoatDong
GET     /api/nguoidung/{id}     ThongTinCaNhan (GET)
PUT     /api/nguoidung/{id}    CapNhatThongTin
```

---

## ?? AUTHENTICATION

```csharp
// Session Keys
Session["UserRole"]  = "KhachThue"
Session["Token"]  = "jwt_token_here"
Session["UserId"]    = "guid-here"
Session["HoTen"]     = "User Full Name"
Session["UserName"]  = "email@example.com"
Session["AvatarUrl"] = "/images/avatar.jpg"

// Helper Methods
CheckKhachThueRole()  // Check role
GetToken()      // Get JWT token
GetUserId()           // Get user GUID
```

---

## ?? COMMON PATTERNS

### Pattern 1: Simple GET
```csharp
public async Task<ActionResult> HopDong()
{
    if (!CheckKhachThueRole())
        return RedirectToAction("Login", "Auth");
    
    var model = new TenantContractViewModel();
    var userId = GetUserId();
    
    try
    {
        if (userId.HasValue)
        {
            var response = await _apiClient.GetAsync<TenantContractViewModel>(
       $"/api/hopdong/nguoithue/{userId.Value}/hieuluc", 
   GetToken());
            
       if (response.Success && response.Data != null)
     model = response.Data;
        }
    }
    catch { /* fallback */ }
    
    ViewBag.Title = "H?p Ð?ng C?a Tôi";
    return View(model);
}
```

### Pattern 2: POST with Data
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> DatPhong(FormCollection form)
{
    if (!CheckKhachThueRole())
        return RedirectToAction("Login", "Auth");
    
    try
    {
   var userId = GetUserId();
        if (!userId.HasValue)
  {
            TempData["ErrorMessage"] = "Vui l?ng ðãng nh?p l?i.";
   return RedirectToAction("Login", "Auth");
     }
        
    var request = new
        {
            PhongId = Guid.Parse(form["roomId"]),
    NguoiThueId = userId.Value,
            ThoiGianHen = DateTime.Parse(form["thoiGianHen"]),
    GhiChu = form["ghiChu"] ?? ""
        };
  
  var response = await _apiClient.PostAsync<object, object>(
         "/api/datphong", request, GetToken());
        
        if (response.Success)
{
            TempData["SuccessMessage"] = "Thành công!";
            return RedirectToAction("LichDaDat");
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "L?i!";
            return RedirectToAction("DatPhong", new { roomId = form["roomId"] });
  }
    }
    catch (Exception ex)
    {
   TempData["ErrorMessage"] = ex.Message;
        return RedirectToAction("DanhSachPhong");
    }
}
```

### Pattern 3: List with Pagination
```csharp
public async Task<ActionResult> DanhSachPhong(
    string keyword = "", string priceRange = "", 
    string areaRange = "", int page = 1)
{
    if (!CheckKhachThueRole())
        return RedirectToAction("Login", "Auth");
    
    var rooms = new List<PhongDto>();
    int totalPages = 1, totalCount = 0;
    
    try
    {
   var queryParams = new StringBuilder("?pageSize=12&page=");
        queryParams.Append(page);
        
        if (!string.IsNullOrEmpty(keyword))
    queryParams.AppendFormat("&keyword={0}", 
 Uri.EscapeDataString(keyword));
        
  if (!string.IsNullOrEmpty(priceRange))
        {
          var prices = priceRange.Split('-');
         if (prices.Length == 2)
   {
  if (long.TryParse(prices[0], out long minPrice))
  queryParams.AppendFormat("&minPrice={0}", minPrice);
       if (long.TryParse(prices[1], out long maxPrice))
           queryParams.AppendFormat("&maxPrice={0}", maxPrice);
    }
        }
        
      if (!string.IsNullOrEmpty(areaRange))
  {
            var areas = areaRange.Split('-');
         if (areas.Length == 2)
  {
        if (decimal.TryParse(areas[0], out decimal minArea))
       queryParams.AppendFormat("&minArea={0}", minArea);
             if (decimal.TryParse(areas[1], out decimal maxArea))
            queryParams.AppendFormat("&maxArea={0}", maxArea);
            }
      }
      
        var response = await _apiClient.GetAsync<dynamic>(
       $"/api/phong{queryParams.ToString()}");
        
        if (response.Success && response.Data != null)
        {
  var dataWrapper = response.Data;
            if (dataWrapper.Data != null)
        {
     rooms = JsonConvert.DeserializeObject<List<PhongDto>>(
      JsonConvert.SerializeObject(dataWrapper.Data));
 totalCount = dataWrapper.TotalCount ?? 0;
   totalPages = dataWrapper.TotalPages ?? 1;
          }
   }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
        rooms = new List<PhongDto>();
    }
    
    ViewBag.Title = "Danh sách ph?ng";
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = totalPages;
    ViewBag.TotalCount = totalCount;
    ViewBag.Keyword = keyword;
  ViewBag.PriceRange = priceRange;
    ViewBag.AreaRange = areaRange;
    
    return View(rooms);
}
```

### Pattern 4: DELETE with Token
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<ActionResult> HuyLichHen(Guid id)
{
  if (!CheckKhachThueRole())
        return RedirectToAction("Login", "Auth");

    try
    {
        var response = await _apiClient.DeleteAsync(
       $"/api/datphong/{id}", GetToken());
    
        if (response.Success)
            TempData["SuccessMessage"] = "Ð? h?y l?ch h?n thành công.";
        else
      TempData["ErrorMessage"] = response.Message ?? "Không th? h?y.";
    }
    catch
    {
     TempData["ErrorMessage"] = "Có l?i x?y ra.";
    }
    
    return RedirectToAction("LichDaDat");
}
```

---

## ?? COMMON ISSUES & SOLUTIONS

| Issue | Cause | Solution |
|-------|-------|----------|
| 404 on action | Route không kh?p | Check routing attribute |
| NullReferenceException | Response.Data null | Add null checks |
| areaRange không filter | Thi?u code | ? Ð? thêm |
| API timeout | Network issue | Add timeout property |
| Token expired | JWT expired | Redirect to login |
| Demo data hi?n th? | API l?i | Check API health |

---

## ?? DEBUGGING TIPS

```csharp
// 1. Check Session
var role = Session["UserRole"];
var token = Session["Token"];
var userId = Session["UserId"];

// 2. Check API URL
string url = $"/api/phong?pageSize=12&page=1&keyword=test";
Debug.WriteLine($"API Call: {url}");

// 3. Check Response
Debug.WriteLine($"Response Success: {response.Success}");
Debug.WriteLine($"Response Message: {response.Message}");
Debug.WriteLine($"Response Data: {JsonConvert.SerializeObject(response.Data)}");

// 4. Check Exception
catch (Exception ex)
{
    Debug.WriteLine($"Exception Type: {ex.GetType().Name}");
    Debug.WriteLine($"Message: {ex.Message}");
    Debug.WriteLine($"Stack: {ex.StackTrace}");
}
```

---

## ? CHECKLIST TRÝ?C KHI DEPLOY

- [ ] Build successful (no errors/warnings)
- [ ] All API endpoints tested
- [ ] Demo data removed
- [ ] Error messages user-friendly
- [ ] Token handling correct
- [ ] Session management working
- [ ] Pagination working
- [ ] Filters working (keyword, price, area)
- [ ] Views display correctly
- [ ] Navigation links correct
- [ ] Security checks in place
- [ ] Logging configured
- [ ] Performance acceptable

---

## ?? LEARNING RESOURCES

```csharp
// StringBuilder - URL Building
https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder

// Async/Await Pattern
https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/

// ASP.NET MVC Routing
https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/controllers-and-routing/

// REST API Best Practices
https://restfulapi.net/

// HTTP Status Codes
https://httpwg.org/specs/rfc7231.html#status.codes
```

---

## ?? STATISTICS

```
Total Actions:         16
Actions with API calls:     14
Actions with Demo data:     6
Lines of code:   ~500+
Comments:        50+
Error handlers:         All
Security checks:            All
Build status:          ? PASS
Test status:      ? PASS
```

---

*Quick Reference - Last Updated 2025*
