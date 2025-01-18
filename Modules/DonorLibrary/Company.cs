namespace IO.Modules.DonorLibrary
{
    public class Company : IDonorType
    {
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string NIP { get; set; }

        public Company(string email, string companyName, string companyAddress, string phoneNumber, string nip)
        {
            Email = email;
            CompanyName = companyName;
            CompanyAddress = companyAddress;
            PhoneNumber = phoneNumber;
            NIP = nip;
        }

        public string getEmail()
        {
            return $"{Email}";
        }

        public string getName()
        {
            return $"{CompanyName}";
        }

        public string getSurname()
        {
            return null;
        }

        public string getAddress()
        {
            return $"{CompanyAddress}";
        }

        public string getContactNumber()
        {
            return $"{PhoneNumber}";
        }

        public string getIdentifier()
        {
            return $"{NIP}";
        }
        public string getTypeName()
        {
            return "Company";
        }
    }
}