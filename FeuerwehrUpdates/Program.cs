using FeuerwehrUpdates;
using FeuerwehrUpdates.Models;
using FeuerwehrUpdates.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddPooledDbContextFactory<FWUpdatesDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(sp => sp
    .GetRequiredService<IDbContextFactory<FWUpdatesDbContext>>()
    .CreateDbContext());

builder.Services.AddScoped<PushService>();
builder.Services.AddScoped<EinsatzListener>();
builder.Services.Configure((FUOptions options) => builder.Configuration.Bind("FeuerwehrUpdates", options));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    IDbContextFactory<FWUpdatesDbContext> contextFactory =
            scope.ServiceProvider.GetRequiredService<IDbContextFactory<FWUpdatesDbContext>>();
    try
    {
        var context = services.GetRequiredService<FWUpdatesDbContext>();
        await context.Database.MigrateAsync();
        new EinsatzListener(services.GetService<PushService>(),
        services.GetService<IOptions<FUOptions>>(),
        services.GetService<ILogger<EinsatzListener>>(),
        contextFactory);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();