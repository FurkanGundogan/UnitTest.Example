using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Services;

namespace Users.Api.Tests.Unit
{
    public class UserServiceTests
    {
        private readonly UserService _sut;
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly ILogger<User> _logger = Substitute.For<ILogger<User>>();

        public UserServiceTests()
        {
            _sut = new UserService(_userRepository, _logger);
         
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist() {
            // Arrange
            _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>().ToList());
            // Act 
            var result = await _sut.GetAllAsync();
            // Assert

            result.Should().BeEmpty();
        }

    }
}
