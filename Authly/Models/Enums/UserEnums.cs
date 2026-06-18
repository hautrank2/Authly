using System.Text.Json.Serialization;

namespace Authly.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Backend,
        Frontend,
        BA,
        Dev,
        TeamLead, 
        Admin
    }
}
