namespace IO.Modules.DonorLibrary
{
    public interface IDonorType
    {
        string getEmail();
        string getName();
        string getSurname();
        string getAddress();
        string getContactNumber();
        string getIdentifier();
        string getTypeName();
    }
}
