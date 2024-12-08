namespace IO.Modules.Security
{
	public class Charity : User
	{
		private string website;
		private string krs;

        public Charity(string website, string krs, string email, string name, string phoneNumber, string address, bool isVerified) : base(email, name, phoneNumber, address, isVerified)
        {
            this.website = website;
            this.krs = krs;
        }

        public string Website { get => website; set => website = value; }
        public string Krs { get => krs; set => krs = value; }
    }
}
