using Microsoft.AspNetCore.Identity;

namespace TPI_2026.Infrastructure.Persistence.Services
{

    public class BCryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        public string HashPassword(TUser user, string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            bool isValid = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);

            return isValid
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
    }
}