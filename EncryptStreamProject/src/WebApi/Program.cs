using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Infrastructure and Application
var Configuration = builder.Configuration;
builder.Services.AddDbContext<SecretDbContext>(options =>
        options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

var appSettings = new ApplicationSettings();
var appSettingsSection = builder.Configuration.GetSection("ApplicationSettings");
appSettingsSection.Bind(appSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


if(appSettings.UseMigration)
{
    // Migrate tables
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SecretDbContext>();
    db.Database.Migrate();
}



app.UseHttpsRedirection();

app.MapGet("", (SecretDbContext context) => {

    return context.Secrets.Include(x => x.HashCryptor).First();
});


app.Run();