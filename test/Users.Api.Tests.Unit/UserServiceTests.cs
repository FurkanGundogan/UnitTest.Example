using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Api.DTOs;
using Users.Api.Logging;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Services;

namespace Users.Api.Tests.Unit
{
    public class UserServiceTests
    {
        private readonly UserService _sut;
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly ILoggerAdapter<UserService> _logger = Substitute.For<ILoggerAdapter<UserService>>();

        public UserServiceTests()
        {
            _sut = new UserService(_userRepository, _logger);

        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>().ToList());
            // Act 
            var result = await _sut.GetAllAsync();
            // Assert

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnsUsers_WhenSomeUsersExist()
        {
            // Arrange
            var testUser = new User()
            {
                FullName = "TestUser FullName",
                Id = Guid.NewGuid()
            };
            var expectedUsers = new List<User>() { testUser };
            _userRepository.GetAllAsync().Returns(expectedUsers);

            // Act 
            var result = await _sut.GetAllAsync();
            // Assert
            /// Be vs BeEquivalentTo - reference
            result.Should().BeEquivalentTo(expectedUsers);
        }

        [Fact]
        public async Task GetAllAsync_ShouldLogMessages_WhenInvoked()
        {
            // Arrange

            _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>().ToList());

            // Act 
            await _sut.GetAllAsync();
            // Assert

            _logger.Received(1).LogInformation(Arg.Is("Retriving all users"));
            _logger.Received(1).LogInformation(Arg.Is("All users retrieved in {0}ms"), Arg.Any<long>());

        }

        [Fact]
        public async Task GetAllAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
        {
            // Arrange
            var exception = new ArgumentException("Something went wrogn while retriving all users");

            _userRepository.GetAllAsync().Throws(exception);

            // Act 
            var requestAction = async () => await _sut.GetAllAsync();
            // Assert

            await requestAction.Should().ThrowAsync<ArgumentException>();
            _logger.Received(1).LogError(Arg.Is(exception), Arg.Is("Something went wrogn while retriving all users"));
            // _logger.Received(1).LogError(exception, "Something went wrogn while retriving all users");

        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNoUserExists()
        {
            // Arrange
            _userRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();
            // Act 
            var result = await _sut.GetByIdAsync(Guid.NewGuid());
            // Assert

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenNoUserExist()
        {
            // Arrange
            var existingUser = new User()
            {
                FullName = "TestUser FullName",
                Id = Guid.NewGuid()
            };

            _userRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(existingUser);

            // Act 
            var result = await _sut.GetByIdAsync(Guid.NewGuid());
            // Assert

            result.Should().BeEquivalentTo(existingUser);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldLogMessages_WhenInvoked()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepository.GetByIdAsync(userId).ReturnsNull();

            // Act 
            await _sut.GetByIdAsync(userId);
            // Assert

            _logger.Received(1).LogInformation(Arg.Is("Retriving user with id: {0}"), userId);
            _logger.Received(1).LogInformation(Arg.Is("User with id: {0} retrieved in {1}ms"), userId, Arg.Any<long>());

        }


        [Fact]
        public async Task GetByIdAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var exception = new ArgumentException("Something went wrong while retriving user");

            _userRepository.GetByIdAsync(userId).Throws(exception);

            // Act 
            var requestAction = async () => await _sut.GetByIdAsync(userId);
            // Assert

            await requestAction.Should().ThrowAsync<ArgumentException>();
            _logger.Received(1).LogError(
                Arg.Is(exception),
                Arg.Is("Something went wrong while retriving user with id: {0}"),
                Arg.Is(userId));


        }

        [Fact]
        public async Task CreateAsync_ShouldThrownAnError_WhenUserCreateDetailsAreNotValid()
        {
            // Arrange
            CreateUserDto request = new CreateUserDto("");

            // Act 
            var requestAction = async () => await _sut.CreateAsync(request);
            // Assert

            await requestAction.Should().ThrowAsync<ValidationException>();


        }


        [Fact]
        public async Task CreateAsync_ShouldThrownAnError_WhenUserNameExists()
        {
            // Arrange
            _userRepository.NameIsExist(Arg.Any<string>()).Returns(true);
            CreateUserDto request = new CreateUserDto("");

            // Act 
            var requestAction = async () => await _sut.CreateAsync(new("Test"));
            // Assert

            await requestAction.Should().ThrowAsync<ArgumentException>();


        }

        [Fact]
        public void CreateAsync_ShouldCreateUserDtoToUserObject()
        {
            // Arrange
            CreateUserDto request = new CreateUserDto("Test");

            // Act
            var user = _sut.CreateUserDtoToUserObject(request);

            // Assert
            user.FullName.Should().BeEquivalentTo(request.FullName);

        }

        [Fact]
        public async Task CreateAsync_ShouldCreateUser_WhenDetailsAreValidAndUnique()
        {
            // Arrange
            CreateUserDto request = new CreateUserDto("Testt");
            _userRepository.NameIsExist(request.FullName).Returns(false);
            _userRepository.CreateAsync(Arg.Any<User>()).Returns(true);

            // Act
            var result = await _sut.CreateAsync(request);

            // Assert
            result.Should().Be(true);

        }

        [Fact]
        public async Task CreateAsync_ShouldLogMessage_WhenInvoked()
        {
            // Arrange
            CreateUserDto request = new CreateUserDto("Testtt");
            _userRepository.NameIsExist(request.FullName).Returns(false);
            _userRepository.CreateAsync(Arg.Any<User>()).Returns(true);

            // Act
            await _sut.CreateAsync(request);

            // Assert
            _logger.Received(1).LogInformation(
                Arg.Is("Creating new user with id: {0} and name: {1}"), 
                Arg.Any<Guid>(), 
                Arg.Is(request.FullName));

            _logger.Received(1).LogInformation(
                Arg.Is("User with id: {0} created in {1}ms"),
                Arg.Any<Guid>(),
                Arg.Any<long>());

        }

        [Fact]
        public async Task CreateAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
        {
            // Arrange
            CreateUserDto request = new CreateUserDto("Testtt");
            var ex = new ArgumentException("Something went wrogn while creating user");
            _userRepository.CreateAsync(Arg.Any<User>()).Throws(ex);

            // Act
            var requestAction = async () => await _sut.CreateAsync(request);

            // Assert
            await requestAction.Should()
                .ThrowAsync<ArgumentException>();

            _logger.Received(1).LogError(
                Arg.Is(ex),
                Arg.Is("Something went wrong while creating user"));

        }

    }
}
