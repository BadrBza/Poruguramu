using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Puroguramu.Infrastructures.Data.models;

namespace Puroguramu.Infrastructures.Data.Config;

public class TentativeMapping : IEntityTypeConfiguration<Tentative>
{
    public void Configure(EntityTypeBuilder<Tentative> builder)
    {
        builder.ToTable("Tentative");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("Id").ValueGeneratedOnAdd();
        builder.Property(t => t.Code).HasColumnName("Code").IsRequired();
        builder.Property(t => t.AttemptedOn).HasColumnName("AttemptedOn").IsRequired();
        builder.Property(t => t.Status).HasColumnName("Status").IsRequired();


        builder.HasOne(t => t.Exo)
            .WithMany(e => e.Tentatives)
            .HasForeignKey(t => t.ExoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Student)
            .WithMany(s => s.Tentatives)
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
