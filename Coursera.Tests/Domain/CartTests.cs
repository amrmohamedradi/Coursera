using Coursera.Domain.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Tests.Domain
{
    public class CartTests
    {
        [Fact]
        public void AddItem_Should_Add_Otm_To_Cart() 
        {
            var cart = new Cart(Guid.NewGuid());
            var courseId = Guid.NewGuid();
            var price = 100;
            cart.AddItem(courseId, price);
            cart.Items.Should().HaveCount(1);
            cart.Items.First().CourseId.Should().Be(courseId);
        }
        [Fact]
        public void RemoveItem_Should_Remove_Otm_From_Cart()
        {
            var cart = new Cart(Guid.NewGuid());
            var courseId = Guid.NewGuid();
            var price = 100;
            cart.AddItem(courseId, price);
            cart.RemoveItem(courseId);
            cart.Items.Should().BeEmpty();
        }
        [Fact]
        public void AddItem_Should_Throw_Exception_When_Item_Already_Exists()
        {
            var cart = new Cart(Guid.NewGuid());
            var courseId = Guid.NewGuid();
            var price = 100;
            cart.AddItem(courseId, price);
            Action act = () => cart.AddItem(courseId, price);
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
