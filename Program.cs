var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models.IRepositorioPropietario, Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models.RepositorioPropietario>();
builder.Services.AddScoped<Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models.IRepositorioInquilino, Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models.RepositorioInquilino>();
builder.Services.AddScoped<Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models.IRepositorioTipoInmueble, Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models.RepositorioTipoInmueble>();
builder.Services.AddScoped<Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models.IRepositorioInmueble, Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models.RepositorioInmueble>();
builder.Services.AddScoped<Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models.IRepositorioReserva, Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models.RepositorioReserva>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
