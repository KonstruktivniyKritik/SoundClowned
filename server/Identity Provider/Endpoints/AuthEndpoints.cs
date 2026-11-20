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
                logger.LogInformation($"Get request {request.ToString()}");
                ApplicationUser? User = await repositoryService.FindByUsernameAsync(request.Email);
                if (User == null)
                    return Results.Unauthorized();

                logger.LogInformation("User {Email} found for login attempt", request.Email);
                if (!passwordService.VerifyPassword(request.Password, User.PasswordHash))
                    return Results.Unauthorized();

                var Claims = new List<Claim>()
                {
                    new Claim("role", User.Role!),
                    new Claim("id", User.Id.ToString()),
                    new Claim("sub", User.Username)
                };

                var jwtToken = tokenService.GenerateToken(Claims);

                return Results.Ok(new LoginResponse("Login Succesfully", jwtToken));
            })
            .WithName("Login")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            authGroup.MapPost("/register", async (
            RegisterRequest request,
            IUserRepository repositoryService,
            IPasswordService passwordService,
            ILogger<AuthLogger> logger,
            IServiceScopeFactory scopeFactory) =>
            {
                try
                {
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

                    var userId = newUser.Id.ToString();
                    var userEvent = new Events.UserRegisteredEvent(
                        newUser.Id,
                        newUser.Username,
                        newUser.Email,
                        newUser.CreatedAt);

                    _ = Task.Run(async () =>
                    {
                        await using var scope = scopeFactory.CreateAsyncScope();
                        var scopedProducer = scope.ServiceProvider.GetRequiredService<IEventProducer>();
                        var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<AuthLogger>>();

                        try
                        {
                            await scopedProducer.ProduceAsync("NewUsers", userId, userEvent);
                        }
                        catch (Exception ex)
                        {
                            scopedLogger.LogError(ex, "Failed to publish UserRegisteredEvent for {UserId}", userId);
                        }
                    });

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