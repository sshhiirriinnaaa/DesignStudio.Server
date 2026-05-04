using DesignStudio.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features; // ОБЯЗАТЕЛЬНО для настройки лимитов

// --- НОВЫЕ ИМПОРТЫ ДЛЯ JWT ---
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. УВЕЛИЧИВАЕМ ЛИМИТЫ (СЕРВЕР) ---
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 104_857_600; // 100 МБ
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 104_857_600; // 100 МБ
});

// Дополнительная страховка для огромных кусков текста (Base64)
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});


// --- 2. ПОДКЛЮЧАЕМ БАЗУ ДАННЫХ ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// --- 3. НАСТРАИВАЕМ CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// --- 4. НАСТРОЙКА JWT АВТОРИЗАЦИИ (НОВОЕ) ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "StoryHome",
            ValidAudience = "StoryHomeAdmin",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyForStoryHomeDiplomaProject2026!"))
        };
    });


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================================================
// ВАЖНО: СТРОГИЙ ПОРЯДОК КОНВЕЙЕРА (ОТ ЭТОГО ЗАВИСИТ ВСЁ)
// ==========================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 1. Сначала отдаем статические файлы (папка wwwroot для картинок)
app.UseStaticFiles();

// 2. Включаем систему маршрутов
app.UseRouting();

// 3. CORS ОБЯЗАТЕЛЬНО ПОСЛЕ Routing, но строго ДО Authorization
app.UseCors("AllowReactApp");

// 4. Проверяем токен (НОВОЕ - строго ДО проверки прав)
app.UseAuthentication();

// 5. Проверяем права (разрешен ли доступ)
app.UseAuthorization();

// 6. Запускаем контроллеры
app.MapControllers();

app.Run();