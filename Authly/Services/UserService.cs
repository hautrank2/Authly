using Authly.Models;
using Authly.Models.Dtos;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Authly.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IOptions<AuthlyDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<User>(settings.Value.UsersCollectionName);

            // Index unique cho username
            var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.Username);
            var indexOptions = new CreateIndexOptions { Unique = true };
            _users.Indexes.CreateOne(new CreateIndexModel<User>(indexKeys, indexOptions));
        }

        public async Task<PaginationDto<User>> GetUsersAsync(UserQueryDto filter)
        {
            var builder = Builders<User>.Filter;
            var conditions = new List<FilterDefinition<User>>();
            var pageSize = filter.PageSize;
            var pageIndex = filter.PageIndex;

            if (!string.IsNullOrWhiteSpace(filter.Name))
                conditions.Add(builder.Regex(u => u.Name, new MongoDB.Bson.BsonRegularExpression(filter.Name, "i")));

            if (filter.Role.HasValue)
                conditions.Add(builder.Eq(u => u.Role, filter.Role.Value));

            if (filter.StartAge.HasValue || filter.EndAge.HasValue)
            {
                var today = DateTime.Today;

                if (filter.StartAge.HasValue)
                {
                    var startDate = today.AddYears(-filter.StartAge.Value);
                    conditions.Add(builder.Lte(u => u.Birthday, startDate.ToString("yyyy-MM-dd")));
                }

                if (filter.EndAge.HasValue)
                {
                    var endDate = today.AddYears(-filter.EndAge.Value - 1).AddDays(1);
                    conditions.Add(builder.Gte(u => u.Birthday, endDate.ToString("yyyy-MM-dd")));
                }
            }

            var finalFilter = conditions.Count > 0 ? builder.And(conditions) : builder.Empty;

            var totalCount = await _users.CountDocumentsAsync(finalFilter);

            var itemQuery = (pageSize >= 0 && pageIndex > 0) ? _users.Find(finalFilter).Skip(((pageIndex - 1) * pageSize)).Limit(pageSize) : _users.Find(finalFilter);

            var items = await itemQuery.ToListAsync();

            return new PaginationDto<User>
            {
                Items = items,
                TotalCount = (int)totalCount,
                TotalPage = (int)Math.Ceiling((double)totalCount / pageSize),
                PageIndex = pageIndex,
                PageSize = pageSize,
            };
        }
    }
}
