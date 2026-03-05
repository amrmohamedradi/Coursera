using Coursera.Domain.Common;

namespace Coursera.Domain.Entities
{
    public class CartItem :BaseEntity
    {
        public Guid CartId { get;  set; }
        public Guid CourseId { get;  set; }
        public decimal Price { get;  set; }
        public Course Course { get; set; }
        private CartItem() { }
        public CartItem(Guid cartId, Guid courseId, decimal price)
        {
            CartId = cartId;
            CourseId = courseId;
            Price = price;
        }

    }
}