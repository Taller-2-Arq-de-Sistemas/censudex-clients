using censudex_clients_service.src.Data;
using censudex_clients_service.src.Helpers.Requests;
using censudex_clients_service.src.Models;
using Microsoft.EntityFrameworkCore;

namespace censudex_clients_service.src.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDBContext _context;

        public ClientRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Client> CreateAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }
        public async Task<(IEnumerable<Client> Clients, int TotalCount)> GetAllAsync(ClientQuery query)
        {
            var clientsQuery = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                clientsQuery = clientsQuery.Where(c =>
                    (c.FirstName + " " + c.LastNames).Contains(query.Name));
            }

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                clientsQuery = clientsQuery.Where(c => c.Email == query.Email);
            }

            if (!string.IsNullOrWhiteSpace(query.Username))
            {
                clientsQuery = clientsQuery.Where(c => c.Username.Contains(query.Username));
            }

            if (query.IsActive.HasValue)
            {
                clientsQuery = clientsQuery.Where(c => c.IsActive == query.IsActive);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                clientsQuery = query.SortBy switch
                {
                    "FirstName" => query.IsDescending ? clientsQuery.OrderByDescending(c => c.FirstName) : clientsQuery.OrderBy(c => c.FirstName),
                    "LastNames" => query.IsDescending ? clientsQuery.OrderByDescending(c => c.LastNames) : clientsQuery.OrderBy(c => c.LastNames),
                    "Email" => query.IsDescending ? clientsQuery.OrderByDescending(c => c.Email) : clientsQuery.OrderBy(c => c.Email),
                    "Username" => query.IsDescending ? clientsQuery.OrderByDescending(c => c.Username) : clientsQuery.OrderBy(c => c.Username),
                    "CreatedAt" => query.IsDescending ? clientsQuery.OrderByDescending(c => c.CreatedAt) : clientsQuery.OrderBy(c => c.CreatedAt),
                    _ => clientsQuery
                };
            }

            var totalCount = await clientsQuery.CountAsync();

            // Pagination
            var clients = await clientsQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (clients, totalCount);
        }

        public async Task<Client?> GetByIdAsync(Guid id)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client?> GetByEmailAsync(string email)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Client?> GetByUsernameAsync(string username)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task AddAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                client.IsActive = false;
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Clients.AnyAsync(c => c.Email == email);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Clients.AnyAsync(c => c.Username == username);
        }

    }
}