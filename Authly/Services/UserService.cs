using Authly.Models;
using Authly.Models.Dtos;
using Authly.Services.Dtos;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Authly.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IOptions<AuthlyDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<User>(settings.Value.UsersCollectionName);
        }


        public async Task<PaginationDto<UserDto>> GetAllAsync(UserQueryDto filter)
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

            var itemFind = _users.Find(finalFilter);
            var itemQuery = (pageSize >= 0 && pageIndex > 0) ? itemFind.Skip(((pageIndex - 1) * pageSize)).Limit(pageSize) : itemFind;

            var items = (await itemQuery.ToListAsync()).Select(UserDto.FromUser).ToList();

            return new PaginationDto<UserDto>
            {
                Items = items,
                TotalCount = (int)totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };
        }

        public async Task<UserDto> CreateAsync(CreateUserDto data)
        {
            var avtUrl = "";
            if (data.Avatar != null)
            {
                // upload
            }

            // Handle password
            var hashPassword = "iaojfpoasjofsadijfoaisdj";

            var userDoc = new User
            {
                Username = data.Username,
                AvtUrl = string.IsNullOrEmpty(avtUrl) ? null : avtUrl,
                Birthday = data.Birthday,
                Name = data.Name,
                Role = data.Role,
                LatestAccess = null,
                Password = hashPassword,
            };

            await _users.InsertOneAsync(userDoc);

            return UserDto.FromUser(userDoc);
        }
        Task<bool> IUserService.AssignRoleAsync(string id, string role)
        {
            throw new NotImplementedException();
        }

        Task<bool> IUserService.DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        Task<UserDto?> IUserService.GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        Task<UserDto?> IUserService.GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        Task<UserDto> IUserService.UpdateAsync(string id, UpdateUserDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
