using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text;
using AvtoMigBussines.Services.Interfaces;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.CarWash.Repositories.Implementations;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.CarWash.Services.Implementations;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AvtoMigBussines.Models;
using AvtoMigBussines.Detailing.Services.Interfaces;
using AvtoMigBussines.Detailing.Services.Implementations;
using AvtoMigBussines.Detailing.Repositories.Interfaces;
using AvtoMigBussines.Detailing.Repositories.Implementations;
using AvtoMigBussines.Attributes;
using StackExchange.Redis;
using AvtoMigBussines.RedisResults.Detailing.Interface;
using AvtoMigBussines.RedisResults.Detailing.Service;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
var botToken = builder.Configuration["TelegramBotToken"];
// Add services to the container.
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IModelCarRepository, ModelCarRepository>();
builder.Services.AddScoped<IModelCarService, ModelCarService>();
builder.Services.AddScoped<IWashOrderRepository, WashOrderRepository>();
builder.Services.AddScoped<IWashOrderService, WashOrderService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IWashServiceService, WashServiceService>();
builder.Services.AddScoped<IWashServiceRepository, WashServiceRepository>();
builder.Services.AddScoped<ISalarySettingRepository, SalarySettingRepository>();
builder.Services.AddScoped<ISalarySettingService, SalarySettingService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IWashOrderTransactionService, WashOrderTransactionService>();
builder.Services.AddScoped<IWashOrderTransactionRepository, WashOrderTransactionRepository>();
builder.Services.AddScoped<INotificationCenterService, NotificationCenterService>();
builder.Services.AddScoped<INotificationCenterRepository, NotificationCenterRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDetailingOrderService, DetailingOrderService>();
builder.Services.AddScoped<IDetailingRepository, DetailingRepository>();
builder.Services.AddScoped<IDetailingServiceRepository, DetailingServiceRepository>();
builder.Services.AddScoped<IDetailingServiceService,  DetailingServiceService>();
builder.Services.AddScoped<ITypeOfOrganizationService, TypeOfOrganizationService>();
builder.Services.AddScoped<ITypeOfOrganizationRepository,  TypeOfOrganizationRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddScoped<IDetailingPriceListRepository, DetailingPriceListRepository>();
builder.Services.AddScoped<IDetailingPriceListService, DetailingPriceListService>();


builder.Services.AddScoped<IClientService>(sp =>
    new ClientService(sp.GetRequiredService<IClientRepository>(), botToken));
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "AvtoMigAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AspNetUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6; // Минимальная длина пароля
    options.Password.RequireNonAlphanumeric = false; // Требование к использованию неалфавитных символов
    options.Password.RequireUppercase = false; // Требование к использованию символов верхнего регистра
    options.Password.RequireLowercase = true; // Требование к использованию символов нижнего регистра
    options.Password.RequireDigit = true; // Требование к использованию цифр
    options.Password.RequiredUniqueChars = 6; // Минимальное количество уникальных символов в пароле
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true, // Попробуйте установить в false
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});
builder.Services.AddScoped<CheckIsDeletedAttribute>();

builder.Services.AddSingleton<WebSocketHandler>();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseWebSockets();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAllOrigins");
app.MapControllers();

// Middleware для обработки WebSocket соединений
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();
            await handler.AddSocket(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

app.Run();
