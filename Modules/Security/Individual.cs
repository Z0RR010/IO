namespace IO.Modules.Security
{
    public class Individual : User
    {
        private string surname;
        private string pesel;
        private string address;

        public Individual(string surname, string pesel, string email, string name, string phoneNumber, string address, bool isVerified) : base(email, name, phoneNumber, address, isVerified)
        {
            this.surname = surname;
            this.pesel = pesel;
            this.address = address;
        }

        public string Surname { get => surname; set => surname = value; }
        public string Pesel { get => pesel; set => pesel = value; }
        public string Address { get => address; set => address = value; }
    }
}
