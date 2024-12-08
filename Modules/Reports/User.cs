public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public ICollection<Request> Reports { get; set; } = new List<Request>();

    public Address Address { get; set; }

    // Konstruktor
    public User(string name, string surname, string email, string password, Address address)
    {
        Name = name;
        Surname = surname;
        Email = email;
        Password = password;
        Address = address;
    }

}
