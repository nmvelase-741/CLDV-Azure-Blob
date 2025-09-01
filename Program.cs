using AzureStorageDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

var cfg = builder.Configuration;

// Register your Azure services
builder.Services.AddSingleton(new TableStorageService(
    cfg["Storage:ConnectionString"]!,
    cfg["Storage:CustomersTable"]!,
    cfg["Storage:ProductsTable"]!));

builder.Services.AddSingleton(new BlobStorageService(
    cfg["Storage:ConnectionString"]!,
    cfg["Storage:BlobContainer"]!));

builder.Services.AddSingleton(new QueueStorageService(
    cfg["Storage:ConnectionString"]!,
    cfg["Storage:QueueName"]!));

builder.Services.AddSingleton(new FileShareService(
    cfg["Storage:ConnectionString"]!,
    cfg["Storage:ContractsShare"]!,
    cfg["Storage:LogsShare"]!));

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Storage}/{action=Index}/{id?}");

app.Run();
