using Grpc.Core;
using censudex_clients_service.src.Repositories;
using censudex_clients_service.src.Protos.Clients;
using censudex_clients_service.src.Mappers;
using censudex_clients_service.src.Models;
using censudex_clients_service.src.Services;
using FluentValidation;

namespace censudex_clients_service.src.GrpcServices
{
    public class ClientsGrpcService : ClientsService.ClientsServiceBase
    {
        private readonly IClientRepository _repository;
        private readonly IVerifyToken _tokenVerifier;
        private readonly IValidator<CreateUserRequestProto> _createValidator;
        private readonly IValidator<UpdateUserRequestProto> _updateValidator;

        public ClientsGrpcService(IClientRepository repository,
                                    IVerifyToken tokenVerifier,
                                    IValidator<CreateUserRequestProto> createValidator,
                                    IValidator<UpdateUserRequestProto> updateValidator)

        {
            _repository = repository;
            _tokenVerifier = tokenVerifier;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // -----------------------------------------------------------
        // CREATE
        // -----------------------------------------------------------
        public override async Task<ViewUserResponseProto> Create(
            CreateUserRequestProto request,
            ServerCallContext context)
        {
            var validation = await _createValidator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                string errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
            }

            // FirstName
            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "FirstName es requerido"));

            if (request.FirstName.Length > 100)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "FirstName excede el máximo de 100 caracteres"));

            // LastName
            if (string.IsNullOrWhiteSpace(request.LastName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "LastName es requerido"));

            if (request.LastName.Length > 100)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "LastName excede el máximo de 100 caracteres"));

            // Email
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email es requerido"));

            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[^@]+@[^@]+\.[^@]+$"))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Formato de Email inválido"));

            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[^@]+@censudex\.cl$"))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email debe pertenecer a @censudex.cl"));

            // Username
            if (string.IsNullOrWhiteSpace(request.Username))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Username es requerido"));

            if (request.Username.Length < 4)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Username debe tener al menos 4 caracteres"));

            if (request.Username.Length > 50)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Username excede el máximo de 50 caracteres"));

            // PhoneNumber (optional)
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(request.PhoneNumber, @"^\+56[2-9]\d{8}$"))
                    throw new RpcException(new Status(StatusCode.InvalidArgument,
                        "Número telefónico no válido. Formato esperado: +56911223344"));
            }

            // Password
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Password es requerido"));

            if (request.Password.Length < 8)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Password debe tener mínimo 8 caracteres"));

            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$"))
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "La contraseña debe incluir mayúscula, minúscula, número y carácter especial"));

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
            var validation = await _updateValidator.ValidateAsync(request);

            if (!validation.IsValid)
            {
                string errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
            }
            // FirstName (required)
            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "FirstName es requerido"));

            if (request.FirstName.Length > 100)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "FirstName excede el máximo de 100 caracteres"));


            // LastName (required)
            if (string.IsNullOrWhiteSpace(request.LastName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "LastName es requerido"));

            if (request.LastName.Length > 100)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "LastName excede el máximo de 100 caracteres"));


            // Email (required)
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email es requerido"));

            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[^@]+@[^@]+\.[^@]+$"))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Formato de Email inválido"));

            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[^@]+@censudex\.cl$"))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email debe pertenecer a @censudex.cl"));


            // Username (required)
            if (string.IsNullOrWhiteSpace(request.Username))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Username es requerido"));

            if (request.Username.Length < 4)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Username debe tener al menos 4 caracteres"));

            if (request.Username.Length > 50)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Username excede el máximo de 50 caracteres"));


            // PhoneNumber (optional but must match pattern if present)
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(request.PhoneNumber, @"^\+56[2-9]\d{8}$"))
                    throw new RpcException(new Status(StatusCode.InvalidArgument,
                        "Número telefónico no válido. Formato esperado: +56911223344"));
            }
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
            // 1. Ensure token exists
            if (string.IsNullOrWhiteSpace(request.Token))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Token required"));

            // 2. Token must start with Bearer (just like REST)
            if (!request.Token.StartsWith("Bearer "))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid token format"));

            var jwt = request.Token.Substring("Bearer ".Length).Trim();

            // 3. Validate token through Auth Service (same as REST)
            var validated = await _tokenVerifier.VerifyTokenAsync(jwt);
            if (validated == null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid or expired token"));

            // 4. Check role (must be "1")
            if (validated.Role != "1")
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Insufficient permissions"));

            // 5. Validate GUID
            if (!Guid.TryParse(request.Id, out var guid))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID"));

            // 6. Verify client exists
            var client = await _repository.GetByIdAsync(guid);
            if (client == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Client not found"));

            // 7. Perform soft delete
            await _repository.SoftDeleteAsync(guid);

            // 204 -> Success = true
            return new SoftDeleteResponseProto
            {
                Success = true
            };
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
