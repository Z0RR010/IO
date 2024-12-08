using Microsoft.AspNetCore.Mvc;
using ResourceManager;

[Route("api/[controller]")]
[ApiController]
public class SignInController : ControllerBase
{
    [HttpPost("SignIn")]
    public IActionResult SignIn([FromBody] LoginRequest request)
    {
        UserExecuter userExecuter = new UserExecuter();
        Console.WriteLine($"Otrzymano dane: {request.Username}, {request.Password}");
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Nazwa użytkownika i hasło są wymagane.");
        }
        
        if(!userExecuter.IsUserInDataBase(request.Username))
        {
			return Unauthorized(new { Message = "Nieprawidłowy login lub hasło." });
		}

        if(userExecuter.IsPasswordCorrect(request.Username, request.Password))
        {
			return Ok(new { Message = "Zalogowano pomyślnie!" });
		}

        return Unauthorized(new { Message = "Nieprawidłowy login lub hasło." });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
