using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Coursera.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coursera.Infrastructure.Data.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);
            builder.Property(c => c.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(c => c.Rating)
                .IsRequired();
            builder.Property(c => c.Level)
                .IsRequired();
            builder.Property(c => c.ImagePath)
                .IsRequired();
            builder.Property(c => c.CreatedAt)
                .IsRequired();
            //builder.HasOne(c => c.Category)
            //    .WithMany()
            //    .HasForeignKey(c => c.CategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //builder.HasOne(c => c.Instructor)
            //    .WithMany(c => c.Courses)
            //    .HasForeignKey(c => c.InstructorId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
