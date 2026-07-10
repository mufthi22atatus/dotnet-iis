using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManager.Data.Entities;
using TaskManager.Data.Repositories;

namespace TaskManager.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IAuditService _audit;

        public AuthService(IUserRepository users, IPasswordHasher hasher, IAuditService audit)
        {
            _users = users;
            _hasher = hasher;
            _audit = audit;
        }

        public async Task<AuthResult> AuthenticateAsync(string email, string password, string ip)
        {
            using ((IDisposable)_users)
            {
                var user = await _users.FindByEmailAsync(email);
                if (user == null)
                {
                    AppLogger.Create<AuthService>()?.LogInformation("Login miss: unknown email {Email}", email);
                    await _audit.RecordAsync("auth.login.miss", null, email, ip, "unknown email");
                    return new AuthResult { Success = false, Error = "Invalid email or password" };
                }

                if (!user.IsActive)
                {
                    await _audit.RecordAsync("auth.login.disabled", user.Id, user.Email, ip, "account disabled");
                    return new AuthResult { Success = false, Error = "Account is disabled" };
                }

                if (!_hasher.Verify(password, user.PasswordHash, user.PasswordSalt))
                {
                    user.FailedLoginCount += 1;
                    _users.Update(user);
                    await _users.SaveChangesAsync();
                    await _audit.RecordAsync("auth.login.fail", user.Id, user.Email, ip, "bad password");
                    return new AuthResult { Success = false, Error = "Invalid email or password" };
                }

                user.LastLoginAt = DateTime.UtcNow;
                user.FailedLoginCount = 0;
                _users.Update(user);
                await _users.SaveChangesAsync();
                await _audit.RecordAsync("auth.login.success", user.Id, user.Email, ip, null);

                return new AuthResult { Success = true, Employee = user };
            }
        }

        public async Task<AuthResult> RegisterAsync(string fullName, string email, string password, string department)
        {
            using ((IDisposable)_users)
            {
                var existing = await _users.FindByEmailAsync(email);
                if (existing != null)
                    return new AuthResult { Success = false, Error = "Email already in use" };

                var (hash, salt) = _hasher.Hash(password);
                var user = new Employee
                {
                    FullName = fullName,
                    Email = email,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    Department = department,
                    Role = "Employee",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _users.Add(user);
                await _users.SaveChangesAsync();
                await _audit.RecordAsync("auth.register", user.Id, user.Email, null, "self-registration");
                AppLogger.Create<AuthService>()?.LogInformation("Registered new user {Email}", email);

                return new AuthResult { Success = true, Employee = user };
            }
        }
    }
}
