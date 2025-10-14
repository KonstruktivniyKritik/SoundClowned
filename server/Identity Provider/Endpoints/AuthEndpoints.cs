using IdentityProvider.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using IdentityProvider.DTOs;
using IdentityProvider.Services;
using System.Security.Claims;
using IdentityProvider.Repositories;
using Microsoft.EntityFrameworkCore;
using IdentityProvider.Data;

namespace IdentityProvider.Endpoints
{
    // Статический класс для группировки всех эндпоинтов, связанных с аутентификацией (Login, Register)
    public static class AuthEndpoints
    {
        // Нестатический класс-заглушка для логирования, чтобы обойти ошибку CS0718
        public class AuthLogger { }

        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            // Создаем группу эндпоинтов с общим префиксом /api/auth
            // Это чище и удобнее для документации Swagger
            var authGroup = app.MapGroup("/api/auth")
                               .WithTags("Authentication");

            // ВАШ ЭНДПОИНТ /login (теперь /api/auth/login)
            authGroup.MapPost("/login", async (LoginRequest request, ITokenService tokenService, IUserRepository repositoryService, IPasswordService passwordService, ILogger<AuthLogger> logger) =>
            {
                ApplicationUser? User = await repositoryService.FindByUsernameAsync(request.Username);
                if (User == null)
                    return Results.Unauthorized();

                logger.LogInformation("User {Username} found for login attempt", request.Username);
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

            authGroup.MapPost("/register", async (RegisterRequest request, IUserRepository repositoryService, IPasswordService passwordService, ILogger<AuthLogger> logger, IEventProducer eventProducer) =>
            {
                try
                {
                    // Проверка сложности пароля
                    if (!passwordService.IsPasswordStrong(request.Password))
                        return Results.BadRequest("Password must be at least 8 characters long and contain uppercase, lowercase letters and numbers.");

                    if (await repositoryService.FindByUsernameAsync(request.Username) != null)
                        return Results.Conflict("User with this username is already exist");

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

                    await eventProducer.ProduceAsync("NewUsers",
                                                     newUser.Id.ToString(),
                                                     new Events.UserRegisteredEvent(newUser.Id,
                                                                                    newUser.Username,
                                                                                    newUser.Email,
                                                                                    newUser.CreatedAt));

                    return Results.Created("/api/auth/users/created", new RegisterResponse("Registered"));
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true)
                {
                    return Results.Conflict("User with this username or email already exists.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error during user registration for {Username}", request.Username);
                    return Results.Problem("An unexpected error occurred during registration.");
                }
            })
            .WithName("Register")
            .Produces<RegisterResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

            return app;
        }
    }


}