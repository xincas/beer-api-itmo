using Auth;
using CsharpBeer.OrderService.Domain.Common.Interfaces;
using CsharpBeer.OrderService.Services.Common.Errors;
using AuthC = Auth.Auth;

namespace CsharpBeer.OrderService.Infrastructure.Auth;

public class AuthService(AuthC.AuthClient authClient) : IAuthService
{
    public async Task<(long UserId, string Email)> GetUserInfoAsync(string? token)
    {
        if (token is null) throw RpcErrors.InvalidToken;

        var request = new GetUserInfoRequest() { Token = token };
        var response = await authClient.GetUserInfoAsync(request);

        return (response.UserId, response.Email);
    }

    public async Task<bool> IsAdminAsync(long userId)
    {
        var request = new IsAdminRequest() { UserId = userId };
        var response = await authClient.IsAdminAsync(request);

        return response.IsAdmin;
    }
}