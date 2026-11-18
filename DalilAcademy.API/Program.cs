using DalilAcademy.API.Middleware;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using DalilAcademy.Core.Validators;
using DalilAcademy.Infra.Data;
using DalilAcademy.Infra.Repositorie;
using DalilAcademy.Infra.Repository;
using DalilAcademy.Infra.Service;
using DalilAcademy.Infra.Payments;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// 1. Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Dalil Academy LMS API",
        Version = "v1",
        Description = "Learning Management System API built with ASP.NET Core"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(1) 
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.Configure<PayPalSettings>(
    builder.Configuration.GetSection("PayPal"));

builder.Services.AddHttpClient<IPaymentGateway, PayPalPaymentGateway>(client =>
{
    var baseUrl = builder.Configuration["PayPal:BaseUrl"]
        ?? throw new InvalidOperationException("PayPal:BaseUrl is missing.");

    if (!baseUrl.EndsWith('/'))
        baseUrl += "/";

    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();

builder.Services.AddScoped<ICourseRepository, EfCourseRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
builder.Services.AddScoped<IInstructorService, InstructorService>();

builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddSingleton<IImageService, LocalImageService>();

builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(client =>
{
    // Key is joined to BaseUrl once, at startup
    client.BaseAddress = new Uri(
        $"{builder.Configuration["ExchangeRate:BaseUrl"]}" +
        $"{builder.Configuration["ExchangeRate:ApiKey"]}/");

});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddValidatorsFromAssemblyContaining<CreateCourseDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEnrollmentDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateInstructorDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentDtoValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<UpdateCourseDtoValidator>();



var app = builder.Build();

app.UseExceptionHandler();
// 2. Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dalil Academy API v1");
        options.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(db);
}
app.Run();