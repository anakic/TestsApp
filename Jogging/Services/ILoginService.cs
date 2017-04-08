using jogging.Model;
using System.Threading.Tasks;

namespace jogging.Services
{
    public interface ILoginService
    {
        User GetCurrentUser();
        Task<User> LoginAsync(string username, string password);

        Task SignOut();
    }
}