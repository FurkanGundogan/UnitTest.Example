using FluentValidation;
using Users.Api.DTOs;

namespace Users.Api.Validators
{
    public sealed class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(p => p.FullName).NotEmpty().WithMessage("FullName cannot be null or empty");
            RuleFor(p => p.FullName).MinimumLength(3).WithMessage("FullName must be greater than 3 letters");
        }
    }
}
