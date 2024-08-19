using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Puroguramu.App.Middlewares;
using Puroguramu.Domains;
using Puroguramu.Domains.Repositories;
using Puroguramu.Infrastructures.Data;
using Puroguramu.Infrastructures.Data.Config;
using Puroguramu.Infrastructures.Data.models;
using Puroguramu.Infrastructures.Dummies;
using Puroguramu.Infrastructures.Roslyn;
using Puroguramu.Infrastructures.Services;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("ConnexionLocal")));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ProductionConnection")));
}
builder.Services.AddDefaultIdentity<Student>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
});


builder.Services.AddScoped<ILogger, Logger<object>>();
builder.Services.AddScoped<ReverseProxyLinksMiddleware>();
builder.Services.AddScoped<IExercisesRepository, DummyExercisesRepository>();
builder.Services.AddScoped<IAssessExercise, RoslynAssessor>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IExoRepository, ExoRepository>();


// middleware neccesaire pour limiter le taux de requetes

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

builder.Services.AddInMemoryRateLimiting();


builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Identity/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Identity/Account/Register");
    options.Conventions.AllowAnonymousToPage("/Index");
});


var app = builder.Build();

app.UsePathBase("/E200382");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<Student>>();

    await RoleSeeder.SeedRoles(services);

    await UserSeeder.SeedUsers(context, userManager);

    await LessonSeeder.SeedLessons(context);

    await ExerciseSeeder.SeedExercises(context);

    await StudentExerciseSeeder.SeedStudentExercises(context);
}




if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Error");
    app.UseHttpsRedirection();
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}



app.UseIpRateLimiting();
app.UseForwardedHeaders(new ForwardedHeadersOptions());
app.UseReverseProxyLinks();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
