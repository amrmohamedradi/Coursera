using Coursera.Domain.Common;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace Coursera.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get;  set; }
        public DateTime CreatedAt { get;  set; }

        public ICollection<CartItem> Items { get;  set; } = new List<CartItem>();

        private Cart() { }
        public Cart(Guid userId)
        {
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(Guid courseId, decimal price)
        {
            if(Items.Any(i => i.CourseId == courseId))
                throw new InvalidOperationException("Course is already in the cart.");
            Items.Add(new CartItem(Id,courseId, price));
        }

        public void RemoveItem(Guid courseId)
        {
            var item = Items.FirstOrDefault(i => i.CourseId == courseId);
            if (item == null)
                throw new InvalidOperationException("Course not found in the cart.");
            Items.Remove(item);
        }
         public void Clear()
        {
            Items.Clear();
        }
    }
}
