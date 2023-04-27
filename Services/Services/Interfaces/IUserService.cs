using Shared.Models.Identify;

namespace Services.Services.Interfaces
{
    public interface IUserService : IService<User>
    {
        bool AuthorizeCustomer(string username, string password, out User user);
        bool AuthorizeAdminUser(string username, string password, out User user);
    }
}
