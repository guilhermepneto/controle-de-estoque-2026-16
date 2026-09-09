using ControleDeEstoque.WebApp;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

InjecaoDependencia.AddInfraestruturaEmJson(builder.Services);
builder.Services.AddInfraestruturaEmJson();

builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

app.UseRouting();
app.MapDefaultControllerRoute();

app.UseStaticFiles();

app.Run();

