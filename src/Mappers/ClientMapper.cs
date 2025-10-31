using censudex_clients_service.src.Dtos;
using censudex_clients_service.src.Models;

namespace censudex_clients_service.src.Mappers
{
    /// <summary>
    /// Provides static methods for mapping between Client entities and data transfer objects.
    /// </summary>
    /// <remarks>
    /// This mapper class handles bidirectional conversions between the Client model
    /// and various DTOs used in the API, ensuring consistent data transformation
    /// and business rule enforcement across the application.
    /// </remarks>
    public static class ClientMapper
    {
        /// <summary>
        /// Converts a CreateUserRequest DTO to a Client entity.
        /// </summary>
        /// <param name="dto">The CreateUserRequest data transfer object containing user registration data.</param>
        /// <returns>A new Client entity populated with data from the DTO.</returns>
        /// <remarks>
        /// <para>
        /// This method performs the following transformations:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description>Generates a new GUID for the client ID</description>
        /// </item>
        /// <item>
        /// <description>Maps FirstName and LastName properties directly</description>
        /// </item>
        /// <item>
        /// <description>Hashes the password using BCrypt before storage</description>
        /// </item>
        /// <item>
        /// <description>Sets default values: Role = 0 (regular user), IsActive = true</description>
        /// </item>
        /// <item>
        /// <description>Sets CreatedAt to current UTC date</description>
        /// </item>
        /// </list>
        /// <example>
        /// The following example shows how to use this method:
        /// <code>
        /// var createRequest = new CreateUserRequest { ... };
        /// var client = createRequest.ToClient();
        /// </code>
        /// </example>
        /// </remarks>
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

        /// <summary>
        /// Converts a Client entity to a ViewUserResponse DTO.
        /// </summary>
        /// <param name="client">The Client entity to convert.</param>
        /// <returns>A ViewUserResponse DTO containing client data suitable for API responses.</returns>
        /// <remarks>
        /// <para>
        /// This method performs the following transformations:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description>Combines FirstName and LastNames into a FullName property</description>
        /// </item>
        /// <item>
        /// <description>Excludes sensitive data like PasswordHash</description>
        /// </item>
        /// <item>
        /// <description>Converts DateOnly Birthdate to DateOnly BirthDate in response</description>
        /// </item>
        /// <item>
        /// <description>Converts DateOnly CreatedAt to DateTime for API compatibility</description>
        /// </item>
        /// <item>
        /// <description>Handles null IsActive values by defaulting to true</description>
        /// </item>
        /// </list>
        /// <example>
        /// The following example shows how to use this method:
        /// <code>
        /// var client = await _repository.GetByIdAsync(id);
        /// var response = client.ToViewUserResponse();
        /// </code>
        /// </example>
        /// </remarks>
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

        /// <summary>
        /// Updates an existing Client entity with data from a CreateUserRequest DTO.
        /// </summary>
        /// <param name="client">The Client entity to update.</param>
        /// <param name="dto">The CreateUserRequest DTO containing updated data.</param>
        /// <remarks>
        /// <para>
        /// This method performs the following updates:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description>Updates all basic client properties (FirstName, LastNames, Email, etc.)</description>
        /// </item>
        /// <item>
        /// <description>Conditionally updates the password hash only if a new password is provided</description>
        /// </item>
        /// <item>
        /// <description>Preserves existing values for Id, Role, IsActive, and CreatedAt</description>
        /// </item>
        /// </list>
        /// <note type="important">
        /// The password is only updated if the DTO contains a non-empty password value.
        /// This allows for partial updates without requiring password re-entry.
        /// </note>
        /// <example>
        /// The following example shows how to use this method:
        /// <code>
        /// var existingClient = await _repository.GetByIdAsync(id);
        /// existingClient.UpdateClientFromDto(updateRequest);
        /// await _repository.UpdateAsync(existingClient);
        /// </code>
        /// </example>
        /// </remarks>
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
