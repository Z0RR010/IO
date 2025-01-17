using IO.Modules.DonorLibrary;

public class DonorManager
{
    private List<Donor> donors;
    private Donor currentDonor;

    public DonorManager()
    {
        donors = new List<Donor>();
    }

    public void RegisterDonor(Donor donor)
    {
        if (donor == null)
            throw new ArgumentNullException(nameof(donor), "Donor cannot be null.");

        if (donors.Any(d => d.donorID == donor.donorID))
            throw new InvalidOperationException($"Darczyńca o ID {donor.donorID} już istnieje.");

        donors.Add(donor);
    }

    public bool Login(int donorID)
    {
        currentDonor = donors.FirstOrDefault(d => d.donorID == donorID);
        return currentDonor != null;
    }

    public bool IsLoggedIn()
    {
        return currentDonor != null;
    }

    public Donor GetCurrentDonor()
    {
        if (currentDonor == null)
            throw new InvalidOperationException("No donor is logged in.");
        return currentDonor;
    }

    public void Logout()
    {
        if (currentDonor == null)
            throw new InvalidOperationException("No donor is currently logged in.");

        currentDonor = null;
    }

    public void RemoveDonor(int donorID)
    {
        var donorToRemove = donors.FirstOrDefault(d => d.donorID == donorID);

        if (donorToRemove == null)
            throw new KeyNotFoundException($"Darczyńca o ID {donorID} nie został znaleziony.");

        donors.Remove(donorToRemove);
    }

    public Donor FindDonorByID(int donorID)
    {
        return donors.FirstOrDefault(d => d.donorID == donorID);
    }
}