using Authly.Models;
using Authly.Models.Dtos;
using Authly.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Authly.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly ICloudinaryService _cloudinaryService;

        public UserService(IOptions<AuthlyDatabaseSettings> settings, ICloudinaryService cloudinaryService)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<User>(settings.Value.UsersCollectionName);
            _cloudinaryService = cloudinaryService;
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
                var uploadResult = await _cloudinaryService.UploadImageAsync(data.Avatar, "user");
                avtUrl = uploadResult.PublicId;
            }

            // Handle password
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);

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

        public async Task<UserDto?> GetByIdAsync(string id)
        {
            var user = await _users.Find(x => x.Id == id).FirstOrDefaultAsync();
            return user != null ? UserDto.FromUser(user) : null;
        }

        public Task<UserDto?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> UpdateAsync(string id, UpdateUserDto dto)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var updateDefinition = new List<UpdateDefinition<User>>();

            if (dto.Name != null)
                updateDefinition.Add(Builders<User>.Update.Set(u => u.Name, dto.Name));

            if (dto.Birthday != null)
                updateDefinition.Add(Builders<User>.Update.Set(u => u.Birthday, dto.Birthday));

            if (dto.Role.HasValue)
                updateDefinition.Add(Builders<User>.Update.Set(u => u.Role, dto.Role.Value));

            if (updateDefinition.Count == 0)
            {
                var user = await _users.Find(filter).FirstOrDefaultAsync();
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} not found.");
                return UserDto.FromUser(user);
            }

            updateDefinition.Add(Builders<User>.Update.Set(u => u.UpdatedAt, DateTime.UtcNow));

            var update = Builders<User>.Update.Combine(updateDefinition);
            var updatedUser = await _users.FindOneAndUpdateAsync(
                filter,
                update,
                new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After }
            );

            if (updatedUser == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");

            return UserDto.FromUser(updatedUser);
        }

        public async Task<UserDto> UpdateImageAsync(string id, IFormFile file)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var user = await _users.Find(filter).FirstOrDefaultAsync();
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");

            if (!string.IsNullOrEmpty(user.AvtUrl))
            {
                try
                {
                    await _cloudinaryService.DeleteImageAsync(user.AvtUrl);
                }
                catch
                {
                    // Ignore or log error to not block the user from uploading a new image
                }
            }

            var uploadResult = await _cloudinaryService.UploadImageAsync(file, "user");
            var publicId = uploadResult.PublicId;

            var update = Builders<User>.Update
                .Set(u => u.AvtUrl, publicId)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            var updatedUser = await _users.FindOneAndUpdateAsync(
                filter,
                update,
                new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After }
            );

            return UserDto.FromUser(updatedUser);
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AssignRoleAsync(string id, string role)
        {
            throw new NotImplementedException();
        }
    }
}
