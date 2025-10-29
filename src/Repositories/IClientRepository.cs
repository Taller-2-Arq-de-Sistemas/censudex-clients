using censudex_clients_service.src.Helpers.Requests;
using censudex_clients_service.src.Models;

namespace censudex_clients_service.src.Repositories
{
    public interface IClientRepository
    {
        Task<Client> CreateAsync(Client client);
        Task<(IEnumerable<Client> Clients, int TotalCount)> GetAllAsync(ClientQuery query);
        Task<Client?> GetByIdAsync(Guid id);
        Task<Client?> GetByEmailAsync(string email);
        Task<Client?> GetByUsernameAsync(string username);
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task SoftDeleteAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);
    }
}