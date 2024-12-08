public class Address
{
    public int Id { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string StreetNumber { get; set; }
    public string Apartment { get; set; }
    public string ZipCode { get; set; }

    // Relacja: jeden adres mo¿e byæ powi¹zany z wieloma raportami
    public ICollection<Request> Reports { get; set; } = new List<Request>(); //wywaliæ?

    // Konstruktor
    public Address(string city, string street, string streetNumber, string apartment, string zipCode)
    {
        City = city;
        Street = street;
        StreetNumber = streetNumber;
        Apartment = apartment;
        ZipCode = zipCode;
    }
}
