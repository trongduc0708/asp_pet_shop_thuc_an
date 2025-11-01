using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework
builder.Services.AddDbContext<PetShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add HttpContextAccessor for VNPay service
builder.Services.AddHttpContextAccessor();

// Add Services
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<BannerService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<AddressService>();
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<VNPayService>();
builder.Services.AddScoped<PromotionService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ChatbotService>();
builder.Services.AddHttpClient<ChatbotService>();

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
