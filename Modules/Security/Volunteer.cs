namespace IO.Modules.Security
{
    public class Volunteer : Individual
    {
        public Volunteer(string surname, string pesel, string email, string name, string phoneNumber, string address, bool isVerified) : base(surname, pesel, email, name, phoneNumber, address, isVerified)
        {
        }
    }
}
