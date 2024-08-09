using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Puroguramu.Infrastructures.Data.Config;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.Infrastructures.Data;

public class ApplicationDbContext : IdentityDbContext<Student>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext>  options)
        : base(options)
    {
    }

    public DbSet<Lesson> Lessons { get; set; }

    public DbSet<Exo> Exercises { get; set; }

    public DbSet<Student> Students { get; set; }
    public DbSet<StudentExercise> StudentExercise { get; set; }
    public DbSet<Tentative> Tentatives { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        var teacher = new IdentityRole("teacher");
        teacher.NormalizedName = "teacher";

        var student = new IdentityRole("student");
        student.NormalizedName = "student";

        modelBuilder.Entity<IdentityRole>().HasData(teacher, student);

        modelBuilder.ApplyConfiguration(new LessonMapping());
        modelBuilder.ApplyConfiguration(new ExerciseMapping());
        modelBuilder.ApplyConfiguration(new StudentMapping());
        modelBuilder.ApplyConfiguration(new StudentExerciseMapping());
        modelBuilder.ApplyConfiguration(new TentativeMapping());

    }
}
