namespace RequestModule
{
    public class Address
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Apartment { get; set; }
        public string ZipCode { get; set; }

        // Konstruktor
        public Address(string city, string street, string streetNumber, string apartment, string zipCode)
        {
            City = city;
            Street = street;
            StreetNumber = streetNumber;
            Apartment = apartment;
            ZipCode = zipCode;
        }
        public Address()
        {
        }
    }
}