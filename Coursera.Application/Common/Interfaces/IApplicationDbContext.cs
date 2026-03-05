using Coursera.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Instructor> Instructors { get; }
        DbSet<Category> Categories { get; }
        DbSet<Cart> Carts { get; }
        DbSet<CartItem> CartItems { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<Order> Orders { get; }
        DbSet<Course> Courses { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
