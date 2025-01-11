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
    public string Surname { get; set; }
    public string Pesel { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
}
