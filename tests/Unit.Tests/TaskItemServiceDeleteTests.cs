
// using FluentAssertions;

// using Microsoft.Extensions.Logging.Abstractions;

// using NSubstitute;

// using TaskTitan.Core;
// using TaskTitan.Infrastructure.Expressions;
// using TaskTitan.Infrastructure.Services;

// namespace Unit.Tests;

// public class TaskItemServiceDeleteTests
// {
//     private readonly NullLogger<TaskItemService> _serviceLogger = new();
//     private readonly ITaskItemRepository repository;
//     private readonly IExpressionParser _parser;
//     private readonly TaskItemService _sut;
//     private readonly IStringFilterConverter<DateTime> _stringConverter;

//     public TaskItemServiceDeleteTests()
//     {
//         repository = Substitute.For<ITaskItemRepository>();
//         _parser = Substitute.For<IExpressionParser>();
//         _stringConverter = Substitute.For<IStringFilterConverter<DateTime>>();
//         _sut = new TaskItemService(repository, _parser, _stringConverter, _serviceLogger);
//     }

//     [Fact]
//     public async Task DeleteWithValidRequestShouldReturnSuccessResult()
//     {
//         // Arrange
//         var request = new TaskItemDeleteRequest()
//         {
//             Filters = []
//         };

//         // Act
//         var response = await _sut.Delete(request);

//         // Assert
//         response.IsSuccess.Should().BeTrue();
//         response.IsFailure.Should().BeFalse();
//     }
// }
