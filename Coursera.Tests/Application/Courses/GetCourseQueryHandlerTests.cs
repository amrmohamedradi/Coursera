using Coursera.Application.Common.Interfaces;
using Coursera.Application.Features.Courses.Queries;

using Coursera.Domain.Entities;
using Coursera.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using MockQueryable.Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Coursera.Tests.Application.Courses;
public class GetCourseQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock = new();

    [Fact]
    public async Task Handle_Should_Return_Paginated_Courses()
    {
        // Arrange
        var courses = new List<Course>
    {
        new Course(
            "ASP.NET",
            "Backend",
            100,
            Level.Beginner,
            "image.png",
            Guid.NewGuid(),
            Guid.NewGuid()
        ),
        new Course(
            "React",
            "Frontend",
            120,
            Level.Beginner,
            "image.png",
            Guid.NewGuid(),
            Guid.NewGuid()
        )
    };

        var mockCourses = courses.BuildMockDbSet<Course>();
        _contextMock
            .Setup(x => x.Courses)
            .Returns(mockCourses.Object);

        var handler = new GetCourseQueryHandler(_contextMock.Object);

        var query = new GetCourseQuery(1, 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
    }
    [Fact]
    public async Task Handle_Should_Filter_Courses_By_Search()
    {
        // Arrange
        var courses = new List<Course>
    {
        new Course(
            "ASP.NET Core",
            "Backend Development",
            100,
            Level.Beginner,
            "image.png",
            Guid.NewGuid(),
            Guid.NewGuid()
        ),
        new Course(
            "React JS",
            "Frontend Development",
            120,
            Level.Beginner,
            "image.png",
            Guid.NewGuid(),
            Guid.NewGuid()
        )
    };

        var mockCourses = courses.BuildMockDbSet<Course>();
        _contextMock
            .Setup(x => x.Courses)
            .Returns(mockCourses.Object);

        var handler = new GetCourseQueryHandler(_contextMock.Object);

        var query = new GetCourseQuery(
            PageNumber: 1,
            PageSize: 10,
            Search: "ASP"
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("ASP.NET Core", result.Items.First().Name);
    }
    [Fact]
    public async Task Handle_Should_Return_Empty_When_No_Courses_Found()
    {
        // Arrange
        var courses = new List<Course>();

        var mockCourses = courses.BuildMockDbSet<Course>();
        _contextMock
            .Setup(x => x.Courses)
            .Returns(mockCourses.Object);

        var handler = new GetCourseQueryHandler(_contextMock.Object);

        var query = new GetCourseQuery(1, 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }
}