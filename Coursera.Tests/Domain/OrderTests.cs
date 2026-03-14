using Coursera.Domain.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Coursera.Tests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void Order_Should_Calculate_Total_Price_With_Tax ()
        {
            var userId =   Guid.NewGuid();
            var items = new List<CartItem>
            {
                new CartItem(Guid.NewGuid(),Guid.NewGuid(), 100),
                new CartItem(Guid.NewGuid(),Guid.NewGuid(), 200),
            };
            var order = new Order(userId, items);
            Assert.Equal(300, order.TotalPrice);
            Assert.Equal(30, order.Tax);
            Assert.Equal(330, order.FinalPrice);
        }

    }
}