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

	public class BasicUser
	{
		private string email;
		private string name;
		private string phoneNumber;
		private string address;
		private bool isVerified;
		private string surname;
		private string pesel;
		private string institution;
		private string website;
		private string krs;
		private string role;

		public BasicUser(string email, string name, string phoneNumber, string address, bool isVerified, string surname, string pesel, string institution, string website, string krs, string role)
		{
			this.email = email;
			this.name = name;
			this.phoneNumber = phoneNumber;
			this.address = address;
			this.isVerified = isVerified;
			this.surname = surname;
			this.pesel = pesel;
			this.institution = institution;
			this.website = website;
			this.krs = krs;
			this.role = role;
		}

		public string Email { get => email; set => email = value; }
		public string Name { get => name; set => name = value; }
		public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
		public string Address { get => address; set => address = value; }
		public bool IsVerified { get => isVerified; set => isVerified = value; }
		public string Surname { get => surname; set => surname = value; }
		public string Pesel { get => pesel; set => pesel = value; }
		public string Institution { get => institution; set => institution = value; }
		public string Website { get => website; set => website = value; }
		public string Krs { get => krs; set => krs = value; }
		public string Role { get => role; set => role = value; }
	}
}
