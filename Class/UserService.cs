
using Microsoft.AspNetCore.Identity;

namespace inicio.models;
public class UserService
{
    private readonly PasswordHasher<string> password;
    public UserService()
    {
        password = new PasswordHasher<string>();
    }
    public string GeneratePassword(string passwordtoscript)
    {
        return password.HashPassword(null, passwordtoscript);
    }
    public bool verifyPassword(string passwordtoverify, string HashPassword)
    {
        var passwordVerificationResult = password.VerifyHashedPassword(null, HashPassword, passwordtoverify);
        return passwordVerificationResult == PasswordVerificationResult.Success;
    }
   
}