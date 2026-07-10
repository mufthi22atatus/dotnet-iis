using System.Threading.Tasks;
using TaskManager.Data.Entities;

namespace TaskManager.Services
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public Employee Employee { get; set; }
    }

    public interface IAuthService
    {
        Task<AuthResult> AuthenticateAsync(string email, string password, string ip);
        Task<AuthResult> RegisterAsync(string fullName, string email, string password, string department);
    }
}
