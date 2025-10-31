using censudex_clients_service.src.Data;
using censudex_clients_service.src.Helpers.Requests;
using censudex_clients_service.src.Models;
using Microsoft.EntityFrameworkCore;

namespace censudex_clients_service.src.Repositories
{
    /// <summary>
    /// Provides data access operations for Client entities in the database.
    /// </summary>
    /// <remarks>
    /// This repository implements the IClientRepository interface and handles
    /// all CRUD operations for Client entities, including filtering, sorting,
    /// and pagination for query operations.
    /// </remarks>
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Initializes a new instance of the ClientRepository class.
        /// </summary>
        /// <param name="context">The database context to be used for data operations.</param>
        public ClientRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new client in the database.
        /// </summary>
        /// <param name="client">The client entity to create.</param>
        /// <returns>The created client entity with any database-generated values.</returns>
        /// <remarks>
        /// This method adds the client to the context and saves changes immediately.
        /// The returned entity includes any database-generated values such as timestamps.
        /// </remarks>
        public async Task<Client> CreateAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        /// <summary>
        /// Retrieves a paginated and filtered list of clients from the database.
        /// </summary>
        /// <param name="query">The query parameters containing filtering, sorting, and pagination options.</param>
        /// <returns>
        /// A tuple containing:
        /// - IEnumerable of Client entities matching the query criteria
        /// - Total count of clients matching the filter criteria (before pagination)
        /// </returns>
        /// <remarks>
        /// <para>
        /// Supported filtering options:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description>FullName: Partial match on combined first name and last names</description>
        /// </item>
        /// <item>
        /// <description>Email: Exact match on email address</description>
        /// </item>
        /// <item>
        /// <description>Username: Partial match on username</description>
        /// </item>
        /// <item>
        /// <description>IsActive: Filter by active status</description>
        /// </item>
        /// </list>
        /// <para>
        /// Supported sorting fields: FirstName, LastNames, Email, Username, CreatedAt
        /// </para>
        /// <para>
        /// Pagination uses zero-based skipping and takes the specified page size.
        /// </para>
        /// </remarks>
        public async Task<(IEnumerable<Client> Clients, int TotalCount)> GetAllAsync(ClientQuery query)
        {
            var clientsQuery = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.FullName))
            {
                clientsQuery = clientsQuery.Where(c =>
                    (c.FirstName + " " + c.LastNames).Contains(query.FullName));
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

        /// <summary>
        /// Retrieves a client by their unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the client to retrieve.</param>
        /// <returns>
        /// The Client entity if found; otherwise, null.
        /// </returns>
        public async Task<Client?> GetByIdAsync(Guid id)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Retrieves a client by their email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>
        /// The Client entity if found; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This method performs a case-sensitive exact match on the email address.
        /// </remarks>
        public async Task<Client?> GetByEmailAsync(string email)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        }

        /// <summary>
        /// Retrieves a client by their username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>
        /// The Client entity if found; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This method performs a case-sensitive exact match on the username.
        /// </remarks>
        public async Task<Client?> GetByUsernameAsync(string username)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Username == username);
        }

        /// <summary>
        /// Adds a new client to the database asynchronously.
        /// </summary>
        /// <param name="client">The client entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method is similar to CreateAsync but does not return the created entity.
        /// Use CreateAsync if you need access to database-generated values.
        /// </remarks>
        public async Task AddAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing client in the database.
        /// </summary>
        /// <param name="client">The client entity with updated values.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method marks the entity as modified and saves all changes to the database.
        /// The client entity must be tracked by the context for updates to work properly.
        /// </remarks>
        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Performs a soft delete on a client by marking them as inactive.
        /// </summary>
        /// <param name="id">The GUID of the client to soft delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method sets the IsActive property to false instead of physically
        /// deleting the record from the database. The client can be reactivated
        /// by updating the IsActive property to true.
        /// </remarks>
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

        /// <summary>
        /// Checks if a client with the specified email address already exists.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>
        /// true if a client with the specified email exists; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method is typically used for validation during client registration
        /// to ensure email uniqueness across the system.
        /// </remarks>
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Clients.AnyAsync(c => c.Email == email);
        }

        /// <summary>
        /// Checks if a client with the specified username already exists.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>
        /// true if a client with the specified username exists; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This method is typically used for validation during client registration
        /// to ensure username uniqueness across the system.
        /// </remarks>
        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Clients.AnyAsync(c => c.Username == username);
        }

    }
}