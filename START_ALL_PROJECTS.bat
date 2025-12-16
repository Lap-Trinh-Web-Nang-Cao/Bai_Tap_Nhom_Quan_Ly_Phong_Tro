@echo off
chcp 65001 >nul
color 0B
title 🚀 Khởi động hệ thống Quản lý Phòng trọ

echo ========================================
echo    🏠 QUẢN LÝ PHÒNG TRỌ - AUTO START
echo ========================================
echo.

REM Đường dẫn Backend
set "BACKEND_PATH=E:\LapTrinhWebNangCao\QuanLyPhongTro\Bai_Tap_Nhom_Quan_Ly_Phong_Tro\Backend\RestAPI_QUANLYPHONGTRO"

echo [1/3] 📦 Khởi động Backend API...
echo.

if exist "%BACKEND_PATH%" (
    REM Mở cửa sổ cmd mới để chạy Backend
    start "Backend API" cmd /k "cd /d "%BACKEND_PATH%" && echo 🚀 Backend API đang khởi động... && dotnet run --launch-profile https"
    
    echo    ✅ Backend API đang khởi động...
    echo    🌐 URL: https://localhost:7039
    echo    📄 Swagger: https://localhost:7039/swagger
    echo.
    
    echo    ⏳ Đang đợi Backend khởi động (10 giây)...
    timeout /t 10 /nobreak >nul
    echo.
) else (
    echo    ❌ KHÔNG TÌM THẤY thư mục Backend!
    echo    📂 Đường dẫn: %BACKEND_PATH%
    pause
    exit
)

echo [2/3] 🔧 Mở Admin MVC project...
echo.

set "ADMIN_PATH=E:\LapTrinhWebNangCao\QuanLyPhongTro\Bai_Tap_Nhom_Quan_Ly_Phong_Tro\ADMIN_QUANLYPHONGTRO\ADMIN_QUANLYPHONGTRO.csproj"

if exist "%ADMIN_PATH%" (
    start "" "%ADMIN_PATH%"
    echo    ✅ Admin MVC project đã được mở
    echo    💡 Nhấn F5 trong Visual Studio để chạy
    echo.
) else (
    echo    ❌ KHÔNG TÌM THẤY Admin MVC project!
    pause
)

echo [3/3] 👤 Mở User MVC project (tùy chọn)...
echo.

set /p "OPEN_USER=   ❓ Bạn có muốn mở User MVC không? (Y/N): "

if /i "%OPEN_USER%"=="Y" (
    set "USER_PATH=E:\LapTrinhWebNangCao\QuanLyPhongTro\Bai_Tap_Nhom_Quan_Ly_Phong_Tro\USER_QUANLYPHONGTRO\USER_QUANLYPHONGTRO.csproj"
    
    if exist "%USER_PATH%" (
        start "" "%USER_PATH%"
        echo    ✅ User MVC project đã được mở
        echo.
    ) else (
        echo    ⚠️  Không tìm thấy User MVC project
        echo.
    )
) else (
    echo    ⏭️  Bỏ qua User MVC
    echo.
)

echo ========================================
echo    ✅ KHỞI ĐỘNG HOÀN TẤT!
echo ========================================
echo.
echo 📋 Các URL quan trọng:
echo    🔷 Backend API: https://localhost:7039/swagger
echo    🔷 Admin MVC: Chạy từ Visual Studio (F5)
echo    🔷 User MVC: Chạy từ Visual Studio (F5)
echo.
echo 💡 Lưu ý:
echo    - Backend API phải chạy TRƯỚC Admin/User MVC
echo    - Nếu gặp lỗi SSL: dotnet dev-certs https --trust
echo    - Database phải setup trước (SQL\QuanLyPhongTro.sql)
echo.

pause
