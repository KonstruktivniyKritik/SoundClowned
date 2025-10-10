using IdentityProvider.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using IdentityProvider.DTOs;
using IdentityProvider.Services;
using System.Security.Claims;
using IdentityProvider.Repositories;

namespace IdentityProvider.Endpoints
{
    // Статический класс для группировки всех эндпоинтов, связанных с аутентификацией (Login, Register)
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            // Создаем группу эндпоинтов с общим префиксом /api/auth
            // Это чище и удобнее для документации Swagger
            var authGroup = app.MapGroup("/api/auth")
                               .WithTags("Authentication");

            // ВАШ ЭНДПОИНТ /login (теперь /api/auth/login)
            authGroup.MapPost("/login", async (LoginRequest request, ITokenService tokenService, IUserRepository repositoryService, IPasswordService passwordService) =>
            {
                ApplicationUser? User = await repositoryService.FindByUsernameAsync(request.Username);
                if (User == null)
                    return Results.Unauthorized();

                Console.WriteLine("User Found");
                if (!passwordService.VerifyPassword(request.Password, User.PasswordHash))
                    return Results.Unauthorized();

                var Claims = new List<Claim>()
                {
                    new Claim("role", User.Role!)
                };

                var jwtToken = tokenService.GenerateToken(Claims);

                return Results.Ok(new LoginResponse("Login Succesfully", jwtToken));
            })
            .WithName("Login")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            authGroup.MapPost("/register", async (RegisterRequest request, IUserRepository repositoryService, IPasswordService passwordService) =>
            {
                try
                {
                    if (await repositoryService.FindByUsernameAsync(request.Username) != null)
                        Results.Conflict("User with this username is already exist");

                    ApplicationUser newUser = new ApplicationUser()
                    {
                        Id = Guid.NewGuid(),
                        Username = request.Username,
                        PasswordHash = passwordService.HashPassword(request.Password),
                        Email = request.Email,
                        Role = "User",
                        CreatedAt = DateTime.UtcNow
                    };

                    await repositoryService.AddUserAsync(newUser);

                    return Results.Created("/api/auth/users/created", new RegisterResponse("Registered"));
                }
                catch
                {
                    return Results.Conflict("An unexpected error occurred during registration.");
                }
            })
            .WithName("Register")
            .Produces<RegisterResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status201Created);

            return app;
        }
    }


}