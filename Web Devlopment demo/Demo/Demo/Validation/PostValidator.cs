using Demo.Entity;
using FluentValidation;

namespace Demo.Validation
{
    public class PostValidator : AbstractValidator<Post>
    {
        public PostValidator()
        {
            RuleFor(p => p.Title).NotEmpty().WithMessage("Title is required.");
            RuleFor(p => p.Content).NotEmpty().WithMessage("Content is required.");
        }
    }

}
