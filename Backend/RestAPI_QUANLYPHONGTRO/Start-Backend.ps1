# ========================================
# QUICK START - RUN BACKEND & TEST
# ========================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  DASHBOARD API - QUICK START" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$backendPath = "E:\LapTrinhWebNangCao\QuanLyPhongTro\Bai_Tap_Nhom_Quan_Ly_Phong_Tro\Backend\RestAPI_QUANLYPHONGTRO"

# Check if path exists
if (!(Test-Path $backendPath)) {
    Write-Host "❌ Backend path not found!" -ForegroundColor Red
    Write-Host "   Expected: $backendPath" -ForegroundColor Yellow
    pause
    exit
}

Write-Host "✅ Backend path found" -ForegroundColor Green
Write-Host ""

# Navigate to backend directory
Set-Location $backendPath
Write-Host "📁 Current directory: $backendPath" -ForegroundColor Yellow
Write-Host ""

# Check .NET SDK
Write-Host "🔍 Checking .NET SDK..." -ForegroundColor Cyan
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ .NET SDK version: $dotnetVersion" -ForegroundColor Green
} else {
    Write-Host "❌ .NET SDK not found! Please install .NET 8 SDK" -ForegroundColor Red
    pause
    exit
}
Write-Host ""

# Build project
Write-Host "🔨 Building Backend project..." -ForegroundColor Cyan
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    pause
    exit
}

Write-Host "✅ Build successful!" -ForegroundColor Green
Write-Host ""

# Ask to run
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Ready to start Backend API server?" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to start..." -ForegroundColor Green
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Write-Host ""

# Run backend
Write-Host "🚀 Starting Backend API..." -ForegroundColor Cyan
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Backend will be available at:" -ForegroundColor Green
Write-Host "  - https://localhost:5001" -ForegroundColor White
Write-Host "  - http://localhost:5000" -ForegroundColor White
Write-Host ""
Write-Host "Swagger UI:" -ForegroundColor Green
Write-Host "  - https://localhost:5001/swagger" -ForegroundColor White
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "⚠️  Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

# Run
dotnet run
