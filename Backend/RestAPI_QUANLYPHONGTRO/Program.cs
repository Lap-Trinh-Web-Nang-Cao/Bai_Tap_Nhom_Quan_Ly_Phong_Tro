using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Implements;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.Seed;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình JSON Reference Loop
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// ===== BẬT CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminClient", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 1. Đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký Service
builder.Services.AddScoped<IBaoCaoViPhamService, BaoCaoViPhamService>();
builder.Services.AddScoped<IBienLaiService, BienLaiService>();
builder.Services.AddScoped<IChuTroThongTinPhapLyService, ChuTroThongTinPhapLyService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<IDanhGiaPhongService, DanhGiaPhongService>();
builder.Services.AddScoped<IDatPhongService, DatPhongService>();
builder.Services.AddScoped<IHanhDongAdminService, HanhDongAdminService>();
builder.Services.AddScoped<IHoSoNguoiDungService, HoSoNguoiDungService>();
builder.Services.AddScoped<IHostService, HostService>();
builder.Services.AddScoped<ILichSuService, LichSuService>();
builder.Services.AddScoped<ILoaiHoTroService, LoaiHoTroService>();
builder.Services.AddScoped<IVaiTroService, VaiTroService>();
builder.Services.AddScoped<INguoiDungVaiTroService, NguoiDungVaiTroService>();
builder.Services.AddScoped<INhaTroService, NhaTroService>();
builder.Services.AddScoped<IPhongService, PhongService>();
builder.Services.AddScoped<ITienIchService, TienIchService>();
builder.Services.AddScoped<IPhongTienIchService, PhongTienIchService>();
builder.Services.AddScoped<IQuanHuyenService, QuanHuyenService>();
builder.Services.AddScoped<IPhuongService, PhuongService>();
builder.Services.AddScoped<ITinNhanService, TinNhanService>();
builder.Services.AddScoped<ITokenThongBaoService, TokenThongBaoService>();
builder.Services.AddScoped<ITrangThaiDatPhongService, TrangThaiDatPhongService>();
builder.Services.AddScoped<IViPhamService, ViPhamService>();
builder.Services.AddScoped<IYeuCauHoTroService, YeuCauHoTroService>();
builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();

// 3. Cấu hình xác thực JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

// ===== INITIALIZE DATABASE =====
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DatabaseInitializer.InitializeDatabase(dbContext);
}

// Middleware order matters!
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAdminClient");

// Development only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // ⚠️ In development, we'll skip HTTPS redirect to avoid SSL issues
    System.Diagnostics.Debug.WriteLine("🔧 Running in Development mode - HTTPS redirect disabled");
}
else
{
    // Production: enable HTTPS redirect
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.MapControllers();

System.Diagnostics.Debug.WriteLine("✅ API Server started successfully");
System.Diagnostics.Debug.WriteLine($"🌐 Environment: {app.Environment.EnvironmentName}");

app.Run();
