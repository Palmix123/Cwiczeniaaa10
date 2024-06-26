using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cwiczenia10.Data;
using Cwiczenia10.DTOs;
using Cwiczenia10.Helpers;
using Cwiczenia10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Cwiczenia10.Services;

public class DbService : IDbService
{
    public UsserContext Context { get; set; }

    public DbService(UsserContext context)
    {
        Context = context;
    }

    public async Task RegisterUser(RegisterModel model)
    {
        var hashedPasswordAndSalt = Logger.GetHashedPasswordAndSalt(model.Password);

        var user = new User()
        {
            Login = model.Login,
            Password = hashedPasswordAndSalt.Item1,
            Salt = hashedPasswordAndSalt.Item2,
            RefreshToken = Logger.GenerateRefreshToken()
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();
    }
    
    public async Task<Tuple<string, string>> LoginUser(LoginRequest request, IConfiguration configuration)
    {
        var user = await Context.Users.SingleOrDefaultAsync(u => u.Login == request.Login);

        if (user == null || user.Password != Logger.GetHashedPasswordWithSalt(request.Password, user.Salt))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, "admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(30)
        );

        user.RefreshToken = Logger.GenerateRefreshToken();
        await Context.SaveChangesAsync();
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new Tuple<string, string>(accessToken, user.RefreshToken);
    }

    
    public async Task<Tuple<string, string>> RefreshAccessToken(string refreshToken, IConfiguration configuration)
    {
        var user = await Context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),  
            new Claim(ClaimTypes.Name, user.Login)
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(30)
        );

        user.RefreshToken = Logger.GenerateRefreshToken();
        await Context.SaveChangesAsync();
        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new Tuple<string, string>(accessToken, user.RefreshToken);
    }
}