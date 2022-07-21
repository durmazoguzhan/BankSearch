using Microsoft.EntityFrameworkCore;
using BankSearch.Data;
using BankSearch.BackgroundServices;


var builder = WebApplication.CreateBuilder(args);
//IAPPRepository scopeunun görüldüğü yerde AppRepository scopeunun işlevlerini gerçekleştir
builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddControllersWithViews();
//appsettings.json'daki connectionStringi çekerek EF DbContext nesnesine inject et
builder.Services.AddDbContext<BankSearchContext>(x=>
x.UseSqlServer(builder.Configuration.GetConnectionString("BankSearchConnection")));

//Arkaplanda çalışacak olan APIService'in injectonı
IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(services =>
	{
		services.AddHostedService<APIService>();
		services.AddScoped<IAppRepository, AppRepository>();
		//appsettings.json'daki connectionStringi çekerek EF DbContext nesnesine inject et
		services.AddDbContext<BankSearchContext>(x =>
x.UseSqlServer(builder.Configuration.GetConnectionString("BankSearchConnection")));
	})
	.Build();
host.RunAsync();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
	app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller}/{action=Index}/{id?}");
app.MapFallbackToFile("index.html"); ;
app.Run();