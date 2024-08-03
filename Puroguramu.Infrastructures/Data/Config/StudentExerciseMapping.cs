using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.Infrastructures.Data.Config;

public class StudentExerciseMapping : IEntityTypeConfiguration<StudentExercise>
{

    public void Configure(EntityTypeBuilder<StudentExercise> builder)
    {
        builder.ToTable("StudentExercise");

        builder.HasKey(se => se.Id);

        builder.HasOne(se => se.Exo)
            .WithMany(e => e.StudentExercises)
            .HasForeignKey(se => se.ExoId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(se => se.Student)
            .WithMany(s => s.StudentExercises)
            .HasForeignKey(se => se.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
