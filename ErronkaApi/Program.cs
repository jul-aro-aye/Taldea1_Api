using ErronkaApi;
using ErronkaApi.Logak;
using ErronkaApi.Middlewareak;
using ErronkaApi.NHibernate;
using ErronkaApi.Repositorioak;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5093);        // HTTP
    options.ListenLocalhost(7236, listenOptions =>
    {
        listenOptions.UseHttps();     // HTTPS
    });
});

// Add services to the container.

// CORS konfigurazioa gehitu => Web-etik errorea ez emateko
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()     //.WithOrigins("http://localhost:8000") Jakiteko zein IPtatik etorri daitekeen
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(NHibernateHelper.SessionFactory);
builder.Services.AddSingleton<Log>();
builder.Services.AddTransient<ErabiltzaileaRepository>();
builder.Services.AddTransient<KategoriaRepository>();
builder.Services.AddTransient<ProduktuaRepository>();
builder.Services.AddTransient<EskaeraRepository>();
builder.Services.AddTransient<MahaiaRepository>();
builder.Services.AddTransient<ErreserbaRepository>();
builder.Services.AddTransient<FakturaRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors();

app.UseMiddleware<TpvEkintzaLogMiddleware>();
app.UseStaticFiles();

app.UseAuthorization();

app.UseMiddleware<NHibernateSessionMiddleware>();
app.MapControllers();

app.Run();
