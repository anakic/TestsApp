using jogging.Model;
using System.Threading.Tasks;

namespace jogging.Services
{
    public interface IUserService
    {
        User GetCurrentUser();
        Task<User> LoginAsync(string username, string password);
    }
}