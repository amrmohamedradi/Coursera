using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Coursera.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coursera.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.UserId)
                .IsRequired();
            builder.Property(o => o.Tax)
                .HasColumnType("decimal(18,2)");
            builder.Property(o => o.FinalPrice)
                .HasColumnType("decimal(18,2)");
            builder.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");
            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
