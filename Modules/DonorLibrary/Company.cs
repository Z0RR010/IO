namespace IO.Modules.DonorLibrary
{
    public class Company : IDonorType
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string NIP { get; set; }

        public Company(string companyName, string companyAddress, string phoneNumber, string nip)
        {
            CompanyName = companyName;
            CompanyAddress = companyAddress;
            PhoneNumber = phoneNumber;
            NIP = nip;
        }

        public string getName()
        {
            return $"{CompanyName}";
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