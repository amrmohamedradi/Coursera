using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Coursera.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Coursera.Infrastructure.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}
