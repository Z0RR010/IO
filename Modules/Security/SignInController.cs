using Microsoft.AspNetCore.Mvc;
using IO.Modules.ResourceManager;
using System.Collections.Concurrent;

[Route("api/[controller]")]
[ApiController]
public class SignInController : ControllerBase
{
    private static ConcurrentDictionary<string, (int attempts, DateTime? lockoutEnd)> loginAttempts = new();

    private const int MaxAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(1);

    [HttpPost("SignIn")]
    public IActionResult SignIn([FromBody] LoginRequest request)
    {
        UserExecuter userExecuter = new UserExecuter();
        Console.WriteLine($"Otrzymano dane: {request.Username}, {request.Password}");

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Nazwa użytkownika i hasło są wymagane.");
        }

        if (loginAttempts.TryGetValue(request.Username, out var attemptInfo))
        {
            if (attemptInfo.lockoutEnd.HasValue && DateTime.UtcNow < attemptInfo.lockoutEnd.Value)
            {
                return Unauthorized(new { Message = $"Zbyt wiele nieudanych prób. Spróbuj ponownie za {(attemptInfo.lockoutEnd.Value - DateTime.UtcNow).ToString(@"mm\:ss")}." });

            }
        }

        if (!userExecuter.IsUserInDataBase(request.Username))
        {
            RegisterFailedAttempt(request.Username);
            return Unauthorized(new { Message = "Nieprawidłowy login lub hasło." });
        }

        if (userExecuter.CustomQuery("SELECT emailVerified FROM users WHERE email=" + "\"" + request.Username + "\"")
                       .Split('\n')[1].Trim() == "False")
        {
            return Unauthorized(new { Message = "Użytkownik nie potwierdził rejestracji przez email." });
        }

        if (userExecuter.IsPasswordCorrect(request.Username, request.Password))
        {
            loginAttempts.TryRemove(request.Username, out _); // Reset attempts on successful login
            return Ok(new { Message = "Zalogowano pomyślnie!" });
        }

        RegisterFailedAttempt(request.Username);
        return Unauthorized(new { Message = "Nieprawidłowy login lub hasło." });
    }

    private void RegisterFailedAttempt(string username)
    {
        loginAttempts.AddOrUpdate(username,
            _ => (1, null), // Dodanie nowego wpisu
            (_, attemptInfo) =>
            {
                int newAttempts = attemptInfo.attempts + 1;
                DateTime? lockoutEnd = attemptInfo.lockoutEnd;

                if (newAttempts >= MaxAttempts)
                {
                    lockoutEnd = DateTime.UtcNow.Add(LockoutDuration);
                    Console.WriteLine($"User {username} is locked out until {lockoutEnd}");
                }

                return (newAttempts, lockoutEnd);
            });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}