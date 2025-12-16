# ============================================
# 🚀 SCRIPT TỰ ĐỘNG KHỞI ĐỘNG TẤT CẢ PROJECT
# ============================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   🏠 KHỞI ĐỘNG HỆ THỐNG QUẢN LÝ PHÒNG TRỌ   " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Đường dẫn thư mục gốc
$rootPath = "E:\LapTrinhWebNangCao\QuanLyPhongTro\Bai_Tap_Nhom_Quan_Ly_Phong_Tro"

# ============================================
# BƯỚC 1: KHỞI ĐỘNG BACKEND API
# ============================================
Write-Host "📦 [1/3] Khởi động Backend API..." -ForegroundColor Yellow
Write-Host ""

$backendPath = Join-Path $rootPath "Backend\RestAPI_QUANLYPHONGTRO"

if (Test-Path $backendPath) {
    # Mở terminal mới và chạy Backend
    Start-Process powershell -ArgumentList @(
        "-NoExit",
        "-Command",
        "cd '$backendPath'; Write-Host '🚀 Backend API đang khởi động...' -ForegroundColor Green; dotnet run --launch-profile https"
    )
    
    Write-Host "   ✅ Backend API đang khởi động ở: https://localhost:7039" -ForegroundColor Green
    Write-Host "   📄 Swagger UI: https://localhost:7039/swagger" -ForegroundColor Gray
    Write-Host ""
    
    # Đợi 5 giây để Backend khởi động
    Write-Host "   ⏳ Đang đợi Backend khởi động hoàn tất (5 giây)..." -ForegroundColor Gray
    Start-Sleep -Seconds 5
} else {
    Write-Host "   ❌ KHÔNG TÌM THẤY thư mục Backend!" -ForegroundColor Red
    Write-Host "   📂 Đường dẫn: $backendPath" -ForegroundColor Red
    exit
}

# ============================================
# BƯỚC 2: KHỞI ĐỘNG ADMIN MVC
# ============================================
Write-Host "🔧 [2/3] Khởi động Admin MVC..." -ForegroundColor Yellow
Write-Host ""

$adminPath = Join-Path $rootPath "ADMIN_QUANLYPHONGTRO"
$adminCsproj = Join-Path $adminPath "ADMIN_QUANLYPHONGTRO.csproj"

if (Test-Path $adminCsproj) {
    # Mở trong VS Code hoặc Visual Studio
    Start-Process $adminCsproj
    
    Write-Host "   ✅ Admin MVC project đã được mở" -ForegroundColor Green
    Write-Host "   💡 Nhấn F5 trong Visual Studio để chạy" -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host "   ❌ KHÔNG TÌM THẤY project Admin MVC!" -ForegroundColor Red
    Write-Host "   📂 Đường dẫn: $adminCsproj" -ForegroundColor Red
}

# ============================================
# BƯỚC 3: KHỞI ĐỘNG USER MVC (TÙY CHỌN)
# ============================================
Write-Host "👤 [3/3] Khởi động User MVC (tùy chọn)..." -ForegroundColor Yellow
Write-Host ""

$userPath = Join-Path $rootPath "USER_QUANLYPHONGTRO"
$userCsproj = Join-Path $userPath "USER_QUANLYPHONGTRO.csproj"

if (Test-Path $userCsproj) {
    $choice = Read-Host "   ❓ Bạn có muốn mở User MVC không? (Y/N)"
    
    if ($choice -eq "Y" -or $choice -eq "y") {
        Start-Process $userCsproj
        Write-Host "   ✅ User MVC project đã được mở" -ForegroundColor Green
        Write-Host ""
    } else {
        Write-Host "   ⏭️  Bỏ qua User MVC" -ForegroundColor Gray
        Write-Host ""
    }
} else {
    Write-Host "   ⚠️  Không tìm thấy project User MVC" -ForegroundColor Yellow
    Write-Host ""
}

# ============================================
# THÔNG BÁO HOÀN TẤT
# ============================================
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   ✅ KHỞI ĐỘNG HOÀN TẤT!   " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Các URL quan trọng:" -ForegroundColor White
Write-Host "   🔷 Backend API (Swagger): https://localhost:7039/swagger" -ForegroundColor Cyan
Write-Host "   🔷 Admin MVC: Chạy từ Visual Studio (F5)" -ForegroundColor Cyan
Write-Host "   🔷 User MVC: Chạy từ Visual Studio (F5)" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 Lưu ý:" -ForegroundColor Yellow
Write-Host "   - Backend API phải chạy TRƯỚC khi sử dụng Admin/User MVC" -ForegroundColor Gray
Write-Host "   - Nếu gặp lỗi SSL: chạy 'dotnet dev-certs https --trust'" -ForegroundColor Gray
Write-Host "   - Database phải được setup trước (xem SQL\QuanLyPhongTro.sql)" -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor DarkGray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
