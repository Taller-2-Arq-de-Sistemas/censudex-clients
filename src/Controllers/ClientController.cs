
using censudex_clients_service.src.Dtos;
using censudex_clients_service.src.Helpers.Requests;
using censudex_clients_service.src.Mappers;
using censudex_clients_service.src.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace censudex_clients_service.src.Controllers
{
    [ApiController]
    [Route("clients")]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _repository;

        public ClientController(IClientRepository repository)
        {
            _repository = repository;
        }
        // POST /clients
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _repository.ExistsByEmailAsync(dto.Email))
                return Conflict("A client with the same email already exists.");

            if (await _repository.ExistsByUsernameAsync(dto.Username))
                return Conflict("A client with the same username already exists.");

            var client = dto.ToClient(); 
            var createdClient = await _repository.CreateAsync(client); 
            var responseDto = createdClient.ToViewUserResponse();

            return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
        }
        // GET /clients
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

        // GET /clients/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ViewUserResponse>> GetById(Guid id)
        {
            var client = await _repository.GetByIdAsync(id);
            if (client == null)
                return NotFound();
            return Ok(ClientMapper.ToViewUserResponse(client));
        }

        // PATCH /clients/{id} (update)
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

        // PATCH /clients/delete/{id} (soft delete)
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