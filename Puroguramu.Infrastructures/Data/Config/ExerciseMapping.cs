using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.Infrastructures.Data.Config;

public class ExerciseMapping : IEntityTypeConfiguration<Exo>
{
    public void Configure(EntityTypeBuilder<Exo> builder)
    {
        builder.ToTable("Exercises");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
        builder.Property(e => e.Title).HasColumnName("Title").IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasColumnName("Description").IsRequired().HasMaxLength(255);
        builder.Property(e => e.Difficulty).HasColumnName("Difficulty").IsRequired();
        builder.Property(e => e.IsPublished).HasColumnName("IsPublished").IsRequired();
        builder.Property(e => e.Order).HasColumnName("Order").IsRequired();
        builder.Property(e => e.LessonId).HasColumnName("LessonId").IsRequired();
        builder.Property(e => e.Template).HasColumnName("Template").IsRequired();
        builder.Property(e => e.Solution).HasColumnName("Solution").IsRequired();

        builder.HasOne(e => e.Lesson)
            .WithMany(l => l.Exercises)
            .HasForeignKey(e => e.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
