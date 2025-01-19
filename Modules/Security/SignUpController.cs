using Microsoft.AspNetCore.Mvc;
using IO.Modules.ResourceManager;

[Route("api/[controller]")]
[ApiController]
public class SignUpController : ControllerBase
{
    [HttpPost("SignUp")]
    public IActionResult SignUp([FromBody] RegisterRequest request)
    {
        UserExecuter userExecuter = new UserExecuter();
        if (!userExecuter.IsUserInDataBase(request.Email))
        {
            return Ok(new { Message = "Poprawnie zarejestrowano" });
        }
        return Unauthorized(new { Message = "Taki użytkownik już istnieje" });
    }
}
public class RegisterRequest
{
	public string Email { get; set; }
}