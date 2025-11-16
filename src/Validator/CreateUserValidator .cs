
using censudex_clients_service.src.Protos.Clients;
using FluentValidation;

namespace censudex_clients_service.src.Validator
{
    public class CreateUserValidator : AbstractValidator<CreateUserRequestProto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName es requerido")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName es requerido")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .Matches(@"^[^@]+@censudex\.cl$")
                .WithMessage("Email debe pertenecer a @censudex.cl");

            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(4)
                .MaximumLength(50);

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+56[2-9]\d{8}$")
                .WithMessage("Número telefónico no válido. Formato esperado: +56911223344");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$")
                .WithMessage("La contraseña debe incluir mayúscula, minúscula, número y carácter especial");
        }
    }

}