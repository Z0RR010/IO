using IO.Modules.ResourceManager;
using IO.Modules.Volunteer;

namespace IO.Modules.DonorLibrary
{
    public class DonationManager
    {
        private List<Donation> donations;

        public DonationManager()
        {
            donations = new List<Donation>();
        }

        public void AddDonation(string itemName, int? quantity, string date, string email)
        {
            int newId = GetNextId();
            var donation = new Donation(newId, itemName, quantity, date, email);
            donations.Add(donation);
            var executor = new DonorExecuter();

            if (executor.SendDonationToDatabase(donation))
            {
                Console.WriteLine("Donation added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update donation.");
            }
            executor.Dispose();
        }

        private int GetNextId()
        {
            return donations.Count == 0 ? 0 : donations.Max(d => d.DonationID) + 1;
        }
        public List<Donation> GetAllDonations()
        {
            return donations;
        }

        public void RemoveDonation(int donationID)
        {
            var donation = donations.FirstOrDefault(d => d.DonationID == donationID);
            if (donation == null)
                throw new KeyNotFoundException($"Donation with ID: {donationID} not found.");

            donations.Remove(donation);
            donation.AssignedDonor?.donations.Remove(donation);
        }

        public Donation GetDonationById(int donationID)
        {
            var donation = donations.FirstOrDefault(d => d.DonationID == donationID);
            if (donation == null)
                throw new KeyNotFoundException($"Donation with ID: {donationID} not found.");
            return donation;
        }

        public IEnumerable<Donation> GetDonationsByDonor(Donor donor)
        {
            return donations.Where(d => d.AssignedDonor == donor);
        }

        public void Load()
        {
            var executor = new DonorExecuter();
            donations = executor.LoadDonationList();
            executor.Dispose();
        }
    }
}
