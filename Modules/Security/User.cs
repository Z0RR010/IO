namespace IO.Modules.Security
{
    public class User
    {
        private string email;
        private string name;
        private string phoneNumber;
        private string address;
        private bool isVerified;

        public User(string email, string name, string phoneNumber, string address, bool isVerified)
        {
            this.email = email;
            this.name = name;
            this.phoneNumber = phoneNumber;
            this.address = address;
            this.isVerified = isVerified;
        }

        public string Email { get => email; set => email = value; }
        public string Name { get => name; set => name = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        public string Address { get => address; set => address = value; }
        public bool IsVerified { get => isVerified; set => isVerified = value; }
    }
}
