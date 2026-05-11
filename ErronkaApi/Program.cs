using ErronkaApi;
using ErronkaApi.Logak;
using ErronkaApi.Middlewareak;
using ErronkaApi.NHibernate;
using ErronkaApi.Repositorioak;
using NHibernate;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5093);        // HTTP
        options.ListenLocalhost(7236, listenOptions =>
        {
            listenOptions.UseHttps();     // HTTPS
        });
    });
}

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
// builder.Services.AddSingleton<Log>();
builder.Services.AddSingleton<LogAnder>();
builder.Services.AddTransient<ErabiltzaileaRepository>();
builder.Services.AddTransient<KategoriaRepository>();
builder.Services.AddTransient<ProduktuaRepository>();
builder.Services.AddTransient<EskaeraRepository>();
builder.Services.AddTransient<MahaiaRepository>();
builder.Services.AddTransient<ErreserbaRepository>();
builder.Services.AddTransient<FakturaRepository>();
builder.Services.AddTransient<OdooRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var sessionFactory = scope.ServiceProvider.GetRequiredService<ISessionFactory>();
    using var session = sessionFactory.OpenSession();
    using var tx = session.BeginTransaction();

    var txandaColumnCount = Convert.ToInt32(session.CreateSQLQuery(@"
        SELECT COUNT(*)
        FROM information_schema.columns
        WHERE table_schema = DATABASE()
          AND table_name = 'eskaerak'
          AND column_name = 'txanda'")
        .UniqueResult());

    if (txandaColumnCount == 0)
    {
        session.CreateSQLQuery(@"
            ALTER TABLE eskaerak
            ADD COLUMN txanda VARCHAR(20) NULL AFTER sortze_data")
            .ExecuteUpdate();
    }

    session.CreateSQLQuery(@"
        UPDATE eskaerak e
        LEFT JOIN erreserbak r ON r.id = e.erreserba_id
        SET e.txanda = CASE
            WHEN r.txanda IS NOT NULL AND TRIM(r.txanda) <> '' THEN
                CASE
                    WHEN LOWER(TRIM(r.txanda)) = 'afaria' THEN 'afaria'
                    ELSE 'bazkaria'
                END
            WHEN HOUR(e.sortze_data) >= 18 THEN 'afaria'
            ELSE 'bazkaria'
        END
        WHERE e.txanda IS NULL OR TRIM(e.txanda) = ''")
        .ExecuteUpdate();

    tx.Commit();
}

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