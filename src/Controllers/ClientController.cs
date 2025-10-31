
using censudex_clients_service.src.Dtos;
using censudex_clients_service.src.Helpers.Requests;
using censudex_clients_service.src.Mappers;
using censudex_clients_service.src.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace censudex_clients_service.src.Controllers
{
    /// <summary>
    /// Controller for managing client operations in the Censudex system.
    /// </summary>
    [ApiController]
    [Route("clients")]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _repository;

        /// <summary>
        /// Initializes a new instance of the ClientController class.
        /// </summary>
        /// <param name="repository">The client repository for data access operations.</param>
        public ClientController(IClientRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Creates a new client in the system.
        /// </summary>
        /// <param name="dto">The client creation data transfer object.</param>
        /// <returns>
        /// Returns 201 Created with the newly created client on success.
        /// Returns 400 Bad Request if the model state is invalid.
        /// Returns 409 Conflict if a client with the same email or username already exists.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _repository.ExistsByEmailAsync(dto.Email))
                return Conflict("Un cliente con el mismo correo electrónico ya existe.");

            if (await _repository.ExistsByUsernameAsync(dto.Username))
                return Conflict("Un cliente con el mismo nombre de usuario ya existe.");

            var client = dto.ToClient();
            var createdClient = await _repository.CreateAsync(client);
            var responseDto = createdClient.ToViewUserResponse();

            return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
        }

        /// <summary>
        /// Retrieves a paginated list of clients with optional filtering.
        /// </summary>
        /// <param name="query">The query parameters for filtering, pagination, and sorting.</param>
        /// <returns>
        /// Returns 200 OK with a paginated list of clients.
        /// Returns 400 Bad Request if the model state is invalid.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ClientQuery query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (clients, totalCount) = await _repository.GetAllAsync(query);
            var clientDtos = clients.Select(ClientMapper.ToViewUserResponse).ToList();
            var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

            var response = new
            {
                Items = clientDtos,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a specific client by their unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the client to retrieve.</param>
        /// <returns>
        /// Returns 200 OK with the client data if found.
        /// Returns 404 Not Found if the client does not exist.
        /// </returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ViewUserResponse>> GetById(Guid id)
        {
            var client = await _repository.GetByIdAsync(id);
            if (client == null)
                return NotFound();
            return Ok(ClientMapper.ToViewUserResponse(client));
        }

        /// <summary>
        /// Updates an existing client's information.
        /// </summary>
        /// <param name="id">The GUID of the client to update.</param>
        /// <param name="dto">The updated client data.</param>
        /// <returns>
        /// Returns 204 No Content on successful update.
        /// Returns 404 Not Found if the client does not exist.
        /// </returns>
        [HttpPatch("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] CreateUserRequest dto)
        {
            var client = await _repository.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            ClientMapper.UpdateClientFromDto(client, dto);
            await _repository.UpdateAsync(client);

            return NoContent();
        }

        /// <summary>
        /// Performs a soft delete of a client by marking them as inactive.
        /// </summary>
        /// <param name="id">The GUID of the client to soft delete.</param>
        /// <returns>
        /// Returns 204 No Content on successful soft delete.
        /// Returns 404 Not Found if the client does not exist.
        /// </returns>
        [HttpPatch("delete/{id:guid}")]
        public async Task<ActionResult> SoftDelete(Guid id)
        {
            var client = await _repository.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            await _repository.SoftDeleteAsync(id);
            return NoContent();
        }
    }
}