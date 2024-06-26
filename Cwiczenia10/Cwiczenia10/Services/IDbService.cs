using System.IdentityModel.Tokens.Jwt;
using Cwiczenia10.DTOs;

namespace Cwiczenia10.Services;

public interface IDbService
{
    Task RegisterUser(RegisterModel model);
    Task<Tuple<string, string>> LoginUser(LoginRequest request, IConfiguration configuration);
    Task<Tuple<string, string>> RefreshAccessToken(string refreshToken, IConfiguration configuration);
}