using FluentValidation;
using Library.Data;
using Library.Models;

namespace Library.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator(LibraryContext db)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password).MinimumLength(6);
            RuleFor(x => x.Password).MaximumLength(24);



            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = db.Users.Any(u => u.Email == value);
                    if (!emailInUse)
                    {
                        context.AddFailure("Email", "Email or password doesn't match.");
                    }
                });
        }
    }
}
