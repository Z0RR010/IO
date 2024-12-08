namespace IO.Modules.Security
{
    public class Authority : Individual
    {
        private string institution;

        public Authority(string institution, string surname, string pesel, string email, string name, string phoneNumber, string address, bool isVerified) : base(surname, pesel, email, name, phoneNumber, address, isVerified)
        {
            this.institution = institution;
        }
    }
}
