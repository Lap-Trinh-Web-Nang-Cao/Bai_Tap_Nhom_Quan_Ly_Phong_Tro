using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Hubs;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Implements;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== ENVIRONMENT CONFIGURATION =====
var environment = builder.Environment;
Console.WriteLine($"🌍 Environment: {environment.EnvironmentName}");

// Load environment variables
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") 
    ?? builder.Configuration["Jwt:Key"];
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") 
    ?? builder.Configuration["Jwt:Issuer"];
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
    ?? builder.Configuration["Jwt:Audience"];
var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"✅ JWT Issuer: {jwtIssuer}");
Console.WriteLine($"✅ Database: {(dbConnectionString?.Contains("192.168") == true ? "Local Dev" : "Cloud (RDS/Azure)")}");

// Validate critical configuration
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("❌ JWT_KEY must be at least 32 characters long!");
}

if (string.IsNullOrEmpty(dbConnectionString))
{
    throw new InvalidOperationException("❌ DB_CONNECTION_STRING is required!");
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== THÊM SIGNALR =====
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// Cấu hình JSON Reference Loop
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// ===== BẬT CORS (Cập nhật cho SignalR) =====
builder.Services.AddCors(options =>
{
    // Development Policy
    if (environment.IsDevelopment())
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    }

    // Production Policy - Specific Origins Only
    var prodOrigins = builder.Configuration.GetSection("AllowedOrigins:Production").Get<string[]>() 
        ?? new[] { "https://yourdomain.com" };
    
    options.AddPolicy("AllowProduction", policy =>
    {
        policy.WithOrigins(prodOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Localhost Development
    var devOrigins = builder.Configuration.GetSection("AllowedOrigins:Development").Get<string[]>() 
        ?? Array.Empty<string>();
    
    if (devOrigins.Length > 0)
    {
        options.AddPolicy("AllowLocalhost", policy =>
        {
            policy.WithOrigins(devOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    }
});

// 1. Đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = dbConnectionString;
    Console.WriteLine($"📊 Using connection string: {connectionString}");
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(30);
    });
});

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
builder.Services.AddScoped<IDbIntrospectionService, DbIntrospectionService>();

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(5)
    };
    
    // Cấu hình cho SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// 4. Cấu hình Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthenticatedOnly", policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("VaiTroId", "1"));

    options.AddPolicy("ChuTroOnly", policy =>
        policy.RequireClaim("VaiTroId", "2"));

    options.AddPolicy("NguoiThueOnly", policy =>
        policy.RequireClaim("VaiTroId", "3"));

    options.AddPolicy("AdminOrChuTro", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "VaiTroId" && (c.Value == "1" || c.Value == "2"))));

    options.AddPolicy("ChuTroOrNguoiThue", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "VaiTroId" && (c.Value == "2" || c.Value == "3"))));
});

var app = builder.Build();

// Middleware order matters!
app.UseRouting();

// Sử dụng CORS policy phù hợp với environment
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("AllowProduction");
}

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// Map endpoints
app.MapControllers();

// ===== MAP SIGNALR HUB =====
app.MapHub<ChatHub>("/chatHub");

// ===== STARTUP LOGS =====
Console.WriteLine("╔════════════════════════════════════════════════════════╗");
Console.WriteLine("║        API Server Started Successfully              ║");
Console.WriteLine("╠════════════════════════════════════════════════════════╣");
Console.WriteLine($"║ Environment: {environment.EnvironmentName,-48} ║");
var dbType = dbConnectionString?.Contains("192.168") == true ? "Local Dev" : "Cloud (RDS)";
Console.WriteLine($"║ Database: {dbType,-52} ║");
Console.WriteLine($"║ JWT Issuer: {jwtIssuer,-42} ║");
Console.WriteLine($"║ SignalR Hub: /chatHub                                   ║");
var swaggerStatus = app.Environment.IsDevelopment() ? "Enabled " : "Disabled";
Console.WriteLine($"║ Swagger: {swaggerStatus,-48} ║");
Console.WriteLine("╚════════════════════════════════════════════════════════╝");

app.Run();
