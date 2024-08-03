using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.Infrastructures.Data.Config;

public class StudentMapping : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("AspNetUsers");

        builder.Property(s => s.Matricule).HasColumnName("Matricule").IsRequired().HasMaxLength(7);
        builder.HasIndex(s => s.Matricule).IsUnique();

        builder.Property(s => s.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(50);
        builder.Property(s => s.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(50);
        builder.Property(s => s.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(s => s.Email).IsUnique();
        builder.Property(s => s.LabGroup).HasColumnName("LabGroup").IsRequired().HasMaxLength(4);
        builder.Property(s => s.ProfilePicture).HasColumnName("ProfilePicture");

        builder.HasKey(s => s.Id);

    }
}
