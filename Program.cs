using Microsoft.EntityFrameworkCore;
using clientIq_api.Data;
using clientIq_api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5174", "http://clientiqdo.com", "http://167.99.53.52", "https://167.99.53.52")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Entity Framework with PostgreSQL
builder.Services.AddDbContext<ClientIqDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");

// Map all endpoints
app.MapProductEndpoints();
app.MapClientEndpoints();
app.MapUserEndpoints();
app.MapOrderEndpoints();

app.MapGet("/", () => "ClientIQ API is running! Visit /swagger to see all endpoints.");

app.Run();
