using FluentValidation;
using Library.Data;
using Library.Models;

namespace Library.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator(LibraryContext db)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .MaximumLength(24);

            RuleFor(x => x.ConfirmPassword)
                .Equal(e => e.Password)
                .WithMessage("The entered passwords do not match");

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = db.Users.Any(u => u.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }
                });
        }
    }
}
