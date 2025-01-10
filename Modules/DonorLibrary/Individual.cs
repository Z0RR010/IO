namespace IO.Modules.DonorLibrary
{
    public class Individual : IDonorType
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string PESEL { get; set; }

        public Individual(string name, string surname, string address, string phoneNumber, string pesel)
        {
            Name = name;
            Surname = surname;
            Address = address;
            PhoneNumber = phoneNumber;
            PESEL = pesel;
        }

        public string getName()
        {
            return $"{Name} {Surname}";
        }

        public string getAddress()
        {
            return $"{Address}";
        }

        public string getContactNumber()
        {
            return $"{PhoneNumber}";
        }

        public string getIdentifier()
        {
            return $"{PESEL}";
        }
        public string getTypeName()
        {
            return "Individual";
        }
    }
}
