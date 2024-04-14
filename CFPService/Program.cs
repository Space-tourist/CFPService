using System.Text.Json.Serialization;
using App;
using DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(t =>
    {
        t.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationContext>();
builder.Services.AddScoped<ApplicationRepository>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<RequestValidator>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();