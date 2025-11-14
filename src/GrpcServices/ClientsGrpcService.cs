using Grpc.Core;
using censudex_clients_service.src.Repositories;
using censudex_clients_service.src.Protos.Clients;
using censudex_clients_service.src.Mappers;
using censudex_clients_service.src.Models;

namespace censudex_clients_service.src.GrpcServices
{
    public class ClientsGrpcService : ClientsService.ClientsServiceBase
    {
        private readonly IClientRepository _repository;

        public ClientsGrpcService(IClientRepository repository)
        {
            _repository = repository;
        }

        // -----------------------------------------------------------
        // CREATE
        // -----------------------------------------------------------
        public override async Task<ViewUserResponseProto> Create(
            CreateUserRequestProto request,
            ServerCallContext context)
        {
            // Email uniqueness validation
            if (await _repository.ExistsByEmailAsync(request.Email))
                throw new RpcException(new Status(StatusCode.AlreadyExists, "Email already exists"));

            // Username uniqueness validation
            if (await _repository.ExistsByUsernameAsync(request.Username))
                throw new RpcException(new Status(StatusCode.AlreadyExists, "Username already exists"));

            // Convert proto → domain model
            var client = ProtoMapper.FromCreateUserProto(request);

            var created = await _repository.CreateAsync(client);

            // Convert domain → proto
            return ProtoMapper.ToViewUserResponseProto(created);
        }

        // -----------------------------------------------------------
        // GET ALL
        // -----------------------------------------------------------
        public override async Task<ClientsListResponseProto> GetAll(
            ClientQueryProto request,
            ServerCallContext context)
        {
            var query = ProtoMapper.FromProto(request);

            var (clients, totalCount) = await _repository.GetAllAsync(query);


            int totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var response = new ClientsListResponseProto
            {
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            response.Items.AddRange(clients.Select(ProtoMapper.ToProto));
            return response;
        }

        // -----------------------------------------------------------
        // GET BY ID
        // -----------------------------------------------------------
        public override async Task<ViewUserResponseProto> GetById(
            GetClientByIdRequestProto request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var guid))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID format"));

            var client = await _repository.GetByIdAsync(guid);

            if (client == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Client not found"));

            return ProtoMapper.ToViewUserResponseProto(client);
        }

        // -----------------------------------------------------------
        // UPDATE
        // -----------------------------------------------------------
        public override async Task<UpdateResponseProto> Update(
            UpdateUserRequestProto request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var guid))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID format"));

            var client = await _repository.GetByIdAsync(guid);
            if (client == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Client not found"));

            // Proto → domain update
            ProtoMapper.UpdateClientFromProto(client, request);

            await _repository.UpdateAsync(client);

            return new UpdateResponseProto { Success = true };
        }

        // -----------------------------------------------------------
        // SOFT DELETE
        // -----------------------------------------------------------
        public override async Task<SoftDeleteResponseProto> SoftDelete(
            SoftDeleteRequestProto request,
            ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Token))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Token required"));

            if (!Guid.TryParse(request.Id, out var guid))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID format"));

            var client = await _repository.GetByIdAsync(guid);
            if (client == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Client not found"));

            await _repository.SoftDeleteAsync(guid);

            return new SoftDeleteResponseProto { Success = true };
        }

        // -----------------------------------------------------------
        // VERIFY CREDENTIALS
        // -----------------------------------------------------------
        public override async Task<VerifyCredentialsResponseProto> VerifyCredentials(
            VerifyCredentialsRequestProto request,
            ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.Username))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email or username required"));

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Password required"));

            // Look up user
            Client? client = !string.IsNullOrWhiteSpace(request.Email)
                ? await _repository.GetByEmailAsync(request.Email)
                : await _repository.GetByUsernameAsync(request.Username);

            if (client == null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid credentials"));

            if (client.IsActive == false)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Account disabled"));

            if (!BCrypt.Net.BCrypt.Verify(request.Password, client.PasswordHash))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid credentials"));

            return new VerifyCredentialsResponseProto
            {
                Id = client.Id.ToString(),
                Role = client.Role.ToString(),
                Username = client.Username,
                Email = client.Email,
                IsActive = client.IsActive ?? false
            };
        }
    }
}
