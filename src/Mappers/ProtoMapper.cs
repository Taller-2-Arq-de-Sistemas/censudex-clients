
using censudex_clients_service.src.Helpers.Requests;
using censudex_clients_service.src.Models;
using censudex_clients_service.src.Protos.Clients;

namespace censudex_clients_service.src.Mappers
{
    public class ProtoMapper
    {
        public static Client FromCreateUserProto(CreateUserRequestProto request)
        {
            return new Client
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastNames = request.LastName,
                Email = request.Email,
                Username = request.Username,
                Birthdate = DateOnly.Parse(request.BirthDate),
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };
        }
        public static void UpdateClientFromProto(Client client, UpdateUserRequestProto request)
        {
            client.FirstName = request.FirstName;
            client.LastNames = request.LastName;
            client.Email = request.Email;
            client.Username = request.Username;
            client.Birthdate = DateOnly.Parse(request.BirthDate);
            client.Address = request.Address;
            client.PhoneNumber = request.PhoneNumber;
        }
        public static ClientProto ToProto(Client client)
        {
            return new ClientProto
            {
                Id = client.Id.ToString(),
                FirstName = client.FirstName,
                LastName = client.LastNames,
                Email = client.Email,
                Username = client.Username,
                BirthDate = client.Birthdate.ToString("yyyy-MM-dd"),
                Address = client.Address,
                PhoneNumber = client.PhoneNumber,
                Role = client.Role.ToString(),
                IsActive = client.IsActive ?? true
            };
        }
        public static ViewUserResponseProto ToViewUserResponseProto(Client client)
        {
            return new ViewUserResponseProto
            {
                Client = ToProto(client)
            };
        }
        public static ClientQuery FromProto(ClientQueryProto proto)
        {
            return new ClientQuery
            {
                FullName = proto.FullName,
                Email = proto.Email,
                Username = proto.Username,
                IsActive = proto.IsActive,
                SortBy = proto.SortBy,
                IsDescending = proto.IsDescending,
                PageNumber = proto.PageNumber,
                PageSize = proto.PageSize
            };
        }

    }
}