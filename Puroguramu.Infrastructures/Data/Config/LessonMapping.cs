using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.Infrastructures.Data.Config;

public class LessonMapping : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("Id").ValueGeneratedOnAdd();
        builder.Property(l => l.Title).HasColumnName("Title").IsRequired().HasMaxLength(100);
        builder.Property(l => l.Description).HasColumnName("Description").IsRequired().HasMaxLength(255);
        builder.Property(l => l.IsPublished).HasColumnName("IsPublished").IsRequired();
        builder.Property(l => l.Order).HasColumnName("Order").IsRequired();

        builder.HasMany(l => l.Exercises)
            .WithOne(e => e.Lesson)
            .HasForeignKey(e => e.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
