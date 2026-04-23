using CakeShop.Data;
using CakeShop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//1. setting SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

//2. setting Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();


//3. registering Email service
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
//重要：啟用動態遷移，確保資料庫與模型同步
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {       
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}
    catch (Exception ex)
    {
        // Log the error (you can use a logging framework here)
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "資料庫建立或遷移失敗。");
    }
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
