using Demo.Entity;
using FluentValidation;

namespace Demo.Validation
{
    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            RuleFor(c => c.Text).NotEmpty().WithMessage("Comment text is required.");
            RuleFor(c => c.Text).MaximumLength(500).WithMessage("Comment text must not exceed 500 characters.");
            //RuleFor(c => c.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(c => c.PostId).NotEmpty().WithMessage("Post ID is required.");
        }
    }

}
