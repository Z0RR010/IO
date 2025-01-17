using Microsoft.AspNetCore.Mvc;
using IO.Modules.ResourceManager;

[Route("api/[controller]")]
[ApiController]
public class SignUpController : ControllerBase
{
    [HttpPost("SignUp")]
    public IActionResult SignUp([FromBody] RegisterRequest request)
    {
        Console.WriteLine("b");
        UserExecuter userExecuter = new UserExecuter();
        if (!userExecuter.IsUserInDataBase(request.Email))
        {
            Console.WriteLine("c");
            return Ok(new { Message = "Poprawnie zarejestrowano" });
        }
        Console.WriteLine("d");
        return Unauthorized(new { Message = "Taki użytkownik już istnieje" });
    }
}

/*public class RegisterRequest
{
    public string Surname { get; set; }
    public string Pesel { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public bool IsVerified { get; set; }
    public string Institution { get; set; }
    public string Website { get; set; }
    public string Krs { get; set; }
    public string Role { get; set; }
}
*/

public class RegisterRequest
{
	public string Email { get; set; }
}