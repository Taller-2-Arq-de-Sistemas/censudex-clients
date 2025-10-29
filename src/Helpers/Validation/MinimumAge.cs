

using System.ComponentModel.DataAnnotations;

namespace censudex_clients_service.src.Helpers.Validation
{
    public class MinimumAge : ValidationAttribute
    {
        private readonly int _minAge;

        public MinimumAge(int minAge)
        {
            _minAge = minAge;
            ErrorMessage = $"Cliente debe tener al menos {minAge} años.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            if (value is not DateOnly birthDate)
                return new ValidationResult("Formato de fecha inválido. Ingrese tipo de dato DateOnly.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            int age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age))
                age--;

            return age >= _minAge
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }

    }
}