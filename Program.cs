using CarCredit.Data;
using CarCredit.Interfaces;
using CarCredit.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICreditService, CreditService>();
builder.Services.AddScoped<IFeeService, FeeService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();