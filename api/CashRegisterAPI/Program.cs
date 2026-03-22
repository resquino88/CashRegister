using CashRegisterAPI.Data;
using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using CashRegisterAPI.Rule;
using CashRegisterAPI.Utility;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<RuleEngineInfo>(builder.Configuration.GetSection("RuleEngineInfo"));

builder.Services.AddTransient<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddTransient<ICountryRepository, CountryRepository>();
builder.Services.AddTransient<IRuleRepository, RuleRepository>();
builder.Services.AddTransient<IFileParser, FileParser>();
builder.Services.AddTransient<IRule<string, BasicRuleInfoDTO>, DivisibleByRule>();
builder.Services.AddTransient<IRule<string, BasicRuleInfoDTO>, MinChangeRule>();
builder.Services.AddTransient<IRuleEngine, RuleEngine>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var databaseConnectionString = builder.Configuration.GetConnectionString("Database");
var webAppConnectionString = builder.Configuration.GetConnectionString("WebApp");

// Register the DbContext service
builder.Services.AddDbContext<CashRegisterDbContext>(options => options.UseNpgsql(databaseConnectionString));
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
        policy.WithOrigins(webAppConnectionString!)
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Initialize DB data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CashRegisterDbContext>();
    await DataSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
