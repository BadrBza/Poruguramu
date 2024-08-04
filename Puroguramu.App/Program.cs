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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ConnexionLocal")));

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
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
});


builder.Services.AddScoped<ILogger, Logger<object>>();
builder.Services.AddScoped<ReverseProxyLinksMiddleware>();
builder.Services.AddScoped<IExercisesRepository, DummyExercisesRepository>();
builder.Services.AddScoped<IAssessExercise, RoslynAssessor>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IExoRepository, ExoRepository>();


builder.Services.AddRazorPages();

var app = builder.Build();
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


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions());
app.UseReverseProxyLinks();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();


app.MapRazorPages();

app.Run();
