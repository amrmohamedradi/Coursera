using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using Coursera.Application.Features.Orders.Commands.Checkout;
using Coursera.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;

namespace Coursera.Tests.Application.Orders;

public class CheckoutHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock = new();
    private readonly Mock<ILogger<CheckoutHandler>> _loggerMock = new();

    [Fact]
    public async Task Checkout_Should_Create_Order_When_Cart_Has_Items()
    {
        var userId = Guid.NewGuid();
        var cart = new Cart(userId);
        cart.AddItem(Guid.NewGuid(), 100);
        cart.AddItem(Guid.NewGuid(), 200);
        var carts = new List<Cart> { cart };
        var orders = new List<Order>();
        _contextMock
            .Setup(x => x.Carts)
            .ReturnsDbSet(carts);
        _contextMock
           .Setup(x => x.Orders)
           .ReturnsDbSet(orders);
        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var handler = new CheckoutHandler(_contextMock.Object, _loggerMock.Object);
        var command = new CheckoutCommand(userId);
        var result = await handler.Handle(command, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, result);
    }
    [Fact]
    public async Task Checkout_Should_Throw_When_Cart_Is_Empty()
    {           
        var userId = Guid.NewGuid();
        var cart = new Cart(userId); // no items
        _contextMock
            .Setup(x => x.Carts)
            .ReturnsDbSet(new List<Cart> { cart });
        _contextMock
            .Setup(x => x.Orders)
            .ReturnsDbSet(new List<Order>());
        var handler = new CheckoutHandler(
            _contextMock.Object,
            _loggerMock.Object
        );
        var command = new CheckoutCommand(userId);
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
    [Fact]
    public async Task Checkout_Should_Calculate_Total_Price_Correctly()
    {
        var userId = Guid.NewGuid();
        var cart = new Cart(userId);
        cart.AddItem(Guid.NewGuid(), 100);
        cart.AddItem(Guid.NewGuid(), 200);
        var carts = new List<Cart> { cart };
        var orders = new List<Order>();
        _contextMock
            .Setup(x => x.Carts)
            .ReturnsDbSet(carts);
        _contextMock
            .Setup(x => x.Orders)
            .ReturnsDbSet(orders);
        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var handler = new CheckoutHandler(
            _contextMock.Object,
            _loggerMock.Object
        );
        var command = new CheckoutCommand(userId);
        var orderId = await handler.Handle(command, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, orderId);
    }
}