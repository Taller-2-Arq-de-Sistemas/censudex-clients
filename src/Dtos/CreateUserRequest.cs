using censudex_clients_service.src.Helpers.Validation;
using System.ComponentModel.DataAnnotations;

namespace censudex_clients_service.src.Dtos
{
    public class CreateUserRequest
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Required, EmailAddress]
        [RegularExpression(@"^[^@]+@censudex\.cl$", ErrorMessage = "Email debe pertenecer a @censudex.cl")]
        public string Email { get; set; } = null!;

        [Required, MinLength(4), MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MinimumAge(18)]
        public DateOnly BirthDate { get; set; }

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\+56[2-9]\d{8}$", ErrorMessage = "Número telefónico no válido. Formato esperado: +56911223344")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$",
            ErrorMessage = "La contraseña debe incluir al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
        public string Password { get; set; } = null!;


    }
}