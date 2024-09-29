using FluentValidation;
using Library.Data;
using Library.Models;

namespace Library.Validators
{
    public class ResourceValidator : AbstractValidator<Resource>
    {
        public ResourceValidator(LibraryContext db)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(100);

            RuleFor(x => x.Author)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(56);

            RuleFor(x => x.Quantity)
                .NotEmpty();

            RuleFor(x => x.Type)
                .NotEmpty();

            RuleFor(x => x.Category)
                .NotEmpty();

        }
    }
}
