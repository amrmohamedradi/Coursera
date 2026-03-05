using Coursera.Domain.Common;

namespace Coursera.Domain.Entities
{
    public class CartItem :BaseEntity
    {
        public Guid CartId { get; private set; }
        public Guid CourseId { get; private set; }
        public decimal Price { get; private set; }
        private CartItem() { }
        public CartItem(Guid cartId, Guid courseId, decimal price)
        {
            CartId = cartId;
            CourseId = courseId;
            Price = price;
        }

    }
}