using System.Collections;
using System.Configuration;
using AspNetCore.Identity.Stores;
using AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;
using Azure.Storage.Blobs;
using ElseForty.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["CosmosDB:connectionString"];
var databaseId = builder.Configuration["CosmosDB:databaseId"];
var googleClientId = builder.Configuration["Google:clientId"];
var googleClientSecret = builder.Configuration["Google:clientSecret"];


builder.Services.Configure<IdentityStoresOptions>(options => options
                 .UseAzureCosmosDB(connectionString, databaseId));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = true
)
                .AddRoles<IdentityRole>()
                .AddAzureCosmosDbStores()
              .AddDefaultTokenProviders(); ; //Add Identity stores



builder.Services.AddAuthentication()
           .AddCookie(o =>
           {
               o.Cookie.IsEssential = true;
           })
            .AddGoogle(options =>
            {
                options.ClientId = googleClientId;
                options.ClientSecret = googleClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
            });


 var storageAccount = builder.Configuration["StorageAccount:connectionString"];
 var containerName = builder.Configuration["StorageAccount:containerName"];
 var blobName = builder.Configuration["StorageAccount:blobName"];
builder.Services.AddDataProtection()
       .PersistKeysToAzureBlobStorage(storageAccount, containerName, blobName)
       .SetApplicationName("Else Forty");

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.LoginPath = "/Admin/SignIn";
    options.AccessDeniedPath = "/Home/Error";
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<IBlogRepo, BlogRepo>();
builder.Services.AddScoped<IBugRepo, BugRepo>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHsts();
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

