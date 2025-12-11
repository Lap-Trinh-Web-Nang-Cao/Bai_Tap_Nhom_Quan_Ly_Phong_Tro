using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Implements;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình JSON Reference Loop: Các bảng của bạn có quan hệ vòng tròn (VD: NhaTro->ChuTro->NhaTro). Khi API trả về dữ liệu sẽ bị lỗi "Cycle detected".
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);


// 1. Đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký Service
builder.Services.AddScoped<IBaoCaoViPhamService, BaoCaoViPhamService>();
builder.Services.AddScoped<IBienLaiService, BienLaiService>();
builder.Services.AddScoped<IChuTroThongTinPhapLyService, ChuTroThongTinPhapLyService>();
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<IDanhGiaPhongService, DanhGiaPhongService>();
builder.Services.AddScoped<IDatPhongService, DatPhongService>();
builder.Services.AddScoped<IHanhDongAdminService, HanhDongAdminService>();
builder.Services.AddScoped<IHoSoNguoiDungService, HoSoNguoiDungService>();
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

// 4. Bật 2 middleware này theo ĐÚNG THỨ TỰ (trước MapControllers)
app.UseAuthentication(); // Xác định danh tính (Bạn là ai?)
app.UseAuthorization();  // Xác định quyền hạn (Bạn được làm gì?)

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
