﻿using FluentValidation;
using System.Diagnostics;
using Users.Api.DTOs;
using Users.Api.Logging;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Validators;

namespace Users.Api.Services
{
    public sealed class UserService(IUserRepository userRepository, ILoggerAdapter<UserService> logger) : IUserService
    {
        public async Task<bool> CreateAsync(CreateUserDto request, CancellationToken cancellationToken = default)
        {
            CreateUserDtoValidator validationRules = new CreateUserDtoValidator();
            var result = validationRules.Validate(request);

            if (!result.IsValid)
            {
                throw new ValidationException(string.Join(", ", result.Errors.Select(s => s.ErrorMessage)));
            }

            var nameIsAlreadyExist = await userRepository.NameIsExist(request.FullName, cancellationToken);
            if (nameIsAlreadyExist)
            {
                throw new ArgumentException("Name already exist");
            }

            var user = CreateUserDtoToUserObject(request);

            logger.LogInformation("Creating new user with id: {0} and name: {1}", user.Id, user.FullName);

            var stopWatch = Stopwatch.StartNew();

            try
            {
                return await userRepository.CreateAsync(user, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrong while creating user");
                throw;
            }
            finally
            {
                stopWatch.Stop();
                logger.LogInformation("User with id: {0} created in {1}ms", user.Id, stopWatch.ElapsedMilliseconds);

            }

        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            User? user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
            {
                throw new ArgumentException("User not found");
            }

            logger.LogInformation("Deleting user with id: {0}", user.Id);

            var stopWatch = Stopwatch.StartNew();

            try
            {
                return await userRepository.DeleteAsync(user, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrong while deleting user");
                throw;
            }
            finally
            {
                stopWatch.Stop();
                logger.LogInformation("User with id: {0} deleted in {1}ms", user.Id, stopWatch.ElapsedMilliseconds);

            }
        }

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {

            logger.LogInformation("Retriving all users");

            var stopWatch = Stopwatch.StartNew();

            try
            {
                return await userRepository.GetAllAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrogn while retriving all users");
                throw;
            }
            finally
            {
                stopWatch.Stop();
                logger.LogInformation("All users retrieved in {0}ms", stopWatch.ElapsedMilliseconds);

            }
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Retriving user with id: {0}", id);

            var stopWatch = Stopwatch.StartNew();

            try
            {
                return await userRepository.GetByIdAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrong while retriving user with id: {0}", id);
                throw;
            }
            finally
            {
                stopWatch.Stop();
                logger.LogInformation("User with id: {0} retrieved in {1}ms", id, stopWatch.ElapsedMilliseconds);

            }
        }

        public User CreateUserDtoToUserObject(CreateUserDto request) {
            User user = new User()
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName
            };

            return user;
        }
    }
}
