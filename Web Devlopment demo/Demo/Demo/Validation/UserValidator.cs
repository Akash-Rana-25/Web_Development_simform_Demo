using Demo.Entity;
using FluentValidation;

namespace Demo.Validation
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.Name).NotEmpty().WithMessage("Name is required.");
        }
    }

    

}
