using Coursera.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Infrastructure.Data.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(i => i.Bio)
                .IsRequired()
                .HasMaxLength(1000);
            builder.Property(i => i.JobTitle)
                .IsRequired();
            builder.Property(i => i.ImagePath)
                .IsRequired();
            builder.HasMany(i => i.Courses)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
