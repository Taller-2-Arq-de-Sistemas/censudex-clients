using censudex_clients_service.src.Dtos;
using censudex_clients_service.src.Models;

namespace censudex_clients_service.src.Mappers
{
    public static class ClientMapper
    {
        public static Client ToClient(this CreateUserRequest dto)
        {
            return new Client
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastNames = dto.LastName,
                Email = dto.Email,
                Username = dto.Username,
                Birthdate = dto.BirthDate,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = 0,
                IsActive = true,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };
        }

        public static ViewUserResponse ToViewUserResponse(this Client client)
        {
            return new ViewUserResponse
            {
                Id = client.Id,
                FullName = $"{client.FirstName} {client.LastNames}",
                Email = client.Email,
                Username = client.Username,
                IsActive = client.IsActive ?? true,
                BirthDate = client.Birthdate,
                Address = client.Address,
                PhoneNumber = client.PhoneNumber,
                CreatedAt = client.CreatedAt.ToDateTime(new TimeOnly(0, 0))
            };
        }

        public static void UpdateClientFromDto(this Client client, CreateUserRequest dto)
        {
            client.FirstName = dto.FirstName;
            client.LastNames = dto.LastName;
            client.Email = dto.Email;
            client.Username = dto.Username;
            client.Birthdate = dto.BirthDate;
            client.Address = dto.Address;
            client.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrEmpty(dto.Password))
                client.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }
    }
}
